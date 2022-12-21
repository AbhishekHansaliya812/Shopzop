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
    /* RegistrationController to register new user */
    public class RegistrationController : Controller
    {
        #region Index View
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Index - Register User
        [HttpPost]
        public ActionResult Index(RegisrationModel model)
        {
            if (ModelState.IsValid)
            {
                // sending user's data to registration Data Access Layer
                RegistrationDataAccessLayer registrationDataAccessLayer = new RegistrationDataAccessLayer();
                string message = registrationDataAccessLayer.SignUpUser(model);
            }
            else
            {
                return View("~/Views/Registration/Index.cshtml");
            }
            TempData["ReisterMessage"] = "Success";
            // redirecting to Login page
            return RedirectToAction("Index","Login");
        }
        #endregion

        #region Unique UserName
        public JsonResult doesUserNameExist(string UserName)
        {
            ShopzopEntities db = new ShopzopEntities();
            // checking if username already exist in database
            return Json(!db.Users.Any(x => x.UserName == UserName),JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}