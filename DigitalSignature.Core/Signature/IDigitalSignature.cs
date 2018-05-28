namespace DigitalSignature.Core.Signature
{
    public interface IDigitalSignature
    {
        byte[] Sign(byte[] input);
        bool Check(byte[] input);
    }
}