using System.IO;
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
using Xunit;
using Xunit.Abstractions;
using XmlSchemaValidator = DataModeling.Tests.TestHelpers.XmlSchemaValidator;

namespace DataModeling.Tests
{
    public class Seres2JsonSchema2SeresTests : FluentTestsBase<Seres2JsonSchema2SeresTests>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private XmlSchema OriginalXsdSchema { get; set; }

        private JsonSchema ConvertedJsonSchema { get; set; }

        private XmlSchema ConvertedXsdSchema { get; set; }

        private string OriginalXsdSchemaString { get; set; }

        private string ConvertedJsonSchemaString { get; set; }

        private string ConvertedXsdSchemaString { get; set; }

        public Seres2JsonSchema2SeresTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("Seres/HvemErHvem.xsd", "Seres/HvemErHvem.xml")]
        [InlineData("Model/XmlSchema/Seres/SeresNillable.xsd", "")]
        [InlineData("Seres/schema_3473_201512_forms_3123_37927.xsd", "")]
        [InlineData("Seres/schema_4008_180226_forms_4186_37199.xsd", "")]
        [InlineData("Seres/schema_3919_2_forms_4623_39043.xsd", "")]
        [InlineData("Seres/schema_4741_4280_forms_5273_41269.xsd", "")]
        [InlineData("Seres/schema_4830_4000_forms_5524_41951.xsd", "")]
        [InlineData("Seres/schema_5222_2_forms_5909_43507.xsd", "")]
        [InlineData("Seres/schema_4532_1_forms_5274_41065.xsd", "")]
        [InlineData("Seres/schema_4527_11500_forms_5273_41269.xsd", "")]
        [InlineData("Seres/schema_4582_2000_forms_5244_42360.xsd", "")]
        [InlineData("Seres/schema_5064_1_forms_5793_42882.xsd", "")]
        [InlineData("Seres/schema_5259_1_forms_9999_50000.xsd", "")]
        [InlineData("Seres/schema_4956_1_forms_5692_42617.xsd", "")]
        [InlineData("Seres/schema_4660_1_forms_2500_2500.xsd", "")]
        public void ConvertSeresXsd_SeresGeneratedXsd_ShouldConvertToJsonSchemaAndBackToXsd(string xsdSchemaPath, string xmlPath)
        {
            Given.That.XsdSchemaLoaded(xsdSchemaPath)
                .And.JsonSchemaKeywordsRegistered()
                .When.LoadedXsdSchemaConvertedToJsonSchema()
                .And.When.ConvertedJsonSchemaConvertedToXsdSchema()
                .Then.OriginalAndConvertedXsdSchemasShouldBeEquivalent()
                .And.XmlShouldBeValidWithOriginalAndConvertedSchema(xmlPath);
        }

        [Theory]

        // // Without issues:
        // // Sequence instead attribute. Added minOccurs=0 in converted and not present in original, nillable attribue removed in converted, seres:elementtype and seres:guid removed
        // [InlineData("Failing/nsm__6244-47141.xsd")]
        //
        // // targetNamespace mess up the schema? or something else?
        // [InlineData("Failing/fd__3320-44007.xsd")]
        // [InlineData("Failing/fd__6107-46232.xsd")]
        // [InlineData("Failing/spk__3374-46279.xsd")]
        //
        // // Empty sequence budsjett failing
        // [InlineData("Failing/hdir__3431-39110.xsd")]
        //
        // // Date simple restriction
        // // https://github.com/Altinn/altinn-studio/issues/8981
        // [InlineData("Failing/brg__RR-0200 Mellombalanse_M_2020-05-18_6301_45717_SERES.xsd")]
        // [InlineData("Failing/brg__1266-42897.xsd")]
        // [InlineData("Failing/brg__1266-43710.xsd")]
        // [InlineData("Failing/brg__1266-44775.xsd")]
        // [InlineData("Failing/brg__3106-39629.xsd")]
        // [InlineData("Failing/brg__3124-39627.xsd")]
        // [InlineData("Failing/brg__3228-39613.xsd")]
        // [InlineData("Failing/brg__3238-39623.xsd")]
        // [InlineData("Failing/brg__3373-36491.xsd")]
        // [InlineData("Failing/brg__3428-39614.xsd")]
        // [InlineData("Failing/brg__3430-39615.xsd")]
        // [InlineData("Failing/brg__4213-39628.xsd")]
        // [InlineData("Failing/brg__6199-44481.xsd")]
        // [InlineData("Failing/brg__6301-45717.xsd")]
        //
        // // Complex type definition gets lost
        // // https://github.com/Altinn/altinn-studio/issues/8978
        // [InlineData("Failing/brg__5202-41077.xsd")]
        // [InlineData("Failing/krt__3443-44403.xsd")]
        // [InlineData("Failing/krt__4702-44390.xsd")]
        // [InlineData("Failing/krt__4943-44355.xsd")]
        // [InlineData("Failing/sfd__6960-46467.xsd")]
        // [InlineData("Failing/slv__6110-44436.xsd")]
        // [InlineData("Failing/ssb__4949-43795.xsd")]
        // [InlineData("Failing/ssb__5272-41450.xsd")]
        // [InlineData("Failing/fd__3303-43421.xsd")]
        //
        // // seres:beskrivelse
        // // https://github.com/Altinn/altinn-studio/issues/8976
        // [InlineData("Failing/ssb__4108-41505.xsd")]
        // [InlineData("Failing/ssb__4257-41169.xsd")]
        // [InlineData("Failing/ssb__4265-41400.xsd")]
        // [InlineData("Failing/ssb__4306-40093.xsd")]
        // [InlineData("Failing/ssb__4465-41280.xsd")]
        // [InlineData("Failing/ssb__6823-46701.xsd")]
        //
        // // minLength and maxLength merged to length
        // // https://github.com/Altinn/altinn-studio/issues/8973 allso missing complex type
        // // https://github.com/Altinn/altinn-studio/issues/8978
        // [InlineData("Failing/krt__4710-45001.xsd")]
        // [InlineData("Failing/krt__4765-44992.xsd")]
        //
        // // minOccurs = 0, maxOccures="unbounded" and type defined. removing type
        // // https://github.com/Altinn/altinn-studio/issues/8965
        // [InlineData("Failing/brg__4687-44089.xsd")]
        // [InlineData("Failing/fd__3365-36837.xsd")]
        // [InlineData("Failing/fd__4388-39288.xsd")]
        // [InlineData("Failing/hmrhf__5460-34460.xsd")]
        // [InlineData("Failing/hmrhf__5660-34542.xsd")]
        // [InlineData("Failing/ok__5262-41499.xsd")]
        // [InlineData("Failing/sfd__4826-43294.xsd")]
        // [InlineData("Failing/slv__5404-41299.xsd")]
        // [InlineData("Failing/srf__6947-46327.xsd")]
        // [InlineData("Failing/tad__6085-44147.xsd")]
        // [InlineData("Failing/ttd__6244-46571.xsd")]
        //
        // // Not only root element
        // // https://github.com/Altinn/altinn-studio/issues/8970
        // [InlineData("Failing/ssb__3101-35865.xsd")]
        // [InlineData("Failing/ssb__3804-37700.xsd")]
        // [InlineData("Failing/ssb__3825-37785.xsd")]
        // [InlineData("Failing/ssb__3983-37382.xsd")]
        //
        // // Simple type restriction ref
        // // https://github.com/Altinn/altinn-studio/issues/8982
        // [InlineData("Failing/skd__4401-32883.xsd")]

        public void FailingSchemas(string xsdSchemaPath)
        {
            Given.That.XsdSchemaLoaded(xsdSchemaPath)
                .And.JsonSchemaKeywordsRegistered()
                .When.LoadedXsdSchemaConvertedToJsonSchema()
                .And.When.ConvertedJsonSchemaConvertedToXsdSchema()
                .And.SchemasSerializedToString()
                .Then.OriginalAndConvertedXsdSchemasShouldBeEquivalent();
        }

        private bool ValidateXml(XmlSchema xmlSchema, string xml)
        {
            var xmlSchemaValidator = new XmlSchemaValidator(xmlSchema);

            var validXml = xmlSchemaValidator.Validate(xml);
            if (!validXml)
            {
                xmlSchemaValidator.ValidationErrors.ForEach(e => _testOutputHelper.WriteLine(e.Message));
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

        internal class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        // Fluent methods for test
        private Seres2JsonSchema2SeresTests JsonSchemaKeywordsRegistered()
        {
            JsonSchemaKeywords.RegisterXsdKeywords();
            return this;
        }

        private Seres2JsonSchema2SeresTests XsdSchemaLoaded(string xsdSchemaPath)
        {
            OriginalXsdSchema = ResourceHelpers.LoadXmlSchemaTestData(xsdSchemaPath);
            return this;
        }

        private Seres2JsonSchema2SeresTests LoadedXsdSchemaConvertedToJsonSchema()
        {
            var xsdToJsonConverter = new XmlSchemaToJsonSchemaConverter();
            ConvertedJsonSchema = xsdToJsonConverter.Convert(OriginalXsdSchema);
            return this;
        }

        private Seres2JsonSchema2SeresTests ConvertedJsonSchemaConvertedToXsdSchema()
        {
            var jsonToXsdConverter = new JsonSchemaToXmlSchemaConverter(new JsonSchemaNormalizer());
            ConvertedXsdSchema = jsonToXsdConverter.Convert(ConvertedJsonSchema);
            return this;
        }

        private Seres2JsonSchema2SeresTests SchemasSerializedToString()
        {
            OriginalXsdSchemaString = Serialize(OriginalXsdSchema).Result;
            ConvertedXsdSchemaString = Serialize(ConvertedXsdSchema).Result;
            ConvertedJsonSchemaString = JsonSerializer.Serialize(ConvertedJsonSchema, new JsonSerializerOptions
            {
                Encoder =
                    JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
                WriteIndented = true
            });
            return this;
        }

        // Assertion methods
        private Seres2JsonSchema2SeresTests OriginalAndConvertedXsdSchemasShouldBeEquivalent()
        {
            XmlSchemaAssertions.IsEquivalentTo(OriginalXsdSchema, ConvertedXsdSchema);
            return this;
        }

        private Seres2JsonSchema2SeresTests XmlShouldBeValidWithOriginalAndConvertedSchema(string xmlPath)
        {
            if (!string.IsNullOrEmpty(xmlPath))
            {
                // The XML should validate against both XSD's
                var xml = ResourceHelpers.LoadTestDataAsString(xmlPath);
                Assert.True(ValidateXml(OriginalXsdSchema, xml));
                Assert.True(ValidateXml(ConvertedXsdSchema, xml));
            }

            return this;
        }
    }
}
