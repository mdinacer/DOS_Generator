using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using DeviceId;

namespace DOS_Generator.License
{
    public static class LicenseService
    {
        #region Properties

        private static readonly byte[] AdditionalEntropy = {9, 8, 7, 6, 5};

        #endregion

        #region Constructors

        #endregion

        #region Functions

        private static byte[] Protect(byte[] data)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                // only by the same current user.
                return ProtectedData.Protect(data, AdditionalEntropy, DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private static byte[] Unprotect(byte[] data)
        {
            try
            {
                //Decrypt the data using DataProtectionScope.CurrentUser.
                return ProtectedData.Unprotect(data, AdditionalEntropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not decrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static License ReadLicenseFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;

            var protectedFile = File.ReadAllBytes(path);
            var data = Unprotect(protectedFile);
            using (var stream = new MemoryStream(data))
            {
                var serializer = new XmlSerializer(typeof(License));
                var deserialized = serializer.Deserialize(stream);
                if (deserialized is License license) return license;
                return null;
            }
        }

        public static void WriteLicenseToFile(string path, License license)
        {
            var serializer = new XmlSerializer(typeof(License));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, license);
                var protectedData = Protect(stream.ToArray());

                using (var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.Write(protectedData);
                }
            }
        }

        public static bool CheckLicenseValidity(License license)
        {
            if (license == null)
                return false;

            var actualId = GetId();

            var isValidLicenseFile = actualId.Equals(license.MachineId);
            var isValidLicenseType = license.Type == LicenseType.Registered 
                                  || (license.Type == LicenseType.Trial 
                                      && license.ExpiryDate != null 
                                      && license.ExpiryDate.Value > DateTime.Now);




            return isValidLicenseFile && isValidLicenseType;
        }

        private static string GetId()
        {
            return new DeviceIdBuilder()
                .AddMachineName()
                .AddMacAddress()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .ToString();
        }

        public static void CreateFakeLicense()
        {
            var path = @".\license.lic";
            var license = new License
            {
                MachineId = GetId(),
                PurchaseDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                LicensedTo = "Mohammedi Nacer",
                Type = LicenseType.Trial
            };

            WriteLicenseToFile(path, license);
        }

        #endregion
    }
}