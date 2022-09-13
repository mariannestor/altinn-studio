using DataModeling.Console;

var analyzer = new RoundtripConversionAnalyzer();

await analyzer.VerifyAltinn2Xsd();

Console.ReadLine();
