using System;
using DigitalSignature.Core.Algorithms.Hash;
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

            return cipherText;
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            var decryptAlgorithm = new ElGamalExt.ElGamalManaged();
            decryptAlgorithm.FromXmlString(Algorithm.ToXmlString(p_include_private: true));

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