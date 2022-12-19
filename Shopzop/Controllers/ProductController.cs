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
    public class ProductController : Controller
    {
        public ActionResult Error()
        {
            return View();
        }

        private SelectListItem[] GetCategoryTypesList()
        {
            SelectListItem[] categoryTypes = null;

            ShopzopEntities db = new ShopzopEntities();

            var items = db.Categories.Select(a => new SelectListItem()
            {
                Text = a.CategoryName,
                Value = a.CategoryId.ToString()
            }).ToList();

            categoryTypes = items.ToArray();


            // Have to create new instances via projection
            // to avoid ModelBinding updates to affect this
            // globally
            return categoryTypes
                .Select(d => new SelectListItem()
                {
                    Value = d.Value,
                    Text = d.Text
                })
             .ToArray();
        }

        public ActionResult Index(int? page)
        {
            if (Session["UserID"] == null)
            {
                return View("Error");
            }
            ShopzopEntities db = new ShopzopEntities();
            //return View(db.Products.Where(model => model.Status == true).ToList());
            ViewBag.categoryTypes = GetCategoryTypesList();
            ViewBag.Action = "Index";
            var product = db.Products.OrderByDescending(o => o.ProductId).ToList().ToPagedList(page ?? 1, 5);
            return View(product);
        }

        public ActionResult Add()
        {
            if (Session["UserID"] != null)
            {
                ViewBag.categoryTypes = GetCategoryTypesList();
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
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult Edit(int? id, int? page)
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
                ViewBag.categoryTypes = GetCategoryTypesList();
                ViewBag.ActionMethod = "Edit";
                ViewBag.page = page;
                return View("Index",product);
            }
            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,ProductPrice,CategoryId,Status,CreateDate")] Product product, int pageNum)
        {
            if (ModelState.IsValid)
            {
                ShopzopEntities db = new ShopzopEntities();
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Product", new { page = pageNum});
            }
            return View("Index",product);
        }

        public ActionResult Inactivate(int? id)
        {
            if (Session["UserID"] != null)
            {
                ShopzopEntities db = new ShopzopEntities();
                var Product = db.Products.Find(id);
                Product.Status = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Error");
        }

        public ActionResult Activate(int? id)
        {
            if (Session["UserID"] != null)
            {
                ShopzopEntities db = new ShopzopEntities();
                var Product = db.Products.Find(id);
                Product.Status = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Error");
        }

        public ActionResult Search(int? CategoryName, string searchName, int? page)
        {
            ShopzopEntities db = new ShopzopEntities();
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Error");
            }

            ViewBag.categoryTypes = GetCategoryTypesList();
            ViewBag.Action = "Search";

            if (CategoryName == null && (searchName == null || searchName.IsEmpty()))
            {
                return RedirectToAction("Index");
            }

            else if (CategoryName == null)
            {
                var product = db.Products.Where(s => s.ProductName.Contains(searchName)).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                return View("Index", product);
            }

            else if (CategoryName == null || searchName.IsEmpty())
            {
                var product = db.Products.Where(s => s.CategoryId == CategoryName).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                return View("Index", product);
            }

            else
            {
                var product = db.Products.Where(s => s.CategoryId == CategoryName).Where(s => s.ProductName.Contains(searchName)).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                return View("Index", product);
            }   
        }
    }
}