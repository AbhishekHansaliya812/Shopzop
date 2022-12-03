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
using Microsoft.Ajax.Utilities;
using Shopzop.Common;
using Shopzop.Models;

namespace Shopzop.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                //Password decryptPassword = new Password();
                PasswordBase64 decryptPassword = new PasswordBase64();
                using (ShopzopEntities db = new ShopzopEntities())
                {
                    var obj = db.Users.ToList().Where(model => model.UserName.Equals(user.UserName) && decryptPassword.DecryptPassword(model.Password).Equals(user.Password)).FirstOrDefault();
                    if (obj != null && decryptPassword.DecryptPassword(obj.Password) == user.Password)
                    {
                        Session["UserID"] = obj.UserId.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        return RedirectToAction("Product");
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
            return RedirectToAction("Login");
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Product()
        {
            if (Session["UserID"] != null)
            {
                ShopzopEntities db = new ShopzopEntities();
                return View(db.Products.Where(model => model.Status == true).ToList());
            }

            else if (Session["UserId"] == null)
            {
                return View("Error");
            }

            else
            {
                ViewBag.Message = "User Name or Password is incorrect";
                return RedirectToAction("Login");
            }
        }


        public ActionResult Add()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }

            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product product)
        {
            if (ModelState.IsValid)
            {
                ShopzopEntities db = new ShopzopEntities();
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Product");
            }
            return View(product);
        }


        public ActionResult Edit(int? id)
        {
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShopzopEntities db = new ShopzopEntities();
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,ProductPrice,CategoryId,Status,CreateDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                ShopzopEntities db = new ShopzopEntities();
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Product");
            }
            return View(product);
        }


        public ActionResult Delete(int? id)
        {
            if (Session["UserID"] != null)
            {
                ShopzopEntities db = new ShopzopEntities();
                var Product = db.Products.Find(id);
                Product.Status = false;
                db.SaveChanges();
                return RedirectToAction("Product");
            }
            return View("Error");
        }

        [HttpPost]
        public ActionResult Product(string searchName)
        {
            ShopzopEntities db = new ShopzopEntities();
            if (Session["UserID"] != null)
            {
                if (searchName == "")
                {
                    return View(db.Products.Where(model => model.Status == true).ToList());
                }
                else if (ModelState.IsValid)
                {
                    
                    if (!String.IsNullOrEmpty(searchName))
                    {
                        
                        var search = db.Products.Where(model => model.ProductName.Contains(searchName) && model.Status == true).ToList();
                        return View(search);

                    }
                }
                return View("Product");
            }
            return View("Error");
        }
    }
}