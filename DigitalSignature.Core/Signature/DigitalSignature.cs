using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;

namespace DigitalSignature.Core.Signature
{
    public class DigitalSignature : IDigitalSignature
    {
        protected readonly IAsymmetricCryptoAlgorithm _algorithm;
        protected readonly IHashAlgorithm _hashAlgorithm;

        public DigitalSignature(IHashAlgorithm hash, IAsymmetricCryptoAlgorithm algorithm)
        {
            _hashAlgorithm = hash;
            _algorithm = algorithm;
        }
        //protected readonly HashAlgorithmType _hashType;
        //protected readonly HashAlgorithmKey _hashKey;

        public byte[] Signature { get; set; }

        public byte[] Sign(byte[] input) // , string privateKey
        {
            var inputHashText = _hashAlgorithm.Calculate(input);

            Signature = _algorithm.Sign(input, _hashAlgorithm); //, _algorithm.PrivateKey);

            return Signature;
        }

        public bool Check(byte[] input) //=envelope , string signature, string publicKey
        {
            //string txt = ""; // File.ReadAllText(ulaznaDatoteka); -- omotnica
            var inputHashText = _hashAlgorithm.Calculate(input);

            //var signature from file

            // var publicKey from file :: 
            return _algorithm.VerifySignature(input, Signature, _hashAlgorithm);
            //DekriptirajRSA(Convert.ToBase64String(Funkcije.FromHexToByte(potpisP)), javniKljuc.modul, javniKljuc.eksponent);

            //return inputHashText == outputDecryptedHashText;
        }
    }
}