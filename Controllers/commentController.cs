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
namespace youknow.Controllers
{
    public class commentController : Controller
    {
        private timelineEntities db = new timelineEntities();

      
        // GET: /comment/
        //Quản lý comment của user, thêm sửa xóa... tìm kiếm..
        public ActionResult Index(string keyword)
        {
            if (keyword==null) keyword="";
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            var r = (from p in db.comments where p.contents.Contains(keyword) select p).OrderByDescending(o => o.datetime).Take(100);
            return View(r.ToList());
        }

        //
        // GET: /comment/Details/5

        public ActionResult Details(int id = 0)
        {
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //
        // GET: /comment/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /comment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(comment comment)
        {
            if (ModelState.IsValid)
            {
                db.comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(comment);
        }
        
        //Chèn comment bình luận của user vào database, 
        // + và trả về id mới nhất cho phần bình luận để xử lý tiếp
        // + Tăng ranking của tin tức đó lên 1
        [HttpPost,ValidateInput(false)]
        public string InsertComment(comment comment, string contents, int idNews, string usertoken, int idReply, string userTokenReply,string username,string usernamereply)
        {
            contents = Uti.convert_html(contents);
            string newId = "0";
            if (ModelState.IsValid && usertoken != null && usertoken != "")
            {
                comment.contents = contents;
                comment.datetime = DateTime.Now.ToUniversalTime();
                comment.datetimeid = Uti.datetimeid();                
                comment.idnews = idNews;
                comment.usertoken = usertoken;
                comment.fullName = username;
                comment.fullNameReply = usernamereply;
                if (idReply != 0)
                {
                    comment.replyid = idReply;
                    comment.usertokenreplyid = userTokenReply;
                }
                db.comments.Add(comment);
                db.SaveChanges();
                //Trả về id mới nhất của comment này
                newId = comment.id.ToString();
                //Tăng ranking cho tin tức vì có comment
                db.Database.ExecuteSqlCommand("update tinviet_admin.titles set ranking=ranking+1 where id=" + idNews);
            }

            return newId;
        }
        //Cập nhật số like của comment với id nào đó. Và ai thích nó? 
        //Trả về tổng số like mới nhất của comment đó
        //idcomment: id của comment, user là usertoken(mã hóa) của user thích nó.
        [HttpPost]
        public string updateLikeComment(like like, Int64 idcomment, string user)
        {
            if (ModelState.IsValid && user != null && user != "")
            {
                like.idcomment = idcomment;
                like.usertoken = user;
                db.likes.Add(like);
                db.SaveChanges();
            }
            //Trả về tổng số like mới nhất của comment đó
            var c=(from p in db.likes where p.idcomment==idcomment select p.id).Distinct().Count();
            return c.ToString();
        }
        //Lấy ra tổng số like của comment nào đó.
        public string getTotalLike(Int64 idcomment)
        {
            var c = (from p in db.likes where p.idcomment == idcomment select p.id).Distinct().Count();
            return c.ToString();
        }
        //Dịch ngược, lấy ra user name từ usertoken.
        public string getUserNameFromToken(string usertoken) {
            return Uti.getUserNameFromToken(usertoken);
        }
        //Lấy ra các comment của tin tức(idNews) nào đó, mà có id nhỏ hơn lastedid. 
        //Ở đoạn user click vào Xem comment trước, mỗi lần show 5 comment, và cập nhật lastedid ở view bằng biến Javascript
        public string getNewsComment(int idNews, long lastestid)
        {
            var UI = (from p in db.comments where p.idnews == idNews && p.id < lastestid orderby p.id descending join q in db.members on p.usertoken equals q.userToken select new { id = p.id, p.contents, p.usertoken, idOA = q.idOA,typeuser=q.type, datetime = p.datetime, p.fullNameReply, q.fullName, q.email,  linkUser = q.link, p.usertokenreplyid, p.replyid }).Distinct().OrderByDescending(t => t.id).Take(5);
            string json = JsonConvert.SerializeObject(UI.ToList());
            return json;
            //return Json(UI.ToList(), JsonRequestBehavior.AllowGet);
        }
        //Lấy ra các bình luận mới nhất để hiển thị trên trang chủ
        public string getFreshComment()
        {
            var UI = (from p in db.comments join q in db.members 
                      on p.usertoken equals q.userToken into A1 from A2 in A1.DefaultIfEmpty()
                      join t in db.titles on p.idnews equals t.id into A3 from A4 in A3.DefaultIfEmpty()
                      select new {                           
                          fullName=A2.fullName,  
                          fullNameReply=p.fullNameReply,
                          email=A2.email,
                          idNews=A4.id,
                          titles=A4.name,
                          id=p.id
                          }
                      ).Distinct().OrderByDescending(t => t.id).Take(15);
            string json = JsonConvert.SerializeObject(UI.ToList());
            return json;
            //return Json(UI.ToList(), JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /comment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //
        // POST: /comment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        //
        // GET: /comment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            comment comment = db.comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //
        // POST: /comment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comment comment = db.comments.Find(id);
            db.comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //Lấy ra tổng số comment của idNews
        [HttpPost]
        public int getTotalComment(int  idNews) {
            var c = (from r in db.comments where r.idnews == idNews select r.id).Count();
            return c;
        }
        //Đọc các thông số của tin tức có id là id, và trả về cho views readNews
        //Với class đại diện là ModelClassViewComment.
        //lastedtid là biến để biết mỗi khi người dùng click vào Xem comment trước, load 5 comment mới và ghi lại biến 
        //lastedid cho biết người dùng đã xem đến các comment nào thuộc tin tức này, để lần sau load tiếp.
        [HttpGet]
        public ActionResult readNews(int id,long lastestId=-1) {
            try
            {
                if (Request.UrlReferrer != null)
                {
                    ViewBag.PointView = Config.PointView;
                    string refer = Request.UrlReferrer.ToString();
                    if (refer.Contains("facebook"))
                    {
                        ViewBag.PointView = Config.PointViewFacebook;
                    }
                    else
                        if (refer.Contains("google"))
                        {
                            ViewBag.PointView = Config.PointViewGoogle;
                        }

                }
            }
            catch (Exception exRe)
            {

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

            //ViewBag.url = Config.domain+"/comment/readNews/" + id;//rl[0].link;
            ViewBag.link = rl[0].link;
            ViewBag.title = rl[0].title;
            ViewBag.urltitle = rl[0].title + " - " + Uti.smDomain(rl[0].maindomain) + " - " + Uti.getCatNameFromId(rl[0].catid);
            ViewBag.des = rl[0].des;
            ViewBag.fullContent = rl[0].fullContent;
            ViewBag.keyword = rl[0].keyword;
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
        //Đọc các thông số của tin tức có id là id, và trả về cho views readNews
        //Với class đại diện là ModelClassViewComment.
        //lastedtid là biến để biết mỗi khi người dùng click vào Xem comment trước, load 5 comment mới và ghi lại biến 
        //lastedid cho biết người dùng đã xem đến các comment nào thuộc tin tức này, để lần sau load tiếp.
        [HttpGet]
        public ActionResult mReadNews(int id, long lastestId = -1)
        {
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

            //ViewBag.url = Config.domain+"/comment/readNews/" + id;//rl[0].link;
            ViewBag.link = rl[0].link;
            //ViewBag.title = rl[0].title;
            ViewBag.title = rl[0].title;
            ViewBag.urltitle = rl[0].title + " - " + Uti.smDomain(rl[0].maindomain) + " - " + Uti.getCatNameFromId(rl[0].catid);
            ViewBag.des = rl[0].des;
            ViewBag.fullContent = rl[0].fullContent;
            ViewBag.keyword = rl[0].keyword;
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
        //Là chuyên mục http://diemtinvietnam.vn/comment/realTime
        //Hiển thị các comment của các tin, dùng class để lấy ra các item cần thiết.
        public ActionResult realTime()
        {

            ViewBag.url = "diemtinvietnam.vn";
            ViewBag.title = "Trò chuyện trực tuyến - Tin tức Việt Nam tổng hợp - Điểm tin Việt Nam";
            ViewBag.des = "Bình luận tin tức trực tuyến realtime";
            ViewBag.image = "";
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Uti.getUserNameFromInfor(ViewBag.userInfo);
            
            var rs = (from p in db.titles
                      select new viewCommentManagerRealtime
                      {
                          idNews = p.id,
                          image = p.image,
                          title = p.name,
                          des = p.des,
                          link = p.link,
                          datetime=p.datetime,
                          maindomain = p.maindomain,
                          ranking = p.ranking,
                          uplikes = p.uplikes,
                          downlikes = p.downlikes,
                          totalComment = (from c in db.comments where p.id == c.idnews select c.id).Count(),
                          //lastDateTimeComment = (from c2 in db.comments where p.id == c2.idnews select new { lastDateTimeComment = c2.datetime }).OrderByDescending(l => l.lastDateTimeComment).FirstOrDefault(),
                          lastDateTimeComment = (from c2 in db.comments where p.id == c2.idnews select new { lastDateTimeComment = c2.datetime }).Max(o=>o.lastDateTimeComment),

                      }
            ).Where(o=>o.totalComment>0).OrderByDescending(d => d.lastDateTimeComment).ThenByDescending(d => d.idNews).Take(10);
            var rl = rs.ToList();
            var viewModel = new ModelClassViewCommentRealtime { ieViewCommentRealtime = rl };
            return View(viewModel);
        }

        private string getCommentRealtime() {
            var rs = (from p in db.titles
                      select new
                      {
                          idNews = p.id,
                          image = p.image,
                          title = p.name,
                          des = p.des,
                          link = p.link,
                          maindomain = p.maindomain,
                          ranking = p.ranking,
                          uplikes = p.uplikes,
                          downlikes = p.downlikes,
                          totalComment = (from c in db.comments where p.id == c.idnews select c.id).Count(),
                          lastDateTimeComment = (from c2 in db.comments where p.id == c2.idnews select new {maxdatetime=c2.datetime}).OrderByDescending(l=>l.maxdatetime).FirstOrDefault(),

                      }
            ).OrderByDescending(d => d.lastDateTimeComment).ThenByDescending(d => d.idNews);
            return JsonConvert.SerializeObject(rs.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}