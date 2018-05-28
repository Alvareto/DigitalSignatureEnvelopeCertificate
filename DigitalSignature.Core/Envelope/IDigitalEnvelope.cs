namespace DigitalSignature.Core.Envelope
{
    public interface IDigitalEnvelope
    {
        byte[] Key { get; }
        byte[] Data { get; }
        byte[] Envelope { get; }

        (byte[] data, byte[] cryptKey) Encrypt(byte[] input);
        byte[] Decrypt();
        void Read();
    }
}