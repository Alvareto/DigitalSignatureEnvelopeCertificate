using System;

namespace DigitalSignature.Web.Models.Output
{
    public class PublicKeyOutputViewModel : OutputViewModel
    {
        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!)
        /// </summary>
        public string KeyLength { get; set; }

        /// <summary>
        /// Modulus: - broj "n" kod RSA, LUC algoritama (heksadecimalno!)
        /// </summary>
        public string Modulus { get; set; }

        /// <summary>
        /// Public exponent: - broj "e" kod RSA, LUC algoritama (dio javnog ključa) (heksadecimalno!)
        /// </summary>
        public string PublicExponent { get; set; }

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