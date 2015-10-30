using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;
using System.IO;
using Microsoft.Owin.Security;
using Microsoft.Reporting.WebForms;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace HackathonPMA.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private Entities db = new Entities();

        // GET: Projects
        public ActionResult Index(string sortBy, string currentFilter, string searchBy, int? page)
        {
            ViewBag.NameSort = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            ViewBag.LocationSort = sortBy == "Location" ? "Location desc" : "Location";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";
            ViewBag.CategorySort = sortBy == "Category" ? "Category desc" : "Category";
            ViewBag.CitySort = sortBy == "City" ? "City desc" : "City";
            ViewBag.StartDateSort = sortBy == "StartDate" ? "StartDate desc" : "StartDate";
            ViewBag.EndDateSort = sortBy == "EndDate" ? "EndDate desc" : "EndDate";

            if (searchBy != null)
            {
                page = 1;
            }
            else
            {
                searchBy = currentFilter;
            }

            ViewBag.CurrentFilter = searchBy;

            List<Project> selProjects = new List<Project>();

            if (!User.IsInRole("Admin"))
            {
                string userId = User.Identity.GetUserId();
                var empProjects = db.EmployeeProjects;

                foreach (var ep in empProjects.ToList())
                {
                    if(ep.EmployeeId.Equals(userId))
                    {
                        Project p = db.Projects.Find(ep.ProjectId);
                        selProjects.Add(p);
                    }
                }
            } else {
                selProjects = db.Projects.ToList();
            }


            var projects = from s in selProjects
                           select s;

            if (!String.IsNullOrEmpty(searchBy))
            {
                projects = projects.Where(s => s.Name.Contains(searchBy)
                                       || s.Description.Contains(searchBy)
                                       || s.Location.Contains(searchBy)
                                       || s.Category.Contains(searchBy)
                                       || s.City.Contains(searchBy)
                                       || s.StartDate.ToString().Contains(searchBy)
                                       || s.EndDate.ToString().Contains(searchBy));
            }

            switch (sortBy)
            {
                case "Name desc":
                    projects = projects.OrderByDescending(s => s.Name);
                    break;
                case  "Name":
                    projects = projects.OrderBy(s => s.Name);
                    break;
                case "Location desc":
                    projects = projects.OrderByDescending(s => s.Location);
                    break;
                case "Location":
                    projects = projects.OrderBy(s => s.Location);
                    break;
                case "StartDate desc":
                    projects = projects.OrderByDescending(s => s.StartDate);
                    break;
                case "StartDate":
                    projects = projects.OrderBy(s => s.StartDate);
                    break;
                case "EndDate desc":
                    projects = projects.OrderByDescending(s => s.EndDate);
                    break;
                case "EndDate":
                    projects = projects.OrderBy(s => s.EndDate);
                    break;
                case "Category desc":
                    projects = projects.OrderByDescending(s => s.Category);
                    break;
                case "Category":
                    projects = projects.OrderBy(s => s.Category);
                    break;
                case "City desc":
                    projects = projects.OrderByDescending(s => s.City);
                    break;
                case "City":
                    projects = projects.OrderBy(s => s.City);
                    break;
                case "Description desc":
                    projects = projects.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    projects = projects.OrderBy(s => s.Description);
                    break;
                default:
                    projects = projects.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(projects.ToPagedList(pageNumber, pageSize));

           // return View(projects.ToList());
        }

        public ActionResult Report(string id)
        {
            LocalReport lr = new LocalReport();

            string path = Path.Combine(Server.MapPath("~/Reports"), "Report.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            List<Project> cm = new List<Project>();

            using (Entities es = new Entities())
            {
                cm = es.Projects.ToList();
            }

            ReportDataSource rd = new ReportDataSource("ProjectDataSet", cm);
            lr.DataSources.Add(rd);

            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType);
        }
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ProjectDetailModel model = new ProjectDetailModel();            
            model.project = project;
            if (project.SpendingDetails != null)
                model.spendingDetails = project.SpendingDetails.Split('^').ToList<string>();

            var applicationDbContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(applicationDbContext));

            var eps = db.EmployeeProjects;
            foreach (var ep in eps.ToList())
            {
                if (ep.ProjectId == project.Id)
                {
                    var Db = new ApplicationDbContext();
                    var user = Db.Users.Find(ep.EmployeeId);
                    if(user != null)
                    {
                        Employee employee = new Employee();
                        employee.user = user;
                        var roles = userManager.GetRoles(user.Id); 
                        
                        if (roles != null)
                            employee.roleName = roles.First();
                        else
                            employee.roleName = "";
                        model.stakeholders.Add(employee);
                    }
                }
            }

            return View(model);
        }

        
        public ActionResult AddSpendings(int Id)
        {
            Project project = db.Projects.Find(Id);
            if(project == null)
            {
                return HttpNotFound();
            }
            EditProjectSpendingsModel model = new EditProjectSpendingsModel();
            model.Id = Id;
            model.projectName = project.Name;
            model.AvailableAmount = project.TotalAllocatedAmount-project.TotalSpentAmount;
            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpendings(EditProjectSpendingsModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = db.Projects.Find(model.Id);
                if(project != null)
                {
                    var spentAmount = project.TotalSpentAmount + Convert.ToDouble(model.spentAmount);
                    if(project.TotalAllocatedAmount < spentAmount)
                    {
                        ViewBag.Message = "Spent Amount can not exceed Available Amount";
                        return View(model);
                    }
                    project.TotalSpentAmount = spentAmount;
                    project.ModifiedOn = DateTime.Now;

                    string temp = model.spentAmount;
                    temp = string.Concat(temp, ":");
                    temp = string.Concat(temp, model.spendingDesc);

                    if ((project.SpendingDetails != null) && (project.SpendingDetails.Length > 0))
                    {
                        temp = string.Concat("^", temp);
                        project.SpendingDetails = string.Concat(project.SpendingDetails, temp);                        
                    }
                    else
                    {
                        project.SpendingDetails = temp;
                    }
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                }               
                
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult CreateSubProject(int Id)
        {
            Project project = db.Projects.Find(Id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.AvailableAmount = project.TotalAllocatedAmount - project.TotalSpentAmount;
            project.TotalAllocatedAmount = 0;
            return View(project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult CreateSubProject([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category")] Project project, string amount)
        {
            if (ModelState.IsValid)
            {
                Double allocatedAmount = 0;
                Project mainproject = db.Projects.Find(project.Id);
                if (mainproject != null)
                {
                    var remaining = mainproject.TotalAllocatedAmount - mainproject.TotalSpentAmount;
                    if((amount != null) && (amount.Length > 0))
                    { 
                        if (!amount.All(char.IsDigit))
                        {
                            ViewBag.AvailableAmount = mainproject.TotalAllocatedAmount - mainproject.TotalSpentAmount;
                            ViewBag.message = "Allocated Amount should be valid number";
                            return View(project);
                        }
                        allocatedAmount = Convert.ToDouble(amount);
                    }
                    if (remaining < allocatedAmount)
                    {
                        ViewBag.AvailableAmount = mainproject.TotalAllocatedAmount - mainproject.TotalSpentAmount;
                        ViewBag.message = "Allocated Amount can not be more than Available Amount";
                        return View(project);
                    }
                    mainproject.TotalSubProjects++;
                    mainproject.TotalSpentAmount = mainproject.TotalSpentAmount + allocatedAmount;
                    if (mainproject.SubProjectIds != null)
                    {
                        if (mainproject.SubProjectIds.Length > 0) { 
                            mainproject.SubProjectIds = string.Concat(mainproject.SubProjectIds, ",");
                        }
                        mainproject.SubProjectIds = string.Concat(mainproject.SubProjectIds, project.Name);
                    }
                    else
                    {
                        mainproject.SubProjectIds = project.Name;
                    }
                    db.Entry(mainproject).State = EntityState.Modified;
                }

                Project subProject = new Project();
                subProject.StartDate = project.StartDate;
                subProject.EndDate = project.EndDate;
                subProject.City = project.City;
                subProject.Location = project.Location;

                subProject.Name = project.Name;
                subProject.Description = project.Description;
                subProject.Category = project.Category;

                subProject.CreatedOn = DateTime.Now;
                subProject.ModifiedOn = DateTime.Now;
                subProject.TotalSpentAmount = 0;
                subProject.ParentProjectId = project.Id;
                subProject.TotalSubProjects = 0;
                subProject.TotalAllocatedAmount = allocatedAmount;
                subProject.IsParent = false;
                subProject.SubProjectIds = "";

                db.Projects.Add(subProject);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }
        // GET: Projects/Create
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create()
        {
            if (TempData["project"] != null)
            {
                TempData["project"] = TempData["project"];

                TempData["fundsMapping"] = TempData["fundsMapping"];
                TempData["hdnUsr"] = TempData["hdnUsr"];
                TempData["hdnRid"] = TempData["hdnRid"];


                Project project = (Project)TempData["project"];
                return View(project);
            }
            return View();  
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category,State")] Project project, string btnAction)
        {
            if (ModelState.IsValid)
            {
                if (btnAction == "Cancel")
                {
                    TempData["project"] = null;
                    TempData["hdnUsr"] = null;
                    TempData["fundsMapping"] = null;
                    return RedirectToAction("Index");
                }
                project.IsParent = true;
                project.CreatedOn = DateTime.Now;
                project.ModifiedOn = DateTime.Now;
                project.TotalAllocatedAmount = 10000;
                project.TotalSpentAmount = 0;
                project.TotalSubProjects = 0;
                project.SubProjectIds = "";                
                if (btnAction == "Next")
                {
                    //TempData["pid"] = id;//
                    TempData["project"] = project;
                    TempData["fundsMapping"] = TempData["fundsMapping"];
                    TempData["hdnUsr"] = TempData["hdnUsr"];
                    TempData["hdnRid"] = TempData["hdnRid"];
                    return RedirectToAction("shMapping", "Account");
                }
                db.Projects.Add(project);
                db.SaveChanges();
                 //ToAdd: start
                int id = project.Id;
                
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category")] Project project, string btnAction)
        {
            if (ModelState.IsValid)
            {
                Project p = db.Projects.Find(project.Id);
                p.Name = project.Name;
                p.Description = project.Description;
                p.StartDate = project.StartDate;
                p.EndDate = project.EndDate;
                p.City = project.City;
                p.Location = project.Location;
                p.Category = project.Category;
                p.ModifiedOn = DateTime.Now;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                //ToAdd:start
                int id = project.Id;
                if (btnAction == "Next")
                {
                    TempData["pid"] = id;
                    return RedirectToAction("shMapping", "Account");
                }
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        public Project FindProjectByName(string name)
        {

            List<Project> allProjects = db.Projects.ToList();
            foreach  (Project p in allProjects)
            {
                if (p.Name.Equals(name))
                    return p;
            }
            return null;
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            if (project != null)
            {
                //Remove all child projects
                if(project.TotalSubProjects > 0)
                {
                    string subPs = project.SubProjectIds;
                    if((subPs != null) && (subPs.Length > 0))
                    {
                        List<string> subPsList = subPs.Split(',').ToList<string>();
                        foreach (string p in subPsList)
                        {
                            Project subp = FindProjectByName(p);
                            if(subp != null)
                            {
                                DeleteConfirmed(subp.Id);
                            }
                        }
                        
                    }

                }

                //update the parent project's subprojectcount 
                int parentProjId = project.ParentProjectId;
                if (parentProjId > 0)
                {
                    Project mainProject = db.Projects.Find(project.ParentProjectId);
                    if (mainProject != null)
                    {
                        mainProject.TotalSubProjects--;
                        Double childRemainingAmount = project.TotalAllocatedAmount - project.TotalSpentAmount;
                        mainProject.TotalSpentAmount = mainProject.TotalSpentAmount - childRemainingAmount;
                        string subProjNames = mainProject.SubProjectIds;
                        if((subProjNames != null) && (subProjNames.Length > 0))
                        {
                            List<string> subProjs = subProjNames.Split(',').ToList<string>();
                            if(subProjs.Contains(project.Name))
                            {
                                subProjs.Remove(project.Name);
                                mainProject.SubProjectIds = string.Join(",", subProjs);
                            }
                        }
                        db.Entry(mainProject).State = EntityState.Modified;
                    }
                }

                db.Projects.Remove(project);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult doesProjectNameExist(string Name, string oldName)
        {
            if (Name.Contains(','))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (oldName.Equals("create") || (Name.Trim().ToLower() != oldName.Trim().ToLower()))
            {
                return Json(!db.Projects.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
