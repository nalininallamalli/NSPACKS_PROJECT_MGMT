using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;

namespace HackathonPMA.Controllers
{
    [Authorize]
    public class FundsController : Controller
    {
        private Entities db = new Entities();

        // GET: Funds
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortBy, string searchBy)
        {
            ViewBag.NameSort = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            ViewBag.AmountSort = sortBy == "Amount" ? "Amount desc" : "Amount";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            var funds = from s in db.Funds
                           select s;
            if (!String.IsNullOrEmpty(searchBy))
            {
                funds = funds.Where(s => s.Name.Contains(searchBy)
                                       || s.Amount.Contains(searchBy)
                                       || s.Description.Contains(searchBy));
            }
            switch (sortBy)
            {
                case "Name desc":
                    funds = funds.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    funds = funds.OrderBy(s => s.Name);
                    break;
                case "Description desc":
                    funds = funds.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    funds = funds.OrderBy(s => s.Description);
                    break;
                case "Amount desc":
                    funds = funds.OrderByDescending(s => s.Amount);
                    break;
                case "Amount":
                    funds = funds.OrderBy(s => s.Amount);
                    break;
                default:
                    funds = funds.OrderBy(s => s.Name);
                    break;
            }
            return View(funds.ToList());
        }

        // GET: Funds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Funds.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        // GET: Funds/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Funds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Amount,Description,Name")] Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Funds.Add(fund);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fund);
        }

        // GET: Funds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Funds.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        // POST: Funds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Amount,Description,Name")] Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fund).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fund);
        }

        // GET: Funds/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fund fund = db.Funds.Find(id);
            if (fund == null)
            {
                return HttpNotFound();
            }
            return View(fund);
        }

        // POST: Funds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Fund fund = db.Funds.Find(id);
            db.Funds.Remove(fund);
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
