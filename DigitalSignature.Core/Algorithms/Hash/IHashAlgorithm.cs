namespace DigitalSignature.Core.Algorithms.Hash
{
    public interface IHashAlgorithm
    {
        HashAlgorithmName AlgorithmName { get; }
        object CryptoServiceProvider { get; }
        byte[] Calculate(byte[] input);
    }
}