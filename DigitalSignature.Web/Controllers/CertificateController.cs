using System;
using System.Web.Mvc;
using DigitalSignature.Web.Helpers;
using DigitalSignature.Web.Models.Input;
using DigitalSignature.Web.Models.Output;

namespace DigitalSignature.Web.Controllers
{
    public class CertificateController : Controller
    {
        // GET: Certificate
        public ActionResult Index(CertificateOutputViewModel vm)
        {
            return View(vm);
        }

        // GET: Certificate/Create
        public ActionResult Create()
        {
            var model = new CertificateInputViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CertificateInputViewModel vm)
        {
            var model = vm.GenerateCertificate();

            return RedirectToAction("Index", model);
        }

        public ActionResult Download()
        {
            throw new NotImplementedException();
        }
    }
}