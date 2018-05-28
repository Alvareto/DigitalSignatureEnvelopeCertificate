using System;
using System.Collections.Generic;
using DigitalSignature.Web.Helpers;

namespace DigitalSignature.Web.Models
{
    [Serializable]
    public abstract class OutputViewModel
    {
        public InputViewModel Input { get; set; }

        /// <summary>
        /// Description: - što je u datoteci: Secret key, Public key, Private key, Signature, Envelope, Crypted file [...]
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Method: - kriptografski algoritam: DES, RSA, AES, SHA-1, [...], kod potpisa i omotnice ima više algoritama, pa se oni svi navode, svaki u novom redu, kao i ostali parametri (duljina ključa)
        /// </summary>
        public IEnumerable<string> Methods { get; set; }

        public string InputText { get; set; }

        /// <summary>
        /// Method: - kriptografski algoritam: DES, RSA, AES, SHA-1, [...], kod potpisa i omotnice ima više algoritama, pa se oni svi navode, svaki u novom redu, kao i ostali parametri (duljina ključa)
        /// </summary>
        public string Method { get; set; }

        public abstract void Write();
        public abstract void Read();

        protected double GetNumberOfLines(int length)
        {
            double numLines = (double) length / Constants.ROW__CHARACTER_COUNT;
            if (Math.Truncate(numLines) < numLines)
                numLines++;

            return Math.Truncate(numLines);
        }
    }
}