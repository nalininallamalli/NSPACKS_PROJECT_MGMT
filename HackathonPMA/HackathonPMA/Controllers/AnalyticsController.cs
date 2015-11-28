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
        ApplicationDbContext employeeDb = new ApplicationDbContext();

        ProjectsController projController = new ProjectsController();

        // GET: Analytics
        public ActionResult AnalyticsView(string ProjectName)
        {
            var model = new ByProjectViewModel();

           /* if (ProjectName != null)
            {
                var listLocations = projectDb.Projects.Where(r => r.Name.Equals(ProjectName)).OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Location.ToString(), Text = rr.Location }).ToList();
                model.LocationList = (IEnumerable<SelectListItem>)listLocations;

                return PartialView("ByProjectChart");
            }*/

            var list = projectDb.Projects.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            model.ProjectList = (IEnumerable<SelectListItem>)list;
            model.LocationList = null;

            return View(model);
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
            var result = projectDb.Projects.Where(p => p.IsParent.Equals(true)).Select(pp => new { pp.Name, pp.TotalAllocatedAmount, pp.TotalSpentAmount }).ToArray();
            List<string> projNameList = new List<string>();
            List<double> projectallocatedList = new List<double>();
            List<double> projectSpentList = new List<double>();

            for (int index = 0; index < result.Length; index++)
            {
                var tempAmount = result.ElementAt(index).TotalAllocatedAmount;
                var spentAmount = result.ElementAt(index).TotalSpentAmount;

                projNameList.Add(result.ElementAt(index).Name);
                projectallocatedList.Add(tempAmount);
                projectSpentList.Add(spentAmount);
            }
            ViewBag.xCol = projNameList.ToArray();
            ViewBag.yCol = projectallocatedList.ToArray();
            ViewBag.zCol = projectSpentList.ToArray();

           // projectDb.EmployeeProjects.Where(p1 => parentProj.Select(p2 => p2.Id).Contains(p1.)).Select(p3 => p3.EmployeeId);
            //projectDb.Projects.GroupBy(pp => pp.City).Where();
          //  projectDb.EmployeeProjects.ToList().Count(e => e.EmployeeId)
            return PartialView();
        }

        public ActionResult ProjEmployeeChart()
        {
            var parentProj = projectDb.Projects.ToList().Where(p => p.IsParent.Equals(true));
            var eps = projectDb.EmployeeProjects;
            int empCount = 0;
            List<string> pNameList = new List<string>();
            List<int> countList = new List<int>();

            foreach (var project in parentProj)
            {
                foreach (var ep in eps.ToList())
                {
                    if (ep.ProjectId == project.Id)
                    {
                        empCount++;
                    }
                }
                pNameList.Add(project.Name);
                countList.Add(empCount);
                empCount = 0;
            }

            ViewBag.aCol = pNameList.ToArray();
            ViewBag.bCol = countList.ToArray();

            return PartialView();
        }

        public ActionResult LocationChart()
        {
            var result = projectDb.Projects.Select(pp => new { pp.City}).ToArray();
            List<string> cityList = new List<string>();
            List<int> countList = new List<int>();

            Dictionary<string, int> projCityDictionary = new Dictionary<string, int>();
          
            for (int index = 0; index < result.Length; index++)
            {
                var tempCity = result.ElementAt(index).City;
                if (tempCity != null)
                {
                    tempCity = tempCity.ToLower();
                    if (projCityDictionary.ContainsKey(tempCity))
                    {
                        projCityDictionary[tempCity] = projCityDictionary[tempCity] + 1;
                    }
                    else
                    {
                        projCityDictionary.Add(tempCity, 1);
                    }
                }
            }
            var enumerator = projCityDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                cityList.Add(pair.Key.ToString().ToUpper());
                countList.Add(pair.Value);
            }
            ViewBag.xCol = cityList.ToArray();
            ViewBag.yCol = countList.ToArray();

            return PartialView();
        }

        public ActionResult FundsInventoryChart() {
           /* var result = projectDb.Projects.Where(p => p.IsParent.Equals(true)).Select(pp => new { pp.Name, pp.TotalAllocatedAmount, pp.TotalSpentAmount }).ToArray();
            List<string> projNameList = new List<string>();
            List<double> projectallocatedList = new List<double>();
            List<double> projectInvList = new List<double>(); */

            var result1 = employeeDb.Users.Select(pp => new { pp.City }).ToArray();
            List<string> cityList = new List<string>();
            List<int> countList = new List<int>();

            Dictionary<string, int> projCityDictionary = new Dictionary<string, int>();

            for (int index = 0; index < result1.Length; index++)
            {
                var tempCity = result1.ElementAt(index).City;
                if (tempCity != null)
                {
                    tempCity = tempCity.ToLower();
                    if (projCityDictionary.ContainsKey(tempCity))
                    {
                        projCityDictionary[tempCity] = projCityDictionary[tempCity] + 1;
                    }
                    else
                    {
                        projCityDictionary.Add(tempCity, 1);
                    }
                }
            }
            var enumerator = projCityDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                cityList.Add(pair.Key.ToString().ToUpper());
                countList.Add(pair.Value);
            }

           /* for (int index = 0; index < result.Length; index++)
            {
                var tempAmount = result.ElementAt(index).TotalAllocatedAmount;
                var availableAmount = (result.ElementAt(index).TotalAllocatedAmount - result.ElementAt(index).TotalSpentAmount);

                projNameList.Add(result.ElementAt(index).Name);
                projectallocatedList.Add(tempAmount);
                projectInvList.Add(availableAmount);
            }
            ViewBag.xCol = projNameList.ToArray();
            ViewBag.yCol = projectallocatedList.ToArray();
            ViewBag.zCol = projectInvList.ToArray(); */
            ViewBag.xCol = cityList.ToArray();
            ViewBag.yCol = countList.ToArray();
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
                double tempAmount = (result.ElementAt(index).amount) * 100 / totalFund;

                projNameList.Add(result.ElementAt(index).Name + " (" + Math.Round(tempAmount, 2) + "%)");
                projellocatedList.Add(Math.Round(tempAmount,2));
                projectsTotal += result.ElementAt(index).amount;
            }
            if (totalFund > projectsTotal)
            {
                var diff = (totalFund - projectsTotal) * 100 / totalFund;
                projNameList.Add("Unallocated" + " (" + Math.Round(diff, 2) + "%)");
                projellocatedList.Add(Math.Round(diff,2));
            }

            var xValue = projNameList.ToArray();
            var yValue = projellocatedList.ToArray();

            ViewBag.xCol = xValue;
            ViewBag.yCol = yValue;

            return PartialView();
        }

        public class MyViewModel
        {
            public string Name  { get; set; }
            public string City { get; set; }
            public int count { get; set; }
        }

        // POST: /Analytics/ByProjectChart
        public ActionResult ByProjectChart(ByProjectViewModel model)
        {
            //projectDb.Projects.Where(x => x.IsParent.Equals(true)).GroupBy(y => y.City).Select(y => new MyViewModel{Name = y.Key})
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