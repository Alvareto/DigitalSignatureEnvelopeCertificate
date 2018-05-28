using DigitalSignature.Core.Algorithms.Asymmetric;
using DigitalSignature.Core.Algorithms.Symmetric;

namespace DigitalSignature.Web.Models.Input
{
    public class EnvelopeInputViewModel
    {
        public string InputText { get; set; }

        public AsymmetricAlgorithmName SelectedAsymmetricAlgorithmName { get; set; }
        public AsymmetricAlgorithmKey SelectedAsymmetricAlgorithmKey { get; set; }
        public SymmetricAlgorithmName SelectedSymmetricAlgorithmName { get; set; }
        public System.Security.Cryptography.CipherMode SelectedSymmetricAlgorithmMode { get; set; }
        public SymmetricAlgorithmKey SelectedSymmetricAlgorithmKey { get; set; }

    }
}