using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;
using PagedList;
using System.Data.Entity.Validation;

namespace HackathonPMA.Controllers
{
    [Authorize]
    public class FundsController : Controller
    {
        private Entities db = new Entities();

        // GET: Funds
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortBy, string currentFilter, string searchBy, int? page)
        {
            ViewBag.CurrentSort = sortBy;
            ViewBag.NameSort = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            ViewBag.AmountSort = sortBy == "Amount" ? "Amount desc" : "Amount";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";

            if (searchBy != null)
            {
                page = 1;
            }
            else
            {
                searchBy = currentFilter;
            }

            ViewBag.CurrentFilter = searchBy;

            var funds = from s in db.Funds
                           select s;
            if (!String.IsNullOrEmpty(searchBy))
            {
                funds = funds.Where(s => s.Name.Contains(searchBy)
                                       || s.TotalAmount.Contains(searchBy)
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
                    funds = funds.OrderByDescending(s => s.TotalAmount);
                    break;
                case "Amount":
                    funds = funds.OrderBy(s => s.TotalAmount);
                    break;
                default:
                    funds = funds.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(funds.ToPagedList(pageNumber, pageSize));
           // return View(funds.ToList());
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
        public ActionResult Create([Bind(Include = "Id,TotalAmount,Description,Name")] Fund fund)
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
        public ActionResult Edit([Bind(Include = "Id,TotalAmount,Description,Name")] Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fund).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fund);
        }
        //ToAdd: start
        public ActionResult fundsMapping()
        {
            TempData["project"] = TempData["project"];
            TempData["hdnRid"] = TempData["hdnRid"];
            //TempData["fundsMapping"] = TempData["fundsMapping"];
            TempData["hdnUsr"] = TempData["hdnUsr"];
            TempData["hdnFunds"] = TempData["hdnFunds"];
            ViewBag.hdnFunds = TempData["hdnFunds"];
            TempData["isEdit"] = TempData["isEdit"];

            Project project = (Project)TempData["project"];
            var fp = from s in db.FundProjects
                     where s.ProjectId == project.Id
                     select s;

            if (Convert.ToString(TempData["isEdit"]) == "1")
            {
                
                var fund = "";
                foreach (FundProject e in fp)
                {
                    fund += "#" + e.FundId + "," + e.SpentAmount.Trim();
                }
                TempData["hdnFunds"] = fund;
                ViewBag.hdnFunds = TempData["hdnFunds"];
            }
            var hdnFunds= Convert.ToString( TempData["hdnFunds"]);
            var funds = from s in db.Funds
                        select s;

            List<Fund> lst = funds.ToList();

            foreach (string s in hdnFunds.Split('#'))
            {
                if (s != null && s != "")
                {
                    if (Convert.ToString(TempData["isEdit"]) == "1")
                    {
                        var rem = Convert.ToInt32(lst.First(d => d.Id == Convert.ToInt32(s.Split(',')[0])).SpentAmount) - Convert.ToInt32(s.Split(',')[1]);
                        lst.First(d => d.Id == Convert.ToInt32(s.Split(',')[0])).SpentAmount=Convert.ToString(rem);
                            
                    }
                }
            }
             TempData["project"] = TempData["project"];
             TempData["hdnRid"] = TempData["hdnRid"];
             //TempData["fundsMapping"] = TempData["fundsMapping"];
             TempData["hdnUsr"] = TempData["hdnUsr"];
             TempData["hdnFunds"] = TempData["hdnFunds"];
             ViewBag.hdnFunds = TempData["hdnFunds"];
             TempData["isEdit"] = TempData["isEdit"];
            return View(lst);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult fundsMapping(List<Fund> lst, string hdnFunds, string btnAction)
        {
            if (btnAction == "Cancel")
            {
                TempData["project"] = null;
                TempData["hdnUsr"] = null;
                TempData["fundsMapping"] = null;
                TempData["hdnUsr"] = null;
                TempData["hdnRid"] = null;

                TempData["hdnFunds"] = null;
                TempData["isEdit"] = null;
                return RedirectToAction("Index", "Projects");
            }
            TempData["project"] = TempData["project"];
            TempData["hdnRid"] = TempData["hdnRid"];
            TempData["hdnUsr"] = TempData["hdnUsr"];
            TempData["hdnFunds"] = hdnFunds;
            var hdnUsr = Convert.ToString(TempData["hdnUsr"]);
            TempData["isEdit"] = TempData["isEdit"];
            if (btnAction == "Back")
            {
                TempData["fundsMapping"] = lst;
                return RedirectToAction("shMapping", "Account");
            }
            //ToDo chk for page
            Project project = (Project)TempData["project"];

            if (Convert.ToString(TempData["isEdit"]) == "1")
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
            }
            else
            {
            db.Projects.Add(project);
            db.SaveChanges();
            }
            //ToAdd: start
            int id = project.Id;
            var Db = new ApplicationDbContext();
            if (Convert.ToString(TempData["isEdit"]) == "1")
            {
                
                var ep = from s in db.EmployeeProjects
                         where s.ProjectId == id
                         select s;
                
                foreach (EmployeeProject e in ep)
                {
                    db.EmployeeProjects.Remove(e);
                }
            }
            foreach (string s in hdnUsr.Split('#'))
            {
                if (s != null && s != "")
                {
                    var cnt = 0;
                    if (db.EmployeeProjects.Count() > 0)
                        cnt = db.EmployeeProjects.Max(x => x.Id);

                    EmployeeProject ep = new EmployeeProject();
                    ep.Id = cnt + 1;
                    ep.EmployeeId = s;
                    ep.ProjectId = Convert.ToInt32(id);

                    db.EmployeeProjects.Add(ep);

                    db.SaveChanges();
                }
            }
            //foreach (Fund f in lst)
            var sAmt = 0;
            var rsAmt = 0.1;
            if (Convert.ToString(TempData["isEdit"]) == "1")
            {

                var fp = from s in db.FundProjects
                         where s.ProjectId == id
                         select s;
                var fundItr = fp.ToList();

                foreach (FundProject e in fundItr)
                {
                    rsAmt += Convert.ToDouble(e.SpentAmount.Trim());

                    Fund p = db.Funds.Find(e.FundId);

                    if (p.SpentAmount == null)
                        p.SpentAmount = "0.0";
                    p.SpentAmount = Convert.ToString(Convert.ToInt32(Convert.ToDouble(p.SpentAmount.Trim()) - Convert.ToDouble(e.SpentAmount.Trim()))  ) ;
                    
                    p.SpentAmount = Convert.ToString(Convert.ToInt32(Convert.ToDouble(p.SpentAmount.Trim())));

                    p.TotalAmount = Convert.ToString(Convert.ToInt32(Convert.ToDouble(p.TotalAmount.Trim())));

                    if (p.SpentAmount == "0" || Convert.ToInt32(p.SpentAmount)<0)
                        p.SpentAmount = null;
                    foreach (FundProject f in p.FundProjects)
                    {
                        if (f.SpentAmount == null)
                            f.SpentAmount = "0.0";

                        f.SpentAmount = Convert.ToString(Convert.ToInt32(Convert.ToDouble(Convert.ToString(f.SpentAmount).Trim())));
                        if (f.SpentAmount == "0" || Convert.ToInt32(f.SpentAmount) < 0)
                            f.SpentAmount = null;
                    }

                    db.Entry(p).State = EntityState.Modified;
                    db.SaveChanges();

                    db.FundProjects.Remove(e);
                }
               
            }
            foreach (string s in hdnFunds.Split('#'))
            {
                if (s != null && s != "")
                {
                        var idf = Convert.ToInt32(s.Split(',')[0]);
                        Fund p = db.Funds.Find(idf);
                        p.TotalAmount = p.TotalAmount.Trim();
                        if (p.SpentAmount == null)
                            p.SpentAmount = "0.0";
                        p.SpentAmount = Convert.ToString(Convert.ToInt32(Convert.ToDouble(Convert.ToString(p.SpentAmount).Trim()) + Convert.ToDouble(s.Split(',')[1])));
                        sAmt += Convert.ToInt32(Convert.ToDouble(s.Split(',')[1]));
                        //sAmt += Convert.ToInt32(Convert.ToInt32(s.Split(',')[1]));
                        p.SpentAmount = p.SpentAmount.Trim();
                        db.Entry(p).State = EntityState.Modified;
                        db.SaveChanges();

                        var cnt = 0;
                        if (db.FundProjects.Count() > 0)
                            cnt = db.FundProjects.Max(x => x.Id);

                        FundProject fp = new FundProject();
                        fp.Id = cnt + 1;
                        fp.FundId = Convert.ToInt32(s.Split(',')[0]);
                        //fp.SpentAmount = Convert.ToString(s.Split(',')[1]);
                        fp.SpentAmount = Convert.ToString(Convert.ToInt32(Convert.ToDouble(s.Split(',')[1])));
                        fp.SpentAmount = fp.SpentAmount.Trim();
                        fp.ProjectId = Convert.ToInt32(id);

                        db.FundProjects.Add(fp);

                        db.SaveChanges();
                }
            }
            Project pe = db.Projects.Find(id);
            pe.TotalAllocatedAmount = Convert.ToInt32(pe.TotalAllocatedAmount) - rsAmt + sAmt;
            db.Entry(pe).State = EntityState.Modified;
            db.SaveChanges();

            TempData["project"] = null;
            TempData["hdnUsr"] = null;
            TempData["fundsMapping"] = null;
            TempData["hdnUsr"] = null;
            TempData["hdnRid"] = null;
            TempData["isEdit"] = null;

            TempData["hdnFunds"] = null;
            return RedirectToAction("Index", "Projects");
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

        public JsonResult doesFundNameExist(string Name, string oldName)
        {
            if (oldName.Equals("create") || (Name.Trim().ToLower() != oldName.Trim().ToLower()))
            { 
                return Json(!db.Funds.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
