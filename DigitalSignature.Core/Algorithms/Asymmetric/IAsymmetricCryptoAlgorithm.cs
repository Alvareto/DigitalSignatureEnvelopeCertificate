using DigitalSignature.Core.Algorithms.Symmetric;

namespace DigitalSignature.Core.Algorithms.Asymmetric
{
    public interface IAsymmetricCryptoAlgorithm : ISymmetricCryptoAlgorithm
    {
        byte[] Sign(byte[] hashCode);

        bool VerifySignature(byte[] hashCode, byte[] signature);
    }
}