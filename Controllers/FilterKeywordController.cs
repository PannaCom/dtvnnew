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
    public class FilterKeywordController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /FilterKeyword/

        public ActionResult Index()
        {
            return View(db.filterkeywords.ToList());
        }

        //
        // GET: /FilterKeyword/Details/5
        //Trang chủ quản lý các từ khóa lọc tin tức, giảm điểm các tin có từ khóa này
        public ActionResult Details(int id = 0)
        {
            filterkeyword filterkeyword = db.filterkeywords.Find(id);
            if (filterkeyword == null)
            {
                return HttpNotFound();
            }
            return View(filterkeyword);
        }

        //
        // GET: /FilterKeyword/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FilterKeyword/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(filterkeyword filterkeyword)
        {
            if (ModelState.IsValid)
            {
                filterkeyword.datetimeid = Uti.datetimeid();
                db.filterkeywords.Add(filterkeyword);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(filterkeyword);
        }

        //
        // GET: /FilterKeyword/Edit/5

        public ActionResult Edit(int id = 0)
        {
            filterkeyword filterkeyword = db.filterkeywords.Find(id);
            if (filterkeyword == null)
            {
                return HttpNotFound();
            }
            return View(filterkeyword);
        }

        //
        // POST: /FilterKeyword/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(filterkeyword filterkeyword)
        {
            if (ModelState.IsValid)
            {
                db.Entry(filterkeyword).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(filterkeyword);
        }

        //
        // GET: /FilterKeyword/Delete/5

        public ActionResult Delete(int id = 0)
        {
            filterkeyword filterkeyword = db.filterkeywords.Find(id);
            if (filterkeyword == null)
            {
                return HttpNotFound();
            }
            return View(filterkeyword);
        }

        //
        // POST: /FilterKeyword/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            filterkeyword filterkeyword = db.filterkeywords.Find(id);
            db.filterkeywords.Remove(filterkeyword);
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