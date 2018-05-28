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

            //model.FileName = "";

            //string fullName = Path.Combine(Environment.CurrentDirectory, filePath, fileName);

            //byte[] fileBytes = GetFile(fullName);
            //return File(
            //    fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            //model.Write();

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
        }
    }
}