using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace youknow.Controllers
{
    public class layComment
    {
        public long id { get; set; }
        public string name { get; set; }
        public string contents { get; set; }
        public string fullName { get; set; }
    }

    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        timelineEntities db = new timelineEntities();

        public ActionResult Index()
        {
            //if (Session["username"] == null) return RedirectToAction("Index", "Home");
            //if (Session["username"] != null && !Session["username"].ToString().Contains("admin")) return RedirectToAction("Index", "Home");
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            return View();
        }
        public ActionResult Logout()
        {
            if (Request.Cookies["logged"] != null)
            {
                Response.Cookies["logged"].Expires = DateTime.Now.AddDays(-1);
            }
            Session.Abandon();
            return View();
        }
        public ActionResult Comment()
        {


            return View();
        }

        //function getComment

        public string getComment(string keyword = "", int typeSearch = 0)
        {
            if (keyword != "" && typeSearch != 0)
            {

                string query = "select comments.id,comments.fullName,comments.contents,titles.name from tinviet_admin.comments INNER join tinviet_admin.titles on comments.idnews = titles.id where  contains(";

                if (typeSearch == 1)
                    query += " fullName ,'\"" + Server.HtmlEncode(keyword) + "\"') ";
                else if(typeSearch == 2)
                    query += " contents ,'\"" + keyword + "\"') ";
                else
                    query += " name ,'\"" + Server.HtmlEncode(keyword) + "\"') ";

                List<layComment> rsp = db.Database.SqlQuery<layComment>(query).ToList();

                return JsonConvert.SerializeObject(rsp);
            }
            else
            {
                string query = "SELECT comments.id,comments.fullName,comments.contents,titles.name FROM tinviet_admin.comments INNER JOIN tinviet_admin.titles on comments.idnews = titles.id";
                List<layComment> rsp = db.Database.SqlQuery<layComment>(query).ToList();

                return JsonConvert.SerializeObject(rsp);
            }
        }

        public void editComment(int idComment = 0, string contentComment = "")
        {
            comment cm = db.comments.Find(idComment);
            cm.contents = contentComment;

            db.SaveChanges();
        }

        public void deleteComment(int id = 0)
        {
            comment commentDelete = db.comments.Find(id);

            if (commentDelete != null)
            {
                db.comments.Remove(commentDelete);
                db.SaveChanges();
            }
        }

        public static string ConvertToUnSign(string text)
        {
            for (int i = 33; i < 48; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            for (int i = 58; i < 65; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            for (int i = 91; i < 97; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            for (int i = 123; i < 127; i++)
            {
                text = text.Replace(((char)i).ToString(), "");
            }
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);
            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

        }
        public class listItem {
            public int idnews { get; set; }
            public string titlenews { get; set; }
            public int viewcount { get; set; }
            public int ranking { get; set; }
            public string images { get; set; }
            public DateTime datetime { get; set; }
            public byte hascontent { get; set; }
            public string link { get; set; }
            public int datetimeid { get; set; }
            public int catid { get; set; }
        }
        //public listItem list;//=new listItem[100];
        public string getListNews(string keyword = "",int catid=-1)
        {
            keyword = Server.HtmlEncode(keyword);
            keyword = keyword.Replace(" ", "%");
            string squery = "";

            squery = "select top 100 id as idnews,name as titlenews,totalviews as viewcount,ranking,image as images,datetime,hasContent as hascontent,link,datetimeid,catid from tinviet_admin.titlesSearch where (name like N'%" + keyword + "%' or id=" + keyword + ") ";

            //squery="select top 100 id as idnews,name as titlenews,totalviews as viewcount,ranking,image as images,datetime,hasContent as hascontent,link,datetimeid,catid from tinviet_admin.titlesSearch where name like N'%"+keyword+"%' ";

            if (catid!=-1) squery+=" and catid="+catid;
            squery += " order by datetimeid desc,ranking desc";
            //list = new listItem[100];
            //for (int i = 0; i < 100; i++) {
            //    list[i] = new listItem();
            //}
            //if (string.IsNullOrEmpty(keyword))
            //{
            //    var q = (from n in db.titles
            //             select new abc
            //             {
            //                 idnews = n.id,
            //                 titlenews = n.name,
            //                 viewcount = n.totalviews,
            //                 ranking = n.ranking,
            //                 images = n.image,
            //                 datetime = n.datetime,
            //                 hascontent = n.hasContent,
            //                 link = n.link,
            //                 datetimeid = n.datetimeid,
            //                 catid=n.catid,

            //             });
            //    if (catid != -1)
            //    {
            //        q = q.Where(c => c.catid == catid);
            //    }
            //    q=q.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).Take(100).ToList();
                

            //    //return JsonConvert.SerializeObject(q); 
            //}
            //else
            //{ 
            //    var q = (from n in db.titles where n.name.Contains(keyword)
            //           select new
            //           {
            //               idnews = n.id,
            //               titlenews = n.name,
            //               viewcount = n.totalviews,
            //               ranking = n.ranking,
            //               images = n.image,
            //               datetime = n.datetime,
            //               hascontent = n.hasContent,
            //               link = n.link,
            //               datetimeid = n.datetimeid,
            //               catid = n.catid,

            //           });//
            //    .OrderByDescending(o => o.datetime).ThenByDescending(o => o.ranking).Take(10).ToList();

               
            //}
            try
            {
                var rs = db.Database.SqlQuery<listItem>(squery);
                var rl = rs.ToList();
                return JsonConvert.SerializeObject(rl);
            }
            catch (Exception ex) {
                return "";
            }
            
             
        }

        public void insertNewImages(string link = "", int idPost = 0)
        {
            string query = "UPDATE tinviet_admin.titles SET image = N'" + link + "' WHERE id = " + idPost;

            db.Database.ExecuteSqlCommand(query);
        }

        public void editNews(int idPost = 0, string titleNews = "", int rank = 0)
        {
            string query = "UPDATE tinviet_admin.titles SET name = N'" + titleNews.Replace("'", "''") + "', ranking = " + rank + " WHERE id = " + idPost;

            db.Database.ExecuteSqlCommand(query);
        }

        public void deleteNews(int idNews = 0)
        {
            string query = "delete tinviet_admin.titles  WHERE id = " + idNews;
            db.Database.ExecuteSqlCommand(query);
            query = "delete tinviet_admin.titlesSearch  WHERE id = " + idNews;
            db.Database.ExecuteSqlCommand(query);
        }
    }
}
