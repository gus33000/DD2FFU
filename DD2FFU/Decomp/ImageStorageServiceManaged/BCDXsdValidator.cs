// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BCDXsdValidator
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BCDXsdValidator
    {
        private bool _fileIsValid = true;
        private IULogger _logger;


        public void ValidateXsd(Stream bcdSchemaStream, string xmlFile, IULogger logger)
        {
            if (!File.Exists(xmlFile))
            {
                throw new BCDXsdValidatorException(
                                "ImageServices!BCDXsdValidator::ValidateXsd: XML file was not found: " + xmlFile);
            }

            _logger = logger;
            _fileIsValid = true;
            XmlReaderSettings settings = new();
            settings.ValidationEventHandler += ValidationHandler;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
            try
            {
                using XmlReader reader = XmlReader.Create(bcdSchemaStream);
                XmlSchema schema = XmlSchema.Read(reader, ValidationHandler);
                _ = settings.Schemas.Add(schema);
            }
            catch (XmlSchemaException ex)
            {
                throw new BCDXsdValidatorException(
                    "ImageServices!BCDXsdValidator::ValidateXsd: Unable to use the schema provided", ex);
            }

            XmlTextReader xmlTextReader = null;
            XmlReader xmlReader = null;
            try
            {
                try
                {
                    xmlTextReader = new XmlTextReader(xmlFile);
                }
                catch (Exception ex)
                {
                    throw new BCDXsdValidatorException(
                        "ImageServices!BCDXsdValidator::ValidateXsd: Unable to access the given XML file", ex);
                }

                try
                {
                    xmlReader = XmlReader.Create(xmlTextReader, settings);
                    do
                    {
                        ;
                    } while (xmlReader.Read());
                }
                catch (XmlException ex)
                {
                    throw new BCDXsdValidatorException(
                        "ImageServices!BCDXsdValidator::ValidateXsd: There was a problem validating the XML file", ex);
                }
            }
            finally
            {
                xmlReader?.Close();
                xmlTextReader?.Close();
            }

            if (!_fileIsValid)
            {
                throw new BCDXsdValidatorException(string.Format(CultureInfo.InvariantCulture,
                                "ImageServices!BCDXsdValidator::ValidateXsd: Validation of {0} failed", new object[1]
                                {
                        xmlFile
                                }));
            }
        }

        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            string format = string.Format(CultureInfo.InvariantCulture,
                "\nImageServices!BCDXsdValidator::ValidateXsd: XML Validation {0}: {1}", new object[2]
                {
                    args.Severity,
                    args.Message
                });
            if (args.Severity == XmlSeverityType.Error)
            {
                _logger?.LogError(format);
                _fileIsValid = false;
            }
            else
            {
                if (_logger == null)
                {
                    return;
                }

                _logger.LogWarning(format);
            }
        }
    }
}