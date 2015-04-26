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
    public class HotKeywordController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /HotKeyword/

        public ActionResult Index()
        {
            return View(db.hotkeywords.ToList());
        }

        //
        // GET: /HotKeyword/Details/5

        public ActionResult Details(int id = 0)
        {
            hotkeyword hotkeyword = db.hotkeywords.Find(id);
            if (hotkeyword == null)
            {
                return HttpNotFound();
            }
            return View(hotkeyword);
        }

        //
        // GET: /HotKeyword/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /HotKeyword/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(hotkeyword hotkeyword)
        {
            if (ModelState.IsValid)
            {
                hotkeyword.datetimeid = Uti.datetimeid();
                db.hotkeywords.Add(hotkeyword);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hotkeyword);
        }

        //
        // GET: /HotKeyword/Edit/5

        public ActionResult Edit(int id = 0)
        {
            hotkeyword hotkeyword = db.hotkeywords.Find(id);
            if (hotkeyword == null)
            {
                return HttpNotFound();
            }
            return View(hotkeyword);
        }

        //
        // POST: /HotKeyword/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(hotkeyword hotkeyword)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotkeyword).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hotkeyword);
        }

        //
        // GET: /HotKeyword/Delete/5

        public ActionResult Delete(int id = 0)
        {
            hotkeyword hotkeyword = db.hotkeywords.Find(id);
            if (hotkeyword == null)
            {
                return HttpNotFound();
            }
            return View(hotkeyword);
        }

        //
        // POST: /HotKeyword/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            hotkeyword hotkeyword = db.hotkeywords.Find(id);
            db.hotkeywords.Remove(hotkeyword);
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