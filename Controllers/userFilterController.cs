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
    public class userFilterController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /userFilter/

        public ActionResult Index()
        {
            return View(db.userFilters.ToList());
        }

        //
        // GET: /userFilter/Details/5

        public ActionResult Details(string id = null)
        {
            userFilter userfilter = db.userFilters.Find(id);
            if (userfilter == null)
            {
                return HttpNotFound();
            }
            return View(userfilter);
        }

        //
        // GET: /userFilter/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /userFilter/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(userFilter userfilter)
        {
            if (ModelState.IsValid)
            {
                db.userFilters.Add(userfilter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userfilter);
        }

        //
        // GET: /userFilter/Edit/5

        public ActionResult Edit(string id = null)
        {
            userFilter userfilter = db.userFilters.Find(id);
            if (userfilter == null)
            {
                return HttpNotFound();
            }
            return View(userfilter);
        }

        //
        // POST: /userFilter/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(userFilter userfilter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userfilter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userfilter);
        }

        //
        // GET: /userFilter/Delete/5

        public ActionResult Delete(string id = null)
        {
            userFilter userfilter = db.userFilters.Find(id);
            if (userfilter == null)
            {
                return HttpNotFound();
            }
            return View(userfilter);
        }

        //
        // POST: /userFilter/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            userFilter userfilter = db.userFilters.Find(id);
            db.userFilters.Remove(userfilter);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult getDomain()
        {   
            var result = (from p in db.domains orderby p.id select p).Distinct().Take(10000);

            return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult getCategory()
        //{
        //    var result = (from p in db.categories orderby p.id select p).Distinct().Take(10000);

        //    return Json(result.ToList(), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult getUserFilterEdit()
        {
            string uID = Session["userinfo"].ToString();
            var UI = (from p in db.userFilters where p.userId == uID select new { p.domain, p.catid}).Distinct().Take(10000);
            return Json(UI.ToList(), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult updateDomainUserFilter(userFilter UF,IEnumerable<HttpPostedFileBase> file, FormCollection fc)
        {

            if (ModelState.IsValid)
            {
                string strFilter = ",domain:-1,";
                int domainId = 0;
                int catId = 0;
                string uID = Session["userinfo"].ToString();
                var UI = (from p in db.userFilters where p.userId==uID select p);
                foreach(var item in UI){
                    db.userFilters.Remove(item);
                }
                //if (UI!=null) db.userFilters.Remove(UI); 
                db.SaveChanges();
                //db.SubmitChanges();
                foreach (var key in fc.AllKeys)
                {
                    var value = fc[key];
                    if (key.Contains("domainId")) domainId = int.Parse(value);
                    if (key.Contains("catId")) catId = int.Parse(value);
                    if (domainId != 0)
                    {
                        strFilter += ",domain:" + domainId + ',';
                        UF.domain = domainId;
                        UF.catid = null;
                        UF.userId = Session["userinfo"].ToString();
                        //update database description of images
                        db.userFilters.Add(UF);
                        db.SaveChanges();
                        domainId =0;
                        catId = 0;
                    }
                    if (catId != 0)
                    {
                        strFilter += ",cat:" + catId + ',';
                        UF.domain = null;
                        UF.catid = catId;
                        UF.userId = Session["userinfo"].ToString();
                        //update database description of images
                        db.userFilters.Add(UF);
                        db.SaveChanges();
                        catId = 0;
                        domainId = 0;
                    }
                   
                }
                //Ghi ra cookie
                //get user information 
                //Huynv added 22/09/2013
                try
                {
                    Session["userfilter"] = strFilter;
                    //Add cookie
                    HttpCookie MyCookie = new HttpCookie("userfilter");
                    DateTime now = DateTime.Now;
                    MyCookie.Value = Session["userfilter"].ToString();
                    MyCookie.Expires = now.AddDays(365);
                    Response.Cookies.Add(MyCookie);
                }
                catch (Exception ex) { 

                }
                
            }
            //return "1";
            return RedirectToAction("Index");
        }
       
    }
}