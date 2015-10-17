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
        Entities projectDb = new Entities();

        // GET: Analytics
        public ActionResult AnalyticsView()
        {
            return View();
        }

        public ActionResult _ViewByProjectTab()
        {
            ByProjectViewModel model = new ByProjectViewModel();
            model.ProjectList = new SelectList(projectDb.Projects.ToList(), "Name", "Name");
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
        public ActionResult CustomChart()
        {
            return PartialView();
        }
        public ActionResult ProjectsFundsChart()
        {
            return PartialView();
        }
        public ActionResult ByProjectChart()
        {
            return PartialView();
        }

        [HttpPost, ActionName("Show Graph")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult show_graph()
        {

            return PartialView("ProjectsFundsChart");
        }
    }
}