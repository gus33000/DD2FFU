// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.XsdValidator
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class XsdValidator
    {
        private readonly List<string> _errorMessages = new List<string>();
        private bool _fileIsValid = true;
        private IULogger _logger;

        public void ValidateXsd(string xsdFile, string xmlFile, IULogger logger)
        {
            if (!LongPathFile.Exists(xmlFile))
                throw new XsdValidatorException("ToolsCommon!XsdValidator::ValidateXsd: XML file was not found: " +
                                                xmlFile);
            using (var fileStream = LongPathFile.OpenRead(xmlFile))
            {
                var name = string.Empty;
                var executingAssembly = Assembly.GetExecutingAssembly();
                foreach (var manifestResourceName in executingAssembly.GetManifestResourceNames())
                    if (manifestResourceName.Contains(xsdFile))
                    {
                        name = manifestResourceName;
                        break;
                    }

                if (string.IsNullOrEmpty(name))
                    throw new XsdValidatorException(
                        "ToolsCommon!XsdValidator::ValidateXsd: XSD resource was not found: " + xsdFile);
                using (var manifestResourceStream = executingAssembly.GetManifestResourceStream(name))
                {
                    ValidateXsd(manifestResourceStream, fileStream, xmlFile, logger);
                }
            }
        }

        public void ValidateXsd(Stream xsdStream, string xmlFile, IULogger logger)
        {
            if (!LongPathFile.Exists(xmlFile))
                throw new XsdValidatorException("ToolsCommon!XsdValidator::ValidateXsd: XML file was not found: " +
                                                xmlFile);
            using (var fileStream = LongPathFile.OpenRead(xmlFile))
            {
                ValidateXsd(xsdStream, fileStream, xmlFile, logger);
            }
        }

        public void ValidateXsd(Stream xsdStream, Stream xmlStream, string xmlName, IULogger logger)
        {
            _logger = logger;
            _fileIsValid = true;
            if (xsdStream == null)
                throw new XsdValidatorException(
                    "ToolsCommon!XsdValidator::ValidateXsd: Failed to load the embeded schema file for xml: " +
                    xmlName);
            XmlDocument xmlDocument;
            try
            {
                var schema = (XmlSchema) null;
                using (var reader = XmlReader.Create(xsdStream))
                {
                    schema = XmlSchema.Read(reader, ValidationHandler);
                }

                xmlDocument = new XmlDocument();
                xmlDocument.Schemas.Add(schema);
            }
            catch (XmlSchemaException ex)
            {
                throw new XsdValidatorException(
                    "ToolsCommon!XsdValidator::ValidateXsd: Unable to use the schema provided for xml: " + xmlName, ex);
            }

            try
            {
                xmlDocument.Load(xmlStream);
                xmlDocument.Validate(ValidationHandler);
            }
            catch (Exception ex)
            {
                throw new XsdValidatorException(
                    "ToolsCommon!XsdValidator::ValidateXsd: There was a problem validating the XML file " + xmlName,
                    ex);
            }

            if (!_fileIsValid)
            {
                var message = string.Format(CultureInfo.InvariantCulture,
                    "ToolsCommon!XsdValidator::ValidateXsd: Validation of {0} failed", new object[1]
                    {
                        xmlName
                    });
                foreach (var errorMessage in _errorMessages)
                    message = message + Environment.NewLine + "\tError : " + errorMessage;
                throw new XsdValidatorException(message);
            }
        }

        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            var format = string.Format(CultureInfo.InvariantCulture,
                "\nToolsCommon!XsdValidator::ValidateXsd: XML Validation {0}: {1}", new object[2]
                {
                    args.Severity,
                    args.Message
                });
            if (args.Severity == XmlSeverityType.Error)
            {
                if (_logger != null)
                    _logger.LogError(format);
                _errorMessages.Add(format);
                _fileIsValid = false;
            }
            else
            {
                if (_logger == null)
                    return;
                _logger.LogWarning(format);
            }
        }
    }
}