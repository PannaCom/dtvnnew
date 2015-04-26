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
    public class DailyKeywordController : Controller
    {
        private timelineEntities db = new timelineEntities();

        //
        // GET: /DailyKeyword/
        //Trang chủ hiển thị DailyKeyword, hiển thị các từ khóa đáng chú ý trong ngày
        //- Thống kê các từ khóa hay xuất hiện ở các tin
        //- Nhóm các tin cùng chủ đề theo từ khóa.
        public ActionResult Index()
        {
            if (Session["username"] == null) return RedirectToAction("Index", "Home");
            //if (Session["login"] == null) return RedirectToAction("Index", "Admin");
            string keyword = Request.Form["keyword"];
            string type = Request.Form["type"];
            string dateView = " and A.datetimeid=B.datetimeid ";
            if (type==null || type.Equals("2"))
            {
                dateView = "";
            }
            int datetimeid = Uti.datetimeidByDay(-1);
            if (string.IsNullOrEmpty(keyword)) keyword = "";
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);

            //var rs = (from p in db.dailykeywords
            //          where p.datetimeid>=datetimeid && p.keyword.Contains(keyword)  
            //          select new viewDailyKeywordManager
            //          { 
            //              id=p.id,
            //              keyword=p.keyword,
            //              title=p.title,
            //              ranking=(from r in db.titles where r.datetimeid>=datetimeid && r.name.Contains(p.keyword) select r).Count(),
            //              status=p.status,
            //              datetime=p.datetime,
            //              datetimeid=p.datetimeid,
            //          }                      
            //          ).Distinct().OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).Take(150);
            string query = "select top 50 A.id,A.title,A.keyword,A.status,A.datetime,A.datetimeid, count(B.idB) as ranking from ";
            query += " (select id,title,keyword,status,ranking as rankKeyword,datetime,datetimeid from tinviet_admin.dailykeyword where  datetimeid>=" + datetimeid + " and ranking>=3 and keyword like '%" + keyword + "%') as A left join ";
            query += " (select id as idB,name,datetimeid  from tinviet_admin.titles where datetimeid>=" + datetimeid + " and name like '%" + keyword + "%') as B on CHARINDEX(A.keyword,B.name)>0 "+dateView;
            query += " group by A.id,A.title,A.keyword,A.status,A.datetime,A.datetimeid order by datetimeid desc,ranking desc";
            var rs = db.Database.SqlQuery<viewDailyKeywordManager>(query);
            var rl = rs.ToList();
            var viewModel = new ModelClassViewDailyKeywordManager { ieViewDailyKeyword = rl };
            return View(viewModel);
        }
        //Tìm tin có chứa từ khóa 
        public string countKeyword(string keyword)
        {
            int datetimeid = Uti.datetimeid();
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            // || p.des.Contains(keyword) || p.keyword.Contains(keyword) 
            int rs = (from p in db.titles where p.name.Contains(keyword) && p.datetimeid >= datetimeid select p).Count();
            return rs.ToString();
            //return Json(rs.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Tìm tin có chứa từ khóa //Tìm tin có chứa từ khóa 
        public string SearchKeyword(string keyword)
        {
            int datetimeid = Uti.datetimeidByDay(-1);
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
            // || p.des.Contains(keyword) || p.keyword.Contains(keyword) 
            var rs = (from p in db.titles where p.name.Contains(keyword) && p.datetimeid >= datetimeid orderby p.datetimeid descending, p.ranking descending select p).Distinct().OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking);
            return JsonConvert.SerializeObject(rs.ToList());
            //return Json(rs.ToList(), JsonRequestBehavior.AllowGet);
        }
        //Tìm tin có chứa từ khóa 
        public string SearchKeywordTrends(string keyword, int datetimeid)
        {
            keyword = System.Web.HttpUtility.HtmlEncode(keyword);
                //System.Web.HttpUtility.HtmlEncode(keyword);
            // || p.des.Contains(keyword) || p.keyword.Contains(keyword) 
            var rs = (from p in db.titles where p.name.Contains(keyword) && p.datetimeid == datetimeid orderby p.datetimeid descending, p.ranking descending select p).Distinct().OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).Take(5);
            return JsonConvert.SerializeObject(rs.ToList());
            //return Json(rs.ToList(), JsonRequestBehavior.AllowGet);
        }

        //Lấy ra các tin cùng chủ đề.
        public string SearchNewsByTopicId(int topicid)
        {   
            var rs = (from p in db.titles where p.topicid==topicid && p.topicid!=p.id orderby p.datetimeid descending, p.ranking descending select p).Distinct().OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).Take(10);
            return JsonConvert.SerializeObject(rs.ToList());
            
        }
        //Active xem từ khóa nào là từ khóa chính của ngày hôm nay
        public string Active(int id,byte type) {
            db.dailykeywords.SingleOrDefault(ItemID => ItemID.id == id).status=type;
            db.SaveChanges();
            return "1";
        }
        //Đặt tên cho chủ đề kèm từ khóa chính
        public string updateTitle(int id, string title)
        {
            db.dailykeywords.SingleOrDefault(ItemID => ItemID.id == id).title = title;
            db.SaveChanges();
            return "1";
        }
        
        //Cập nhật các tin nằm trong list id listNewsId vào cùng 1 chủ đề có id là topicid
        public string updateMapNews(string listNewsId, int topicid)
        {
            string query = "update titles set topicid=" + topicid + " where id in " + listNewsId;
            //db.titles.SqlQuery(query);
            db.Database.ExecuteSqlCommand(query);
            return "1";
        }
        //
        // GET: /DailyKeyword/Details/5

        public ActionResult Details(int id = 0)
        {
            dailykeyword dailykeyword = db.dailykeywords.Find(id);
            if (dailykeyword == null)
            {
                return HttpNotFound();
            }
            return View(dailykeyword);
        }
        

        //
        // GET: /DailyKeyword/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DailyKeyword/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(dailykeyword dailykeyword)
        {
            if (ModelState.IsValid)
            {
                db.dailykeywords.Add(dailykeyword);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dailykeyword);
        }

        //
        // GET: /DailyKeyword/Edit/5

        public ActionResult Edit(int id = 0)
        {
            dailykeyword dailykeyword = db.dailykeywords.Find(id);
            if (dailykeyword == null)
            {
                return HttpNotFound();
            }
            return View(dailykeyword);
        }
        //Xóa tin ra khỏi chủ đề đã gán trước đó.
        //Trả về 1 nếu xóa thành công
        public string removeTopicId(int id){
            title dailykeyword = db.titles.Find(id);
            if (dailykeyword == null)
            {
                return "0";
            }
            dailykeyword.topicid = null;
            db.Entry(dailykeyword).State = EntityState.Modified;
            db.SaveChanges();
            return "1";
        }
        //
        // POST: /DailyKeyword/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(dailykeyword dailykeyword)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailykeyword).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dailykeyword);
        }

        //
        // GET: /DailyKeyword/Delete/5

        public ActionResult Delete(int id = 0)
        {
            dailykeyword dailykeyword = db.dailykeywords.Find(id);
            if (dailykeyword == null)
            {
                return HttpNotFound();
            }
            return View(dailykeyword);
        }

        //
        // POST: /DailyKeyword/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            dailykeyword dailykeyword = db.dailykeywords.Find(id);
            db.dailykeywords.Remove(dailykeyword);
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