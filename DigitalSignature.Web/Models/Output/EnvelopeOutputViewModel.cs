using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DigitalSignature.Core;
using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Symmetric;
using DigitalSignature.Web.Helpers;

namespace DigitalSignature.Web.Models.Output
{
    public class EnvelopeOutputViewModel : OutputViewModel
    {
        /// <summary>
        /// File name: - ime datoteke koja se kriptira, čiji se sažetak radi, (opcionalno) ...
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!)
        /// </summary>
        public List<string> KeyLengths { get; set; }

        /// <summary>
        /// Envelope data: - poruka kriptirana simetričnim ključem kod omotnice, base 64 kodirano (ne heksa)
        /// </summary>
        public string EnvelopeData { get; set; }

        /// <summary>
        /// Envelope crypt key: - simetrični ključ kriptiran javnim ključem primatelja poruke (heksadecimalno!)
        /// </summary>
        public string EnvelopeCryptKey { get; set; }

        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!)
        /// </summary>
        public string KeyLength { get; set; }

        public EnvelopeOutputViewModel()
        {
        }


        public EnvelopeOutputViewModel(byte[] data, byte[] key, SymmetricAlgorithmName sym,
            SymmetricAlgorithmKey symKey, AsymmetricAlgorithmName alg, AsymmetricAlgorithmKey algKey, string file)
        {
            this.Description = "Envelope";
            this.EnvelopeData = Convert.ToBase64String(data);
            this.EnvelopeCryptKey = key.ConvertToHex();
            this.Methods = new List<string>()
            {
                sym.ToString(),
                alg.ToString()
            };
            this.Method = string.Join("\n", Methods);
            this.KeyLengths = new List<string>()
            {
                ((int) symKey).ToString("X"), // hex
                ((int) algKey).ToString("X") // hex
            };
            this.KeyLength = string.Join("\n", KeyLengths);
            this.FileName = file;
        }

        public override void Write()
        {
            //public static void WriteEnvelope(string FileName, Envelope EnvelopeData, int RSAKeyLength)
            using (StreamWriter envelopeWriter =
                new StreamWriter(Environment.CurrentDirectory + Constants.File.Path.ENVELOPE + FileName))
            {
                envelopeWriter.WriteLine(Constants.START);
                envelopeWriter.WriteLine();

                envelopeWriter.WriteLine(Constants.DESCRIPTION);
                envelopeWriter.WriteLine(Constants.TAB + Constants.DESCRIPTION_ENVELOPE);
                envelopeWriter.WriteLine();

                envelopeWriter.WriteLine(Constants.FILE_NAME);
                string last = FileName.Split('\\').LastOrDefault();
                envelopeWriter.WriteLine(Constants.TAB + last);
                envelopeWriter.WriteLine();

                envelopeWriter.WriteLine(Constants.METHOD);
                foreach (var m in Methods)
                {
                    envelopeWriter.WriteLine(Constants.TAB + m); // "SHA-1"
                }
                envelopeWriter.WriteLine();

                envelopeWriter.WriteLine(Constants.KEY_LENGTH);
                foreach (var k in KeyLengths)
                {
                    envelopeWriter.WriteLine(Constants.TAB + k);
                }
                envelopeWriter.WriteLine();

                envelopeWriter.WriteLine(Constants.ENVELOPE_DATA);
                double numLines = (double) EnvelopeData.Length / Constants.ROW__CHARACTER_COUNT;
                if (Math.Truncate(numLines) < numLines) numLines++;

                for (int i = 0; i < Math.Truncate(numLines); i++)
                {
                    if ((EnvelopeData.Length - (i * Constants.ROW__CHARACTER_COUNT)) < Constants.ROW__CHARACTER_COUNT)
                        envelopeWriter.WriteLine(Constants.TAB + EnvelopeData.Substring(
                                                     i * Constants.ROW__CHARACTER_COUNT,
                                                     (EnvelopeData.Length - i * Constants.ROW__CHARACTER_COUNT)));
                    else
                        envelopeWriter.WriteLine(Constants.TAB +
                                                 EnvelopeData.Substring(i * Constants.ROW__CHARACTER_COUNT,
                                                     Constants.ROW__CHARACTER_COUNT));
                }
                envelopeWriter.WriteLine();


                envelopeWriter.WriteLine(Constants.ENVELOPE_KEY);
                for (int i = 0; i < GetNumberOfLines(EnvelopeCryptKey.Length); i++)
                {
                    if ((EnvelopeCryptKey.Length - (i * Constants.ROW__CHARACTER_COUNT)) <
                        Constants.ROW__CHARACTER_COUNT)
                        envelopeWriter.WriteLine("    " + EnvelopeCryptKey.Substring(i * Constants.ROW__CHARACTER_COUNT,
                                                     (EnvelopeCryptKey.Length - i * Constants.ROW__CHARACTER_COUNT)));
                    else
                        envelopeWriter.WriteLine("    " + EnvelopeCryptKey.Substring(i * Constants.ROW__CHARACTER_COUNT,
                                                     Constants.ROW__CHARACTER_COUNT));
                }

                envelopeWriter.WriteLine();
                envelopeWriter.WriteLine(Constants.END);
            }
        }

        public override void Read()
        {
            using (StreamReader envelopeStream =
                new StreamReader(Environment.CurrentDirectory + Constants.File.Path.ENVELOPE + FileName))
            {
                string currentLine = "";
                EnvelopeData = "";
                EnvelopeCryptKey = "";

                while ((envelopeStream.ReadLine()) != Constants.ENVELOPE_DATA) ;
                while ((currentLine = envelopeStream.ReadLine()) != "")
                    EnvelopeData += currentLine.Substring(Constants.TAB.Length);

                while ((envelopeStream.ReadLine()) != Constants.ENVELOPE_KEY) ;
                while (((currentLine = envelopeStream.ReadLine()) != Constants.END) && (currentLine != ""))
                    EnvelopeCryptKey += currentLine.Substring(4);
            }
        }
    }
}