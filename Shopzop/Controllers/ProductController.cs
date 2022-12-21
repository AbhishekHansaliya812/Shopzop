using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Shopzop.Controllers
{
    /* ProductController to view all products, add products, edit products, activate/inactivate products */
    public class ProductController : Controller
    {
        private readonly ShopzopEntities db = new ShopzopEntities();

        #region CategoryList
        // Category List for dropdown
        private SelectListItem[] GetCategoryTypesList()
        {
            SelectListItem[] categoryTypes = null;

            var items = db.Categories.Select(a => new SelectListItem()
            {
                Text = a.CategoryName,
                Value = a.CategoryId.ToString()
            }).ToList();

            categoryTypes = items.ToArray();

            return categoryTypes
                .Select(d => new SelectListItem()
                {
                    Value = d.Value,
                    Text = d.Text
                })
             .ToArray();
        }
        #endregion

        #region Index - Products View
        public ActionResult Index(int? page)
        {
            // check for active User Session
            if (Session["UserID"] == null)
            {
                TempData["NotLogin"] = "Success";
                // redirect to login page
                return RedirectToAction("Index","Login");
            }
            ViewBag.categoryTypes = GetCategoryTypesList();
            ViewBag.Action = "Index";
            var product = db.Products.OrderByDescending(o => o.ProductId).ToList().ToPagedList(page ?? 1, 5);

            // toastr messages for adding/editing/activating/inactivating products 
            if (TempData["AddMessage"] != null)
            {
                ViewBag.AddMessage = "Success";
            }
            else if (TempData["EditMessage"] != null)
            {
                ViewBag.EditMessage = "Success";
            }
            else if (TempData["InactivateMessage"] != null)
            {
                ViewBag.InactivateMessage = "Success";
            }
            else if (TempData["ActivateMessage"] != null)
            {
                ViewBag.ActivateMessage = "Success";
            }
            return View(product);
        }
        #endregion

        #region Add View
        public ActionResult Add()
        {
            // check for active User Session
            if (Session["UserID"] != null)
            {
                ViewBag.categoryTypes = GetCategoryTypesList();
                return View();
            }
            TempData["NotLogin"] = "Success";
            // redirecting to login page
            return RedirectToAction("Index", "Login");
        }
        #endregion

        #region Add Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product product)
        {
            if (ModelState.IsValid)
            {
                // adding product to database
                db.Products.Add(product);
                db.SaveChanges();
                // updating log table in database
                var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                log.UserId = Convert.ToInt32(Session["UserID"]);
                db.SaveChanges();
                TempData["AddMessage"] = "Success";
                // redirecting to products page
                return RedirectToAction("Index");
            }
            return View(product);
        }
        #endregion

        #region Edit View
        public ActionResult Edit(int? id)
        {
            // check for active User Session
            if (Session["UserID"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // fetching details of product
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.categoryTypes = GetCategoryTypesList();
                return View(product);
            }
            TempData["NotLogin"] = "Success";
            // redirecting to login page
            return RedirectToAction("Index", "Login");
        }
        #endregion

        #region Edit Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,ProductPrice,CategoryId,Status,CreateDate")] Product product)
        {
            
            if (ModelState.IsValid)
            {
                // updating changes in product details in database
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                // updating log table in database
                var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                log.UserId = Convert.ToInt32(Session["UserID"]);
                db.SaveChanges();
                TempData["EditMessage"] = "Success";
                // redirecting to products page
                return RedirectToAction("Index");
            }
            return View(product);
        }
        #endregion

        #region Inactivate Product
        public ActionResult Inactivate(int? id)
        {
            // check for active User Session
            if (Session["UserID"] != null)
            {
                // Inactivating product
                var Product = db.Products.Find(id);
                Product.Status = false;
                db.SaveChanges();
                // updating log table in database
                var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                log.UserId = Convert.ToInt32(Session["UserID"]);
                db.SaveChanges();
                TempData["InactivateMessage"] = "Success";
                // redirecting to products page
                return RedirectToAction("Index");
            }
            TempData["NotLogin"] = "Success";
            // redirecting to login page
            return RedirectToAction("Index", "Login");
        }
        #endregion

        #region Activate Product
        public ActionResult Activate(int? id)
        {
            // check for active User Session
            if (Session["UserID"] != null)
            {
                // Activating product
                var Product = db.Products.Find(id);
                Product.Status = true;
                db.SaveChanges();
                // updating log table in database
                var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                log.UserId = Convert.ToInt32(Session["UserID"]);
                db.SaveChanges();
                TempData["ActivateMessage"] = "Success";
                // redirecting to products page
                return RedirectToAction("Index");
            }
            TempData["NotLogin"] = "Success";
            // redirecting to login page
            return RedirectToAction("Index", "Login");
        }
        #endregion

        #region Search
        public ActionResult Search(int? CategoryName, string searchName, int? page)
        {
            // check for active User Session
            if (Session["UserName"] == null)
            {
                TempData["NotLogin"] = "Success";
                // redirecting to login page
                return RedirectToAction("Index", "Login");
            }

            ViewBag.categoryTypes = GetCategoryTypesList();
            ViewBag.Action = "Search";

            // for empty search it will dispaly all products
            if (CategoryName == null && (searchName == null || searchName.IsEmpty()))
            {
                return RedirectToAction("Index");
            }

            // displays searched products 
            else if (CategoryName == null)
            {
                var product = db.Products.Where(s => s.ProductName.Contains(searchName)).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                return View("Index", product);
            }

            // displays category wise products
            else if (CategoryName == null || searchName.IsEmpty())
            {
                var product = db.Products.Where(s => s.CategoryId == CategoryName).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                return View("Index", product);
            }

            // displays category wise and searched products
            else
            {
                var product = db.Products.Where(s => s.CategoryId == CategoryName).Where(s => s.ProductName.Contains(searchName)).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                return View("Index", product);
            }   
        }
        #endregion
    }
}