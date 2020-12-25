using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace DOS_Generator.WPF.Services
{
    public static class DataProtectionService
    {
        #region Properties

        private static readonly byte[] AdditionalEntropy = { 9, 8, 7, 6, 5 };
        #endregion

        #region Constructors

        #endregion

        #region Functions

        public static byte[] EncodeString(string value)
        {
            if(string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value), "The passed value can't be null");
            return Encoding.UTF8.GetBytes(value);
        }

        public static string DecodeString(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "The passed value can't be null");
            return Encoding.UTF8.GetString(value);
        }

        public static SecureString DecodeSecureString(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "The passed value can't be null");

            var password = Encoding.UTF8.GetString(value);

            var securePassword = new SecureString();

            foreach (var c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }

        public static byte[] Protect(byte[] data)
        {
            try
            {
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                // only by the same current user.
                return ProtectedData.Protect(data, AdditionalEntropy, DataProtectionScope.CurrentUser);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static byte[] Unprotect(byte[] data)
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

        #endregion

        #region Events

        #endregion

        #region Commands

        #endregion
    }
}