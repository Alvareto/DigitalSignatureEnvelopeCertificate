using DigitalSignature.Core.Envelope;
using DigitalSignature.Core.Signature;

namespace DigitalSignature.Core.Certificate
{
    public class DigitalCertificate : IDigitalCertificate
    {
        protected readonly IDigitalEnvelope _envelope;
        protected readonly IDigitalSignature _signature;

        public DigitalCertificate(IDigitalEnvelope envelope, IDigitalSignature signature)
        {
            this._envelope = envelope;
            this._signature = signature;
        }

        public byte[] Create(byte[] input)
        {
            _envelope.Encrypt(input);
            return _signature.Sign(_envelope.Envelope);
        }

        public (bool, byte[]) Check()
        {
            //if (_signature.Check(_envelope.Envelope))
            //{
            //    _envelope.Decrypt();
            //}

            var valid = _signature.Check(_envelope.Envelope);

            return (valid, valid ? _envelope.Decrypt() : null);
        }
    }
}