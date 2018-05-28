using DigitalSignature.Core.Algorithms.Hash;
using ElGamalExt;

namespace DigitalSignature.Core.Algorithms.Asymmetric
{
    public class ElGamal : IAsymmetricCryptoAlgorithm
    {
        protected readonly ElGamalExt.ElGamal Algorithm;

        public ElGamal(int keySize)
        {
            //var padding = ElGamalPaddingMode.Zeros;
            Algorithm = new ElGamalManaged
            {
                KeySize = keySize
            };
        }

        public byte[] PrivateKey { get; set; }

        public byte[] Encrypt(byte[] plainText)
        {
            var encryptAlgorithm = new ElGamalManaged();
            encryptAlgorithm.FromXmlString(Algorithm.ToXmlString(false));

            var cipherText = encryptAlgorithm.EncryptData(plainText);

            return cipherText;
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            var decryptAlgorithm = new ElGamalManaged();
            decryptAlgorithm.FromXmlString(Algorithm.ToXmlString(true));

            var candidatePlainText = decryptAlgorithm.DecryptData(cipherText);

            return candidatePlainText;
        }

        public byte[] Sign(byte[] hashCode, IHashAlgorithm hash)
        {
            return Algorithm.Sign(hashCode);
        }

        public bool VerifySignature(byte[] hashCode, byte[] signature, IHashAlgorithm hash)
        {
            return Algorithm.VerifySignature(hashCode, signature);
        }

        public void Dispose()
        {
            Algorithm?.Dispose();
        }
    }
}