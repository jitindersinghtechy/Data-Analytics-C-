using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace S2TAnalytics.Web.Controllers.SuperAdmin
{
  
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View("/Views/Admin/Admin.cshtml");
        }
    }
}