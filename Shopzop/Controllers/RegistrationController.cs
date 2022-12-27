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

        #region Index Method
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Index Method - Register User
        [HttpPost]
        public ActionResult Index(RegisrationModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Sending user's data to registration Data Access Layer
                    RegistrationDataAccessLayer registrationDataAccessLayer = new RegistrationDataAccessLayer();
                    string message = registrationDataAccessLayer.SignUpUser(model);
                }
                else
                {
                    return View("~/Views/Registration/Index.cshtml");
                }
                TempData["ReisterMessage"] = "Success";

                // Log the User Registration Information
                logger.Info("New User Registered, User Id :" + " " + model.UserId);

                // Redirecting to Login page
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Log the error message
                logger.Error(ex, "RegistrationController Index[Post] Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region DoesUserNameExist Method - Unique UserName
        public JsonResult DoesUserNameExist(string UserName)
        {
            // Checking if username already exist in database
            return Json(!db.Users.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Error Method
        public ActionResult Error()
        {
            return View();
        }
        #endregion
    }
}