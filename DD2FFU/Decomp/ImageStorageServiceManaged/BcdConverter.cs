// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.BcdConverter
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class BcdConverter
    {
        public static readonly string HiveBase = "HKEY_LOCAL_MACHINE\\BCD";
        public static readonly string Header = "Windows Registry Editor Version 5.00";
        private BcdInput _bcdInput;
        private readonly IULogger _logger;

        
        public BcdConverter(IULogger logger)
        {
            _logger = logger;
        }

        public void ProcessInputXml(string bcdLayoutFile, string bcdLayoutSchema)
        {
            if (!File.Exists(bcdLayoutSchema))
                throw new ImageStorageException(string.Format("{0}: BCD layout schema file is not found: {1}.",
                    MethodBase.GetCurrentMethod().Name, bcdLayoutSchema));
            using (var fileStream = new FileStream(bcdLayoutSchema, FileMode.Open, FileAccess.Read))
            {
                ProcessInputXml(bcdLayoutFile, fileStream);
            }
        }

        public void ProcessInputXml(string bcdLayoutFile, Stream bcdLayoutSchema)
        {
            if (!File.Exists(bcdLayoutFile))
                throw new ImageStorageException(string.Format("{0}: BCD layout file is not found: {1}.",
                    MethodBase.GetCurrentMethod().Name, bcdLayoutFile));
            var bcdXsdValidator = new BCDXsdValidator();
            try
            {
                bcdXsdValidator.ValidateXsd(bcdLayoutSchema, bcdLayoutFile, _logger);
            }
            catch (BCDXsdValidatorException ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Failed to validate the BCD layout file: {1}.",
                        MethodBase.GetCurrentMethod().Name, bcdLayoutFile), ex);
            }

            var fileStream = (FileStream) null;
            var xmlReader = (XmlReader) null;
            var xmlSerializer = (XmlSerializer) null;
            try
            {
                fileStream = new FileStream(bcdLayoutFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                _bcdInput = (BcdInput) new XmlSerializer(typeof(BcdInput)).Deserialize(XmlReader.Create(fileStream));
            }
            catch (SecurityException ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Unable to access the BCD layout file.", MethodBase.GetCurrentMethod().Name),
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Unable to open the BCD layout file for reading.",
                        MethodBase.GetCurrentMethod().Name), ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ImageStorageException(
                    string.Format("{0}: Unable to deserialize the BCD layout file.",
                        MethodBase.GetCurrentMethod().Name), ex);
            }
            finally
            {
                fileStream?.Close();
                xmlSerializer = null;
                xmlReader = null;
            }
        }

        public void SaveToRegFile(Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.Unicode);
            try
            {
                if (_bcdInput.IncludeRegistryHeader)
                {
                    writer.WriteLine("{0}", Header);
                    writer.WriteLine();
                }

                if (_bcdInput.SaveKeyToRegistry)
                {
                    writer.WriteLine("[{0}]", HiveBase);
                    writer.WriteLine();
                }

                if (_bcdInput.IncludeDescriptions)
                {
                    writer.WriteLine("[{0}\\Description]", HiveBase);
                    writer.WriteLine("\"KeyName\"=\"BCD00000000\"");
                    writer.WriteLine("\"System\"=dword:{0:x8}", 1);
                    writer.WriteLine("\"TreatAsSystem\"=dword:{0:x8}", 1);
                    writer.WriteLine();
                }

                _bcdInput.SaveAsRegFile(writer, HiveBase);
            }
            finally
            {
                writer.Flush();
            }
        }

        public void SaveToRegData(BcdRegData bcdRegData)
        {
            if (_bcdInput.SaveKeyToRegistry)
                bcdRegData.AddRegKey(HiveBase);
            if (_bcdInput.IncludeDescriptions)
            {
                var regKey = string.Format("{0}\\Description", HiveBase);
                bcdRegData.AddRegKey(regKey);
                bcdRegData.AddRegValue(regKey, "KeyName", "BCD00000000", "REG_SZ");
                bcdRegData.AddRegValue(regKey, "System", string.Format("{0:x8}", 1), "REG_DWORD");
                bcdRegData.AddRegValue(regKey, "TreatAsSystem", string.Format("{0:x8}", 1), "REG_DWORD");
            }

            _bcdInput.SaveAsRegData(bcdRegData, HiveBase);
        }

        public static void ConvertBCD(string inputFile, string outputFile)
        {
            var bcdConverter = new BcdConverter(new IULogger());
            using (var manifestResourceStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("BcdLayout.xsd"))
            {
                bcdConverter.ProcessInputXml(inputFile, manifestResourceStream);
            }

            using (var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite))
            {
                bcdConverter.SaveToRegFile(fileStream);
            }
        }
    }
}