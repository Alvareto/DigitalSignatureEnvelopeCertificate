using System.Text;
using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;
using DigitalSignature.Core.Algorithms.Hash.SHA1;
using DigitalSignature.Core.Algorithms.Hash.SHA2;
using DigitalSignature.Core.Algorithms.Symmetric;
using DigitalSignature.Core.Certificate;
using DigitalSignature.Core.Envelope;
using DigitalSignature.Web.Models.Input;
using DigitalSignature.Web.Models.Output;

namespace DigitalSignature.Web.Helpers
{
    public static class Extensions
    {
        public static SignatureOutputViewModel GenerateSignature(this SignatureInputViewModel vm)
        {
            var inputBytes = Encoding.ASCII.GetBytes(vm.InputText); // new byte[] { };

            IHashAlgorithm hash = GetHashAlgorithm(vm.SelectedHashAlgorithmName);

            IAsymmetricCryptoAlgorithm asymmetric =
                GetAsymmetricAlgorithm(vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey);

            var signature = new Core.Signature.DigitalSignature(hash: hash, algorithm: asymmetric);

            var _sign = signature.Sign(input: inputBytes);

            var valid = signature.Check(_sign);

            var file = "";
            var output = new SignatureOutputViewModel(_sign, vm.SelectedHashAlgorithmName,
                    vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey,
                    file: Constants.File.Name.SIGNATURE)
                {InputText = vm.InputText};

            return output;
        }


        public static EnvelopeOutputViewModel GenerateEnvelope(this EnvelopeInputViewModel vm)
        {
            var inputBytes = Encoding.ASCII.GetBytes(vm.InputText); // new byte[] { };

            ISymmetricCryptoAlgorithm symmetric = GetSymmetricAlgorithm(vm.SelectedSymmetricAlgorithmName,
                vm.SelectedSymmetricAlgorithmKey, vm.SelectedSymmetricAlgorithmMode);

            IAsymmetricCryptoAlgorithm asymmetric =
                GetAsymmetricAlgorithm(vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey);

            var envelope = new DigitalEnvelope(symmetric: symmetric, asymmetric: asymmetric);

            var _env = envelope.Encrypt(input: inputBytes);

            var data = envelope.Decrypt();

            var model = new EnvelopeOutputViewModel(_env.data, _env.cryptKey, vm.SelectedSymmetricAlgorithmName,
                    vm.SelectedSymmetricAlgorithmKey, vm.SelectedAsymmetricAlgorithmName,
                    vm.SelectedAsymmetricAlgorithmKey,
                    file: Constants.File.Name.ENVELOPE)
                {InputText = vm.InputText};

            return model;
        }


        public static CertificateOutputViewModel GenerateCertificate(this CertificateInputViewModel vm)
        {
            var inputBytes = Encoding.ASCII.GetBytes(vm.InputText); // new byte[] { };

            ISymmetricCryptoAlgorithm symmetric = GetSymmetricAlgorithm(vm.SelectedSymmetricAlgorithmName,
                vm.SelectedSymmetricAlgorithmKey, vm.SelectedSymmetricAlgorithmMode);

            IAsymmetricCryptoAlgorithm asymmetric =
                GetAsymmetricAlgorithm(vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey);

            var envelope = new DigitalEnvelope(symmetric: symmetric, asymmetric: asymmetric);

            IHashAlgorithm hash = GetHashAlgorithm(vm.SelectedHashAlgorithmName);

            var signature = new Core.Signature.DigitalSignature(hash: hash, algorithm: asymmetric);

            var certificate = new DigitalCertificate(
                envelope: envelope,
                signature: signature
            );

            byte[] _gen = certificate.Create(input: inputBytes);

            (bool, byte[]) _degen = certificate.Check();

            var model = new CertificateOutputViewModel(_gen, envelope.Data, envelope.Key, hash.AlgorithmName,
                    vm.SelectedSymmetricAlgorithmName, vm.SelectedSymmetricAlgorithmKey,
                    vm.SelectedAsymmetricAlgorithmName, vm.SelectedAsymmetricAlgorithmKey,
                    file: Constants.File.Name.CERTIFICATE)
                {InputText = vm.InputText};

            return model;
        }

        private static ISymmetricCryptoAlgorithm GetSymmetricAlgorithm(this SymmetricAlgorithmName name,
            SymmetricAlgorithmKey keySize, System.Security.Cryptography.CipherMode mode)
        {
            switch (name)
            {
                case SymmetricAlgorithmName.TripleDES:
                    return new TripleDES(keySize: (int) keySize, mode: mode);
                    break;
                case SymmetricAlgorithmName.AES:
                default:
                    return new AES(keySize: (int) keySize, mode: mode);
                    break;
            }
        }

        private static IAsymmetricCryptoAlgorithm GetAsymmetricAlgorithm(this AsymmetricAlgorithmName name,
            AsymmetricAlgorithmKey keySize)
        {
            switch (name)
            {
                case AsymmetricAlgorithmName.ElGamal:
                    return new ElGamal(keySize: (int) keySize);
                    break;
                case AsymmetricAlgorithmName.RSA:
                default:
                    return new RSA(keySize: (int) keySize);
                    break;
            }
        }

        private static IHashAlgorithm GetHashAlgorithm(this HashAlgorithmName name)
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
}