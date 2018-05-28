namespace DigitalSignature.Core.Algorithms.Hash
{
    public interface IHashAlgorithm
    {
        HashAlgorithmName AlgorithmName { get; }
        byte[] Calculate(byte[] input);
    }
}