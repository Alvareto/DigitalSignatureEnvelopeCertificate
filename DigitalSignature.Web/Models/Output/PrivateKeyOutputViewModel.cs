using System;
using System.IO;
using DigitalSignature.Web.Helpers;

namespace DigitalSignature.Web.Models.Output
{
    public class PrivateKeyOutputViewModel : OutputViewModel
    {
        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!).
        /// RSA: (duljina ključa je duljina broja n, tj. modulusa)
        /// </summary>
        public string KeyLength { get; set; }

        /// <summary>
        /// Modulus: - broj "n" kod RSA, LUC algoritama (heksadecimalno!)
        /// </summary>
        public string Modulus { get; set; }

        /// <summary>
        /// Private exponent: - broj "d" kod RSA, LUC algoritama (privatni ključ, uz broj n) (heksadecimalno!)
        /// </summary>
        public string PrivateExponent { get; set; }

        public string FileName { get; set; }

        public override void Write()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            using (StreamReader sKeyFileReader = new StreamReader(Environment.CurrentDirectory + Constants.File.Path.PRIVATE_KEY + FileName))
            {
                string currentLine = "";

                while ((currentLine = sKeyFileReader.ReadLine()) != "Modulus:") ;
                while ((currentLine = sKeyFileReader.ReadLine()) != "") Modulus += currentLine.Substring(4);

                while (((currentLine = sKeyFileReader.ReadLine()) != "Private exponent:") && (currentLine != "Public exponent:")) ;
                while ((currentLine = sKeyFileReader.ReadLine()) != "---END OS2 CRYPTO DATA---" && (currentLine != "")) PrivateExponent += currentLine.Substring(4);
            }

        }
    }
}