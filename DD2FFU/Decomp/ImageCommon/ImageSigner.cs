// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.ImageSigner
// Assembly: ImageCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: E18B3E30-3683-4CE0-B9AC-BA2B871D9398
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ImageCommon.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    public class ImageSigner
    {
        public const string ProdCertRootThumbprint = "3B1EFD3A66EA28B16697394703A72CA340A05BD5";
        public const string TestCertRootThumbprint = "8A334AA8052DD244A647306A76B8178FA215F344";
        public const string FlightCertPCAThumbprint = "9E594333273339A97051B0F82E86F266B917EDB3";
        public const string FlightCertWindowsThumbprint = "5f444a6740b7ca2434c7a5925222c2339ee0f1b7";
        private static readonly Dictionary<string, bool> certPublicKeys = new Dictionary<string, bool>();
        private string _catalogFileName;
        private FullFlashUpdateImage _ffuImage;
        private IULogger _logger;
        private readonly SHA256 _sha256;

        public ImageSigner()
        {
            _sha256 = new SHA256CryptoServiceProvider();
        }

        public void Initialize(FullFlashUpdateImage ffuImage, string catalogFile, IULogger logger)
        {
            _logger = logger;
            if (logger == null)
                _logger = new IULogger();
            _ffuImage = ffuImage;
            _catalogFileName = catalogFile;
        }

        public void SignFFUImage()
        {
            if (_ffuImage == null)
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::SignFFUImage: ImageSigner has not been initialized.");
            if (!File.Exists(Environment.ExpandEnvironmentVariables(_catalogFileName)))
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::SignFFUImage: Unable to generate signed image - missing Catalog file: " +
                    _catalogFileName);
            if (!IsCatalogFile(IntPtr.Zero, _catalogFileName))
                throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: The file '" + _catalogFileName +
                                               "' is not a catalog file.");
            if (!HasSignature(_catalogFileName, true))
                throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage:  The file '" + _catalogFileName +
                                               "' is not signed.");
            try
            {
                if (!VerifyCatalogData(_catalogFileName, _ffuImage.HashTableData))
                    throw new ImageCommonException(
                        "ImageCommon!ImageSigner::SignFFUImage: The catalog provided does not match the image.");
                _ffuImage.CatalogData = File.ReadAllBytes(_catalogFileName);
            }
            catch (ImageCommonException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("ImageCommon!ImageSigner::SignFFUImage: Error while signing FFU image: {0}",
                    (object) ex.Message);
                throw new ImageCommonException("ImageCommon!ImageSigner::SignFFUImage: Exception occurred.", ex);
            }
        }

        public void VerifyCatalog()
        {
            if (_ffuImage == null)
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::VerifyCatalog: ImageSigner has not been initialized.");
            if (_ffuImage.CatalogData == null || _ffuImage.CatalogData.Length == 0)
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::VerifyCatalog: The FFU does not contain a catalog.");
            if (!VerifyCatalogData(_ffuImage.CatalogData, _ffuImage.HashTableData))
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::VerifyCatalog: The Catalog in the image does not match the Hash Table in the image.  The image appears to be corrupt or modified outside ImageApp.");
            if (!VerifyHashTable())
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::VerifyCatalog: The Hash Table in the image does not match the payload.  The image appears to be corrupt or modified outside ImageApp.");
        }

        private bool VerifyCatalogData(byte[] catalogData, byte[] hashTableData)
        {
            _logger.LogInfo("ImageCommon: Verfiying Hash Table against catalog...");
            var tempFileName = Path.GetTempFileName();
            File.WriteAllBytes(tempFileName, catalogData);
            var flag = VerifyCatalogData(tempFileName, hashTableData);
            File.Delete(tempFileName);
            return flag;
        }

        public static bool VerifyCatalogData(string catalogFile, byte[] hashTableData)
        {
            var shA1Managed = new SHA1Managed();
            var catalogHash = GetCatalogHash(catalogFile);
            var hash = shA1Managed.ComputeHash(hashTableData);
            if (catalogHash.Length != hash.Length)
                return false;
            for (var index = 0; index < hash.Length; ++index)
                if (catalogHash[index] != hash[index])
                    return false;
            return true;
        }

        internal bool VerifyHashTable()
        {
            var index1 = 0;
            var num1 = 0;
            if (_ffuImage == null)
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::VerifyHashTable: ImageSigner has not been initialized.");
            _logger.LogInfo("ImageCommon: Verfiying Hash Table entries...");
            _logger.LogInfo("ImageCommon: Using Chunksize: {0}KB", (object) _ffuImage.ChunkSize);
            try
            {
                var hashTableData = _ffuImage.HashTableData;
                using (var imageStream = _ffuImage.GetImageStream())
                {
                    imageStream.Position = _ffuImage.StartOfImageHeader;
                    var numArray = GetFirstChunkHash(imageStream);
                    var num2 = num1 + 1;
                    while (numArray != null)
                    {
                        for (var index2 = 0; index2 < numArray.Length; ++index2)
                        {
                            if (index1 > hashTableData.Length)
                                throw new ImageCommonException(
                                    "ImageCommon!ImageSigner::VerifyHashTable: Hash Table too small for this FFU.");
                            if (numArray[index2] != hashTableData[index1])
                            {
                                _logger.LogInfo(
                                    "ImageCommon!ImageSigner::VerifyHashTable: Failed to match Chunk {0} Hash value [{1}]: {2} with {3}",
                                    (object) num2, (object) index2, (object) numArray[index2].ToString("X2"),
                                    (object) hashTableData[index1].ToString("X2"));
                                throw new ImageCommonException(
                                    "ImageCommon!ImageSigner::VerifyHashTable: Hash Table entry does not match hash of FFU.");
                            }

                            ++index1;
                        }

                        numArray = GetNextChunkHash(imageStream);
                        ++num2;
                    }
                }

                _logger.LogInfo("ImageCommon: The Hash Table has been sucessfully verified..");
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::VerifyHashTable: Error while retrieving Hash Table from FFU", ex);
            }

            return true;
        }

        public static byte[] GenerateCatalogFile(byte[] hashData)
        {
            var tempFileName1 = Path.GetTempFileName();
            var tempFileName2 = Path.GetTempFileName();
            var tempFileName3 = Path.GetTempFileName();
            File.WriteAllBytes(tempFileName3, hashData);
            using (var streamWriter = new StreamWriter(tempFileName2))
            {
                streamWriter.WriteLine("[CatalogHeader]");
                streamWriter.WriteLine("Name={0}", tempFileName1);
                streamWriter.WriteLine("[CatalogFiles]");
                streamWriter.WriteLine("{0}={1}", "HashTable.blob", tempFileName3);
            }

            using (var process = new Process())
            {
                process.StartInfo.FileName = "MakeCat.exe";
                process.StartInfo.Arguments = string.Format("\"{0}\"", tempFileName2);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                try
                {
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("CDF File: {0}\n", tempFileName2);
                    if (!File.Exists(tempFileName2))
                        stringBuilder.AppendFormat("CDF File could not be found.\n");
                    try
                    {
                        stringBuilder.AppendFormat("Arguments : {0}\n", process.StartInfo.Arguments);
                    }
                    catch
                    {
                    }

                    try
                    {
                        stringBuilder.AppendFormat("StandardError : {0}\n", process.StandardError);
                    }
                    catch
                    {
                    }

                    try
                    {
                        stringBuilder.AppendFormat("StandardOutput : {0}\n", process.StandardOutput);
                    }
                    catch
                    {
                    }

                    throw new ImageCommonException(stringBuilder.ToString(), ex);
                }

                if (process.ExitCode != 0)
                    throw new ImageCommonException(
                        "ImageCommon!ImageSigner::GenerateCatalogFile: Failed call to MakeCat.");
            }

            var numArray = File.ReadAllBytes(tempFileName1);
            File.Delete(tempFileName1);
            File.Delete(tempFileName3);
            File.Delete(tempFileName2);
            return numArray;
        }

        private uint GetSecurityDataSize()
        {
            var getSecureHeader = _ffuImage.GetSecureHeader;
            var byteCount = (int) getSecureHeader.ByteCount;
            getSecureHeader = _ffuImage.GetSecureHeader;
            var catalogSize = (int) getSecureHeader.CatalogSize;
            var num = byteCount + catalogSize;
            getSecureHeader = _ffuImage.GetSecureHeader;
            var hashTableSize = (int) getSecureHeader.HashTableSize;
            return (uint) (num + hashTableSize) + _ffuImage.SecurityPadding;
        }

        private byte[] GetFirstChunkHash(Stream stream)
        {
            stream.Position = GetSecurityDataSize();
            return GetNextChunkHash(stream);
        }

        private byte[] GetNextChunkHash(Stream stream)
        {
            var buffer = new byte[(int) _ffuImage.ChunkSizeInBytes];
            if (stream.Position == stream.Length)
                return null;
            stream.Read(buffer, 0, buffer.Length);
            return _sha256.ComputeHash(buffer);
        }

        public static bool HasSignature(string filename, bool EnsureMicrosoftIssuer)
        {
            var flag = false;
            try
            {
                var certificate = new X509Certificate2(filename);
                if (EnsureMicrosoftIssuer)
                {
                    if (!certPublicKeys.TryGetValue(certificate.Thumbprint, out flag))
                    {
                        var x509Chain = new X509Chain(true);
                        x509Chain.Build(certificate);
                        var ignoreCase = true;
                        foreach (var chainElement in x509Chain.ChainElements)
                            if (string.Compare("3B1EFD3A66EA28B16697394703A72CA340A05BD5",
                                    chainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) ==
                                0 || string.Compare("9E594333273339A97051B0F82E86F266B917EDB3",
                                    chainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) ==
                                0 || string.Compare("5f444a6740b7ca2434c7a5925222c2339ee0f1b7",
                                    chainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) ==
                                0 || string.Compare("8A334AA8052DD244A647306A76B8178FA215F344",
                                    chainElement.Certificate.Thumbprint, ignoreCase, CultureInfo.InvariantCulture) == 0)
                            {
                                flag = true;
                                break;
                            }

                        foreach (var chainElement in x509Chain.ChainElements)
                            certPublicKeys[chainElement.Certificate.Thumbprint] = flag;
                    }
                }
                else
                {
                    flag = certificate != null && !string.IsNullOrEmpty(certificate.Subject);
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        public static bool HasValidSignature(string filename, List<string> validRootThumbprints)
        {
            var flag = false;
            try
            {
                var certificate = new X509Certificate2(filename);
                var x509Chain = new X509Chain(true);
                x509Chain.Build(certificate);
                foreach (var chainElement in x509Chain.ChainElements)
                {
                    var element = chainElement;
                    if (validRootThumbprints.Any(tp =>
                        tp.Equals(element.Certificate.Thumbprint, StringComparison.OrdinalIgnoreCase)))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        public static string GetSignatureIssuer(string filename)
        {
            var str = "File not found";
            if (File.Exists(filename))
            {
                str = "Not signed";
                var x509Certificate2 = (X509Certificate2) null;
                try
                {
                    x509Certificate2 = new X509Certificate2(filename);
                }
                catch
                {
                }

                if (x509Certificate2 != null)
                    str = x509Certificate2.Subject;
            }

            return str;
        }

        [DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CryptCATOpen(string pwszFileName, uint fdwOpenFlags, IntPtr hProv,
            uint dwPublicVersion, uint dwEncodingType);

        [DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CryptCATClose(IntPtr hCatalog);

        [DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CryptCATEnumerateMember(IntPtr hCatalog, IntPtr pPrevMember);

        [DllImport("WinTrust.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsCatalogFile(IntPtr hFile, string pwszFileName);

        internal static byte[] GetCatalogHash(string catalogFile)
        {
            var num = new IntPtr(-1);
            var hCatalog = num;
            var zero = IntPtr.Zero;
            var destination = (byte[]) null;
            try
            {
                hCatalog = CryptCATOpen(catalogFile, 2U, IntPtr.Zero, 0U, 0U);
                var ptr = CryptCATEnumerateMember(hCatalog, IntPtr.Zero);
                if (ptr == IntPtr.Zero)
                    throw new ImageCommonException(
                        "ImageCommon!ImageSigner::GetCatalogHash: Failed to get the Hash Table Hash from the Catalog '" +
                        catalogFile + "'.  The catalog appears to be corrupt.");
                var structure = (CRYPTCATMEMBER) Marshal.PtrToStructure(ptr, typeof(CRYPTCATMEMBER));
                destination = new byte[20];
                Marshal.Copy(
                    IntPtr.Add(structure.sEncodedIndirectData.pbData,
                        (int) structure.sEncodedIndirectData.cbData - destination.Length), destination, 0,
                    destination.Length);
            }
            catch (Exception ex)
            {
                throw new ImageCommonException(
                    "ImageCommon!ImageSigner::GetCatalogHash: Failed to get the Hash Table Hash from the Catalog: " +
                    ex.Message);
            }
            finally
            {
                if (hCatalog != num)
                    CryptCATClose(hCatalog);
            }

            return destination;
        }

        
        public struct CRYPT_ATTR_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }

        
        public struct CRYPTCATMEMBER
        {
            private uint cbStruct;
            [MarshalAs(UnmanagedType.LPWStr)] private string pwszReferenceTag;
            [MarshalAs(UnmanagedType.LPWStr)] private string pwszFileName;
            private Guid gSubjectType;
            private uint fdwMemberFlags;
            private IntPtr pIndirectData;
            private uint dwCertVersion;
            private uint dwReserved;
            private IntPtr hReserved;
            public CRYPT_ATTR_BLOB sEncodedIndirectData;
            private CRYPT_ATTR_BLOB sEncodedMemberInfo;
        }
    }
}