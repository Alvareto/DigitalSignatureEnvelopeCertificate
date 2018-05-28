using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;

namespace DigitalSignature.Core.Signature
{
    public class DigitalSignature : IDigitalSignature
    {
        protected readonly IHashAlgorithm _hashAlgorithm;
        protected readonly IAsymmetricCryptoAlgorithm _algorithm;
        //protected readonly HashAlgorithmType _hashType;
        //protected readonly HashAlgorithmKey _hashKey;

        public byte[] Signature { get; set; }

        public DigitalSignature(IHashAlgorithm hash, IAsymmetricCryptoAlgorithm algorithm)
        {
            this._hashAlgorithm = hash;
            this._algorithm = algorithm;
        }

        public byte[] Sign(byte[] input) // , string privateKey
        {
            var inputHashText = _hashAlgorithm.Calculate(input);

            // var privateKey from file
            // sažetak se kriptira privatnim ključem pošiljatelja
            Signature = _algorithm.Sign(inputHashText); //, _algorithm.PrivateKey);
            //RSA.KriptirajRSA(hash, privatniKljuc.modul, privatniKljuc.eksponent);
            return Signature;

            // from_bytes_2_hex(from_base64string_2_bytes)

            // clean file
            // write transformed signature to file

        }

        public bool Check(byte[] input) //=envelope , string signature, string publicKey
        {
            //string txt = ""; // File.ReadAllText(ulaznaDatoteka); -- omotnica
            var inputHashText = _hashAlgorithm.Calculate(input);

            //var signature from file

            // var publicKey from file :: 
            return _algorithm.VerifySignature(inputHashText, Signature);
            //DekriptirajRSA(Convert.ToBase64String(Funkcije.FromHexToByte(potpisP)), javniKljuc.modul, javniKljuc.eksponent);

            //return inputHashText == outputDecryptedHashText;
        }
    }
}