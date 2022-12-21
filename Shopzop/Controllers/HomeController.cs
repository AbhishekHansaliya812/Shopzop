using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopzop.Controllers
{
    /* Controller for Home Page (Dashboard) */
    public class HomeController : Controller
    {
        #region Index
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}