using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using youknow.Models;
using Newtonsoft.Json;
using PagedList;
namespace youknow.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /News/
        private timelineEntities db = new timelineEntities();
        public ActionResult Index(int? page,string keyword)
        {

            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            if (keyword == null) keyword = "";
            var p = (from q in db.titles where q.name.Contains(keyword) && (q.isHot>=1 || q.catid==0 || q.ranking>=500) select q).OrderByDescending(o => o.id).Take(10000);
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            ViewBag.keyword = keyword;
            return View(p.ToPagedList(pageNumber, pageSize));
        }

        public string getDomainList() {
            var p = (from q in db.domains select new { id = q.id, name = q.name }).OrderBy(o=>o.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        public string getCategoryList()
        {
            var p = (from q in db.categories select new { id = q.id, name = q.name }).OrderBy(o => o.name);
            return JsonConvert.SerializeObject(p.ToList());
        }
        //
        // GET: /News/Details/5

        public ActionResult Details(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            title title = db.titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        //
        // GET: /News/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            return View();
        }

        //
        // POST: /News/Create

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(title title)
        {
            try
            {
                // TODO: Add insert logic here
                //datetime,datetimeid,lastUpdateRanking
                title.fullcontent = HttpUtility.HtmlEncode(title.fullcontent);
                title.datetime = DateTime.Now;
                title.datetimeid = Uti.datetimeid();
                title.lastUpdateRanking = DateTime.Now;
                db.titles.Add(title);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcess(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename + "-" + Guid.NewGuid().ToString());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            for (int i = 0; i < countFile; i++)
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                break;
            }
            //string ok = resizeImage(Config.imgWidthNews, Config.imgHeightNews, fullPath, Config.NewsImagePath + "/" + nameFile);
            return Config.domain+Config.NewsImagePath + "/" + nameFile;
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadImageProcessContent(HttpPostedFileBase file, string filename)
        {
            string physicalPath = HttpContext.Server.MapPath("../" + Config.NewsImagePath + "\\");
            string nameFile = String.Format("{0}.jpg", filename + "-" + Guid.NewGuid().ToString());
            int countFile = Request.Files.Count;
            string fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
            string content = "";
            for (int i = 0; i < countFile; i++)
            {
                nameFile = String.Format("{0}.jpg", filename + "-" + Guid.NewGuid().ToString());
                fullPath = physicalPath + System.IO.Path.GetFileName(nameFile);
                content += "<img src=\"" + Config.domain + Config.NewsImagePath + "/" + nameFile + "\" width=200 height=126>";
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                Request.Files[i].SaveAs(fullPath);
                //break;
            }
            //string ok = resizeImage(Config.imgWidthNews, Config.imgHeightNews, fullPath, Config.NewsImagePath + "/" + nameFile);
            //return Config.NewsImagePath + "/" + nameFile;
            return content;
        }
        //
        // GET: /News/Edit/5

        public ActionResult Edit(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            title title = db.titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        //
        // POST: /News/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(title title)
        {
            try
            {
                // TODO: Add update logic here
                //title.datetime = DateTime.Now;
                //title.datetimeid = Uti.datetimeid();
                //title.lastUpdateRanking = DateTime.Now;
                title.fullcontent = HttpUtility.HtmlEncode(title.fullcontent);
                db.Entry(title).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /News/Delete/5

        public ActionResult Delete(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            title title = db.titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        //
        // POST: /News/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                title title = db.titles.Find(id);
                db.titles.Remove(title);
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
