using System;
using System.IO;
using DigitalSignature.Web.Helpers;

namespace DigitalSignature.Web.Models.Output
{
    public class SecretKeyOutputViewModel : OutputViewModel
    {
        /// <summary>
        /// Secret key: - tajni ključ (za simetrične algoritme) (heksadecimalno!)
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Initialization vector: - inicijalizacijski vektor (ako treba za određeni način kriptiranja)
        /// </summary>
        public string InitializationVector { get; set; }

        /// <summary>
        /// File name: - ime datoteke koja se kriptira, čiji se sažetak radi, (opcionalno) ...
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Key length: - duljina ključa u bitovima (heksadecimalno!)
        /// </summary>
        public string KeyLength { get; set; }

        public override void Write()
        {
            using (StreamWriter keyFile = new StreamWriter(Environment.CurrentDirectory + Constants.File.Path.SECRET_KEY + FileName))
            {
                keyFile.WriteLine(Constants.START);
                keyFile.WriteLine();

                keyFile.WriteLine(Constants.DESCRIPTION);
                keyFile.WriteLine(Constants.TAB + Constants.DESCRIPTION_SECRET_KEY);
                keyFile.WriteLine();

                keyFile.WriteLine(Constants.METHOD);
                foreach (var m in Method)
                {
                    keyFile.WriteLine(Constants.TAB + m); // "SHA-1"
                }
                keyFile.WriteLine();

                keyFile.WriteLine(Constants.KEY_LENGTH);
                foreach (var m in KeyLength)
                {
                    keyFile.WriteLine(Constants.TAB + m); // "SHA-1"
                }
                keyFile.WriteLine();

                keyFile.WriteLine(Constants.SECRET_KEY);
                for (int i = 0; i < GetNumberOfLines(SecretKey.Length); i++)
                {
                    if ((SecretKey.Length - i * Constants.ROW__CHARACTER_COUNT) < Constants.ROW__CHARACTER_COUNT)
                        keyFile.WriteLine(Constants.TAB + SecretKey.Substring(i * Constants.ROW__CHARACTER_COUNT, (SecretKey.Length - i * Constants.ROW__CHARACTER_COUNT)));
                    else
                        keyFile.WriteLine(Constants.TAB + SecretKey.Substring(i * Constants.ROW__CHARACTER_COUNT, Constants.ROW__CHARACTER_COUNT));
                }
                keyFile.WriteLine();


                keyFile.WriteLine(Constants.INIT_VECTOR);
                for (int i = 0; i < GetNumberOfLines(InitializationVector.Length); i++)
                {
                    if ((InitializationVector.Length - i * Constants.ROW__CHARACTER_COUNT) < Constants.ROW__CHARACTER_COUNT)
                        keyFile.WriteLine(Constants.TAB + InitializationVector.Substring(i * Constants.ROW__CHARACTER_COUNT, (InitializationVector.Length - i * Constants.ROW__CHARACTER_COUNT)));
                    else
                        keyFile.WriteLine(Constants.TAB + InitializationVector.Substring(i * Constants.ROW__CHARACTER_COUNT, Constants.ROW__CHARACTER_COUNT));
                }
                keyFile.WriteLine();

                keyFile.WriteLine(Constants.END);
            }
        }

        public override void Read()
        {
            using (StreamReader KeyFile = new StreamReader(Environment.CurrentDirectory + Constants.File.Path.SECRET_KEY + FileName))
            {
                string currentLine = string.Empty;

                string Key = string.Empty;
                string IVector = string.Empty;

                //read to start of key data
                while ((currentLine = KeyFile.ReadLine()) != Constants.SECRET_KEY) ;
                //read key data
                while (((currentLine = KeyFile.ReadLine()) != string.Empty))
                    Key += currentLine.Substring(4);
                SecretKey = Key;

                //read to start of IV data
                while ((currentLine = KeyFile.ReadLine()) != Constants.INIT_VECTOR) ;
                //read IV data
                while (((currentLine = KeyFile.ReadLine()) != Constants.END) && (currentLine != string.Empty))
                    IVector += currentLine.Substring(4);

                InitializationVector = IVector;
            }
        }
    }
}