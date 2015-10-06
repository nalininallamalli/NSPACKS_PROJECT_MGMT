using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hackathon.Controllers
{
    public class AnalyticsController : Controller
    {
        // GET: Analytics
        public ActionResult AnalyticsView()
        {
            return View();
        }

        public ActionResult _ViewByProjectTab()
        {
            return PartialView();
        }
        public ActionResult _ViewByFundsTab()
        {
            return PartialView();
        }
        public ActionResult _CustomViewTab()
        {
            return PartialView();
        }
    }
}