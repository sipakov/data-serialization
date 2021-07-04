using System;
using System.IO;
using System.Threading.Tasks;
using data_serialization.Implementations;
using data_serialization.Interfaceses;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class XmlToJsonConverterTests
    {
        private IXmlToJsonConverter _xmlToJsonConverter;
        [SetUp]
        public void Setup()
        {
            _xmlToJsonConverter = new XmlToJsonConverter();
        }

        [Test]
        public async Task CommonTestThatXmlConvertedToJson()
        {
            // Arrange
            var pathToXml = Path.Combine(Environment.CurrentDirectory, "fileToTest.xml");
            var pathToOutputJson = Path.Combine(Environment.CurrentDirectory, "outputTestFile.json");
            var pathToExpectedJson = Path.Combine(Environment.CurrentDirectory, "expectedJson.json");

            var expectedString = await File.ReadAllTextAsync(pathToExpectedJson);

            // Act
            var _ = await _xmlToJsonConverter.Convert(pathToXml, pathToOutputJson);
            // Assert
            var convertedJsonDocumentInString = await File.ReadAllTextAsync(pathToOutputJson);
            
            Assert.AreEqual(expectedString, convertedJsonDocumentInString);

        }
    }
}