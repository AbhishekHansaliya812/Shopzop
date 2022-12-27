using NLog;
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
        
        // Private readonly variables
        private readonly ShopzopEntities db = new ShopzopEntities();
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

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

        #region Index Method - Products
        public ActionResult Index(int? page)
        {
            try
            {
                // Check for active User Session
                if (Session["UserID"] == null)
                {
                    TempData["NotLogin"] = "Success";
                    // Redirect to login page
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.categoryTypes = GetCategoryTypesList();
                ViewBag.Action = "Index";
                var product = db.Products.OrderByDescending(o => o.ProductId).ToList().ToPagedList(page ?? 1, 5);

                // Toastr messages for adding/editing/activating/inactivating products 
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
            catch (Exception ex)
            {
                // Log the error message
                logger.Error(ex, "ProductController Index Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Add Method
        public ActionResult Add()
        {
            try
            {
                // Check for active User Session
                if (Session["UserID"] != null)
                {
                    ViewBag.categoryTypes = GetCategoryTypesList();
                    return View();
                }
                TempData["NotLogin"] = "Success";
                // Redirecting to login page
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Log the error message
                logger.Error(ex, "ProductController Add Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Add Method - Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Adding product to database
                    db.Products.Add(product);
                    db.SaveChanges();
                    // Updating log table in database
                    var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                    log.UserId = Convert.ToInt32(Session["UserID"]);
                    db.SaveChanges();
                    TempData["AddMessage"] = "Success";

                    // Log the added product information
                    logger.Info("Product Added by User Id :" + " " + Session["UserId"] + " , Product Id :" + " " + product.ProductId);

                    // Redirecting to products page
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                // Log the error message in log file
                logger.Error(ex, "ProductController Add[Post] Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Edit Method - Fetch Product Detail
        public ActionResult Edit(int? id)
        {
            try
            {
                // Check for active User Session
                if (Session["UserID"] != null)
                {
                    if (id == null)
                    {
                        ViewBag.errorMessage = "Something went wrong please try again..";
                        return View("Error");
                    }
                    // Fetching details of product
                    Product product = db.Products.Find(id);
                    if (product == null)
                    {
                        ViewBag.errorMessage = "Something went wrong please try again..";
                        return View("Error");
                    }
                    else 
                    {
                        ViewBag.categoryTypes = GetCategoryTypesList();
                        return View(product);
                    }  
                }
                TempData["NotLogin"] = "Success";
                // Redirecting to login page
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Log the error message in log file
                logger.Error(ex, "ProductController Edit Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Edit Method - Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,ProductPrice,CategoryId,Status,CreateDate")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Updating changes in product details in database
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    // Updating log table in database
                    var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                    log.UserId = Convert.ToInt32(Session["UserID"]);
                    db.SaveChanges();
                    TempData["EditMessage"] = "Success";

                    // Log the edited product information
                    logger.Info("Product Edited by User Id :" + " " + Session["UserId"] + " , Product Id :" + " " + product.ProductId);

                    // Redirecting to products page
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                // Log the error message in log file
                logger.Error(ex, "ProductController Edit[Post] Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Inactivate Method - Product
        public ActionResult Inactivate(int? id)
        {
            try
            {
                // Check for active User Session
                if (Session["UserID"] != null)
                {
                    // Inactivating product
                    var product = db.Products.Find(id);
                    product.Status = false;
                    db.SaveChanges();
                    // Updating log table in database
                    var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                    log.UserId = Convert.ToInt32(Session["UserID"]);
                    db.SaveChanges();
                    TempData["InactivateMessage"] = "Success";

                    // Log the Inactivated product information
                    logger.Info("Product Inactivated by User Id :" + " " + Session["UserId"] + " , Product Id :" + " " + product.ProductId);

                    // Redirecting to products page
                    return RedirectToAction("Index");
                }
                TempData["NotLogin"] = "Success";
                // Redirecting to login page
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Log the error message in log file
                logger.Error(ex, "ProductController Inactivate Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Activate Method - Product
        public ActionResult Activate(int? id)
        {
            try
            {
                // Check for active User Session
                if (Session["UserID"] != null)
                {
                    // Activating product
                    var product = db.Products.Find(id);
                    product.Status = true;
                    db.SaveChanges();
                    // Updating log table in database
                    var log = db.ProductsLogs.OrderByDescending(obj => obj.LogId).ToList().FirstOrDefault();
                    log.UserId = Convert.ToInt32(Session["UserID"]);
                    db.SaveChanges();
                    TempData["ActivateMessage"] = "Success";

                    // Log the Activated product information
                    logger.Info("Product Activated by User Id :" + " " + Session["UserId"] + " , Product Id :" + " " + product.ProductId);

                    // Redirecting to products page
                    return RedirectToAction("Index");
                }
                TempData["NotLogin"] = "Success";
                // Redirecting to login page
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Log the error message in log file
                logger.Error(ex, "ProductController Activate Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
        }
        #endregion

        #region Search Method
        public ActionResult Search(int? CategoryName, string searchName, int? page)
        {
            try
            {
                // Check for active User Session
                if (Session["UserName"] == null)
                {
                    TempData["NotLogin"] = "Success";
                    // Redirecting to login page
                    return RedirectToAction("Index", "Login");
                }

                ViewBag.categoryTypes = GetCategoryTypesList();
                ViewBag.Action = "Search";

                // For empty search it will dispaly all products
                if (CategoryName == null && (searchName == null || searchName.IsEmpty()))
                {
                    return RedirectToAction("Index");
                }

                // Displays products searched by name 
                else if (CategoryName == null)
                {
                    var product = db.Products.Where(s => s.ProductName.Contains(searchName)).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                    return View("Index", product);
                }

                // Displays products category wise
                else if (CategoryName == null || searchName.IsEmpty())
                {
                    var product = db.Products.Where(s => s.CategoryId == CategoryName).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                    return View("Index", product);
                }

                // Displays products category wise and searched by name 
                else
                {
                    var product = db.Products.Where(s => s.CategoryId == CategoryName).Where(s => s.ProductName.Contains(searchName)).OrderByDescending(s => s.ProductId).ToList().ToPagedList(page ?? 1, 5);
                    return View("Index", product);
                }
            }
            catch (Exception ex)
            {
                // Log the error message in log file
                logger.Error(ex, "ProductController Search Action");

                // Return the error view with message
                ViewBag.errorMessage = "Something went wrong please try again..";
                return View("Error");
            }
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