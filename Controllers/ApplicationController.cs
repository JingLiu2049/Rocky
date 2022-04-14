using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky.Data;
using Rocky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Rocky.Controllers
{
    [Authorize(Roles=WC.AdminRole)]
    public class ApplicationController : Controller
    {
        private readonly RockyDbContext _db;
        public ApplicationController(RockyDbContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Application> apps = _db.Application;
            return View(apps);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Application obj)
        {
            if (ModelState.IsValid)
            {
                _db.Application.Add(obj);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
            
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id==0)
            {
                return NotFound();
            }
            Application app = _db.Application.Find(id);
            if(app == null)
            {
                return NotFound();
            }
            return View(app);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Application app)
        {
            if (ModelState.IsValid)
            {
                _db.Application.Update(app);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(app);
            }
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Application app = _db.Application.Find(id);
            if (app == null)
            {
                return NotFound();
            }
            return View(app);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Application app = _db.Application.Find(id);
            if (app == null)
            {
                return NotFound();
            }
            _db.Application.Remove(app);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
