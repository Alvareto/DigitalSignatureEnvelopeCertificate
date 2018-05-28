using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Hash;

namespace DigitalSignature.Web.Models.Input
{
    public class SignatureInputViewModel
    {
        public string InputText { get; set; }

        public AsymmetricAlgorithmName SelectedAsymmetricAlgorithmName { get; set; }
        public AsymmetricAlgorithmKey SelectedAsymmetricAlgorithmKey { get; set; }
        public HashAlgorithmName SelectedHashAlgorithmName { get; set; }

    }
}