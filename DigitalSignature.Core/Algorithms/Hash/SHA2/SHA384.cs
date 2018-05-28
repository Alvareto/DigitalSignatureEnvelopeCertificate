﻿using System.Security.Cryptography;

namespace DigitalSignature.Core.Algorithms.Hash.SHA2
{
    public class SHA384 : IHashAlgorithm
    {
        protected readonly SHA384CryptoServiceProvider Algorithm;
        public HashAlgorithmName AlgorithmName => HashAlgorithmName.SHA384;

        public SHA384()
        {
            Algorithm = new SHA384CryptoServiceProvider();
        }

        public byte[] Calculate(byte[] input)
        {
            return Algorithm.ComputeHash(input);
        }
    }
}