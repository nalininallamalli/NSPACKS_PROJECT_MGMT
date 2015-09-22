using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    public class Default2Controller : Controller
    {
        private Manage db = new Manage();

        //
        // GET: /Default2/

        public ActionResult Index()
        {
            return View(db.project.ToList());
        }

        //
        // GET: /Default2/Details/5

        public ActionResult Details(int id = 0)
        {
            Projects projects = db.project.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        //
        // GET: /Default2/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Default2/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.project.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        //
        // GET: /Default2/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Projects projects = db.project.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        //
        // POST: /Default2/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        //
        // GET: /Default2/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Projects projects = db.project.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        //
        // POST: /Default2/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.project.Find(id);
            db.project.Remove(projects);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}