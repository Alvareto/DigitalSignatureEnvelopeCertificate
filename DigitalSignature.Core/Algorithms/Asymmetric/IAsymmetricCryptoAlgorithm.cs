using DigitalSignature.Core.Algorithms.Hash;
using DigitalSignature.Core.Algorithms.Symmetric;

namespace DigitalSignature.Core.Algorithms.Asymmetric
{
    public interface IAsymmetricCryptoAlgorithm : ISymmetricCryptoAlgorithm
    {
        byte[] Sign(byte[] hashCode, IHashAlgorithm hash);

        bool VerifySignature(byte[] hashCode, byte[] signature, IHashAlgorithm hash);
    }
}