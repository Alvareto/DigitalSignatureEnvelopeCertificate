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
    }
}