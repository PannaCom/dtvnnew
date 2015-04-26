using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
using System.Data.Entity;
using System.Data;
namespace youknow.Controllers
{
    public class BlockController : Controller
    {
        //
        // GET: /Block/
        private timelineEntities db = new timelineEntities();
        public ActionResult Index()
        {

            return View(db.blockurls.ToList());
        }

        //
        // GET: /Block/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Block/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Block/Create

        [HttpPost]
        public ActionResult Create(blockurl blockurl)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    db.blockurls.Add(blockurl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Block/Edit/5

        public ActionResult Edit(int id)
        {
            blockurl blockurl = db.blockurls.Find(id);
            if (blockurl == null)
            {
                return HttpNotFound();
            }
            return View(blockurl);
        }

        //
        // POST: /Block/Edit/5

        [HttpPost]
        public ActionResult Edit(blockurl blockurl)
        {
            try
            {
                // TODO: Add update logic here
                db.Entry(blockurl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Block/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Block/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                blockurl blockurl = db.blockurls.Find(id);
                db.blockurls.Remove(blockurl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
