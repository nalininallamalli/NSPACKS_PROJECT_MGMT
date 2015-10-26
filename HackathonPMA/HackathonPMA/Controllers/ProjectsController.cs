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
using Microsoft.Reporting.WebForms;
using PagedList;

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

            var projects = from s in db.Projects
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
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create()
        {
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
                db.Projects.Add(project);
                db.SaveChanges();
                 //ToAdd: start
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
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
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

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
    }
}
