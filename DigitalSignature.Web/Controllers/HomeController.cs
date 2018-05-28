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
    public partial class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new InputViewModel();

            return View(vm);
        }

        [HttpPost]
        public ActionResult Generate(InputViewModel vm)
        {

            return View();
        }

        public ActionResult GenerateSignature(InputViewModel vm)
        {
            var model = GenerateSignature(new SignatureInputViewModel
            {
                InputText = vm.InputText,
                SelectedHashAlgorithmName = vm.SelectedHashAlgorithmName,
                SelectedAsymmetricAlgorithmName = vm.SelectedAsymmetricAlgorithmName,
                SelectedAsymmetricAlgorithmKey = vm.SelectedAsymmetricAlgorithmKey
            });

            return View(model);

            //model.FileName = "";

            //string fullName = Path.Combine(Environment.CurrentDirectory, filePath, fileName);

            //byte[] fileBytes = GetFile(fullName);
            //return File(
            //    fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            //model.Write();
        }

        //byte[] GetFile(string s)
        //{
        //    System.IO.File.ReadAllBytes(s);
        //    System.IO.FileStream fs = System.IO.File.OpenRead(s);
        //    byte[] data = new byte[fs.Length];
        //    int br = fs.Read(data, 0, data.Length);
        //    if (br != fs.Length)
        //        throw new System.IO.IOException(s);
        //    return data;
        //}

        public ActionResult GenerateEnvelope(InputViewModel vm)
        {
            var model = GenerateEnvelope(new EnvelopeInputViewModel
            {
                InputText = vm.InputText,
                SelectedAsymmetricAlgorithmName = vm.SelectedAsymmetricAlgorithmName,
                SelectedAsymmetricAlgorithmKey = vm.SelectedAsymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmName = vm.SelectedSymmetricAlgorithmName,
                SelectedSymmetricAlgorithmKey = vm.SelectedSymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmMode = vm.SelectedSymmetricAlgorithmMode
            });

            return View(model);
        }

        public ActionResult GenerateCertificate(InputViewModel vm)
        {
            var model = GenerateCertificate(new CertificateInputViewModel
            {
                InputText = vm.InputText,
                SelectedAsymmetricAlgorithmName = vm.SelectedAsymmetricAlgorithmName,
                SelectedAsymmetricAlgorithmKey = vm.SelectedAsymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmName = vm.SelectedSymmetricAlgorithmName,
                SelectedSymmetricAlgorithmKey = vm.SelectedSymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmMode = vm.SelectedSymmetricAlgorithmMode
            });

            return View(model);
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

            protected double GetNumberOfLines(int length)
            {
                double numLines = (double)length / Constants.ROW__CHARACTER_COUNT;
                if (Math.Truncate(numLines) < numLines)
                    numLines++;

                return Math.Truncate(numLines);
            }
        }

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
                this.Method = new List<string>()
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
                using (StreamWriter signatureWriter = new StreamWriter(Environment.CurrentDirectory + Constants.File.Path.SIGNATURE + FileName))
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


                    for (int i = 0; i < GetNumberOfLines(Signature.Length); i++)
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

                    while (SignatureStream.ReadLine() != Constants.SIGNATURE)
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
                this.Method = new List<string>()
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
                using (StreamWriter envelopeWriter = new StreamWriter(Environment.CurrentDirectory + Constants.File.Path.ENVELOPE + FileName))
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
                    for (int i = 0; i < GetNumberOfLines(EnvelopeCryptKey.Length); i++)
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
                using (StreamReader envelopeStream = new StreamReader(Environment.CurrentDirectory + Constants.File.Path.ENVELOPE + FileName))
                {
                    string currentLine = "";
                    EnvelopeData = "";
                    EnvelopeCryptKey = "";

                    while ((envelopeStream.ReadLine()) != Constants.ENVELOPE_DATA) ;
                    while ((currentLine = envelopeStream.ReadLine()) != "") EnvelopeData += currentLine.Substring(Constants.TAB.Length);

                    while ((envelopeStream.ReadLine()) != Constants.ENVELOPE_KEY) ;
                    while (((currentLine = envelopeStream.ReadLine()) != Constants.END) && (currentLine != "")) EnvelopeCryptKey += currentLine.Substring(4);

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
                    foreach (var m in Method)
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
                StreamReader RSAFile = new StreamReader(Environment.CurrentDirectory + @"\Keys\" + KeyFile);

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



        public CertificateOutputViewModel GenerateCertificate(CertificateInputViewModel vm)
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

            var model = new CertificateOutputViewModel(_gen, envelope.Data, envelope.Key, hash.AlgorithmName, vm.SelectedSymmetricAlgorithmName, vm.SelectedSymmetricAlgorithmKey, vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey,
            file: "");

            return model;
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