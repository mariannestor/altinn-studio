using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Altinn.Studio.DataModeling.Converter.Json;
using Altinn.Studio.DataModeling.Converter.Xml;
using Altinn.Studio.DataModeling.Json;
using Altinn.Studio.DataModeling.Json.Keywords;
using DataModeling.Tests.Assertions;
using Json.Schema;
using Microsoft.Extensions.Logging;

namespace DataModeling.Console
{
    public class RoundtripConversionAnalyzer
    {
        private const string SchemaRootPath = @"C:\Users\misha\Documents\digdir\data\ai2dm_13.09.2022";
        private const string JsonSchemaUploadLocation = @"C:\Users\misha\Documents\digdir\data\JsonSchemas";

        public async Task VerifyAltinn2Xsd(bool exportJsonSchemas = false)
        {
            var seresSchemas = new List<(string Name, XmlSchema Schema)>();
            var otherSchemas = new List<(string Name, XmlSchema Schema)>();

            var failed = new List<(string, string)>();

            var failedList = JsonSerializer.Deserialize<List<string>>(
                 await File.ReadAllTextAsync(@"C:\Users\misha\Documents\digdir\data\failedSchemaNames.json")
                );

            var schemaFiles = Directory
                .EnumerateFiles(SchemaRootPath, "*.xsd", SearchOption.AllDirectories)
                .ToArray();

            var fileStruc = schemaFiles.Select(x =>
            {
                var shortFileName = Path.GetFileName(x);

                var org = x[(SchemaRootPath.Length + 1)..].Split("\\").First();
                return new
                {
                    FileName = shortFileName,
                    Org = org
                };
            }).Where(x => failedList.Contains(x.FileName)).ToArray();

            var grouped = fileStruc.GroupBy(x => x.Org)
                .Select(g =>
                {
                    return new
                    {
                        Org = g.Key,
                        Files = g.Select(g => g.FileName).ToList(),
                        Count = g.Count()
                    };
                }).ToList();

            var s = "";
            foreach (var item in grouped)
            {
                s += $"Organization: {item.Org}, Count: {item.Count}, Items:{Environment.NewLine}";
                s = item.Files.Aggregate(s, (current, file) => current + $"    - {file}{Environment.NewLine}");
            }


            PopulateSchemaCollections(schemaFiles, seresSchemas, otherSchemas);

            JsonSchemaKeywords.RegisterXsdKeywords();

            int failedCount = 0;
            int successCount = 0;
            foreach (var schemaItem in seresSchemas)
            {
                System.Console.WriteLine($"Processing schema {schemaItem.Name}");

                try
                {
                    // Convert the XSD to JSON Schema
                    var xsdToJsonConverter = new XmlSchemaToJsonSchemaConverter();
                    JsonSchema convertedJsonSchema = xsdToJsonConverter.Convert(schemaItem.Schema);

                    if (exportJsonSchemas)
                    {
                        var convertedJsonSchemaString = JsonSerializer.Serialize(convertedJsonSchema, new JsonSerializerOptions() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement), WriteIndented = true });
                        var jsonSchemaName = Path.Combine(JsonSchemaUploadLocation,
                            Path.GetFileNameWithoutExtension(schemaItem.Name) + ".json");
                        await File.WriteAllTextAsync(jsonSchemaName, convertedJsonSchemaString);
                    }

                    // Convert the converted JSON Schema back to XSD
                    var jsonToXsdConverter = new JsonSchemaToXmlSchemaConverter(new JsonSchemaNormalizer());
                    var convertedXsd = jsonToXsdConverter.Convert(convertedJsonSchema);

                    var convertedXsdString = await Serialize(convertedXsd);
                    var originalXsdString = await Serialize(schemaItem.Schema);

                    // The two XSD's should be structural equal, but there might be minor differences if you compare the text
                    XmlSchemaAssertions.IsEquivalentTo(schemaItem.Schema, convertedXsd);

                    successCount++;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Failed while converting {schemaItem.Name} with {ex.Message}");
                    failedCount++;
                }
            }

            System.Console.WriteLine($"Successfully converted: {successCount} files  |   Failed converting: {failedCount} files");
        }

        private void PopulateSchemaCollections(string[] schemaFiles, List<(string Name, XmlSchema Schema)> seresSchemas, List<(string Name, XmlSchema Schema)> otherSchemas)
        {
            foreach (var (filePath, idx) in schemaFiles.Select((filePath, idx) => (filePath, idx)))
            {
                XmlSchema schema;
                using (var stream = File.OpenRead(filePath))
                {
                    schema = XmlSchema.Read(stream, null);
                }

                if (schema == null)
                {
                    continue;
                }

                var schemaName = Path.GetFileName(filePath);

                if (IsSeresSchema(schema, schemaName))
                {
                    seresSchemas.Add((schemaName, schema));
                }
                else
                {
                    otherSchemas.Add((schemaName, schema));
                }
            }
        }

        private bool ValidateXml(XmlSchema xmlSchema, string xml)
        {
            var xmlSchemaValidator = new Tests.TestHelpers.XmlSchemaValidator(xmlSchema);

            var validXml = xmlSchemaValidator.Validate(xml);
            if (!validXml)
            {
                xmlSchemaValidator.ValidationErrors.ForEach(e => System.Console.WriteLine(e.Message));
            }

            return validXml;
        }

        private static async Task<string> Serialize(XmlSchema xmlSchema)
        {
            string actualXml;
            await using (var sw = new Utf8StringWriter())
            await using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { Indent = true, Async = true }))
            {
                xmlSchema.Write(xw);
                actualXml = sw.ToString();
            }

            return actualXml;
        }

        private static bool IsSeresSchema(XmlSchema schema, string path)
        {
            bool isSeresSchema = false;

            var annotations = schema.Items
                .Cast<XmlSchemaObject>()
                .Where(item => item is XmlSchemaAnnotation)
                .Cast<XmlSchemaAnnotation>();

            foreach (var annotation in annotations)
            {
                var documentations = annotation.Items
                    .Cast<XmlSchemaObject>()
                    .Where(item => item is XmlSchemaDocumentation)
                    .Cast<XmlSchemaDocumentation>();

                foreach (var documentation in documentations)
                {
                    var markup = documentation.Markup;
                    if (markup == null)
                    {
                        continue;
                    }

                    foreach (var node in markup)
                    {
                        if (node == null)
                        {
                            continue;
                        }

                        if (node.NamespaceURI == "http://www.w3.org/2001/XMLSchema" && node.LocalName == "attribute")
                        {
                            var name = node.Attributes?["name"]?.Value;
                            if (name == "XSLT-skriptnavn")
                            {
                                var value = node.Attributes["fixed"]?.Value;
                                if (value == "SERES_XSD_GEN")
                                {
                                    isSeresSchema = true;
                                    //seresSchemas.Add((path, schema));
                                }
                            }
                        }
                    }
                }
            }

            return isSeresSchema;
        }

        internal class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
