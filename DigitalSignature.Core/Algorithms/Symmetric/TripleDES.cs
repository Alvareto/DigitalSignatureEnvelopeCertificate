using System.IO;
using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Symmetric
{
    public class TripleDES : ISymmetricCryptoAlgorithm
    {
        protected readonly TripleDESCryptoServiceProvider Algorithm;

        public TripleDES(int keySize, CipherMode mode)
        {
            Algorithm = new TripleDESCryptoServiceProvider
            {
                KeySize = keySize,
                Mode = mode
            };

            //// Create a string to encrypt.
            //string sData = "Here is some data to encrypt.";

            //// Encrypt the string to an in-memory buffer.
            //byte[] Data = EncryptTextToMemory(sData, tDESalg.Key, tDESalg.IV);

            //// Decrypt the buffer back to a string.
            //string Final = DecryptTextFromMemory(Data, tDESalg.Key, tDESalg.IV);
        }

        public byte[] PrivateKey
        {
            get => Algorithm.Key;
            set => Algorithm.Key = value;
        }

        public byte[] Encrypt(byte[] Data)
        {
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.tripledescryptoserviceprovider(v=vs.110).aspx

            byte[] ret;

            // Create a memorystream
            using (var mStream = new MemoryStream())
            {
                using (
                    // Create a CryptoStream using the MemoryStream
                    // and pass key and initialization vector (IV)
                    var cStream = new CryptoStream(
                        mStream,
                        new TripleDESCryptoServiceProvider()
                            .CreateEncryptor(Algorithm.Key, Algorithm.IV),
                        CryptoStreamMode.Write
                    ))
                {
                    // Convert the passed string to a byte array.
                    //byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);

                    // Write the byte array to the crypto stream and flush it.
                    cStream.Write(Data, 0, Data.Length);
                    cStream.FlushFinalBlock();

                    // Get an array of bytes from the MemoryStream 
                    // that holds the encrypted data.
                    ret = mStream.ToArray();
                }
            } // close the streams.

            // return the encrypted buffer
            return ret;
        }

        public byte[] Decrypt(byte[] Data)
        {
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.tripledescryptoserviceprovider(v=vs.110).aspx

            var fromEncrypt = new byte[Data.Length];

            // Create a MemoryStream using the passed array of encrypted data.
            using (var mStream = new MemoryStream(Data))
            {
                using (
                    // Create a CryptoStream using the MemoryStream
                    // and pass key and initialization vector (IV)
                    var csDecrypt = new CryptoStream(
                        mStream,
                        new TripleDESCryptoServiceProvider()
                            .CreateDecryptor(Algorithm.Key, Algorithm.IV),
                        CryptoStreamMode.Read
                    ))
                {
                    // Convert the passed string to a byte array.
                    //byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);

                    // Read the decrypted data out of the crypto stream
                    // and place it into the temporary buffer
                    csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                    // Convert the buffer into a string and return it
                    //return new ASCIIEncoding().GetString(fromEncrypt);
                }
            } // close the streams.

            return fromEncrypt;
        }

        public void Dispose()
        {
            Algorithm?.Dispose();
        }
    }
}