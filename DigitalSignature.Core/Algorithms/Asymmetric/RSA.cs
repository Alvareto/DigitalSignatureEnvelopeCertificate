using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Asymmetric
{
    public class RSA : IAsymmetricCryptoAlgorithm
    {
        protected readonly RSACryptoServiceProvider Algorithm;
        public byte[] PrivateKey { get; set; }

        public RSA(int keySize)
        {
            Algorithm = new RSACryptoServiceProvider(keySize);
            //RSAParameters publicKey = Algorithm.ExportParameters(false);
            //RSAParameters privateKey = Algorithm.ExportParameters(true);
        }

        public byte[] Encrypt(byte[] text)
        {
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider.aspx
            byte[] encrypted;

            using (var encryptAlgorithm = new RSACryptoServiceProvider(Algorithm.KeySize))
            {
                encryptAlgorithm.ImportParameters(Algorithm.ExportParameters(false));

                encrypted = encryptAlgorithm.Encrypt(text, true);
            }

            return encrypted;
        }

        public byte[] Decrypt(byte[] text)
        {
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider.aspx
            byte[] decrypted;

            using (var decryptAlgorithm = new RSACryptoServiceProvider(Algorithm.KeySize))
            {
                decryptAlgorithm.ImportParameters(Algorithm.ExportParameters(true));

                decrypted = decryptAlgorithm.Decrypt(text, true);
            }

            return decrypted;
        }

        public byte[] Sign(byte[] hashCode)
        {
            var sha256 = new SHA1CryptoServiceProvider();
            Algorithm.SignData(hashCode, sha256);

            return Algorithm.SignHash(hashCode, "SHA1");
        }

        public bool VerifySignature(byte[] hashCode, byte[] signature)
        {
            var sha256 = new SHA1CryptoServiceProvider();
            return Algorithm.VerifyData(hashCode, sha256, signature);
        }

        public void Dispose()
        {
            Algorithm?.Dispose();
        }
    }
}