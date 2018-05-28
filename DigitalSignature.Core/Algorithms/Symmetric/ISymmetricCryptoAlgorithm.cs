using System;

namespace DigitalSignature.Core.Algorithms.Symmetric
{
    public interface ISymmetricCryptoAlgorithm : IDisposable
    {
        byte[] PrivateKey { get; set; }

        byte[] Encrypt(byte[] text);
        byte[] Decrypt(byte[] text);
    }
}