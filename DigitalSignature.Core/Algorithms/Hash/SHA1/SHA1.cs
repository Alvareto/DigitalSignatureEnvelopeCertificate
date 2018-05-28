using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Hash.SHA1
{
    public class SHA1 : IHashAlgorithm
    {
        protected readonly SHA1CryptoServiceProvider Algorithm;

        public SHA1()
        {
            Algorithm = new SHA1CryptoServiceProvider();
        }

        public HashAlgorithmName AlgorithmName => HashAlgorithmName.SHA1;
        public object CryptoServiceProvider => Algorithm;

        public byte[] Calculate(byte[] input)
        {
            return Algorithm.ComputeHash(input);
        }
    }
}