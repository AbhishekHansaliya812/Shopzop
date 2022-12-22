using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using Microsoft.Extensions.Logging;
using NLog;
using PagedList;
using Shopzop.Common;
using Shopzop.Models;
using WebGrease;
using LogManager = NLog.LogManager;

namespace Shopzop.Controllers
{
    /* LoginController to authenticate user while Login and to Logout from application */
    public class LoginController : Controller
    {
        // Private readonly variables
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region Index - Login View
        public ActionResult Index()
        {
            // clears the existing user session
            Session.Clear();

            // to display success message after successful user registration 
            if (TempData["ReisterMessage"] != null)
            {
                ViewBag.SuccessMessage = "Success";
            }
            // to display error message if user is not logged in and trying to use functionality of apllication  
            else if (TempData["NotLogin"] != null)
            {
                ViewBag.NotLogin = "Success";
            }
            return View();
        }
        #endregion

        #region Index - Authenticate User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // intialising object for password decryption
                    Password decryptPassword = new Password();
                    using (ShopzopEntities db = new ShopzopEntities())
                    {
                        // authenticating user name and password
                        var obj = db.Users.ToList().Where(model => model.UserName.Equals(user.UserName) && decryptPassword.DecryptPassword(model.Password).Equals(user.Password)).FirstOrDefault();
                        if (obj != null && decryptPassword.DecryptPassword(obj.Password) == user.Password)
                        {
                            // Intiating User Session
                            Session["UserID"] = obj.UserId.ToString();
                            Session["UserName"] = obj.UserName.ToString();

                            // Redirecting to Product Page
                            return RedirectToAction("Index", "Product");
                        }
                        else
                        {
                            ViewBag.Message = "User Name or Password is incorrect";
                        }
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {
                // log the error message in log file
                logger.Error(ex, "LoginController Index[Post] Action");

                // return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            // Session will be terminated
            Session["UserId"] = null;

            // redirecting to Login page
            return RedirectToAction("Index");
        }
        #endregion

        #region Error Page
        public ActionResult Error()
        {
            return View();
        }
        #endregion
    }
}