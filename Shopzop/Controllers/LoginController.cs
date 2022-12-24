using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        private readonly ShopzopEntities db = new ShopzopEntities();
        private readonly Password decryptPassword = new Password();

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
            else if (TempData["EmailSent"] != null)
            {
                ViewBag.EmailSent = "Success";
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
                            ViewBag.InvalidUserNamePassword = "Success";
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

        #region ForgetPassword View
        public ActionResult ForgetPassword()
        {
            ViewBag.MailInfo = "Success";
            return View();
        }
        #endregion

        #region ForgetPassword Validate Username
        [HttpPost]
        public ActionResult ForgetPassword(string UserName)
        {
            try
            {
                var user = db.Users.ToList().Where(obj => obj.UserName == UserName).FirstOrDefault();

                if (user != null)
                {
                    SendEmail(user.Email);
                }
                else
                {
                    ViewBag.InvalidUserName = "Success";
                    return View();
                }
                TempData["EmailSent"] = "Success";
                // Redirecting to Login Page
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // log the error message in log file
                logger.Error(ex, "LoginController ForgetPassword[Post] Action");

                // return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region SendEmail for Froget Password
        public void SendEmail(string Email)
        {
            var user = db.Users.ToList().Where(obj => obj.Email == Email).FirstOrDefault();

            // Email Address
            var fromEmail = new MailAddress("autodidact.project4@gmail.com", "Shopzop Support Center");
            var toEmail = new MailAddress(Email);

            var fromEmailPassword = "**************";


            // Email Subject
            string subject = "Password Recovery";

            // Email Body
            string body = "<br/> Hello " + user.UserName +
                            "<br/><br/> We have successfuly processed your request for forget password." +
                            "<br/><br/> Your <br/> User Name : " + user.UserName +
                            "<br/>      Password : " + decryptPassword.DecryptPassword(user.Password) +
                            "<br/><br/> We were happy to help you..! :)" +
                            "<br/><br/> Regards" +
                            "<br/> Shopzop Support Center";

            // SMTP Connection
            var smtpRequest = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            // Send Email
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) smtpRequest.Send(message);
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