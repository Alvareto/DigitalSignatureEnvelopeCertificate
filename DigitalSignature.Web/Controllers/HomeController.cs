using System.Web.Mvc;
using DigitalSignature.Web.Helpers;
using DigitalSignature.Web.Models;
using DigitalSignature.Web.Models.Input;

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
            var signatureInput = new SignatureInputViewModel
            {
                InputText = vm.InputText,
                SelectedHashAlgorithmName = vm.SelectedHashAlgorithmName,
                SelectedAsymmetricAlgorithmName = vm.SelectedAsymmetricAlgorithmName,
                SelectedAsymmetricAlgorithmKey = vm.SelectedAsymmetricAlgorithmKey
            };

            var model = signatureInput.GenerateSignature();

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
            var envelopeInput = new EnvelopeInputViewModel
            {
                InputText = vm.InputText,
                SelectedAsymmetricAlgorithmName = vm.SelectedAsymmetricAlgorithmName,
                SelectedAsymmetricAlgorithmKey = vm.SelectedAsymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmName = vm.SelectedSymmetricAlgorithmName,
                SelectedSymmetricAlgorithmKey = vm.SelectedSymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmMode = vm.SelectedSymmetricAlgorithmMode
            };

            var model = envelopeInput.GenerateEnvelope();

            return View(model);
        }

        public ActionResult GenerateCertificate(InputViewModel vm)
        {
            var certificateInput = new CertificateInputViewModel
            {
                InputText = vm.InputText,
                SelectedAsymmetricAlgorithmName = vm.SelectedAsymmetricAlgorithmName,
                SelectedAsymmetricAlgorithmKey = vm.SelectedAsymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmName = vm.SelectedSymmetricAlgorithmName,
                SelectedSymmetricAlgorithmKey = vm.SelectedSymmetricAlgorithmKey,
                SelectedSymmetricAlgorithmMode = vm.SelectedSymmetricAlgorithmMode
            };


            var model = certificateInput.GenerateCertificate();

            return View(model);
        }
    }
}