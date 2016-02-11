using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
namespace youknow.Controllers
{
    public class rssController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /rss/

        public ActionResult Index()
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            return View(db.rsses.ToList());
        }
        [HttpPost]
        [ValidateInput(false)]
        public string getRegex(string regex, string link)
        {
            string fullContent = Config.getTemplateContent(regex, link);
            fullContent = Regex.Replace(fullContent, "<script>(?s).*</script>", "");
            return fullContent;
        }
        [HttpPost]
        [ValidateInput(false)]
        public string getAllContent(string link)
        {
            string fullContent = "";
            fullContent = Config.getFullContent(link);
            return fullContent;
        }
        public string getAllLink(string url)
        {
            string allLinked = "";
            try
            {
                var doc = new HtmlDocument();
                string html;
                using (var wc = new Config.GZipWebClient())
                    html = wc.DownloadString(url);
                //html = DecodeFromUtf8(html);
                doc.LoadHtml(html);
                
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//article[contains(@class,'')]"))
                {
                    string stitle = link.SelectSingleNode(".//h1[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").InnerText;
                    stitle = stitle.Replace("\n", "");
                    allLinked += stitle + "\r\n<br>";
                    string slink = link.SelectSingleNode(".//h1[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    string simage = "";
                    string sdatetime = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//div[contains(@class,'cover')]").Attributes["style"].Value;
                        sdatetime = link.SelectSingleNode(".//time[contains(@class,'')]").Attributes["datetime"].Value;
                        allLinked += simage + "\r\n<br>";
                        allLinked += sdatetime + "\r\n<br>";
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    simage = simage.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                    simage = simage.Replace("background-image:", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                    slink = "http://news.zing.vn" + slink;
                    if (!allLinked.Contains("," + slink + ","))
                    {
                        allLinked += "," + slink + ",";
                    }
                    else continue;
                    sdatetime = Uti.isDate(sdatetime);
                    if (sdatetime.Equals("")) sdatetime = DateTime.Now.ToString();
                    allLinked += simage + "\r\n<br>";
                    allLinked += sdatetime + "\r\n<br>";
                    if (Uti.dateDiff(sdatetime, DateTime.Now.ToString()) > 4)
                    {
                        continue;
                    }
                    
                }
            }
            catch (Exception ex)
            {
            }
            return allLinked;
           
        }
        //
        // GET: /rss/Details/5

        public ActionResult Details(int id = 0)
        {
            rss rss = db.rsses.Find(id);
            if (rss == null)
            {
                return HttpNotFound();
            }
            return View(rss);
        }

        //
        // GET: /rss/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /rss/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(rss rss)
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                if (rss.parentcatid == null) rss.parentcatid = rss.catid;
                db.rsses.Add(rss);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rss);
        }

        //
        // GET: /rss/Edit/5

        public ActionResult Edit(int id = 0)
        {
            rss rss = db.rsses.Find(id);
            if (rss == null)
            {
                return HttpNotFound();
            }
            return View(rss);
        }

        //
        // POST: /rss/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(rss rss)
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(rss).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rss);
        }

        //
        // GET: /rss/Delete/5

        public ActionResult Delete(int id = 0)
        {
            rss rss = db.rsses.Find(id);
            if (rss == null)
            {
                return HttpNotFound();
            }
            return View(rss);
        }

        //
        // POST: /rss/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            rss rss = db.rsses.Find(id);
            db.rsses.Remove(rss);
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