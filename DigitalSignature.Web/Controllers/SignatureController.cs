using System;
using System.Web.Mvc;
using DigitalSignature.Web.Helpers;
using DigitalSignature.Web.Models.Input;
using DigitalSignature.Web.Models.Output;

namespace DigitalSignature.Web.Controllers
{
    public class SignatureController : Controller
    {
        // GET: Signature
        public ActionResult Index(SignatureOutputViewModel vm)
        {
            return View(vm);
        }

        // GET: Signature/Create
        public ActionResult Create()
        {
            var model = new SignatureInputViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SignatureInputViewModel vm)
        {
            var model = vm.GenerateSignature();

            return RedirectToAction("Index", model);
        }

        public ActionResult Download()
        {
            throw new NotImplementedException();
        }
    }
}