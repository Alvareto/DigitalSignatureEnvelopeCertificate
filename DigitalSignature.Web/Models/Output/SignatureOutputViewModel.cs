using System;
using System.Collections.Generic;
using System.IO;
using DigitalSignature.Core;
using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;
using DigitalSignature.Web.Helpers;

namespace DigitalSignature.Web.Models.Output
{
    [Serializable]
    public class SignatureOutputViewModel : OutputViewModel
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
        public IEnumerable<string> KeyLengths { get; set; }

        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!)
        /// </summary>
        public string KeyLength { get; set; }

        public SignatureOutputViewModel()
        {
        }

        public SignatureOutputViewModel(byte[] signature, HashAlgorithmName hash, AsymmetricAlgorithmName alg,
            AsymmetricAlgorithmKey algKey, string file)
        {
            this.Description = "Signature";
            this.Signature = signature.ConvertToHex();
            this.Methods = new List<string>()
            {
                hash.ToString(),
                alg.ToString()
            };
            this.Method = string.Join("\n", Methods);
            this.KeyLengths = new List<string>()
            {
                "0A",
                ((int) algKey).ToString("X") // hex
            };
            this.KeyLength = string.Join("\n", KeyLengths);
            this.FileName = file;
        }


        public override void Write()
        {
            //public static void WriteSignature(string Signature, string FileName, int RSAKeyLength)
            using (StreamWriter signatureWriter =
                new StreamWriter(Environment.CurrentDirectory + Constants.File.Path.SIGNATURE + FileName))
            {
                signatureWriter.WriteLine(Constants.START);
                signatureWriter.WriteLine();

                signatureWriter.WriteLine(Constants.DESCRIPTION);
                signatureWriter.WriteLine(Constants.TAB + Constants.DESCRIPTION_SIGNATURE);
                signatureWriter.WriteLine();

                signatureWriter.WriteLine(Constants.FILE_NAME);
                string[] BreadCrumbs = FileName.Split('\\');
                signatureWriter.WriteLine(Constants.TAB + BreadCrumbs[BreadCrumbs.Length - 1]);
                signatureWriter.WriteLine();

                signatureWriter.WriteLine(Constants.METHOD);
                foreach (var m in Methods)
                {
                    signatureWriter.WriteLine(Constants.TAB + m); // "SHA-1"
                }
                signatureWriter.WriteLine();

                signatureWriter.WriteLine(Constants.KEY_LENGTH);
                foreach (var k in KeyLengths)
                {
                    signatureWriter.WriteLine(Constants.TAB + k);
                }
                signatureWriter.WriteLine();

                signatureWriter.WriteLine(Constants.SIGNATURE);


                for (int i = 0; i < GetNumberOfLines(Signature.Length); i++)
                {
                    if ((Signature.Length - (i * Constants.ROW__CHARACTER_COUNT)) < Constants.ROW__CHARACTER_COUNT)
                        signatureWriter.WriteLine(Constants.TAB + Signature.Substring(
                                                      i * Constants.ROW__CHARACTER_COUNT,
                                                      (Signature.Length - i * Constants.ROW__CHARACTER_COUNT)));
                    else
                        signatureWriter.WriteLine(Constants.TAB +
                                                  Signature.Substring(i * Constants.ROW__CHARACTER_COUNT,
                                                      Constants.ROW__CHARACTER_COUNT));
                }
                signatureWriter.WriteLine();

                signatureWriter.WriteLine(Constants.END);
            }
        }


        public override void Read()
        {
            string _signature = "";

            using (StreamReader SignatureStream =
                new StreamReader(Environment.CurrentDirectory + @"\Signature\" + FileName))
            {
                string currentLine = "";

                while (SignatureStream.ReadLine() != Constants.SIGNATURE)
                {
                }
                while (((currentLine = SignatureStream.ReadLine()) != "---END OS2 CRYPTO DATA---") &&
                       (currentLine != ""))
                {
                    _signature += currentLine.Substring(4);
                }

                SignatureStream.Close();
            }

            Signature = _signature;
        }
    }
}