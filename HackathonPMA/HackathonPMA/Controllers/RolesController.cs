﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace HackathonPMA.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        //private Entities db = new Entities();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Roles
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortBy, string currentFilter, string searchBy, int? page)
        {
            ViewBag.CurrentSort = sortBy;
            ViewBag.NameSort = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            //ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            if (searchBy != null)
            {
                page = 1;
            }
            else
            {
                searchBy = currentFilter;
            }

            ViewBag.CurrentFilter = searchBy;

            var roles = from s in db.Roles
                        select s;
            switch (sortBy)
            {
                case "Name desc":
                    roles = roles.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    roles = roles.OrderBy(s => s.Name);
                    break;
                default:
                    roles = roles.OrderBy(s => s.Name);
                    break;
            }
            if (!String.IsNullOrEmpty(searchBy))
            {
                roles = roles.Where(s => s.Name.Contains(searchBy));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(roles.ToPagedList(pageNumber, pageSize));
           // return View(roles.ToList());
        }

        // GET: Roles/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        public ActionResult Report(string id)
        {
            LocalReport lr = new LocalReport();

            string path = Path.Combine(Server.MapPath("~/Reports"), "Roles.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            List<AspNetRole> cm = new List<AspNetRole>();

            using (Entities es = new Entities())
            {
                cm = es.AspNetRoles.ToList();
            }

            ReportDataSource rd = new ReportDataSource("RolesDataSet", cm);
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

        // GET: Roles/Create
        public ActionResult Create(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                string message = "This role name has already been used";
                RoleManager<IdentityRole> _roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));
                if (!_roleManager.RoleExists(role.Name))
                {
                    db.Roles.Add(role);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Message = message;
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View(role);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // GET: Roles/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IdentityRole role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            string userId = User.Identity.GetUserId();
            ApplicationDbContext db1 = new ApplicationDbContext();
            var users = db1.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(id)).ToList();
            if (users.Count > 0)
            {
                // Users exist in the selected Role
                ViewBag.Message = "There are users associated with selected Role. Unassociate them from this role and try deleting the role";                
            }
            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            IdentityRole aspNetRole = db.Roles.Find(id);

            string userId = User.Identity.GetUserId();
            ApplicationDbContext db1 = new ApplicationDbContext();
            var users = db1.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(id)).ToList();
            if (users.Count > 0)
            {
                // Users exist in the selected Role
                return RedirectToAction("Delete");
            }

            db.Roles.Remove(aspNetRole);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult doesRoleNameExist(string Name, string oldName)
        {
            if (oldName.Equals("create") || (Name.Trim().ToLower() != oldName.Trim().ToLower()))
            {
                return Json(!db.Roles.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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
