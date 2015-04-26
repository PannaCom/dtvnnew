using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
using youknow.Views;
namespace youknow.Controllers
{
    public class domainController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /domain/

        public ActionResult Index()
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            return View(db.domains.ToList());
        }

        //
        // GET: /domain/Details/5

        public ActionResult Details(int id = 0)
        {
            domain domain = db.domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        //
        // GET: /domain/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /domain/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(domain domain)
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.domains.Add(domain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(domain);
        }

        //
        // GET: /domain/Edit/5

        public ActionResult Edit(int id = 0)
        {
            domain domain = db.domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        //
        // POST: /domain/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(domain domain)
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(domain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(domain);
        }

        //
        // GET: /domain/Delete/5

        public ActionResult Delete(int id = 0)
        {
            domain domain = db.domains.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        //
        // POST: /domain/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            domain domain = db.domains.Find(id);
            db.domains.Remove(domain);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult DomainNews(int domain, int cat)
        {
            //domain = int.Parse(Request.QueryString["domain"].ToString());
            //cat = int.Parse(Request.QueryString["cat"].ToString());
            Config.loadDomainCatLink();
            Config.CatLinkItem[] catList = Config.domainLink[domain].item;
            //string rs = "";
            //for (int i = 0; i <= 1; i++)
            //{
            //    Config.CatLinkItem[] abc = Config.domainLink[i].item;
            //    rs += Config.domainLink[i].domain + "\r\n";
            //    for (int j = 0; j < abc.Length;j++ )
            //    {
            //        rs += abc[j].catname + ":" + abc[j].link + "\r\n";
            //    }
            //}
            //return rs;
            Rss Rs = null;
            Rs = new Rss(catList[cat].link, -1, Config.domainLink[domain].domain, -1, 10);
            var rl = Rs.arrNewsDomainCat;
            //if (Rs.Length < 5) return RedirectToAction("News", "Home");
            ViewBag.Title = Config.domainLink[domain].domain.Replace("http://", "").Replace("www.", "") + "-" + Config.domainLink[domain].item[cat].catname;
            ViewBag.Url = "/domain/DomainNews/" + domain + "/" + cat;
            ViewBag.Domain = Config.domainLink[domain].domain;
            ViewBag.DomainIndex = domain;
            var viewModel = new ModelClassViewNewsDomainCat { ieViewNewsDomainCat = rl };
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult mDomainNews(int domain, int cat)
        {
            //domain = int.Parse(Request.QueryString["domain"].ToString());
            //cat = int.Parse(Request.QueryString["cat"].ToString());
            Config.loadDomainCatLink();
            Config.CatLinkItem[] catList = Config.domainLink[domain].item;
            //string rs = "";
            //for (int i = 0; i <= 1; i++)
            //{
            //    Config.CatLinkItem[] abc = Config.domainLink[i].item;
            //    rs += Config.domainLink[i].domain + "\r\n";
            //    for (int j = 0; j < abc.Length;j++ )
            //    {
            //        rs += abc[j].catname + ":" + abc[j].link + "\r\n";
            //    }
            //}
            //return rs;
            Rss Rs = null;
            Rs = new Rss(catList[cat].link, -1, Config.domainLink[domain].domain, -1, 10);
            var rl = Rs.arrNewsDomainCat;
            //if (Rs.Length < 5) return RedirectToAction("News", "Home");
            ViewBag.title = Config.domainLink[domain].domain.Replace("http://", "").Replace("www.", "") + "-" + Config.domainLink[domain].item[cat].catname;
            ViewBag.url = "/domain/mDomainNews/" + domain + "/" + cat;
            ViewBag.Domain = Config.domainLink[domain].domain;
            ViewBag.DomainIndex = domain;
            var viewModel = new ModelClassViewNewsDomainCat { ieViewNewsDomainCat = rl };
            return View(viewModel);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}