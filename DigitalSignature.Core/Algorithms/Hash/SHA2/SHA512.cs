using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Hash.SHA2
{
    public class SHA512 : IHashAlgorithm
    {
        protected readonly SHA512CryptoServiceProvider Algorithm;
        public HashAlgorithmName AlgorithmName => HashAlgorithmName.SHA512;
        public object CryptoServiceProvider => Algorithm;

        public SHA512()
        {
            Algorithm = new SHA512CryptoServiceProvider();
        }

        public byte[] Calculate(byte[] input)
        {
            return Algorithm.ComputeHash(input);
        }
    }
}