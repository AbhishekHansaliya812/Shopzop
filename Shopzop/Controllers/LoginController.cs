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
using PagedList;
using Shopzop.Common;
using Shopzop.Models;

namespace Shopzop.Controllers
{
    public class LoginController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                Password decryptPassword = new Password();
                //PasswordBase64 decryptPassword = new PasswordBase64();
                using (ShopzopEntities db = new ShopzopEntities())
                {
                    var obj = db.Users.ToList().Where(model => model.UserName.Equals(user.UserName) && decryptPassword.DecryptPassword(model.Password).Equals(user.Password)).FirstOrDefault();
                    if (obj != null && decryptPassword.DecryptPassword(obj.Password) == user.Password)
                    {
                        Session["UserID"] = obj.UserId.ToString();
                        Session["UserName"] = obj.UserName.ToString();
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

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            return RedirectToAction("Index");
        }
    }
}