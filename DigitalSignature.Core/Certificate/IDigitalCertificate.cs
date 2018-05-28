namespace DigitalSignature.Core.Certificate
{
    public interface IDigitalCertificate
    {
        byte[] Create(byte[] input);
        (bool, byte[]) Check();
    }
}