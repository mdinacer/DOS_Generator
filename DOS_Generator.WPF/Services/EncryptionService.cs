using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace DOS_Generator.WPF.Services
{
    public class EncryptionService
    {
        public static void CryptStream(string password,
            Stream inStream, Stream outStream, bool encrypt)
        {
            // Make an AES service provider.
            var aesProvider =
                new AesCryptoServiceProvider();

            // Find a valid key size for this provider.
            var keySizeBits = 0;
            for (var i = 1024; i > 1; i--)
                if (aesProvider.ValidKeySize(i))
                {
                    keySizeBits = i;
                    break;
                }

            Debug.Assert(keySizeBits > 0);
            Console.WriteLine("Key size: " + keySizeBits);

            // Get the block size for this provider.
            var blockSizeBits = aesProvider.BlockSize;

            // Generate the key and initialization vector.
            byte[] salt =
            {
                0x0, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6,
                0xF1, 0xF0, 0xEE, 0x21, 0x22, 0x45
            };
            MakeKeyAndIv(password, salt, keySizeBits, blockSizeBits,
                out var key, out var iv);

            // Make the encryptor or decryptor.
            ICryptoTransform cryptoTransform;
            if (encrypt)
                cryptoTransform = aesProvider.CreateEncryptor(key, iv);
            else
                cryptoTransform = aesProvider.CreateDecryptor(key, iv);

            // Attach a crypto stream to the output stream.
            // Closing crypto_stream sometimes throws an
            // exception if the decryption didn't work
            // (e.g. if we use the wrong password).
            try
            {
                using var cryptoStream =
                    new CryptoStream(outStream, cryptoTransform,
                        CryptoStreamMode.Write);
                // Encrypt or decrypt the file.
                const int blockSize = 1024;
                var buffer = new byte[blockSize];
                while (true)
                {
                    // Read some bytes.
                    var bytesRead = inStream.Read(buffer, 0, blockSize);
                    if (bytesRead == 0) break;

                    // Write the bytes into the CryptoStream.
                    cryptoStream.Write(buffer, 0, bytesRead);
                }
            }
            catch
            {
                // ignored
            }

            cryptoTransform.Dispose();
        }

        private static void MakeKeyAndIv(string password, byte[] salt,
            int keySizeBits, int blockSizeBits,
            out byte[] key, out byte[] iv)
        {
            var deriveBytes =
                new Rfc2898DeriveBytes(password, salt, 1000);

            key = deriveBytes.GetBytes(keySizeBits / 8);
            iv = deriveBytes.GetBytes(blockSizeBits / 8);
        }

        public static void EncryptFile(string password,
            string inFile, string outFile)
        {
            CryptFile(password, inFile, outFile, true);
        }

        public static void DecryptFile(string password,
            string inFile, string outFile)
        {
            CryptFile(password, inFile, outFile, false);
        }

        private static void CryptFile(string password, string inFile, string outFile, bool encrypt)
        {
            // Create input and output file streams.
            using var inStream = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            using var outStream = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            // Encrypt/decrypt the input stream into the output stream.
            CryptStream(password, inStream, outStream, encrypt);
        }
    }
}