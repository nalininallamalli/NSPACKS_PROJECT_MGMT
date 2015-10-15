using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;


namespace HackathonPMA.Controllers
{
    public class AnalyticsController : Controller
    {
        //ProjectDbContext projectDb = new ProjectDbContext();

        // GET: Analytics
        public ActionResult AnalyticsView()
        {
            return View();
        }

        public ActionResult _ViewByProjectTab()
        {
            ByProjectViewModel model = new ByProjectViewModel();
            //model.ProjectList = new SelectList(projectDb.Projects.ToList(), "Name", "Name");
            return PartialView(model);
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