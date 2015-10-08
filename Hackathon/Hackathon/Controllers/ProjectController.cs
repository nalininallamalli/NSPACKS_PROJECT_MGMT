using Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Hackathon.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        ProjectDbContext projectDb = new ProjectDbContext();

        // GET: Project
        public ActionResult Index()
        {
            var projects = projectDb.Projects.ToList();
            return View(projects);
        }

        // GET: Project/Details/5
        public ActionResult Details(int id)
        {
            return View(projectDb.Projects.Find(id));
        }

        // GET: Project/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        public ActionResult Create(ProjectViewModels model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var project = new Project { Name = model.Name, Description = model.Description, StartDate = model.StartDate, EndDate = model.EndDate };
                    projectDb.Projects.Add(project);
                    projectDb.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            return View(model);
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int id)
        {
            Project project = projectDb.Projects.Find(id);
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ProjectViewModels model)
        {
            try
            {                
                Project project = projectDb.Projects.Find(id);
                project.Name = model.Name;
                project.Description = model.Description;
                project.StartDate = model.StartDate;
                project.EndDate = model.EndDate;

                projectDb.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Project/Delete/5
        public ActionResult Delete(int id = 0)
        {
            return View(projectDb.Projects.Find(id));
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult delete_conf(int id)
        {
            try
            {
                Project project = projectDb.Projects.Find(id);
                projectDb.Projects.Remove(project);
                projectDb.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
