using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DigitalSignature.Core;
using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;
using DigitalSignature.Core.Algorithms.Hash.SHA1;
using DigitalSignature.Core.Algorithms.Hash.SHA2;
using DigitalSignature.Core.Algorithms.Symmetric;
using DigitalSignature.Core.Certificate;
using DigitalSignature.Core.Envelope;
using HashAlgorithmName = DigitalSignature.Core.Algorithms.Hash.HashAlgorithmName;
using SHA256 = DigitalSignature.Core.Algorithms.Hash.SHA2.SHA256;

namespace DigitalSignature.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new InputViewModel();

            return View(vm);
        }

        [HttpPost]
        public ActionResult Generate(
            InputViewModel vm)
        {

            return View();
        }

        public SignatureOutputViewModel GenerateSignature(SignatureInputViewModel vm)
        {
            var inputBytes = Encoding.ASCII.GetBytes(vm.InputText); // new byte[] { };

            IHashAlgorithm hash = GetHashAlgorithm(vm.SelectedHashAlgorithmName);

            IAsymmetricCryptoAlgorithm asymmetric = GetAsymmetricAlgorithm(vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey);

            var signature = new Core.Signature.DigitalSignature(hash: hash, algorithm: asymmetric);

            var _sign = signature.Sign(input: new byte[] { });

            var valid = signature.Check(_sign);

            var file = "";
            var output = new SignatureOutputViewModel(_sign, vm.SelectedHashAlgorithmName,
                vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey, file);

            return output;
        }

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
            public List<string> Method { get; set; }

            public abstract void Write();
            public abstract void Read();
        }

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
            public List<string> KeyLength { get; set; }

            public SignatureOutputViewModel(byte[] signature, HashAlgorithmName hash, AsymmetricAlgorithmName alg, AsymmetricAlgorithmKey algKey, string file)
            {
                this.Description = "Signature";
                this.Signature = signature.ConvertToHex();
                this.Method = new List<String>()
                {
                    hash.ToString(),
                    alg.ToString()
                };
                this.KeyLength = new List<string>()
                {
                    "",
                    ((int) algKey).ToString("X") // hex
                };
                this.FileName = file;
            }



            public override void Write()
            {
                //public static void WriteSignature(string Signature, string FileName, int RSAKeyLength)
                using (StreamWriter signatureWriter = new StreamWriter(Environment.CurrentDirectory + @"\Signature\" + FileName))
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
                    foreach (var m in Method)
                    {
                        signatureWriter.WriteLine(Constants.TAB + m); // "SHA-1"
                    }
                    signatureWriter.WriteLine();

                    signatureWriter.WriteLine(Constants.KEY_LENGTH);
                    foreach (var k in KeyLength)
                    {
                        signatureWriter.WriteLine(Constants.TAB + k);
                    }
                    signatureWriter.WriteLine();

                    signatureWriter.WriteLine(Constants.SIGNATURE);

                    double numLines = (double)Signature.Length / Constants.ROW__CHARACTER_COUNT;

                    if (Math.Truncate(numLines) < numLines) numLines++;
                    for (int i = 0; i < Math.Truncate(numLines); i++)
                    {
                        if ((Signature.Length - (i * Constants.ROW__CHARACTER_COUNT)) < Constants.ROW__CHARACTER_COUNT)
                            signatureWriter.WriteLine(Constants.TAB + Signature.Substring(i * Constants.ROW__CHARACTER_COUNT, (Signature.Length - i * Constants.ROW__CHARACTER_COUNT)));
                        else signatureWriter.WriteLine(Constants.TAB + Signature.Substring(i * Constants.ROW__CHARACTER_COUNT, Constants.ROW__CHARACTER_COUNT));
                    }
                    signatureWriter.WriteLine();

                    signatureWriter.WriteLine(Constants.END);
                }
            }


            public override void Read()
            {
                string _signature = "";

                using (StreamReader SignatureStream = new StreamReader(Environment.CurrentDirectory + @"\Signature\" + FileName))
                {
                    string currentLine = "";

                    while (SignatureStream.ReadLine() != "Signature:")
                    {
                    }
                    while (((currentLine = SignatureStream.ReadLine()) != "---END OS2 CRYPTO DATA---") && (currentLine != ""))
                    {
                        _signature += currentLine.Substring(4);
                    }

                    SignatureStream.Close();
                }

                Signature = _signature;
            }
        }

        public class Constants
        {
            public static string START = "---BEGIN OS 2 CRYPTO DATA---";
            public static string END = "---END OS2 CRYPTO DATA---";

            public static string DESCRIPTION = "Description:";
            public static string METHOD = "Method:";
            public static string SIGNATURE = "Signature:";
            public static string KEY_LENGTH = "Key length:";
            public static string MODULUS = "Modulus:";
            public static string PUBLIC_EXPONENT = "Public exponent:";
            public static string PRIVATE_EXPONENT = "Private exponent:";
            public static string SECRET_KEY = "Secret key:";
            public static string FILE_NAME = "File name:";
            public static string DATA = "Data:";
            public static string INIT_VECTOR = "Initialization vector:";
            public static string ENVELOPE_DATA = "Envelope data:";
            public static string ENVELOPE_KEY = "Envelope crypt key:";

            public static string TAB = "    ";

            public static string DESCRIPTION_SECRET_KEY = "Secret key";
            public static string DESCRIPTION_PUBLIC_KEY = "Public key";
            public static string DESCRIPTION_PRIVATE_KEY = "Private key";
            public static string DESCRIPTION_SIGNATURE = "Signature";
            public static string DESCRIPTION_ENVELOPE = "Envelope";
            public static string DESCRIPTION_CRYPTED_FILE = "Crypted file";

            public static int ROW__CHARACTER_COUNT = 60;

        }

        public class EnvelopeOutputViewModel : OutputViewModel
        {
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

            public EnvelopeOutputViewModel(byte[] data, byte[] key, SymmetricAlgorithmName sym, SymmetricAlgorithmKey symKey, AsymmetricAlgorithmName alg, AsymmetricAlgorithmKey algKey, string file)
            {
                this.Description = "Envelope";
                this.EnvelopeData = Convert.ToBase64String(data);
                this.EnvelopeCryptKey = key.ConvertToHex();
                this.Method = new List<String>()
                {
                    sym.ToString(),
                    alg.ToString()
                };
                this.KeyLength = new List<string>()
                {
                    ((int) symKey).ToString("X"), // hex
                    ((int) algKey).ToString("X") // hex
                };
                this.FileName = file;
            }

            public override void Write()
            {
                //public static void WriteEnvelope(string FileName, Envelope EnvelopeData, int RSAKeyLength)
                using (StreamWriter envelopeWriter = new StreamWriter(Environment.CurrentDirectory + @"\Envelope\" + FileName))
                {
                    envelopeWriter.WriteLine(Constants.START);
                    envelopeWriter.WriteLine();

                    envelopeWriter.WriteLine(Constants.DESCRIPTION);
                    envelopeWriter.WriteLine(Constants.TAB + Constants.DESCRIPTION_ENVELOPE);
                    envelopeWriter.WriteLine();

                    envelopeWriter.WriteLine(Constants.FILE_NAME);
                    string[] BreadCrumbs = FileName.Split('\\');
                    envelopeWriter.WriteLine(Constants.TAB + BreadCrumbs[BreadCrumbs.Length - 1]);
                    envelopeWriter.WriteLine();

                    envelopeWriter.WriteLine(Constants.METHOD);
                    foreach (var m in Method)
                    {
                        envelopeWriter.WriteLine(Constants.TAB + m); // "SHA-1"
                    }
                    envelopeWriter.WriteLine();

                    envelopeWriter.WriteLine(Constants.KEY_LENGTH);
                    foreach (var k in KeyLength)
                    {
                        envelopeWriter.WriteLine(Constants.TAB + k);
                    }
                    envelopeWriter.WriteLine();

                    envelopeWriter.WriteLine(Constants.ENVELOPE_DATA);
                    double numLines = (double)EnvelopeData.Length / Constants.ROW__CHARACTER_COUNT;
                    if (Math.Truncate(numLines) < numLines) numLines++;

                    for (int i = 0; i < Math.Truncate(numLines); i++)
                    {
                        if ((EnvelopeData.Length - (i * Constants.ROW__CHARACTER_COUNT)) < Constants.ROW__CHARACTER_COUNT)
                            envelopeWriter.WriteLine(Constants.TAB + EnvelopeData.Substring(i * Constants.ROW__CHARACTER_COUNT, (EnvelopeData.Length - i * Constants.ROW__CHARACTER_COUNT)));
                        else envelopeWriter.WriteLine(Constants.TAB + EnvelopeData.Substring(i * Constants.ROW__CHARACTER_COUNT, Constants.ROW__CHARACTER_COUNT));
                    }
                    envelopeWriter.WriteLine();


                    envelopeWriter.WriteLine(Constants.ENVELOPE_KEY);
                    numLines = (double)EnvelopeCryptKey.Length / Constants.ROW__CHARACTER_COUNT;
                    if (Math.Truncate(numLines) < numLines) numLines++;
                    for (int i = 0; i < Math.Truncate(numLines); i++)
                    {
                        if ((EnvelopeCryptKey.Length - (i * Constants.ROW__CHARACTER_COUNT)) < Constants.ROW__CHARACTER_COUNT)
                            envelopeWriter.WriteLine("    " + EnvelopeCryptKey.Substring(i * Constants.ROW__CHARACTER_COUNT, (EnvelopeCryptKey.Length - i * Constants.ROW__CHARACTER_COUNT)));
                        else envelopeWriter.WriteLine("    " + EnvelopeCryptKey.Substring(i * Constants.ROW__CHARACTER_COUNT, Constants.ROW__CHARACTER_COUNT));
                    }

                    envelopeWriter.WriteLine();
                    envelopeWriter.WriteLine(Constants.END);
                }
            }

            public override void Read()
            {
                using (StreamReader EnvelopeStream =
                    new StreamReader(Environment.CurrentDirectory + @"\Envelope\" + FileName))
                {
                    string CurrentLine = "";
                    EnvelopeData = "";
                    EnvelopeCryptKey = "";

                    while ((EnvelopeStream.ReadLine()) != Constants.ENVELOPE_DATA) ;
                    while ((CurrentLine = EnvelopeStream.ReadLine()) != "") EnvelopeData += CurrentLine.Substring(Constants.TAB.Length);

                    while ((EnvelopeStream.ReadLine()) != Constants.ENVELOPE_KEY) ;
                    while (((CurrentLine = EnvelopeStream.ReadLine()) != Constants.END) && (CurrentLine != "")) EnvelopeCryptKey += CurrentLine.Substring(4);

                }
            }
        }

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
                string Cyphertext = Convert.ToBase64String(input);

                //dodaj .txt na kraju imena datoteke ako ga nema
                if (FileName.Substring(FileName.Length - 4, 4) != ".txt") FileName += ".txt";
                //begin writing process
                StreamWriter CypherTextFile = new StreamWriter(Program.direktorij + @"\Cyphertext\" + FileName);

                CypherTextFile.WriteLine("---BEGIN OS2 CRYPTO DATA---");
                CypherTextFile.WriteLine();
                CypherTextFile.WriteLine("Description:");
                CypherTextFile.WriteLine("    Crypted file");
                CypherTextFile.WriteLine();
                CypherTextFile.WriteLine("Method:");
                CypherTextFile.WriteLine("    AES");
                CypherTextFile.WriteLine();
                CypherTextFile.WriteLine("File name:");
                CypherTextFile.WriteLine("    " + FileName);
                CypherTextFile.WriteLine();
                CypherTextFile.WriteLine("Data:");

                double NumLines = (double)Cyphertext.Length / 60;

                if (Math.Truncate(NumLines) < NumLines) NumLines++;

                for (int i = 0; i < Math.Truncate(NumLines); i++)
                {
                    if ((Cyphertext.Length - i * 60) < 60)
                        CypherTextFile.WriteLine("    " + Cyphertext.Substring(i * 60, (Cyphertext.Length - i * 60)));
                    else
                        CypherTextFile.WriteLine("    " + Cyphertext.Substring(i * 60, 60));
                }
                CypherTextFile.WriteLine();
                CypherTextFile.WriteLine("---END OS2 CRYPTO DATA---");

                CypherTextFile.Close();
            }

            public override void Read()
            {
                StreamReader CypherTextFile = new StreamReader(Program.direktorij + @"\Cyphertext\" + FileName);

                string CurrentLine = "";

                string CypherText = "";

                //read to start of data
                while ((CurrentLine = CypherTextFile.ReadLine()) != "Data:") ;
                //read data
                while (((CurrentLine = CypherTextFile.ReadLine()) != "---END OS2 CRYPTO DATA---") && (CurrentLine != "")) CypherText += CurrentLine.Substring(4);

                //convert from base64 to text
                byte[] DecodingBuffer = Convert.FromBase64String(CypherText);

                return DecodingBuffer;
            }
        }

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
            /// Key length: - duljina ključa u bitovima (heksadecimalno!)
            /// </summary>
            public string KeyLength { get; set; }

            public override void Write()
            {
                byte[] key = Funkcije.KeyGenerator(Size);

                string KeyString = Funkcije.FromByteToHexString(key);
                //kreiranje datoteke            

                //dodaj .txt na kraju imena datoteke ako ga nema
                if (FileName.Substring(FileName.Length - 4, 4) != ".txt") FileName += ".txt";

                StreamWriter KeyFile = new StreamWriter(Program.direktorij + @"\Keys\" + FileName);

                KeyFile.WriteLine("---BEGIN OS2 CRYPTO DATA---");
                KeyFile.WriteLine();
                KeyFile.WriteLine("Description:");
                KeyFile.WriteLine("    Secret Key");
                KeyFile.WriteLine();
                KeyFile.WriteLine("Method:");
                KeyFile.WriteLine("    AES");
                KeyFile.WriteLine();
                KeyFile.WriteLine("Key Length:");
                KeyFile.WriteLine("    " + Funkcije.IntToHex(Size));
                KeyFile.WriteLine();
                KeyFile.WriteLine("Secret Key:");

                double NumLines = (double)KeyString.Length / 60;

                if (Math.Truncate(NumLines) < NumLines) NumLines++;

                for (int i = 0; i < Math.Truncate(NumLines); i++)
                {
                    if ((KeyString.Length - i * 60) < 60)
                        KeyFile.WriteLine("    " + KeyString.Substring(i * 60, (KeyString.Length - i * 60)));
                    else
                        KeyFile.WriteLine("    " + KeyString.Substring(i * 60, 60));
                }
                KeyFile.WriteLine();
                KeyFile.WriteLine("---END OS2 CRYPTO DATA---");
                KeyFile.Close();



                byte[] IVector = Funkcije.KeyGenerator(128);

                string VectorString = Funkcije.FromByteToHexString(IVector);
                //kreiranje datoteke            

                //dodaj .txt na kraju imena datoteke ako ga nema
                if (FileName.Substring(FileName.Length - 4, 4) != ".txt") FileName += ".txt";

                StreamWriter IVectorFile = new StreamWriter(Program.direktorij + @"\Keys\" + FileName);

                IVectorFile.WriteLine("---BEGIN OS2 CRYPTO DATA---");
                IVectorFile.WriteLine();
                IVectorFile.WriteLine("Description:");
                IVectorFile.WriteLine("    Initialization vector");
                IVectorFile.WriteLine();
                IVectorFile.WriteLine("Method:");
                IVectorFile.WriteLine("    AES");
                IVectorFile.WriteLine();
                IVectorFile.WriteLine("Key Length:");
                IVectorFile.WriteLine("    " + Funkcije.IntToHex(128));
                IVectorFile.WriteLine();
                IVectorFile.WriteLine("Initialization vector:");

                double NumLines = (double)VectorString.Length / 60;

                if (Math.Truncate(NumLines) < NumLines) NumLines++;

                for (int i = 0; i < Math.Truncate(NumLines); i++)
                {
                    if ((VectorString.Length - i * 60) < 60)
                        IVectorFile.WriteLine("    " + VectorString.Substring(i * 60, (VectorString.Length - i * 60)));
                    else
                        IVectorFile.WriteLine("    " + VectorString.Substring(i * 60, 60));
                }
                IVectorFile.WriteLine();
                IVectorFile.WriteLine("---END OS2 CRYPTO DATA---");
                IVectorFile.Close();
            }

            public override void Read()
            {
                using (StreamReader KeyFile = new StreamReader(Environment.CurrentDirectory + @"\Keys\" + FileName))
                {
                    String CurrentLine = "";

                    String Key = "";

                    //read to start of key data
                    while ((CurrentLine = KeyFile.ReadLine()) != Constants.SECRET_KEY) ;
                    //read key data
                    while (((CurrentLine = KeyFile.ReadLine()) != Constants.END) && (CurrentLine != ""))
                        Key += CurrentLine.Substring(4);


                    SecretKey = Key;
                }




                StreamReader IVectorFile = new StreamReader(Program.direktorij + @"\Keys\" + FileName);

                String CurrentLine = "";

                String Vector = "";

                //read to start of key data
                while ((CurrentLine = IVectorFile.ReadLine()) != "Initialization vector:") ;
                //read key data
                while (((CurrentLine = IVectorFile.ReadLine()) != "---END OS2 CRYPTO DATA---") && (CurrentLine != "")) Vector += CurrentLine.Substring(4);

                byte[] DecodingBuffer = Funkcije.FromHexToByte(Vector);

                return DecodingBuffer;
            }
        }

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

            public override void Write()
            {
                throw new NotImplementedException();
            }

            public override void Read()
            {
                StreamReader RSAFile = new StreamReader(Program.direktorij + @"\Keys\" + KeyFile);

                string CurrentLine = "";
                RSAKey RSA = new RSAKey();

                while ((CurrentLine = RSAFile.ReadLine()) != "Modulus:") ;
                while ((CurrentLine = RSAFile.ReadLine()) != "") RSA.Modulus += CurrentLine.Substring(4);

                while (((CurrentLine = RSAFile.ReadLine()) != "Private exponent:") && (CurrentLine != "Public exponent:")) ;
                while ((CurrentLine = RSAFile.ReadLine()) != "---END OS2 CRYPTO DATA---" && (CurrentLine != "")) RSA.Exponent += CurrentLine.Substring(4);

                return RSA;
            }
        }

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

        public EnvelopeOutputViewModel GenerateEnvelope(EnvelopeInputViewModel vm)
        {
            var inputBytes = Encoding.ASCII.GetBytes(vm.InputText); // new byte[] { };

            ISymmetricCryptoAlgorithm symmetric = GetSymmetricAlgorithm(vm.SelectedSymmetricAlgorithmName, vm.SelectedSymmetricAlgorithmKey, vm.SelectedSymmetricAlgorithmMode);

            IAsymmetricCryptoAlgorithm asymmetric = GetAsymmetricAlgorithm(vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey);

            var envelope = new DigitalEnvelope(symmetric: symmetric, asymmetric: asymmetric);

            var _env = envelope.Encrypt(input: new byte[] { });

            var data = envelope.Decrypt();

            var model = new EnvelopeOutputViewModel(_env.data, _env.cryptKey, vm.SelectedSymmetricAlgorithmName, vm.SelectedSymmetricAlgorithmKey, vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey,
                "");

            return model;
        }

        public void GenerateCertificate(CertificateInputViewModel vm)
        {
            var inputBytes = Encoding.ASCII.GetBytes(vm.InputText); // new byte[] { };

            ISymmetricCryptoAlgorithm symmetric = GetSymmetricAlgorithm(vm.SelectedSymmetricAlgorithmName, vm.SelectedSymmetricAlgorithmKey, vm.SelectedSymmetricAlgorithmMode);

            IAsymmetricCryptoAlgorithm asymmetric = GetAsymmetricAlgorithm(vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey);

            var envelope = new DigitalEnvelope(symmetric: symmetric, asymmetric: asymmetric);

            IHashAlgorithm hash = GetHashAlgorithm(vm.SelectedHashAlgorithmName);

            var signature = new Core.Signature.DigitalSignature(hash: hash, algorithm: asymmetric);

            var certificate = new DigitalCertificate(
                envelope: envelope,
                signature: signature
            );

            byte[] _gen = certificate.Create(input: inputBytes);

            (bool, byte[]) _degen = certificate.Check();
        }

        private ISymmetricCryptoAlgorithm GetSymmetricAlgorithm(SymmetricAlgorithmName name, SymmetricAlgorithmKey keySize, System.Security.Cryptography.CipherMode mode)
        {
            switch (name)
            {
                case SymmetricAlgorithmName.TripleDES:
                    return new TripleDES(keySize: (int)keySize, mode: mode);
                    break;
                case SymmetricAlgorithmName.AES:
                default:
                    return new AES(keySize: (int)keySize, mode: mode);
                    break;
            }
        }

        private IAsymmetricCryptoAlgorithm GetAsymmetricAlgorithm(AsymmetricAlgorithmName name, AsymmetricAlgorithmKey keySize)
        {
            switch (name)
            {
                case AsymmetricAlgorithmName.ElGamal:
                    return new ElGamal(keySize: (int)keySize);
                    break;
                case AsymmetricAlgorithmName.RSA:
                default:
                    return new RSA(keySize: (int)keySize);
                    break;
            }
        }

        private IHashAlgorithm GetHashAlgorithm(HashAlgorithmName name)
        {
            switch (name)
            {
                case HashAlgorithmName.SHA1:
                    return new SHA1();
                    break;
                case HashAlgorithmName.SHA256:
                    return new SHA256();
                    break;
                case HashAlgorithmName.SHA384:
                    return new SHA384();
                    break;
                case HashAlgorithmName.SHA512:
                default:
                    return new SHA512();
                    break;
            }
        }
    }

    public class SignatureInputViewModel
    {
        public string InputText { get; set; }

        public AsymmetricAlgorithmName SelectedAsymmetricAlgorithmName { get; set; }
        public AsymmetricAlgorithmKey SelectedAsymmetricAlgorithmKey { get; set; }
        public HashAlgorithmName SelectedHashAlgorithmName { get; set; }

    }

    public class EnvelopeInputViewModel
    {
        public string InputText { get; set; }

        public AsymmetricAlgorithmName SelectedAsymmetricAlgorithmName { get; set; }
        public AsymmetricAlgorithmKey SelectedAsymmetricAlgorithmKey { get; set; }
        public SymmetricAlgorithmName SelectedSymmetricAlgorithmName { get; set; }
        public System.Security.Cryptography.CipherMode SelectedSymmetricAlgorithmMode { get; set; }
        public SymmetricAlgorithmKey SelectedSymmetricAlgorithmKey { get; set; }

    }

    public class CertificateInputViewModel
    {
        public string InputText { get; set; }

        public AsymmetricAlgorithmName SelectedAsymmetricAlgorithmName { get; set; }
        public AsymmetricAlgorithmKey SelectedAsymmetricAlgorithmKey { get; set; }
        public SymmetricAlgorithmName SelectedSymmetricAlgorithmName { get; set; }
        public System.Security.Cryptography.CipherMode SelectedSymmetricAlgorithmMode { get; set; }
        public SymmetricAlgorithmKey SelectedSymmetricAlgorithmKey { get; set; }
        public HashAlgorithmName SelectedHashAlgorithmName { get; set; }

    }

    public class InputViewModel
    {
        public string InputText { get; set; }
        public AsymmetricAlgorithmName SelectedAsymmetricAlgorithmName { get; set; }
        public AsymmetricAlgorithmKey SelectedAsymmetricAlgorithmKey { get; set; }
        public SymmetricAlgorithmName SelectedSymmetricAlgorithmName { get; set; }
        public System.Security.Cryptography.CipherMode SelectedSymmetricAlgorithmMode { get; set; }
        public SymmetricAlgorithmKey SelectedSymmetricAlgorithmKey { get; set; }
        public HashAlgorithmName SelectedHashAlgorithmName { get; set; }
    }


}