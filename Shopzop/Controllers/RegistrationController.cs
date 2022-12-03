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
        // GET: Registration
        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult Register(RegisrationModel model)
        {
            if (ModelState.IsValid)
            {
                RegistrationDataAccessLayer registrationDataAccessLayer = new RegistrationDataAccessLayer();
                string message = registrationDataAccessLayer.SignUpUser(model);
            }
            else
            {
                return View("~/Views/Registration/Registration.cshtml");
            }

            return RedirectToAction("Login","Login");
        }

        public JsonResult doesUserNameExist(string UserName)
        {
            ShopzopEntities db = new ShopzopEntities();
            return Json(!db.Users.Any(x => x.UserName == UserName),JsonRequestBehavior.AllowGet);
        }
    }
}