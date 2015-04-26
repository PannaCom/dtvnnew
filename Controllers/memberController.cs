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
    public class memberController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /member/

        public ActionResult Index()
        {
            return View(db.members.ToList());
        }

        //
        // GET: /member/Details/5

        public ActionResult Details(int id = 0)
        {
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // GET: /member/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /member/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(member member)
        {
            if (ModelState.IsValid)
            {
                db.members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(member);
        }
        public void setCookieUserName(string v) {
            HttpCookie MyCookie = new HttpCookie("username");
            MyCookie.Value = HttpUtility.UrlEncode(v);
            MyCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(MyCookie);
        }
        public void setCookieUserId(string v)
        {
            HttpCookie MyCookie = new HttpCookie("userid");
            MyCookie.Value = v;
            MyCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(MyCookie);
        }
        public ActionResult addNewMember(member member)
        {
            if (Session["userinfo"] != null && Session["usertoken"] != null)
            {
                string uTK = Session["usertoken"].ToString();
                string uIF = Session["userinfo"].ToString();
                string[] words = uIF.Split('|');
                string smail = words[0];
                string stype = words[1];
                var UI = (from p in db.members where p.userToken.Contains(uTK) select p);
                if (UI.FirstOrDefault() == null)
                {
                    if (words[1].Contains("facebook"))//facebook
                    {
                        member.userToken = uTK;
                        member.email = words[0];
                        member.type = words[1];
                        member.idOA = words[2];
                        member.fullName = words[3];
                        member.link = words[4];
                        db.members.Add(member);
                        db.SaveChanges();
                        int newid = member.id;
                        //Session["userid"] = newid;
                        Session["userid"] = newid; 
                        //Nếu full name mà chưa có thì cũng dẫn đến cập nhật
                        if (words[3] == "" || words[3] == null) return RedirectToAction("Edit", "member", new { id = newid });
                        Session["username"] = words[3];
                        setCookieUserName(words[3]);
                        setCookieUserId(newid.ToString());
                    }
                    else
                    {
                        member.userToken = uTK;
                        member.type = words[1];
                        member.idOA = words[2];
                        if (words[1].Contains("twitter"))
                        {
                            member.fullName = words[0];
                        }
                        else
                        {
                            member.email = words[0];
                        }
                        db.members.Add(member);
                        db.SaveChanges();
                        int newid = member.id;
                        string fullName = member.fullName;
                        Session["userid"] = newid;                       
                        //Nếu full name mà chưa có thì cũng dẫn đến cập nhật
                        if (fullName == "" || fullName == null) return RedirectToAction("Edit", "member", new { id = newid });
                        Session["username"] = fullName;
                        setCookieUserName(fullName);
                        setCookieUserId(newid.ToString());
                    }
                    
                }//Nếu có rồi thì bắt cập nhật fullName
                else {
                    string fullName = UI.ToList()[0].fullName;
                    int newid = UI.ToList()[0].id;                    
                    Session["userid"] = newid;
                    if (fullName == "" || fullName == null) return RedirectToAction("Edit", "member", new { id = newid });
                    Session["username"] = fullName;
                    setCookieUserName(fullName);
                    setCookieUserId(newid.ToString());
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /member/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session["userid"] == null || (int.Parse(Session["userid"].ToString()) != id))
            {
                return RedirectToAction("HotNews", "Home");
            }
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // POST: /member/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                Session["username"] = Server.HtmlEncode(member.fullName);
                //Session["usertoken"] = Config.EncodeStr(Session["username"] + "_" + Session["provider"]);
                //Session["userinfo"] = Session["username"] + "|" + Session["provider"] + "|" + Session["provideruserid"];
                db.SaveChanges();
                HttpCookie MyCookie = new HttpCookie("userinfo");
                DateTime now = DateTime.Now;
                MyCookie = new HttpCookie("username");
                MyCookie.Value = Server.HtmlEncode(Session["username"].ToString());
                MyCookie.Expires = now.AddDays(365);
                Response.Cookies.Add(MyCookie);

                MyCookie = new HttpCookie("userid");
                MyCookie.Value = Server.HtmlEncode(Session["userid"].ToString());
                MyCookie.Expires = now.AddDays(365);
                Response.Cookies.Add(MyCookie);

                return RedirectToAction("Index","Home");
            }
            return View(member);
        }

        //
        // GET: /member/Delete/5

        public ActionResult Delete(int id = 0)
        {
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // POST: /member/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            member member = db.members.Find(id);
            db.members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult MyPage() {
            if (Session["usertoken"] == null) return View();
            try
            {
                string usertoken = Session["usertoken"].ToString();
                string query = "select top 10 title,link,linktoken,idnews,datetime,contents from ";
                query += " (select C.name as title,C.link as link,C.tokenid as linktoken,B.idnews as idnews,B.idContent as idContent from ";
                query += " (select * from tinviet_admin.members where userToken='" + usertoken + "') as A ";
                query += " left join (select userToken,idnews,max(id) as idContent from ";
                query += " tinviet_admin.comments group by userToken,idnews) as B on A.userToken=B.usertoken ";
                query += " left join (select * from tinviet_admin.titles) as C on B.idnews=C.id ";
                query += " ) as D left join ";
                query += " (select id,contents,datetime,datetimeid from tinviet_admin.comments) as E on D.idContent=E.id order by datetime desc";
                var viewModel = new ModelClassViewMyPage { ieViewMyPage = db.Database.SqlQuery<viewMyPageManager>(query).ToList() };
                ViewBag.err = 0;
                return View(viewModel);
            }
            catch (Exception ex) {
                ViewBag.err = 1;
                return View();
            }

        }
        public string getTotalComments() {
            if (Session["usertoken"] == null) return "0";
            string usertoken = Session["usertoken"].ToString();
            var c = (from p in db.comments where p.usertoken.Contains(usertoken) select p.id).Count();
            return c.ToString();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}