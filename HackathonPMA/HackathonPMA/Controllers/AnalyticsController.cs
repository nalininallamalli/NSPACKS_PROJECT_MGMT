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
        ProjectsController projController = new ProjectsController();

        // GET: Analytics
        public ActionResult AnalyticsView(string ProjectName)
        {
            var model = new ByProjectViewModel();

            var totalFund = projectDb.Funds.ToList().Sum(f => Convert.ToDouble(f.TotalAmount));
            var result = projectDb.Projects.ToList().Where(p => p.IsParent).OrderBy(p => p.Name).Select(pp => new {pp.Name, amount = Convert.ToDouble(pp.TotalAllocatedAmount)}).ToArray();
           /* var yValues = (from p in projectDb.Projects
                           join f in projectDb.FundProjects on p.Id equals f.ProjectId
                           group f by f.ProjectId into projGroup
                           select new 
                           {
                               pName = 
                               amount = projGroup.Sum(fp => Convert.ToDouble(fp.TotalAmount))
                           });*/
            
            List<string> projNameList = new List<string>();
            List<double> projellocatedList = new List<double>();
            double projectsTotal = 0;
            for (int index = 0; index < result.Length; index++)
            {
                projNameList.Add(result.ElementAt(index).Name);
                projellocatedList.Add(result.ElementAt(index).amount);
                projectsTotal += result.ElementAt(index).amount;
            }
            if (totalFund > projectsTotal) {
                projNameList.Add("Unallocated");
                projellocatedList.Add(totalFund - projectsTotal);
            }

            var xValue = projNameList.ToArray();
            var yValue = projellocatedList.ToArray();

            ViewBag.xCol = xValue;
            ViewBag.yCol = yValue;

            if (ProjectName != null)
            {
                var listLocations = projectDb.Projects.Where(r => r.Name.Equals(ProjectName)).OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Location.ToString(), Text = rr.Location }).ToList();
                model.LocationList = (IEnumerable<SelectListItem>)listLocations;

                return PartialView("ByProjectChart");
            }

            var list = projectDb.Projects.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            model.ProjectList = (IEnumerable<SelectListItem>)list;
            model.LocationList = null;

            return View(model);
        }

        [HttpPost]
        public ActionResult LoadLocations(string projName)
        {
            var locations = projectDb.Projects.Where(item => item.Name == projName).Select(x => x);
            return Json(locations.ToList());
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
        public ActionResult CustomChart()
        {
            //projectDb.Projects.GroupBy(pp => pp.City).Select(p2 => new { p2.Name, p2. });
          //  projectDb.EmployeeProjects.ToList().Count(e => e.EmployeeId)
            return PartialView();
        }
        public ActionResult ProjectsFundsChart()
        {
            var model = new ByProjectViewModel();

            var totalFund = projectDb.Funds.ToList().Sum(f => Convert.ToDouble(f.TotalAmount));
            var result = projectDb.Projects.ToList().Where(p => p.IsParent).OrderBy(p => p.Name).Select(pp => new { pp.Name, amount = Convert.ToDouble(pp.TotalAllocatedAmount) }).ToArray();

            List<string> projNameList = new List<string>();
            List<double> projellocatedList = new List<double>();
            double projectsTotal = 0;
            for (int index = 0; index < result.Length; index++)
            {
                var tempAmount = (result.ElementAt(index).amount) * 100 / totalFund;

                projNameList.Add(result.ElementAt(index).Name + " (" + tempAmount + "%)");
                projellocatedList.Add(tempAmount);
                projectsTotal += tempAmount;
            }
            if (totalFund > projectsTotal)
            {
                var diff = (totalFund - projectsTotal) * 100 / totalFund;
                projNameList.Add("Unallocated" + " (" + diff + "%)");
                projellocatedList.Add(diff);
            }

            var xValue = projNameList.ToArray();
            var yValue = projellocatedList.ToArray();

            ViewBag.xCol = xValue;
            ViewBag.yCol = yValue;

            return PartialView();
        }

        // POST: /Analytics/ByProjectChart
        public ActionResult ByProjectChart(ByProjectViewModel model)
        {
           List<Project> projList = new List<Project>();

           findSubProjects(model.ProjectName, projList);
           /* from student in db.Students
               group student by student.EnrollmentDate into dateGroup */
            List<string> projNameList = new List<string>();
            List<double> projFundList = new List<double>();

            for(int i=0; i < projList.Count; i++){
                projNameList.Add(projList.ElementAt(i).Name + " (Rs." + projList.ElementAt(i).TotalAllocatedAmount + ")");
                projFundList.Add(projList.ElementAt(i).TotalAllocatedAmount);
            }

            Project parent = projController.FindProjectByName(model.ProjectName);
            if (parent != null) {
                var remaining = parent.TotalAllocatedAmount - parent.TotalSpentAmount;
                projNameList.Add(model.ProjectName + " (Rs." + remaining + ")");
                projFundList.Add(remaining);
            }

            var xValue = projNameList.ToArray();
            var yValue = projFundList.ToArray();

            ViewBag.xCol = xValue;
            ViewBag.yCol = yValue;
            return PartialView(); //ByProjectChart
        }

        private void findSubProjects(string projName, List<Project> projList)
        {
           Project project = projController.FindProjectByName(projName);
            
           if (project != null)
           {
               //projList.Add(project);

               if (project.TotalSubProjects > 0)
               {
                   string subPs = project.SubProjectIds;
                   if ((subPs != null) && (subPs.Length > 0))
                   {
                       List<string> subPsList = subPs.Split(',').ToList<string>();
                       foreach (string p in subPsList)
                       {
                           Project subp = projController.FindProjectByName(p);
                           projList.Add(subp);

                           if (subp != null)
                           {
                               findSubProjects(p, projList);
                           }
                       }

                   }
               }
           }
        }

        // POST: /Analytics/ShowGraph
        [HttpPost, ActionName("Show Graph")]
        [ValidateAntiForgeryToken]
        public ActionResult ShowGraph()
        {

            return PartialView("ByProjectChart");
        }
    }
}