using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
using Newtonsoft.Json;
using youknow.Views;
using System.Text;
using System.Text.RegularExpressions;

namespace youknow.Controllers
{
    public class TitleController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /Title/

        public ActionResult Index(string keyword)
        {
            if (keyword == null) keyword = "";
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            var r = (from p in db.titles where p.name.Contains(keyword) select p).OrderByDescending(o => o.datetime).Take(100);
            return View(r.ToList());
        }

        //
        // GET: /Title/Details/5

        public ActionResult Details(int id = 0)
        {
            try
            {
                //if (Request.Browser.IsMobileDevice) {
                //    return RedirectToAction("readNews", "comment", new { id = id });
                //}
                ViewBag.PointView = Config.PointView;
                if (Request.UrlReferrer != null)
                {
                    ViewBag.PointView = Config.PointView;
                    string refer=Request.UrlReferrer.ToString();
                    if (refer.Contains("facebook")){
                        ViewBag.PointView = Config.PointViewFacebook;
                    }
                    else
                        if (refer.Contains("google"))
                        {
                            ViewBag.PointView = Config.PointViewGoogle;
                        }
                       
                }
            }
            catch (Exception exRe) { 

            }
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.idNews = id;

            var rs = (from p in db.titles
                      where p.id == id
                      join q in db.comments on p.id equals q.idnews into A1
                      from A2 in A1.DefaultIfEmpty()
                      join o in db.members on A2.usertoken equals o.userToken into A3
                      from A4 in A3.DefaultIfEmpty()
                      select new viewCommentManager
                      {
                          id = A2.id,
                          idNews = A2.idnews,
                          image = p.image,
                          userName = A4.fullName,
                          userNameReply = A2.fullNameReply,
                          userToken = A2.usertoken,
                          email = A4.email,
                          datetimenews = p.datetime,
                          datetime = A2.datetime,
                          datetimeid = A2.datetimeid,
                          title = p.name,
                          des = p.des,
                          fullContent = p.fullcontent,
                          keyword = p.keyword,
                          link = p.link,
                          contents = A2.contents,
                          userTokenReply = A2.usertokenreplyid,
                          maindomain = p.maindomain,
                          ranking = p.ranking,
                          uplikes = p.uplikes,
                          downlikes = p.downlikes,
                          topicid = p.topicid,
                          catid = p.catid,
                      }
                      ).OrderByDescending(t => (long?)t.id).Take(5);

            
            try
            {
                if (rs.ToList().Count <= 0)//.ToList()[0].id
                {
                    rs = (from p in db.titlesSearches
                          where p.id == id
                          join q in db.comments on p.id equals q.idnews into A1
                          from A2 in A1.DefaultIfEmpty()
                          join o in db.members on A2.usertoken equals o.userToken into A3
                          from A4 in A3.DefaultIfEmpty()
                          select new viewCommentManager
                          {
                              id = A2.id,
                              idNews = A2.idnews,
                              image = p.image,
                              userName = A4.fullName,
                              userNameReply = A2.fullNameReply,
                              userToken = A2.usertoken,
                              email = A4.email,
                              datetimenews = p.datetime,
                              datetime = A2.datetime,
                              datetimeid = A2.datetimeid,
                              title = p.name,
                              des = p.des,
                              fullContent = p.fullcontent,
                              keyword = p.keyword,
                              link = p.link,
                              contents = A2.contents,
                              userTokenReply = A2.usertokenreplyid,
                              maindomain = p.maindomain,
                              ranking = p.ranking,
                              uplikes = p.uplikes,
                              downlikes = p.downlikes,
                              topicid = p.topicid,
                              catid = p.catid,
                          }
                         ).OrderByDescending(t => (long?)t.id).Take(5);

                }
            }
            catch (Exception ex)
            {
            }
           
            var rl = rs.ToList();

            ViewBag.murl = Config.domain+"/comment/mReadNews/" + id;
            //ViewBag.purl = Config.domain + "/comment/ReadNews/" + id;
            ViewBag.link = rl[0].link;
            ViewBag.title = rl[0].title+" - "+Uti.smDomain(rl[0].maindomain)+" - "+Uti.getCatNameFromId(rl[0].catid);
            ViewBag.rawTitle = rl[0].title;
            ViewBag.des = rl[0].des;
            ViewBag.fullContent = rl[0].fullContent;
            ViewBag.keyword = rl[0].keyword;
            if (ViewBag.keyword == "") { ViewBag.keyword = ViewBag.title; }
            ViewBag.image = rl[0].image;
            ViewBag.ranking = rl[0].ranking;
            ViewBag.uplikes = rl[0].uplikes;
            ViewBag.downlikes = rl[0].downlikes;
            ViewBag.datetimenews = rl[0].datetimenews;
            ViewBag.maindomain = rl[0].maindomain;
            ViewBag.catid = rl[0].catid;
            if (rl[0].topicid != null && rl[0].topicid != 0)
            {
                ViewBag.topicid = rl[0].topicid;
            }
            else
            {
                ViewBag.topicid = -1;
            }
            //ViewBag.userName = rl[0].userName;
            var viewModel = new ModelClassViewComment { ieViewComment = rl };
            return View(viewModel);
        }
       
        //
        // GET: /Title/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Title/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(title title)
        {
            if (ModelState.IsValid)
            {
                db.titles.Add(title);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(title);
        }

        //
        // GET: /Title/Edit/5

        public ActionResult Edit(int id = 0)
        {
            title title = db.titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

        //
        // POST: /Title/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(title title)
        {
            if (ModelState.IsValid)
            {
                db.Entry(title).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(title);
        }

        //
        // GET: /Title/Delete/5

        public ActionResult Delete(int id = 0)
        {
            title title = db.titles.Find(id);
            if (title == null)
            {
                return HttpNotFound();
            }
            return View(title);
        }

         

        //
        // POST: /Title/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            title title = db.titles.Find(id);
            db.titles.Remove(title);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public string Search(string keyword){
            //keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            //var rs = (from p in db.titles where p.name.Contains(keyword) || p.des.Contains(keyword) || p.keyword.Contains(keyword) orderby p.datetime descending, p.ranking descending select new { id=p.id,token = p.tokenid, title = p.name, des = p.des, image = p.image,source=p.source, link = p.link, date = p.datetime, ranking = p.ranking, totalcomment = p.totalcomment, maindomain = p.maindomain,p.uplikes,p.downlikes,catid=p.catid,hasContent=p.hasContent}).Take(100);
            //return JsonConvert.SerializeObject(rs.ToList());
            //return Json(rs.ToList(), JsonRequestBehavior.AllowGet);
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            //var rs = (from p in db.titles where p.name.Contains(keyword) || p.des.Contains(keyword) || p.keyword.Contains(keyword) orderby p.datetime descending, p.ranking descending select new { id = p.id, token = p.tokenid, title = p.name, des = p.des, image = p.image, source = p.source, link = p.link, date = p.datetime, ranking = p.ranking, totalcomment = p.totalcomment, maindomain = p.maindomain, p.uplikes, p.downlikes, catid = p.catid, hasContent = p.hasContent }).Take(100);
            string query = "SELECT top 100 ";
            query += " FT_TBL.id,FT_TBL.name as title,FT_TBL.source,FT_TBL.datetime as date, ";
            query += " FT_TBL.datetimeid as datetimeid,FT_TBL.ranking as ranking,FT_TBL.link as link, ";
            query += " FT_TBL.image as image,FT_TBL.totalcomment as totalcomment,FT_TBL.maindomain as maindomain, ";
            query += " FT_TBL.uplikes as uplikes,FT_TBL.downlikes as downlikes,FT_TBL.topicid as topicid,FT_TBL.catid as catid, ";
            query += " FT_TBL.hasContent as hasContent, KEY_TBL.RANK FROM tinviet_admin.titles AS FT_TBL INNER JOIN FREETEXTTABLE(titles, name,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,datetimeid desc,ranking desc";

            var rs = db.Database.SqlQuery<viewNewsSearchManager>(query).Distinct();
            var rl = rs.ToList();
            return JsonConvert.SerializeObject(rs.ToList());
        }
        
        public string SearchMobile(string keyword)
        {
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            //var rs = (from p in db.titles where p.name.Contains(keyword) || p.des.Contains(keyword) || p.keyword.Contains(keyword) orderby p.datetime descending, p.ranking descending select new { id = p.id, token = p.tokenid, title = p.name, des = p.des, image = p.image, source = p.source, link = p.link, date = p.datetime, ranking = p.ranking, totalcomment = p.totalcomment, maindomain = p.maindomain, p.uplikes, p.downlikes, catid = p.catid, hasContent = p.hasContent }).Take(100);
            string query = "SELECT top 100 ";
            query += " FT_TBL.id,FT_TBL.name as title,FT_TBL.source,FT_TBL.datetime as date, ";
            query += " FT_TBL.datetimeid as datetimeid,FT_TBL.ranking as ranking,FT_TBL.link as link, ";
            query += " FT_TBL.image as image,FT_TBL.totalcomment as totalcomment,FT_TBL.maindomain as maindomain, ";
            query += " FT_TBL.uplikes as uplikes,FT_TBL.downlikes as downlikes,FT_TBL.topicid as topicid,FT_TBL.catid as catid, ";
            query += " FT_TBL.hasContent as hasContent, KEY_TBL.RANK FROM tinviet_admin.titles AS FT_TBL INNER JOIN FREETEXTTABLE(tinviet_admin.titles, name,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,datetimeid desc,ranking desc";
            
            var rs = db.Database.SqlQuery<viewNewsSearchManager>(query).Distinct();
            var rl = rs.ToList();
            return JsonConvert.SerializeObject(rs.ToList());
            //return Json(rs.ToList(), JsonRequestBehavior.AllowGet);
        }
        public string searchMobileApp(string keyword) {

            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            //var rs = (from p in db.titles where p.name.Contains(keyword) || p.des.Contains(keyword) || p.keyword.Contains(keyword) orderby p.datetime descending, p.ranking descending select new { id = p.id, token = p.tokenid, title = p.name, des = p.des, image = p.image, source = p.source, link = p.link, date = p.datetime, ranking = p.ranking, totalcomment = p.totalcomment, maindomain = p.maindomain, p.uplikes, p.downlikes, catid = p.catid, hasContent = p.hasContent }).Take(100);
            string query = "SELECT top 100 ";
            query += " FT_TBL.id,FT_TBL.name as title,FT_TBL.source,FT_TBL.datetime as date,datediff(minute,FT_TBL.datetime,getdate()) as timediff,";
            query += " FT_TBL.datetimeid as datetimeid,FT_TBL.ranking as ranking,FT_TBL.link as link, ";
            query += " FT_TBL.image as image,FT_TBL.totalcomment as totalcomment,FT_TBL.maindomain as maindomain, ";
            query += " FT_TBL.uplikes as uplikes,FT_TBL.downlikes as downlikes,FT_TBL.topicid as topicid,FT_TBL.catid as catid, ";
            query += " FT_TBL.hasContent as hasContent, KEY_TBL.RANK FROM tinviet_admin.titles AS FT_TBL INNER JOIN FREETEXTTABLE(tinviet_admin.titles, name,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,datetimeid desc,ranking desc";

            var rs = db.Database.SqlQuery<viewNewsSearchManager>(query).Distinct();
            var rl = rs.ToList();
            return JsonConvert.SerializeObject(rs.ToList());
        }
        //public JsonResult getDetail(string tokenid)
        //{
        //    var result = (from p in db.titles where p.tokenid.Contains(tokenid) select p).Distinct().Take(1);
        //    return Json( result.ToList(), JsonRequestBehavior.AllowGet);
        //}
        
        public ActionResult getTitle(string tokenid)
        {
            ViewBag.tokenid = tokenid;
            if (Session["usertoken"]!=null) ViewBag.userToken = Session["usertoken"].ToString();
            if (Session["userinfo"]!=null){
                string fullName = Session["userinfo"].ToString().Split('|')[3];
                ViewBag.userFullName = fullName;
            }
            return View();
        }
        [HttpGet]
        public ActionResult getDomainContent(string url)
        {
            string content = getAllLinkContent(url);
            content = Server.HtmlEncode(content);
            ViewBag.fullContent = content;
            ViewBag.url = url;
            ViewBag.title = "";
            return View();
        }
        public ActionResult getFullContent(int id=0) {
            title title = db.titles.Find(id);
            //if (title == null)
            //{
            //    return HttpNotFound();
            //}
            ViewBag.url = Config.domain + "/Title/getFullContent?id=" + id;

            return View(title);
        }
        private string getAllLinkContent(string url) {
            string fullContent = "";
            fullContent = Config.getFullContent(url);
            //Bỏ hết các link
            //fullContent = Regex.Replace(fullContent, "href=\"(.*?)\"", "");
            return fullContent;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}