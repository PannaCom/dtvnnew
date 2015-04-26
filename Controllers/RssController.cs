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
            //if (url.Contains("giaoduc.net.vn"))
            //{
                
            //    return "";
            //}

            string allLink = "";
            string allImage = "";
            string slink = "";
            string stitle = "";
            string simage = "";
            
            string html;
            string sdes = "";
            string sdate="";
            //using (var wc = new GZipWebClient())
            //    html = wc.DownloadString(url);
            WebClient client = new WebClient();
            var data = client.DownloadData(url);
            html = Encoding.UTF8.GetString(data);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            string content = "";
            content = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'cnn_maintt1imgbul')]").InnerHtml;
            content += doc.DocumentNode.SelectSingleNode("//div[contains(@class,'cnn_relpostn')]").InnerHtml;
            content += doc.DocumentNode.SelectSingleNode("//div[contains(@class,'cnn_mc2nodecntr')]").InnerHtml;
            return content;
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'cnn_maintt1imgbul')]"))
            {
                return link.InnerHtml;
                //HtmlAttribute att = link.Attributes["href"];
                foreach (HtmlNode link2 in link.SelectNodes(".//div[contains(@class,'clearfix')]"))
                {

                    slink = "";
                    slink = link2.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    try
                    {
                        simage = link2.SelectSingleNode(".//a[contains(@class,'')]").SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                    }
                    catch (Exception eximage) { 
                    }
                    stitle = link2.SelectSingleNode(".//h3[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                    sdate = "";
                    try
                    {
                        sdate = link2.SelectSingleNode(".//p[contains(@class,'datetime')]").InnerText;
                    }
                    catch (Exception ex)
                    {
                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;

                    if (!slink.Contains("http://nld.com.vn")) slink = "http://nld.com.vn" + slink;
                    //if (image.Equals("")) continue;
                    //image = image.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                    //string link = link.SelectSingleNode(".//li[contains(@class,'nameAuPost')]").InnerText;
                    if (!sdate.Equals(""))
                    {
                        sdate = sdate.Substring(3, 2) + "/" + sdate.Substring(0, 2) + "/" + DateTime.Now.Year.ToString()+" "+DateTime.Now.ToShortTimeString();
                        sdate = Uti.isDate(sdate);
                        if (sdate.Equals(""))
                        {
                            sdate = DateTime.Now.ToString();
                        }

                    }
                    allLink += stitle + "<img src=" + simage + ">" + sdate;

                }
            }
           
            //ViewBag.allLink = allLink + allImage;
            return allLink;
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