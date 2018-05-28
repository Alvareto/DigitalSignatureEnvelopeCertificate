using System;
using System.IO;
using DigitalSignature.Web.Helpers;

namespace DigitalSignature.Web.Models.Output
{
    public class CryptedFileOutputViewModel : OutputViewModel
    {
        /// <summary>
        /// File name: - ime datoteke koja se kriptira, čiji se sažetak radi, (opcionalno) ...
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Data: - kriptirana datoteka, base 64 kodirano (ne heksa)
        /// </summary>
        public string Data { get; set; }

        public override void Write()
        {
            //convert byte[] letters to base64            
            //string Cyphertext = Convert.ToBase64String(input);

            //dodaj .txt na kraju imena datoteke ako ga nema
            if (FileName.Substring(FileName.Length - 4, 4) != ".txt") FileName += ".txt";
            //begin writing process
            using (StreamWriter cypherTextFile = new StreamWriter(Environment.CurrentDirectory + Constants.File.Path.CYPHERTEXT + FileName))
            {
                cypherTextFile.WriteLine(Constants.START);
                cypherTextFile.WriteLine();

                cypherTextFile.WriteLine(Constants.DESCRIPTION);
                cypherTextFile.WriteLine(Constants.TAB + Constants.DESCRIPTION_ENVELOPE);
                cypherTextFile.WriteLine();

                cypherTextFile.WriteLine(Constants.METHOD);
                foreach (var m in Methods)
                {
                    cypherTextFile.WriteLine(Constants.TAB + m); // "SHA-1"
                }
                cypherTextFile.WriteLine();

                cypherTextFile.WriteLine(Constants.FILE_NAME);
                string[] BreadCrumbs = FileName.Split('\\');
                cypherTextFile.WriteLine(Constants.TAB + BreadCrumbs[BreadCrumbs.Length - 1]);
                cypherTextFile.WriteLine();

                cypherTextFile.WriteLine(Constants.DATA);
                for (int i = 0; i < GetNumberOfLines(Data.Length); i++)
                {
                    if ((Data.Length - i * Constants.ROW__CHARACTER_COUNT) < Constants.ROW__CHARACTER_COUNT)
                        cypherTextFile.WriteLine(Constants.TAB + Data.Substring(i * Constants.ROW__CHARACTER_COUNT, (Data.Length - i * Constants.ROW__CHARACTER_COUNT)));
                    else
                        cypherTextFile.WriteLine(Constants.TAB + Data.Substring(i * Constants.ROW__CHARACTER_COUNT, Constants.ROW__CHARACTER_COUNT));
                }
                cypherTextFile.WriteLine();
                cypherTextFile.WriteLine(Constants.END);

            }
        }

        public override void Read()
        {
            StreamReader CypherTextFile = new StreamReader(Environment.CurrentDirectory + Constants.File.Path.CYPHERTEXT + FileName);

            string CurrentLine = "";

            string CypherText = "";

            //read to start of data
            while ((CurrentLine = CypherTextFile.ReadLine()) != Constants.DATA) ;
            //read data
            while (((CurrentLine = CypherTextFile.ReadLine()) != Constants.END) && (CurrentLine != "")) CypherText += CurrentLine.Substring(4);

            //convert from base64 to text
            //byte[] DecodingBuffer = Convert.FromBase64String(CypherText);
            Data = CypherText;
        }
    }
}