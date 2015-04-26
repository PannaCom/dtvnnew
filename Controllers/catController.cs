using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;

namespace youknow.Controllers
{
    public class catController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /cat/

        //public ActionResult Index()
        //{
        //    if (Session["username"] == null) return RedirectToAction("Index", "Home");
        //    return View(db.categories.ToList());
        //}

        //
        // GET: /cat/Details/5

        //public ActionResult Details(int id = 0)
        //{
        //    category category = db.categories.Find(id);
        //    if (category == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(category);
        //}

        //
        // GET: /cat/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /cat/Create

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(category category)
        //{
        //    if (Session["username"] == null) return RedirectToAction("Index", "Home");
        //    if (ModelState.IsValid)
        //    {
        //        db.categories.Add(category);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(category);
        //}

        //
        // GET: /cat/Edit/5

        //public ActionResult Edit(int id = 0)
        //{
        //    category category = db.categories.Find(id);
        //    if (category == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(category);
        //}

        //
        // POST: /cat/Edit/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(category).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}

        //
        // GET: /cat/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    category category = db.categories.Find(id);
        //    if (category == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(category);
        //}

        ////
        //// POST: /cat/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    category category = db.categories.Find(id);
        //    db.categories.Remove(category);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}