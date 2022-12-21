using Shopzop.Common;
using Shopzop.DAL;
using Shopzop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shopzop.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(RegisrationModel model)
        {
            if (ModelState.IsValid)
            {
                RegistrationDataAccessLayer registrationDataAccessLayer = new RegistrationDataAccessLayer();
                string message = registrationDataAccessLayer.SignUpUser(model);
            }
            else
            {
                return View("~/Views/Registration/Index.cshtml");
            }
            TempData["ReisterMessage"] = "Success";
            return RedirectToAction("Index","Login");
        }

        public JsonResult doesUserNameExist(string UserName)
        {
            ShopzopEntities db = new ShopzopEntities();
            return Json(!db.Users.Any(x => x.UserName == UserName),JsonRequestBehavior.AllowGet);
        }
    }
}