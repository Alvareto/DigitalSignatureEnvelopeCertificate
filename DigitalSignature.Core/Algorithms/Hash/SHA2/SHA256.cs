using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Hash.SHA2
{
    public class SHA256 : IHashAlgorithm
    {
        protected readonly SHA256CryptoServiceProvider Algorithm;
        public HashAlgorithmName AlgorithmName => HashAlgorithmName.SHA256;

        public SHA256()
        {
            Algorithm = new SHA256CryptoServiceProvider();
        }

        public byte[] Calculate(byte[] input)
        {
            return Algorithm.ComputeHash(input);
        }
    }
}