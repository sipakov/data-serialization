using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using data_serialization.Interfaceses;
using data_serialization.Models;

namespace data_serialization.Implementations
{
    public class XmlToJsonConverter : IXmlToJsonConverter
    {
        public async Task<bool> Convert(string pathToTargetXml, string pathToOutputJson)
        {
            if (string.IsNullOrEmpty(pathToTargetXml))
                throw new ArgumentException("Value cannot be null or empty.", nameof(pathToTargetXml));
            if (string.IsNullOrEmpty(pathToOutputJson))
                throw new ArgumentException("Value cannot be null or empty.", nameof(pathToOutputJson));

            await using var fileStreamReader = File.OpenRead(pathToTargetXml);
            await using var fileStreamWriter = new FileStream(pathToOutputJson, FileMode.Create);

            var settings = new XmlReaderSettings {Async = true};
            using var xmlReader = XmlReader.Create(fileStreamReader, settings);

            var result = await TranslateAsync(fileStreamWriter, xmlReader);
            return result;
        }

        private static async Task<bool> TranslateAsync(Stream fileStreamWriter, XmlReader xmlReader)
        {
            if (fileStreamWriter == null) throw new ArgumentNullException(nameof(fileStreamWriter));
            if (xmlReader == null) throw new ArgumentNullException(nameof(xmlReader));

            await WriteToFileAsync(fileStreamWriter, Constants.SquareOpenBracket);

            var commaNeeded = false;
            while (await xmlReader.ReadAsync())
            {
                if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "ProductOccurence") continue;

                if (commaNeeded)
                {
                    await WriteToFileAsync(fileStreamWriter, Constants.Comma);
                    commaNeeded = false;
                }

                await WriteToFileAsync(fileStreamWriter, Constants.NewString);

                if (!(XNode.ReadFrom(xmlReader) is XElement xElement)) continue;

                await WriteToFileAsync(fileStreamWriter, Constants.BraceOpen);
                var xAttributes = xElement.Attributes();

                var targetAttributesToWrite = GetAttributesToWrite(xAttributes);

                foreach (var targetAttributeToWrite in targetAttributesToWrite)
                {
                    await WriteToFileAsync(fileStreamWriter, Constants.NewString);
                    await WriteToFileAsync(fileStreamWriter, targetAttributeToWrite);
                    await WriteToFileAsync(fileStreamWriter, Constants.Comma);
                }

                await WriteToFileAsync(fileStreamWriter, Constants.NewString);
                await WriteToFileAsync(fileStreamWriter, $"{Constants.Quotas}Props{Constants.Quotas}: {Constants.SquareOpenBracket}");

                var xElementAttributes = xElement.Element("Attributes");
                if (xElementAttributes != null)
                {
                    var xElementAttrs = xElementAttributes.Elements("Attr");

                    var commaNeededToAttributes = false;
                    foreach (var xElementAttr in xElementAttrs)
                    {
                        if (commaNeededToAttributes)
                            await WriteToFileAsync(fileStreamWriter, Constants.Comma);

                        await WriteToFileAsync(fileStreamWriter, Constants.NewString);

                        await WriteToFileAsync(fileStreamWriter, Constants.BraceOpen);

                        xAttributes = xElementAttr.Attributes();
                        targetAttributesToWrite = GetAttributesToWrite(xAttributes);

                        var commaNeededToAttrs = false;
                        foreach (var targetAttributeToWrite in targetAttributesToWrite)
                        {
                            if (commaNeededToAttrs)
                                await WriteToFileAsync(fileStreamWriter, Constants.Comma);

                            await WriteToFileAsync(fileStreamWriter, Constants.NewString);
                            await WriteToFileAsync(fileStreamWriter, targetAttributeToWrite);
                            commaNeededToAttrs = true;
                        }

                        await WriteToFileAsync(fileStreamWriter, Constants.NewString);
                        await WriteToFileAsync(fileStreamWriter, Constants.BraceClose);

                        commaNeededToAttributes = true;
                    }

                    await WriteToFileAsync(fileStreamWriter, Constants.NewString);
                }

                await WriteToFileAsync(fileStreamWriter, Constants.SquareCloseBracket);
                await WriteToFileAsync(fileStreamWriter, Constants.NewString);
                await WriteToFileAsync(fileStreamWriter, Constants.BraceClose);
                commaNeeded = true;
            }

            await WriteToFileAsync(fileStreamWriter, Constants.NewString);
            await WriteToFileAsync(fileStreamWriter, Constants.SquareCloseBracket);

            return true;
        }

        private static async Task WriteToFileAsync(Stream fileStreamWriter, string targetString)
        {
            var targetBytesToWrite = Encoding.UTF8.GetBytes(targetString);

            await fileStreamWriter.WriteAsync(targetBytesToWrite, 0, targetBytesToWrite.Length);
        }

        private static IEnumerable<string> GetAttributesToWrite(IEnumerable<XAttribute> attributes)
        {
            foreach (var xAttribute in attributes)
            {
                var checkedValue = GetCheckedForSpecialCharactersValue(xAttribute.Value);
                yield return $"{Constants.Quotas}{xAttribute.Name}{Constants.Quotas}: {Constants.Quotas}{checkedValue}{Constants.Quotas}";
            }
        }

        private static string GetCheckedForSpecialCharactersValue(string targetValue)
        {
            var resultValue = targetValue;
            foreach (var specialChar in Constants.SpecialChars)
            {
                if (!targetValue.Contains(specialChar)) continue;
                var stringArray = resultValue.Split(specialChar);
                resultValue = string.Empty;
                for (var i = 0; i < stringArray.Length; i++)
                {
                    if (i < stringArray.Length - 1)
                    {
                        resultValue += stringArray[i] + $"\\{specialChar}";
                    }
                    else
                    {
                        resultValue += stringArray[i];
                    }
                }
            }

            return resultValue;
        }
    }
}