using System;
using System.Collections.Generic;
using DigitalSignature.Core;
using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;
using DigitalSignature.Core.Algorithms.Symmetric;

namespace DigitalSignature.Web.Models.Output
{
    public class CertificateOutputViewModel : OutputViewModel
    {
        /// <summary>
        /// Signature: - sažetak poruke (ili omotnice) potpisan tajnim ključem pošiljatelja poruke (heksadecimalno!)
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// File name: - ime datoteke koja se kriptira, čiji se sažetak radi, (opcionalno) ...
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!)
        /// </summary>
        public List<string> KeyLength { get; set; }

        /// <summary>
        /// Envelope data: - poruka kriptirana simetričnim ključem kod omotnice, base 64 kodirano (ne heksa)
        /// </summary>
        public string EnvelopeData { get; set; }

        /// <summary>
        /// Envelope crypt key: - simetrični ključ kriptiran javnim ključem primatelja poruke (heksadecimalno!)
        /// </summary>
        public string EnvelopeCryptKey { get; set; }

        public CertificateOutputViewModel(byte[] signature, byte[] data, byte[] key, HashAlgorithmName hash, SymmetricAlgorithmName sym, SymmetricAlgorithmKey symKey, AsymmetricAlgorithmName alg, AsymmetricAlgorithmKey algKey, string file)
        {
            this.Description = "Certificate";

            this.EnvelopeData = Convert.ToBase64String(data);
            this.EnvelopeCryptKey = key.ConvertToHex();
            this.Signature = signature.ConvertToHex();

            this.Method = new List<string>()
            {
                hash.ToString(),
                sym.ToString(),
                alg.ToString()
            };
            this.KeyLength = new List<string>()
            {
                "",
                ((int) symKey).ToString("X"), // hex
                ((int) algKey).ToString("X") // hex
            };
            this.FileName = file;
        }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }
    }
}