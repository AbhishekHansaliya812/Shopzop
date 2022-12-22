using NLog;
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

        // Private readonly variables
        private readonly ShopzopEntities db = new ShopzopEntities();
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

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
            try
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
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // log the error message in log file
                logger.Error(ex, "RegistrationController Index[Post] Action");

                // return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Unique UserName
        public JsonResult doesUserNameExist(string UserName)
        {
            // checking if username already exist in database
            return Json(!db.Users.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}