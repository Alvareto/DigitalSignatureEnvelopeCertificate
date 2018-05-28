using System;
using ElGamalExt;

namespace DigitalSignature.Core.Algorithms.Asymmetric
{
    public class ElGamal : IAsymmetricCryptoAlgorithm
    {
        protected readonly ElGamalExt.ElGamal Algorithm;
        public byte[] PrivateKey { get; set; }

        public ElGamal(int keySize)
        {
            //var padding = ElGamalPaddingMode.Zeros;
            Algorithm = new ElGamalManaged()
            {
                KeySize = keySize
            };
        }

        public byte[] Encrypt(byte[] plainText)
        {
            var encryptAlgorithm = new ElGamalExt.ElGamalManaged();
            encryptAlgorithm.FromXmlString(Algorithm.ToXmlString(p_include_private: false));

            var cipherText = encryptAlgorithm.EncryptData(plainText);

            // TODO: return byte[] implementation
            throw new NotImplementedException();
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            var decryptAlgorithm = new ElGamalExt.ElGamalManaged();
            decryptAlgorithm.FromXmlString(Algorithm.ToXmlString(p_include_private: true));

            var candidatePlainText = decryptAlgorithm.DecryptData(cipherText);

            // TODO: return byte[] implementation
            throw new NotImplementedException();
        }

        public byte[] Sign(byte[] hashCode)
        {
            return Algorithm.Sign(hashCode);
        }

        public bool VerifySignature(byte[] hashCode, byte[] signature)
        {
            return Algorithm.VerifySignature(hashCode, signature);
        }

        public void Dispose()
        {
            Algorithm?.Dispose();
        }
    }
}