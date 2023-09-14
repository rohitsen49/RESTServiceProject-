using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RESTServiceProject
{
    public class Token
    {
        public int UserId { get; set; }
        public DateTime Expires { get; set; }
    }

    public static class TokenHelper
    {
        public static string GetToken(string userName, string password)
        {
            // do a db lookup to confirm the userName and password
            // create the token
            var token = new Token
            {
                UserId = 10,
                Expires = DateTime.UtcNow.AddMinutes(1),
            };
            var jsonString = JsonSerializer.Serialize(token);
            var encryptedJsonString = Crypto.EncryptStringAES(jsonString);
            return encryptedJsonString;
        }

        public static Token DecodeToken(string token)
        {
            var decryptedJsonString = Crypto.DecryptStringAES(token);
            var tokenObject = JsonSerializer.Deserialize<Token>(decryptedJsonString);
            if (tokenObject.Expires < DateTime.UtcNow)
            {
                return null;
            }
            return tokenObject;
        }
    }

    public class Crypto
    {
        private static readonly byte[] Salt =
            Encoding.ASCII.GetBytes("B78A07A7-14D8-4890-BC99-9145A14713C1");
        private const string Password = "sharedSecretPassword";
        /// <summary>
        /// Encrypt the given string using AES.
        /// The string can be decrypted using DecryptStringAES().
        ///</summary>
        /// <param name="plainText">The text to encrypt.</param>
        public static string EncryptStringAES(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }

            string outStr;                   // Encrypted string to return
            RijndaelManaged aesAlg = null;   // Used to encrypt the data
            try
            {
                var key = new Rfc2898DeriveBytes(Password, Salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }
            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using
        /// EncryptStringAES(), using an identical password.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        public static string DecryptStringAES(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }

            RijndaelManaged aesAlg = null;
            string plaintext;
            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(Password, Salt);

                // Create the streams used for decryption.
                var bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);

                    // Create a decrytor to perform the stream transform.
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }
            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }
            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }
            return buffer;
        }
    }
}
