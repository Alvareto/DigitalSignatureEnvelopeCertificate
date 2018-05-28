using System;
using System.Linq;
using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Symmetric;

namespace DigitalSignature.Core.Envelope
{
    public class DigitalEnvelope : IDigitalEnvelope
    {
        protected readonly IAsymmetricCryptoAlgorithm _asymmetric;
        protected readonly ISymmetricCryptoAlgorithm _symmetric;

        public DigitalEnvelope(ISymmetricCryptoAlgorithm symmetric, IAsymmetricCryptoAlgorithm asymmetric)
        {
            _symmetric = symmetric;
            _asymmetric = asymmetric;
        }

        public byte[] Key { get; set; }
        public byte[] Data { get; set; }

        public byte[] Envelope => Key.Concat(Data).ToArray();


        public (byte[] data, byte[] cryptKey) Encrypt(byte[] input)
        {
            Data = _symmetric.Encrypt(input); // data

            Key = _asymmetric.Encrypt(
                _symmetric.PrivateKey
            ); // key

            // write
            return (data: Data, cryptKey: Key);
        }

        public byte[] Decrypt()
        {
            var _key = _asymmetric.Decrypt(Key);

            _symmetric.PrivateKey = _key;

            var _data = _symmetric.Decrypt(Data);

            // write
            return _data;
        }

        public void Read()
        {
            throw new NotImplementedException();
        }
    }
}