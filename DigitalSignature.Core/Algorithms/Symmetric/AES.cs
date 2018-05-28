using System.IO;
using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Symmetric
{
    public class AES : ISymmetricCryptoAlgorithm
    {
        protected readonly AesCryptoServiceProvider Algorithm;

        public AES(int keySize, CipherMode mode)
        {
            // Create a new instance of the AesCryptoServiceProvider
            // class.  This generates a new key and initialization 
            // vector (IV).
            Algorithm = new AesCryptoServiceProvider
            {
                KeySize = keySize,
                Mode = mode
            };
            //Console.WriteLine("AES Mode is : " + Algorithm.Mode);
            //Console.WriteLine("AES Padding is : " + Algorithm.Padding);
            //Console.WriteLine("AES Key Size : " + Algorithm.KeySize);

            //// Encrypt the string to an array of bytes.
            //byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

            //// Decrypt the bytes to a string.
            //string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
        }

        public byte[] PrivateKey
        {
            get => Algorithm.Key;
            set => Algorithm.Key = value;
        }

        public byte[] Encrypt(byte[] Data)
        {
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.aescryptoserviceprovider(v=vs.110).aspx

            byte[] encrypted;

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV.
            using (var encryptAlgorithm = new AesCryptoServiceProvider())
            {
                encryptAlgorithm.Key = Algorithm.Key;
                encryptAlgorithm.IV = Algorithm.IV;

                // Create a encryptor to perform the stream transform
                var encryptor = encryptAlgorithm.CreateEncryptor(encryptAlgorithm.Key, encryptAlgorithm.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Write the byte array to the crypto stream and flush it.
                        csEncrypt.Write(Data, 0, Data.Length);
                        csEncrypt.FlushFinalBlock();

                        // Get an array of bytes from the MemoryStream 
                        // that holds the encrypted data.
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public byte[] Decrypt(byte[] Data)
        {
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.aescryptoserviceprovider(v=vs.110).aspx

            var decrypted = new byte[Data.Length];

            // Create an AesCryptoServiceProvider object 
            // with the specified key and IV.
            using (var decryptAlgorithm = new AesCryptoServiceProvider())
            {
                decryptAlgorithm.Key = Algorithm.Key;
                decryptAlgorithm.IV = Algorithm.IV;

                // Create a encryptor to perform the stream transform
                var decryptor = decryptAlgorithm.CreateEncryptor(decryptAlgorithm.Key, decryptAlgorithm.IV);

                // Create the streams used for encryption.
                using (var msDecrypt = new MemoryStream(Data))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        csDecrypt.Read(decrypted, 0, decrypted.Length);
                    }
                }
            }


            return decrypted;
        }

        public void Dispose()
        {
            Algorithm?.Dispose();
        }
    }
}