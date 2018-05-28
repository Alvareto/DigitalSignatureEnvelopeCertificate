using System;
using System.Web.Mvc;
using DigitalSignature.Web.Helpers;
using DigitalSignature.Web.Models.Input;
using DigitalSignature.Web.Models.Output;

namespace DigitalSignature.Web.Controllers
{
    public class EnvelopeController : Controller
    {
        // GET: Envelope
        public ActionResult Index(EnvelopeOutputViewModel vm)
        {
            return View(vm);
        }

        // GET: Envelope/Create
        public ActionResult Create()
        {
            var model = new EnvelopeInputViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EnvelopeInputViewModel vm)
        {
            var model = vm.GenerateEnvelope();

            return RedirectToAction("Index", model);
        }

        public ActionResult Download()
        {
            throw new NotImplementedException();
        }
    }
}