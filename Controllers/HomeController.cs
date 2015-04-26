using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Data.Entity;
using System.Data;
using youknow.Models;
using youknow.Views;
namespace youknow.Controllers
{
    public class HomeController : Controller
    {
        //IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        Rss Rs = null;
        private timelineEntities db = new timelineEntities();
        private void getCookieUser() {
            try
            {
                if (Session["userinfo"] == null)
                {
                    //if (User.Identity.Name == null || User.Identity.Name == "")
                    //{
                    if (System.Web.HttpContext.Current.Request.Cookies["userinfo"] != null) Session["userinfo"] = (string)System.Web.HttpContext.Current.Request.Cookies["userinfo"].Value;
                    //}
                }
                if (Session["usertoken"] == null)
                {
                    //if (User.Identity.Name == null || User.Identity.Name == "")
                    //{
                    if (System.Web.HttpContext.Current.Request.Cookies["usertoken"] != null) Session["usertoken"] = (string)System.Web.HttpContext.Current.Request.Cookies["usertoken"].Value;
                    //}
                }
                else ViewBag.UserToken = Session["usertoken"].ToString();
                if (Session["username"] == null)
                {

                    if (System.Web.HttpContext.Current.Request.Cookies["username"] != null) Session["username"] = HttpUtility.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["username"].Value);

                }
                //else 
                ViewBag.userName = Session["username"].ToString();
                //ViewBag.userName = Server.HtmlDecode(ViewBag.userName);

                if (Session["userid"] == null)
                {

                    if (System.Web.HttpContext.Current.Request.Cookies["userid"] != null) Session["userid"] = (string)System.Web.HttpContext.Current.Request.Cookies["userid"].Value;

                }

                //if (Session["userfilter"] == null)
                //{
                //    Session["userfilter"] = (string)System.Web.HttpContext.Current.Request.Cookies["userfilter"].Value;
                //    if (Session["userfilter"] == null) Session["userfilter"] = "";
                //}
            }
            catch (Exception ex)
            {

            }
        }
        //Trang chủ, trả về view Index, lấy ra cookie userinfo và ghi vào session
        [ValidateInput(false)]
        public ActionResult Index()
        {
            //return RedirectToAction("TinNong", "Home");
            ViewBag.Message = "Home.";
            ViewBag.Title = "Home.";
            ViewBag.UserToken = "";
            getCookieUser();
            //return PartialView("~/Views/Home/Default.cshtml");
            //if (Request.Browser.IsMobileDevice)
            //    return RedirectToAction("mHotNews", "Home");
            //else
              return RedirectToAction("HotNews", "Home");
           
        }
        //Trang chủ dành riêng cho Mobile
        public ActionResult Mobile()
        {
            return RedirectToAction("HotNews", "Home");
            //return RedirectToAction("mHotNews", "Home");
            //return View();
        }
        //function này bỏ hiện ko dùng
        //Lấy ra các tin có domain là domain, từ file xml
        public string getNews(int domain,int catId)
        {
            Rs = new Rss(HttpRuntime.AppDomainAppPath + domain + "_" + catId + ".xml", domain,"", catId, 2);
            string json = JsonConvert.SerializeObject(Rs.arrItem);
            Rs.Clear();
            return json;
        }
        //mNew.xml, Chuyên mục, mvc for Mobile App
        public string getNewsMobileApp()
        {
            //ViewBag.userToken = Session["usertoken"];
            //ViewBag.userInfo = Session["userinfo"];
            //ViewBag.userName = Session["username"];
            //int datetimeid = Uti.datetimeid();            
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "mNew.xml", 0, "", 0, 9);
            var rl = Rs.arrNewsMobileAppManager.ToList();
            //var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            string json = JsonConvert.SerializeObject(rl.ToList());
            return json;
        }

        //mAllNew.xml, Chuyên mục, mvc for Mobile App
        public string getCatNewsMobileApp(int catid)
        {
            //ViewBag.userToken = Session["usertoken"];
            //ViewBag.userInfo = Session["userinfo"];
            //ViewBag.userName = Session["username"];
            //int datetimeid = Uti.datetimeid();            
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "mCat_"+catid+".xml", 0, "", 0, 9);
            var rl = Rs.arrNewsMobileAppManager.ToList();
            //var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            string json = JsonConvert.SerializeObject(rl.ToList());
            return json;
        }
        //cat.xml, All Tin nóng chuyên mục, mvc for Mobile App new way
        public string getCatNewsMobileApp2(int catid)
        {
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Cat_" + catid + ".xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
        }        
        public ActionResult testRegex() {
            return View();
        }

        //Trả về danh sách Category của trang báo có domain thứ tự=index
        public string getCatListFromDomainRssApp(int index) {
            Config.loadDomainCatLink();
            Config.CatLinkItem[] catList = Config.domainLink[index].item;
            var rl = catList.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
        }
        [HttpGet]
        public ActionResult DomainNews(int domain, int cat=0) {
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
        public string getContentRssFromUrlApp(int indexDomain,int indexCat) {

            Config.loadDomainCatLink();
            Config.CatLinkItem[] catList = Config.domainLink[indexDomain].item;
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
            Rs = new Rss(catList[indexCat].link, -1, Config.domainLink[indexDomain].domain, -1, 10);
            var rl = Rs.arrNewsDomainCat.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
        }
        //AllNew.xml, All Tin nóng, mvc for Mobile App new way
        public string getAllNewsMobileApp2()
        {
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "AllNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
        }
        public string getTopReadNewsMobileApp2()
        {
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "TopReadNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
            //string query = "select top 10 name as title,id,totalviews from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and totalviews>0 order by datetimeid desc,totalviews desc";
            //var rs = db.Database.SqlQuery<viewNewsManager>(query).Distinct();
            //string json = JsonConvert.SerializeObject(rs.ToList());
            ////json = Server.HtmlDecode(json);
            //return json;
        }
        //mAllNew.xml, Điểm Tin, mvc for Mobile App
        public string getAllNewsMobileApp()
        {
            //ViewBag.userToken = Session["usertoken"];
            //ViewBag.userInfo = Session["userinfo"];
            //ViewBag.userName = Session["username"];
            //int datetimeid = Uti.datetimeid();            
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "mAllNew.xml", 0, "", 0, 9);
            var rl = Rs.arrNewsMobileAppManager.ToList();
            //var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            string json = JsonConvert.SerializeObject(rl.ToList());
            return json;
        }
        //New.xml, Tin nóng, mvc for Mobile App new way
        public string getLatestNewsMobileApp2()
        {
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "New.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
        }
        //HotNew.xml, Tin nóng, mvc for Mobile App new way
        public string getHotNewsMobileApp2()
        {
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "HotNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            string json = JsonConvert.SerializeObject(rl);
            return json;
        }
        //mHotNew.xml, Tin nóng, mvc for Mobile App
        public string getHotNewsMobileApp()
        {
            //ViewBag.userToken = Session["usertoken"];
            //ViewBag.userInfo = Session["userinfo"];
            //ViewBag.userName = Session["username"];
            //int datetimeid = Uti.datetimeid();            
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "mHotNew.xml", 0, "", 0, 9);
            var rl = Rs.arrNewsMobileAppManager.ToList();
            //var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            string json = JsonConvert.SerializeObject(rl.ToList());
            return json;
        }
        //Trens, xu hướng đọc, mvc for Mobile App
        public string getTrendsMobile()
        {
            //ViewBag.userToken = Session["usertoken"];
            //ViewBag.userInfo = Session["userinfo"];
            //ViewBag.userName = Session["username"];
            //int datetimeid = Uti.datetimeid();            
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl = Rs.arrTrendsManager.ToList();
            //var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            string json = JsonConvert.SerializeObject(rl.ToList());
            return json;
        }
        //Trens, xu hướng đọc, mvc for Mobile WEB
        public ActionResult mTrends()
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            int datetimeid = Uti.datetimeid();

            //var rs = (from p in db.dailykeywords where p.status == 1 && p.datetimeid==datetimeid
            ////join t in db.titles on Uti.getContain(p.keyword,t.name) && t.datetimdid equals datetimeid                                                                        
            //          select new viewTrendsManager
            //    { 
            //        title=p.title,
            //        keyword=p.keyword,
            //        rankKeyword=p.ranking,
            //        datetimeid=p.datetimeid,
            //        //id=t.id,
            //        //name=t.name,
            //        //link=t.link,
            //        //hasContent=t.hasContent,
            //        //maindomain=t.maindomain,                   
            //    }).Distinct();
            //rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.rankKeyword);
            //rs = rs.Take(20);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl = Rs.arrTrendsManager.ToList();
            var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            return View(viewModel);
        }
        //Trens, xu hướng đọc, mvc
        public ActionResult Trends() {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            int datetimeid = Uti.datetimeid();
            
            //var rs = (from p in db.dailykeywords where p.status == 1 && p.datetimeid==datetimeid
            ////join t in db.titles on Uti.getContain(p.keyword,t.name) && t.datetimdid equals datetimeid                                                                        
            //          select new viewTrendsManager
            //    { 
            //        title=p.title,
            //        keyword=p.keyword,
            //        rankKeyword=p.ranking,
            //        datetimeid=p.datetimeid,
            //        //id=t.id,
            //        //name=t.name,
            //        //link=t.link,
            //        //hasContent=t.hasContent,
            //        //maindomain=t.maindomain,                   
            //    }).Distinct();
            //rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.rankKeyword);
            //rs = rs.Take(20);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl = Rs.arrTrendsManager.ToList();
            var viewModel = new ModelClassViewTrendsManager { ieViewTrends = rl };
            return View(viewModel);
        }
        //Xem tin theo chu de
        public ActionResult topicNews(int topicid) {
            string query = "";
            query="SELECT top 100 ";
            query+=" id,name as title,source,datetime as date,datediff(minute,datetime,getdate()) as timediff, ";
            query += " datetimeid,ranking,link,image,totalcomment,maindomain,uplikes,downlikes,topicid,catid,hasContent FROM tinviet_admin.titles where topicid="+topicid+" order by datetimeid desc,ranking desc "; 
            var rs = db.Database.SqlQuery<viewNewsSearchManager>(query).Distinct();
            var rl = rs.ToList();
            ViewBag.title = rl[0].title;
            ViewBag.topicid = topicid;
            ViewBag.image = rl[0].image;
            var viewModel = new ModelClassViewNewsSearchManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Danh bạ website
        //Xem các website Việt Nam
        public ActionResult DanhBa()
        {            
            string query = "SELECT site,des,rank from zalexa order by rank ";
            var rs = db.Database.SqlQuery<viewNewsAlexa>(query).Distinct();
            var rl = rs.ToList();
            //ViewBag.Title = "Danh bạ Website Việt Nam";
            var viewModel = new ModelClassViewNewsAlexa { ieViewNewsAlexa = rl };
            return View(viewModel);
        }
        //Tìm kiếm tin tức 
        //Dùng khi user click vào Tim kiem
        public ActionResult SearchNews(string keyword)
        {
            keyword = Server.HtmlEncode(keyword);
            int datetimeid = Uti.datetimeid() - 1;
            string query = "SELECT top 100 ";
            query += " FT_TBL.id,FT_TBL.name as title,FT_TBL.source,FT_TBL.datetime as date, ";
            query += " FT_TBL.datetimeid as datetimeid,FT_TBL.ranking as ranking,FT_TBL.link as link, ";
            query += " FT_TBL.image as image,FT_TBL.totalcomment as totalcomment,FT_TBL.maindomain as maindomain, ";
            query += " FT_TBL.uplikes as uplikes,FT_TBL.downlikes as downlikes,FT_TBL.topicid as topicid,FT_TBL.catid as catid,";
            query += " FT_TBL.hasContent as hasContent, KEY_TBL.RANK FROM tinviet_admin.titles AS FT_TBL INNER JOIN FREETEXTTABLE(tinviet_admin.titles, name,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,datetimeid desc,ranking desc";
            //string query = "select top 100 id,name as title,source,datetime as date,datetimeid,ranking,link,image,totalcomment,maindomain,uplikes,downlikes,topicid,hasContent from tinviet_admin.titles where name like N'%" + keyword + "%' order by datetimeid desc,ranking desc";
            //var rs = (from p in db.titles
            //          where p.name.Contains(keyword) || p.des.Contains(keyword)
            //          select new viewNewsSearchManager
            //          {
            //              id = p.id,
            //              title = p.name,
            //              //des = p.des,
            //              source = p.source,
            //              date = p.datetime,
            //              datetimeid = p.datetimeid,
            //              //token = p.tokenid,
            //              ranking = p.ranking,
            //              link = p.link,
            //              image = p.image,
            //              totalcomment = p.totalcomment,
            //              maindomain = p.maindomain,
            //              uplikes = p.uplikes,
            //              downlikes = p.downlikes,
            //              topicid = p.topicid,
            //              hasContent = p.hasContent,
            //          }
            //).Distinct();
            //rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
            //rs = rs.Take(100);
            var rs = db.Database.SqlQuery<viewNewsSearchManager>(query).Distinct();
            var rl = rs.ToList();
            ViewBag.Title = keyword;
            var viewModel = new ModelClassViewNewsSearchManager { ieViewNews = rl };
            return View(viewModel);
        }

        //Tìm kiếm tin tức 
        //Dùng khi user click vào Tim kiem
        public ActionResult mSearchNews(string keyword)
        {
            keyword = Server.HtmlEncode(keyword);
            int datetimeid = Uti.datetimeid() - 1;
            string query = "SELECT top 100 ";
            query += " FT_TBL.id,FT_TBL.name as title,FT_TBL.source,FT_TBL.datetime as date,datediff(minute,FT_TBL.datetime,getdate()) as timediff, ";
            query += " FT_TBL.datetimeid as datetimeid,FT_TBL.ranking as ranking,FT_TBL.link as link, ";
            query += " FT_TBL.image as image,FT_TBL.totalcomment as totalcomment,FT_TBL.maindomain as maindomain, ";
            query += " FT_TBL.uplikes as uplikes,FT_TBL.downlikes as downlikes,FT_TBL.topicid as topicid,FT_TBL.catid as catid,";
            query += " FT_TBL.hasContent as hasContent, KEY_TBL.RANK FROM tinviet_admin.titles AS FT_TBL INNER JOIN FREETEXTTABLE(tinviet_admin.titles, name,'" + keyword + "') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,datetimeid desc,ranking desc";
            //var rs = (from p in db.titles
            //          where p.name.Contains(keyword)||p.des.Contains(keyword)
            //          select new viewNewsManager
            //          {
            //              id = p.id,
            //              title = p.name,
            //              //des = p.des,
            //              source = p.source,
            //              date = p.datetime,
            //              datetimeid = p.datetimeid,
            //              //token = p.tokenid,
            //              ranking = p.ranking,
            //              link = p.link,
            //              image = p.image,
            //              totalcomment = p.totalcomment,
            //              maindomain = p.maindomain,
            //              uplikes = p.uplikes,
            //              downlikes = p.downlikes,
            //              topicid = p.topicid,
            //              hasContent=p.hasContent,
            //          }
            //).Distinct();
            //rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
            //rs = rs.Take(100);
            var rs = db.Database.SqlQuery<viewNewsSearchManager>(query).Distinct();
            var rl = rs.ToList();
            ViewBag.Title = keyword;
            var viewModel = new ModelClassViewNewsSearchManager { ieViewNews = rl };
            return View(viewModel);
        }

        //Tin chuyên mục, dùng mô hình MVC        
        //Dùng khi user click vào Điểm tin trong ngày
        public ActionResult CatNews(int id)
        {
            //return RedirectToAction("ChuyenMuc", "Home", new {id=id});
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.des = Uti.getDesMetaCatFromId(id);
            ViewBag.keyword = Uti.getKeywordMetaCatFromId(id);
            if (System.Web.HttpContext.Current.Request.Cookies["filterCat"] != null) ViewBag.filterCat = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterCat"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["filterDomain"] != null) ViewBag.filterDomain = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterDomain"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["haveRead"] != null)
            {
                ViewBag.haveRead = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["haveRead"].Value.ToString());
                ViewBag.haveRead += "-1";
            }
            
            try
            {
                ViewBag.Title = Uti.getCatNameFromId(id);
            }
            catch { 
            }
            ViewBag.catid = id;
            //var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            //return View(viewModel);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Cat_"+id+".xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();

            //Lấy ra xu hướng đọc
            //Rs.readTrends(HttpRuntime.AppDomainAppPath + "Trends.xml");
            //string title = "0";
            //string href="";
            //string predId="-1";
            //int No=0;
            //string image = "";
            //int countItem = 0;
            //string duplicate = "";
            //string content="";
            //        //alert(news+json_parsed.news.length);
            //        for (var i = 0; i < Rs.arrTrendsManager.Length;i++) {
            //            if (!Rs.arrTrendsManager[i].keyword.Equals("")) {

            //                if (!Rs.arrTrendsManager[i].keyword.Equals(predId))
            //                {
            //                    No++;
            //                    if (Rs.arrTrendsManager[i].title != null && Rs.arrTrendsManager[i].title != "")
            //                    { title = Rs.arrTrendsManager[i].title; }
            //                    else
            //                    { title = Rs.arrTrendsManager[i].keyword; }
            //                    //title = title.replace(/&#39;/g, "");
            //                    predId = Rs.arrTrendsManager[i].keyword;
            //                    if (No >= 2) { 
            //                        content+="<li class=\"trend-name last-list-item\">";
            //                        content += "<span class=\"close-trend\" onclick=\"showLi(" + (No-1) + ");\">Đóng lại</span>";
            //                        content+="</li>";
            //                    }
            //                    content += "</ul><ul class=\"list-trend\">";
            //                    //Nếu là từ ban đầu thì hiển thị                                
            //                    content += "<li class=\"trend-name\">";
            //                    content+="<span class=\"title-trend-item\">"+title+"</span>";
            //                    content += "<span class=\"trend-icon\" onclick=\"showLi(" + No+ ");\">";
            //                    content+="Chi tiết";
            //                    content+="<i class=\"fa fa-caret-square-o-down\"></i>";
            //                    content+="</span>";
            //                    content += "</li>";
            //                    countItem = 0;
            //                    duplicate = "";
            //                    //$("#item-trend-content").append(content);
            //                } else {
            //                    countItem++;//Đếm số item mỗi li                               
            //                    title = Rs.arrTrendsManager[i].name;//.replace(/&#39;/g, ""); 
            //                    if (countItem >= 6 || duplicate.Contains(title + ",")) continue;
            //                    duplicate += title+",";
            //                    href = "href=\"/" + Uti.unicodeToNoMark(title) + "-" + Rs.arrTrendsManager[i].id + "\"";
            //                    if (Rs.arrTrendsManager[i].hasContent == 0)
            //                    {
            //                        href = "href=\"" + Rs.arrTrendsManager[i].link + "\" target=\"_blank\"";
            //                    }
            //                    image = Rs.arrTrendsManager[i].image;
            //                    if (!Uti.isImage(Rs.arrTrendsManager[i].image))
            //                    {
            //                        image="/Images/logo.png";
            //                    }
            //                    if (No == 1) {
            //                        content += "<li style=\"display:list-item;\" id=li" + No + "_" + countItem + ">";
            //                    } else {
            //                        content += "<li style=\"display:none;\" id=li" + No + "_" + countItem + ">";
            //                    }
            //                    content+="<img class=\"img-trend-item\" src=\""+image+"\" />";
            //                    content+="<a "+href+" class=\"link-trend-item\" title=\""+title+"\">";
            //                    content+="<span class=\"title-trend-item\">";
            //                    content+=title;
            //                    content+="</span>";
            //                    content+="</a>";
            //                    content += "</li>";
                                
            //                    //$("#item-trend-content").append(content);
            //                }//if diffrent keyword
            //            }//if exist
            //        }//for  
            //        content += "<li class=\"trend-name last-list-item\">";
            //        content += "<span class=\"close-trend\" onclick=\"showLi(" + (No - 1) + ");\">Đóng lại</span>";
            //        content += "</li>";
            //        content += "</ul>";
            //ViewBag.trends = content;
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel); 
        }
        //Tin chuyên mục, dùng mô hình MVC for Mobile        
        //Dùng khi user click vào Các chuyên mục
        public ActionResult mCatNews(int id)
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.mainMenu = id;
            try
            {
                ViewBag.title = Uti.getCatNameFromId(id);
            }
            catch
            {
            }
            ViewBag.catid = id;
            //var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            //return View(viewModel);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Cat_" + id + ".xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Điểm tin trong ngày, dùng mô hình MVC        
        //Dùng khi user click vào Điểm tin trong ngày
        public ActionResult News()
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.des = "Điểm tin thời sự, giải trí, văn hóa, xã hội, thể thao, công nghệ trong ngày từ các báo vnexpress,zing,vietnamnet, lao động, thanh niên...";
            ViewBag.keyword = "điểm tin, đọc báo, tin nóng, tin tức, thời sự, giải trí, văn hóa, xã hội, kinh tế, thế giới, pháp luật, văn hóa, thể thao, sức khỏe,công nghệ, ô tô, xe máy, vnexpress.net,zing.vn,vietnamnet.vn,laodong.com.vn,thanhnien.com.vn";
            if (System.Web.HttpContext.Current.Request.Cookies["filterCat"] != null) ViewBag.filterCat = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterCat"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["filterDomain"] != null) ViewBag.filterDomain = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterDomain"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["haveRead"] != null)
            {
                ViewBag.haveRead = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["haveRead"].Value.ToString());
                ViewBag.haveRead += "-1";
            }

            
            
            //var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            //return View(viewModel);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "AllNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();

            //Lấy ra xu hướng đọc
            //Rs.readTrends(HttpRuntime.AppDomainAppPath + "Trends.xml");
            //string title = "0";
            //string href = "";
            //string predId = "-1";
            //int No = 0;
            //string image = "";
            //int countItem = 0;
            //string duplicate = "";
            //string content = "";
            ////alert(news+json_parsed.news.length);
            //for (var i = 0; i < Rs.arrTrendsManager.Length; i++)
            //{
            //    if (!Rs.arrTrendsManager[i].keyword.Equals(""))
            //    {

            //        if (!Rs.arrTrendsManager[i].keyword.Equals(predId))
            //        {
            //            No++;
            //            if (Rs.arrTrendsManager[i].title != null && Rs.arrTrendsManager[i].title != "")
            //            { title = Rs.arrTrendsManager[i].title; }
            //            else
            //            { title = Rs.arrTrendsManager[i].keyword; }
            //            //title = title.replace(/&#39;/g, "");
            //            predId = Rs.arrTrendsManager[i].keyword;
            //            if (No >= 2)
            //            {
            //                content += "<li class=\"trend-name last-list-item\">";
            //                content += "<span class=\"close-trend\" onclick=\"showLi(" + (No - 1) + ");\">Đóng lại</span>";
            //                content += "</li>";
            //            }
            //            content += "</ul><ul class=\"list-trend\">";
            //            //Nếu là từ ban đầu thì hiển thị                                
            //            content += "<li class=\"trend-name\">";
            //            content += "<span class=\"title-trend-item\">" + title + "</span>";
            //            content += "<span class=\"trend-icon\" onclick=\"showLi(" + No + ");\">";
            //            content += "Chi tiết";
            //            content += "<i class=\"fa fa-caret-square-o-down\"></i>";
            //            content += "</span>";
            //            content += "</li>";
            //            countItem = 0;
            //            duplicate = "";
            //            //$("#item-trend-content").append(content);
            //        }
            //        else
            //        {
            //            countItem++;//Đếm số item mỗi li                               
            //            title = Rs.arrTrendsManager[i].name;//.replace(/&#39;/g, ""); 
            //            if (countItem >= 6 || duplicate.Contains(title + ",")) continue;
            //            duplicate += title + ",";
            //            href = "href=\"/" + Uti.unicodeToNoMark(title) + "-" + Rs.arrTrendsManager[i].id + "\"";
            //            if (Rs.arrTrendsManager[i].hasContent == 0)
            //            {
            //                href = "href=\"" + Rs.arrTrendsManager[i].link + "\" target=\"_blank\"";
            //            }
            //            image = Rs.arrTrendsManager[i].image;
            //            if (!Uti.isImage(Rs.arrTrendsManager[i].image))
            //            {
            //                image = "/Images/logo.png";
            //            }
            //            if (No == 1)
            //            {
            //                content += "<li style=\"display:list-item;\" id=li" + No + "_" + countItem + ">";
            //            }
            //            else
            //            {
            //                content += "<li style=\"display:none;\" id=li" + No + "_" + countItem + ">";
            //            }
            //            content += "<img class=\"img-trend-item\" src=\"" + image + "\" />";
            //            content += "<a " + href + " class=\"link-trend-item\" title=\"" + title + "\">";
            //            content += "<span class=\"title-trend-item\">";
            //            content += title;
            //            content += "</span>";
            //            content += "</a>";
            //            content += "</li>";

            //            //$("#item-trend-content").append(content);
            //        }//if diffrent keyword
            //    }//if exist
            //}//for  
            //content += "<li class=\"trend-name last-list-item\">";
            //content += "<span class=\"close-trend\" onclick=\"showLi(" + (No - 1) + ");\">Đóng lại</span>";
            //content += "</li>";
            //content += "</ul>";
            //ViewBag.trends = content;
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel); 
        }
        //Điểm tin trong ngày, dùng mô hình MVC for Mobile  
        //Dùng khi user click vào Điểm tin trong ngày
        public ActionResult mNews()
        {
            return RedirectToAction("HotNews", "Home");
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.mainMenu = 0;
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "AllNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();            
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Lấy ra các tin nóng, có isHot=1, dùng mô hình MVC        
        //Dùng khi user click vào tin nóng
        public ActionResult Test()
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            int datetimeid = Uti.datetimeid() - 1;
            var rs = (from p in db.titles
                      where p.isHot == 1 && (p.topicid == p.id || p.topicid == null) && p.maindomain.Contains("thanhnien")
                      select new viewNewsManager
                      {
                          id = p.id,
                          title = p.name,
                          //des = p.des,
                          source = p.source,
                          date = p.datetime,
                          datetimeid = p.datetimeid,
                          //token = p.tokenid,
                          ranking = p.ranking,
                          link = p.link,
                          image = p.image,
                          totalcomment = p.totalcomment,
                          maindomain = p.maindomain,
                          uplikes = p.uplikes,
                          downlikes = p.downlikes,
                          topicid = p.topicid,
                      }
            ).Distinct();
            rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
            rs = rs.Take(100);
            var rl = rs.ToList();
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Tin nong trong tuan       
        //Dùng khi user click vào tin nóng tuan
        public ActionResult TinNongTuan()
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
           // if (System.Web.HttpContext.Current.Request.Cookies["filterCat"] != null) ViewBag.filterCat = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterCat"].Value.ToString());
           // if (System.Web.HttpContext.Current.Request.Cookies["filterDomain"] != null) ViewBag.filterDomain = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterDomain"].Value.ToString());
            //int datetimeid = Uti.datetimeid() - 1;
            //var rs = (from p in db.titles
            //          where p.isHot == 1 && (p.topicid==p.id || p.topicid==null)
            //          select new viewNewsManager
            //{ 
            //  id = p.id, title = p.name,
            //  //des = p.des, 
            //  source = p.source,
            //  date = p.datetime, datetimeid = p.datetimeid,
            //  //token = p.tokenid, ranking = p.ranking,
            //  link = p.link, image = p.image, 
            //  totalcomment = p.totalcomment, maindomain = p.maindomain, 
            //  uplikes = p.uplikes, downlikes = p.downlikes,
            //  topicid = p.topicid,
            //  catid=p.catid,
            //  hasContent=p.hasContent,
            //}
            //).Distinct();            
            //rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
            //rs = rs.Take(100);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "TopNewsWeekly.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            if (Rs.Length < 5) return RedirectToAction("News", "Home");
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Lấy ra các tin moi nhat, dùng mô hình MVC        
        //Dùng khi user click vào tin moi nhat
        public ActionResult LatestNews()
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            //if (System.Web.HttpContext.Current.Request.Cookies["filterCat"] != null) ViewBag.filterCat = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterCat"].Value.ToString());
            //if (System.Web.HttpContext.Current.Request.Cookies["filterDomain"] != null) ViewBag.filterDomain = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterDomain"].Value.ToString());
            //int datetimeid = Uti.datetimeid() - 1;
            //var rs = (from p in db.titles
            //          where p.isHot == 1 && (p.topicid==p.id || p.topicid==null)
            //          select new viewNewsManager
            //{ 
            //  id = p.id, title = p.name,
            //  //des = p.des, 
            //  source = p.source,
            //  date = p.datetime, datetimeid = p.datetimeid,
            //  //token = p.tokenid, ranking = p.ranking,
            //  link = p.link, image = p.image, 
            //  totalcomment = p.totalcomment, maindomain = p.maindomain, 
            //  uplikes = p.uplikes, downlikes = p.downlikes,
            //  topicid = p.topicid,
            //  catid=p.catid,
            //  hasContent=p.hasContent,
            //}
            //).Distinct();            
            //rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
            //rs = rs.Take(100);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "New.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            if (Rs.Length < 5) return RedirectToAction("News", "Home");
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        public string getTopNewsRead()
        {
            //haveReadId += "-1";
            string query = "select top 10 name as title,id,totalviews,maindomain,catid from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and totalviews>0 order by datetimeid desc,totalviews desc";
            var rs = db.Database.SqlQuery<viewNewsManager>(query).Distinct();
            string json = JsonConvert.SerializeObject(rs.ToList());
            //json = Server.HtmlDecode(json);
            return json;
        }
        public string getTopNewsReadCat(int catid)
        {
            //haveReadId += "-1";
            string query = "select top 10 name as title,id,totalviews,maindomain,catid from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and totalviews>0 and catid=" + catid + " order by datetimeid desc,totalviews desc";
            var rs = db.Database.SqlQuery<viewNewsManager>(query).Distinct();
            string json = JsonConvert.SerializeObject(rs.ToList());
            //json = Server.HtmlDecode(json);
            return json;
        }
        public string getHaveRead(string haveReadId) {
            haveReadId += "-1";
            string query = "select top 10 ";
            query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,";
            query += " token=A.tokenid,ranking=A.ranking,link=A.link,";
            query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
            query += " uplikes=A.uplikes,downlikes=A.downlikes,";
            query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
            query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
            query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
            query += " (select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,tokenid,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-7) + " and id in (" + haveReadId + ") order by  datetimeid desc, ranking desc, id desc) as A left join ";
            query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid<>0) as B on (A.topicid=B.topicidRelated and A.topicid=A.id) ";
            query += " order by A.datetimeid desc,A.ranking desc,A.id desc,B.idRelated desc,B.rankingRelated desc";
            var rs = db.Database.SqlQuery<viewNewsManager>(query).Distinct();
            string json = JsonConvert.SerializeObject(rs.ToList());
            return json;
        }
        //Lẩy ra các tin đã xem qua Cookie      
        //Dùng khi user click vào tin Đã xem
        public ActionResult Store()
        {
            //return RedirectToAction("TinNong", "Home");
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            if (System.Web.HttpContext.Current.Request.Cookies["filterCat"] != null) ViewBag.filterCat = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterCat"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["filterDomain"] != null) ViewBag.filterDomain = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterDomain"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["haveRead"] != null)
                ViewBag.haveRead = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["haveRead"].Value.ToString());
            else
                return RedirectToAction("News", "Home");
            ViewBag.haveRead += "-1";
            string query = "select top 300 ";
            query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,";
            query += " token=A.tokenid,ranking=A.ranking,link=A.link,";
            query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
            query += " uplikes=A.uplikes,downlikes=A.downlikes,";
            query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
            query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
            query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
            query += " (select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,tokenid,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and id in (" + ViewBag.haveRead + ") order by  datetimeid desc, ranking desc, id desc) as A left join ";
            query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid<>0) as B on (A.topicid=B.topicidRelated and A.topicid=A.id) ";
            query += " order by A.datetimeid desc,A.ranking desc,A.id desc,B.idRelated desc,B.rankingRelated desc";
            var rs = db.Database.SqlQuery<viewNewsManager>(query).Distinct();
            try
            {
                if (rs.ToList().Count<= 0)//.ToList()[0].id
                {
                    return RedirectToAction("News", "Home");
                }
            }
            catch (Exception ex) { 

            }
            var rl = rs.ToList();
            //if (Rs.Length < 1) return RedirectToAction("News", "Home");
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        public class catItem {
            public string view;
        }
        //Lấy ra các tin nóng, có isHot=1, dùng mô hình MVC        
        //Dùng khi user click vào tin nóng
        public ActionResult HotNews()
        {
            //return RedirectToAction("TinNong", "Home");
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];            
            ViewBag.userName = Session["username"];
            ViewBag.haveRead = "-1";
            ViewBag.des = "Tin nóng thời sự, báo 24h, giải trí văn hóa xã hội thể thao, xếp hạng tin tức, tin nổi bật từ các báo chính thống Việt Nam";
            ViewBag.keyword = "đọc báo, tin nóng, tin tức,báo 24, thời sự, giải trí, văn hóa, xã hội, kinh tế, thế giới, pháp luật, văn hóa, thể thao, sức khỏe,công nghệ, ô tô, xe máy, điện thoại, di động";
            if (System.Web.HttpContext.Current.Request.Cookies["filterCat"] != null) ViewBag.filterCat = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterCat"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["filterDomain"] != null) ViewBag.filterDomain = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["filterDomain"].Value.ToString());
            if (System.Web.HttpContext.Current.Request.Cookies["haveRead"] != null)
            {
                ViewBag.haveRead = Server.UrlDecode((string)System.Web.HttpContext.Current.Request.Cookies["haveRead"].Value.ToString());
                ViewBag.haveRead += "-1";
            }
            
            Rss Rs = null;
            ViewBag.duplicateIdHotNews = ",-1,";
            string urllink = "";
            try
            {
                Rs = new Rss(HttpRuntime.AppDomainAppPath + "TopHotNew.xml", 0, "", 0, 7);
                var bigRl = Rs.arrNewsManager.ToList();
                urllink = Uti.unicodeToNoMark(System.Web.HttpUtility.HtmlDecode(bigRl[0].title) + " " + Uti.smDomainNew(bigRl[0].maindomain) + " " + Uti.getCatNameFromId(bigRl[0].catid)) + "-" + bigRl[0].id;
                ViewBag.bigHotNew1 = "";
                ViewBag.bigHotNew1 += "<div id=hotnews1 class=hotnews1>";
                ViewBag.bigHotNew1 += "  <div class=row>";
                ViewBag.bigHotNew1 += "  <div class=bighotnews>";
                ViewBag.bigHotNew1 += "		<a href=\"/" + urllink + "\" title=\"" + bigRl[0].title + "\">";//Uti.unicodeToNoMark(bigRl[0].title) + "-" + bigRl[0].id
                ViewBag.bigHotNew1 += "<img src=\"" + bigRl[0].image + "\" width=\"100%\" alt=\"" + bigRl[0].title + "\" style=\"max-height:349px;\">";
                ViewBag.bigHotNew1 += "<span class=bighottitle>" + bigRl[0].title + "<span>";
                ViewBag.bigHotNew1 += "  </a>";
                ViewBag.bigHotNew1 += "</div>";
                ViewBag.bigHotNew1 += "	</div>";
                ViewBag.bigHotNew1 += "	</div>";
                ViewBag.duplicateIdHotNews += "," + bigRl[0].id + ",";
                ViewBag.bigHotNew2 = "<div class=\"box-news\">";
                ViewBag.bigHotNew2 += " <div class=\"box-news-title\">";
                ViewBag.bigHotNew2 += "   <span>Tin nổi bật</span>";
                ViewBag.bigHotNew2 += " <span class=\"unread-box-news\">Tổng hợp <i class=\"fa fa-refresh box-refresh\"></i></span>";
                ViewBag.bigHotNew2 +="  </div>";
                ViewBag.bigHotNew2 +="  <div class=\"box-news-contents\">";
                ViewBag.bigHotNew2 +="   <ul class=\"list-content\">";

                for (int k = 1; k < bigRl.Count; k++) {
                    urllink = Uti.unicodeToNoMark(System.Web.HttpUtility.HtmlDecode(bigRl[k].title) + " " + Uti.smDomainNew(bigRl[k].maindomain) + " " + Uti.getCatNameFromId(bigRl[k].catid)) + "-" + bigRl[k].id;
                    ViewBag.duplicateIdHotNews += "," + bigRl[k].id + ",";
                    ViewBag.bigHotNew2 += "<li>";
                    ViewBag.bigHotNew2 += "<a href=\"/" + urllink + "\" class=\"link-image-box-news\" title=\"" + bigRl[k].title + "\">";//Uti.unicodeToNoMark(bigRl[k].title) + "-" + bigRl[k].id 
                    ViewBag.bigHotNew2 +=  "<img src=\""+bigRl[k].image+"\" class=\"images-box-news\" alt=\""+bigRl[k].title+"\"/>";
                    ViewBag.bigHotNew2 += " </a>";
                    ViewBag.bigHotNew2 += " <a href=\"" + urllink + "\" class=\"link-box-news\" title=\"" + bigRl[k].title + "\">" + bigRl[k].title + "</a>";//Uti.unicodeToNoMark(bigRl[k].title) + "-" + bigRl[k].id 
                    ViewBag.bigHotNew2 +=" </li>";
                }
                ViewBag.bigHotNew2 += "</ul><div class=\"clear-fix\"></div></div></div>";
            }
            catch (Exception ex) { 
            }
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "HotNew.xml", 0, "", 0, 7);
            ViewBag.catItem=new catItem[15];
            var rl = Rs.arrNewsManager.ToList();
            //Doc tin moi cua cac chuyen muc
            string href = "";
            string image = "";
            for(int i=1;i<=13;i++){
                try
                {
                    ViewBag.catItem[i] = new catItem();
                    ViewBag.catItem[i].view = "";
                    if (System.IO.File.Exists(HttpRuntime.AppDomainAppPath + "Cat_" + i + "_New.xml"))
                    {
                        Rs.readCatNewsLatest(HttpRuntime.AppDomainAppPath + "Cat_" + i + "_New.xml");
                        
                        //ViewBag.catItem[i] = new catItem();
                        for (int j = 0; j < Rs.LengthCatNewsLatest; j++)
                        {
                            //ViewBag.catItem[i].view += Rs.arrCatNewsLatestManager[j].title;
                            urllink = Uti.unicodeToNoMark(System.Web.HttpUtility.HtmlDecode(Rs.arrCatNewsLatestManager[j].title) + " " + Uti.smDomainNew(Rs.arrCatNewsLatestManager[j].maindomain) + " " + Uti.getCatNameFromId(Rs.arrCatNewsLatestManager[j].catid)) + "-" + Rs.arrCatNewsLatestManager[j].id;
                            if (Rs.arrCatNewsLatestManager[j].hasContent == 1)
                            {
                                href = "href=\"/" + urllink + "\"";//Uti.unicodeToNoMark(Rs.arrCatNewsLatestManager[j].title) + "-" + Rs.arrCatNewsLatestManager[j].id 
                            }
                            else
                            {
                                href = "href=\""+Rs.arrCatNewsLatestManager[j].link+"\" target=\"_blank\"";
                            }
                            if (!Rs.arrCatNewsLatestManager[j].image.Equals(""))
                            {
                                image = Rs.arrCatNewsLatestManager[j].image;
                            }
                            else
                            {
                                image = "/Images/logo.png";
                            }
                            if (j == 0)
                            {                                
                                ViewBag.catItem[i].view += "<div class=\"box-news-title\">";
                                ViewBag.catItem[i].view += "<span>" + Uti.getCatNameFromId(i) + "</span>";
                                ViewBag.catItem[i].view += "<span class=\"unread-box-news\">" + Uti.getDiffTimeFromNow(Rs.arrCatNewsLatestManager[j].datetime.ToString()) + "<i class=\"fa fa-refresh box-refresh\"></i></span>";
                                ViewBag.catItem[i].view += "</div>";
                                ViewBag.catItem[i].view += "<div class=\"box-news-contents\">";
                                ViewBag.catItem[i].view += "<ul class=\"list-content\">";
                                ViewBag.catItem[i].view += "<li>";
                                ViewBag.catItem[i].view += " <a " + href + " class=\"link-image-box-news\" title=\"" + Rs.arrCatNewsLatestManager[j].title + "\">";
                                ViewBag.catItem[i].view += "  <img src=\"" + image + "\" class=\"images-box-news\" alt=\"" + Rs.arrCatNewsLatestManager[j].title + "\" />";
                                ViewBag.catItem[i].view += "</a>";
                                ViewBag.catItem[i].view += "<a " + href + " class=\"link-box-news\" title=\"" + Rs.arrCatNewsLatestManager[j].title + "\">" + Rs.arrCatNewsLatestManager[j].title + "</a>";
                                ViewBag.catItem[i].view += "</li>";
                            }
                            else
                            {
                                ViewBag.catItem[i].view += "<li><a " + href + " class=\"link-box-news\" title=\"" + Rs.arrCatNewsLatestManager[j].title + "\">" + Rs.arrCatNewsLatestManager[j].title + "</a></li>";
                            }
                        }
                        if (!ViewBag.catItem[i].view.ToString().Equals(""))
                        {
                            ViewBag.catItem[i].view += "</ul>";
                            ViewBag.catItem[i].view += "<div class=\"clear-fix\"></div>";
                            ViewBag.catItem[i].view += "</div>";
                        }
                    }
                }
                catch (Exception excat) { 
                }
            }//for
            if (Rs.Length < 5) return RedirectToAction("News", "Home");
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);           
        }
        [HttpGet]
        public ActionResult DocBao(int id) {
            return RedirectToAction("readNews", "comment", new { id = id });
            ViewBag.isMobile = 0;
            ViewBag.idNews = id;
            if (Request.Browser.IsMobileDevice)
            {
                ViewBag.isMobile = 1;
            }
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
                          link = p.link,
                          contents = A2.contents,
                          userTokenReply = A2.usertokenreplyid,
                          maindomain = p.maindomain,
                          ranking = p.ranking,
                          uplikes = p.uplikes,
                          downlikes = p.downlikes,
                          topicid=p.topicid,
                      }
                      ).OrderByDescending(t => (long?)t.id).Take(5);

            
            //bool found=true;
            //try
            //{
            //    var temp = rs.ToList();
            //}
            //catch (Exception ex) {
            //    found = false;
            //}
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
                              link = p.link,
                              contents = A2.contents,
                              userTokenReply = A2.usertokenreplyid,
                              maindomain = p.maindomain,
                              ranking = p.ranking,
                              uplikes = p.uplikes,
                              downlikes = p.downlikes,
                              topicid = p.topicid,
                          }
                         ).OrderByDescending(t => (long?)t.id).Take(5);

                   

                }
            }
            catch (Exception ex)
            {
            }
            //string query = " select  ISNULL(B.id,-1) as id,A.idNews,A.image,B.userToken,C.userName,B.userTokenReply,C.email,ISNULL(B.datetime,getdate()) as datetime,A.datetimenews,ISNULL(B.datetimeid,0) as datetimeid, ";
            //       query +=" A.title,A.des,A.fullContent,A.link,B.contents,A.maindomain,A.ranking,A.uplikes,A.downlikes,B.userNameReply from ";
            //       query += " (select id as idNews,image,name as title,datetime as datetimenews,des,fullContent,link,maindomain,ranking,uplikes,downlikes from tinviet_admin.titles where id=" + id + ") as A left join ";
            //       query += " (select ISNULL(id,0) as id,fullNameReply as userNameReply,usertoken as userToken,datetime,datetimeid,contents,usertokenreplyid as userTokenReply from tinviet_admin.comments) as B on A.idNews=B.id left join ";
            //       query +=" (select usertoken,fullName as userName,email from tinviet_admin.members) as C on B.usertoken=C.usertoken ";
            //       query += " order by B.id desc ";
            //var rs = db.Database.SqlQuery<viewCommentManager>(query);
            var rl = rs.ToList();

            ViewBag.url = Config.domain + "/comment/readNews/" + id;//rl[0].link;
            ViewBag.link = rl[0].link;
            ViewBag.title = rl[0].title;
            ViewBag.des = rl[0].des;
            ViewBag.fullContent = rl[0].fullContent;
            ViewBag.image = rl[0].image;
            ViewBag.ranking = rl[0].ranking;
            ViewBag.uplikes = rl[0].uplikes;
            ViewBag.downlikes = rl[0].downlikes;
            ViewBag.datetimenews = rl[0].datetimenews;
            ViewBag.maindomain = rl[0].maindomain;
            if (rl[0].topicid != null && rl[0].topicid != 0)
            {
                ViewBag.topicid = rl[0].topicid;
            }
            else {
                ViewBag.topicid = -1;
            }
            //ViewBag.userName = rl[0].userName;
            var viewModel = new ModelClassViewComment { ieViewComment = rl };
            return View(viewModel);
        }
        public ActionResult TinNong()
        {
            return RedirectToAction("HotNews", "Home");
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "HotNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            //Lay ra xu huong doc
            Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl2 = Rs.arrTrendsManager.ToList();
            string preTitle = "";
            string title = "";
            string trends = "";
            for (int i = 0; i < rl2.Count; i++) {
                title = rl2[i].keyword;
                if (rl2[i].title != "") title = rl2[i].title;
                if (!preTitle.Equals(title)) {
                    trends+="<li>";
                    trends += "<a href=\"/Home/SearchNews?keyword=" + title + "\" title=\"" + title + "\">";
                    trends+="<span class=\"play-icon\"></span>";
                    trends+="<span class=\"background-video\"></span>";
                    trends+="<img src=\""+rl2[i].image+"\" alt=\""+rl2[i].name+"\" class=\"hot-video-img\">";
                    trends += "<span class=\"hot-video-title\">" + title + "</span>";
                    trends+="</a>";
                    trends += "</li>";
                    preTitle = title;
                }
            }
            ViewBag.trends = trends;
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            
            return View(viewModel); 
        }
        public ActionResult DiemTin()
        {
            return RedirectToAction("News", "Home");
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "AllNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            //Lay ra xu huong doc
            Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl2 = Rs.arrTrendsManager.ToList();
            string preTitle = "";
            string title = "";
            string trends = "";
            for (int i = 0; i < rl2.Count; i++)
            {
                title = rl2[i].keyword;
                if (rl2[i].title != "") title = rl2[i].title;
                if (!preTitle.Equals(title))
                {
                    trends += "<li>";
                    trends += "<a href=\"/Home/SearchNews?keyword=" + title + "\" title=\"" + title + "\">";
                    trends += "<span class=\"play-icon\"></span>";
                    trends += "<span class=\"background-video\"></span>";
                    trends += "<img src=\"" + rl2[i].image + "\" alt=\"" + rl2[i].name + "\" class=\"hot-video-img\">";
                    trends += "<span class=\"hot-video-title\">" + title + "</span>";
                    trends += "</a>";
                    trends += "</li>";
                    preTitle = title;
                }
            }
            ViewBag.trends = trends;
            //ViewBag.Title = "Tin Nóng Trong Ngày ";

            return View(viewModel);
        }
        public ActionResult MoiNhat()
        {
            return RedirectToAction("LatestNews", "Home");
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "New.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            //Lay ra xu huong doc
            Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl2 = Rs.arrTrendsManager.ToList();
            string preTitle = "";
            string title = "";
            string trends = "";
            for (int i = 0; i < rl2.Count; i++)
            {
                title = rl2[i].keyword;
                if (rl2[i].title != "") title = rl2[i].title;
                if (!preTitle.Equals(title))
                {
                    trends += "<li>";
                    trends += "<a href=\"/Home/SearchNews?keyword=" + title + "\" title=\"" + title + "\">";
                    trends += "<span class=\"play-icon\"></span>";
                    trends += "<span class=\"background-video\"></span>";
                    trends += "<img src=\"" + rl2[i].image + "\" alt=\"" + rl2[i].name + "\" class=\"hot-video-img\">";
                    trends += "<span class=\"hot-video-title\">" + title + "</span>";
                    trends += "</a>";
                    trends += "</li>";
                    preTitle = title;
                }
            }
            ViewBag.trends = trends;
            //ViewBag.Title = "Tin Nóng Trong Ngày ";

            return View(viewModel);
        }
        public ActionResult ChuyenMuc(int id)
        {
            return RedirectToAction("CatNews", "Home", new {id=id});
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Cat_"+id+".xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            try
            {
                ViewBag.Title = Uti.getCatNameFromId(id);
            }
            catch
            {
            }
            ViewBag.catid = id;
            //Lay ra xu huong doc
            Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "Trends.xml", 0, "", 0, 8);
            var rl2 = Rs.arrTrendsManager.ToList();
            string preTitle = "";
            string title = "";
            string trends = "";
            for (int i = 0; i < rl2.Count; i++)
            {
                title = rl2[i].keyword;
                if (rl2[i].title != "") title = rl2[i].title;
                if (!preTitle.Equals(title))
                {
                    trends += "<li>";
                    trends += "<a href=\"/Home/SearchNews?keyword=" + title + "\" title=\"" + title + "\">";
                    trends += "<span class=\"play-icon\"></span>";
                    trends += "<span class=\"background-video\"></span>";
                    trends += "<img src=\"" + rl2[i].image + "\" alt=\"" + rl2[i].name + "\" class=\"hot-video-img\">";
                    trends += "<span class=\"hot-video-title\">" + title + "</span>";
                    trends += "</a>";
                    trends += "</li>";
                    preTitle = title;
                }
            }
            ViewBag.trends = trends;
            //ViewBag.Title = "Tin Nóng Trong Ngày ";

            return View(viewModel);
        }
        public ActionResult Tin() {

            return View();
        }
        //Lấy ra các tin moi nhat, dùng mô hình MVC cho Mobile     
        //Dùng khi user click vào tin moi nhat
        public ActionResult mLatestNews()
        {
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.mainMenu = -1;
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "New.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            if (Rs.Length < 5) return RedirectToAction("mNews", "Home");
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Lấy ra các tin nóng, có isHot=1, dùng mô hình MVC cho Mobile     
        //Dùng khi user click vào tin nóng
        public ActionResult mHotNews()
        {
            return RedirectToAction("HotNews", "Home");
            //return RedirectToAction("TinNong", "Home");
            ViewBag.userToken = Session["usertoken"];
            ViewBag.userInfo = Session["userinfo"];
            ViewBag.userName = Session["username"];
            ViewBag.mainMenu = -1;
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "HotNew.xml", 0, "", 0, 7);
            var rl = Rs.arrNewsManager.ToList();
            if (Rs.Length < 5) return RedirectToAction("mNews", "Home");
            //ViewBag.Title = "Tin Nóng Trong Ngày ";
            var viewModel = new ModelClassViewNewsManager { ieViewNews = rl };
            return View(viewModel);
        }
        //Lấy ra các tin nóng, có isHot=1
        //có datetimeid<datetimeid, hiện đang chưa dùng datetimeid, cân nhắc
        //Dùng khi user click vào tin nóng
        //public string getHotNewsFromFile(string datetime,int orderby)
        //{
        //    int datetimeid = Uti.datetimeidfromdate(datetime); //Uti.datetimeid() - 1;
        //    var rs = (from p in db.titles where p.isHot == 1 && (p.topicid == p.id || p.topicid == null) select new { id = p.id, title = p.name, des = p.des, source = p.source, date = p.datetime, datetimeid = p.datetimeid, token = p.tokenid, ranking = p.ranking, link = p.link, image = p.image, totalcomment = p.totalcomment, maindomain = p.maindomain, uplikes = p.uplikes, downlikes = p.downlikes, p.topicid, p.catid,p.hasContent }).Distinct();
        //    if (orderby == 0)
        //    {
        //        rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
        //    }
        //    else
        //    {
        //        rs = rs.OrderByDescending(o => o.date);
        //    }
        //    rs = rs.Take(100);
        //    string json = JsonConvert.SerializeObject(rs.ToList());
        //    return json;
        //}
        //Lấy ra các tin của các chuyên mục có catid=catid
        //có datetimeid<datetimeid, hiện đang chưa dùng datetimeid, cân nhắc
        //Dùng khi user click vào các chuyên mục như Thể thao, xã hội.....
        //public string getNewsFromFile(int catid, string datetime,int orderby)
        //{
        //    int datetimeid = Uti.datetimeidfromdate(datetime);//Uti.datetimeid() - 1;
        //    var rs = (from p in db.titles where p.catid == catid && (p.topicid == p.id || p.topicid == null) select new { id = p.id, title = p.name, des = p.des, source = p.source, date = p.datetime, datetimeid = p.datetimeid, token = p.tokenid, ranking = p.ranking, link = p.link, image = p.image, totalcomment = p.totalcomment, maindomain = p.maindomain, uplikes = p.uplikes, downlikes = p.downlikes, p.topicid, p.catid,p.hasContent }).Distinct();
        //    if (orderby == 0)
        //    {
        //        rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
        //    }
        //    else
        //    {
        //        rs = rs.OrderByDescending(o => o.date);
        //    }
        //    rs = rs.Take(100);
        //    string json = JsonConvert.SerializeObject(rs.ToList());
        //    //Rs.Clear();
        //    return json;
        //}
        [HttpGet]
        public string getAllDomain() {
            var rs = (from p in db.domains where p.status==0  select new { id = p.id, name = p.name }).Distinct();
            string json = JsonConvert.SerializeObject(rs.ToList());
            return json;
        }
        [HttpGet]
        public string getDetailNews(int id)
        {
            var rs = (from p in db.titles where p.id==id select new { id = p.id, title = p.name,uplikes=p.uplikes,des=p.des,fullContent=p.fullcontent,ranking=p.ranking,datetime=p.datetime,datetimeid=p.datetimeid,link=p.link,hasContent=p.hasContent}).Distinct().Take(1);
            string json = JsonConvert.SerializeObject(rs.ToList());
            return json;
        }
        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        //Lấy ra Điểm tin trong ngày
        //có datetimeid<datetimeid, hiện đang chưa dùng datetimeid, cân nhắc
        //Dùng khi user click vào Điểm tin trong ngày
        //public string getDailyTopNews(string datetime,int orderby=0)
        //{
        //    int datetimeid = Uti.datetimeidfromdate(datetime);
        //    var rs = (from p in db.titles where p.topicid==p.id || p.topicid==null  select new { id=p.id,title = p.name, des = p.des,source=p.source, date = p.datetime,datetimeid=p.datetimeid, token = p.tokenid, ranking = p.ranking, link = p.link, image = p.image, totalcomment = p.totalcomment, maindomain = p.maindomain,uplikes=p.uplikes,downlikes=p.downlikes,p.topicid,p.catid,p.hasContent}).Distinct();
            
        //    if (orderby == 0)
        //    {
        //        rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
        //    }
        //    else {
        //        rs = rs.OrderByDescending(o => o.date);
        //    }
        //    rs = rs.Take(100);
        //    string json = JsonConvert.SerializeObject(rs.ToList());            
        //    return json;
        //}
        //Lấy ra top comment ở file xml, cần xem lại có còn dùng chức năng này không?
        public string getTopComment()
        {
            //int datetimeid=Uti.datetimeid();
            //var rs = (from p in db.titles where p.datetimeid == datetimeid select new { p.name, p.ranking,p.link,p.image }).Distinct().OrderByDescending(o=>o.ranking).Take(5);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "TopComment.xml", 0, "", 0, 4);
            string json = JsonConvert.SerializeObject(Rs.arrItem);
            Rs.Clear();
            return json;
        }
        //Lấy ra các tin nổi bật trong tuần, lấy ra từ file topNewsWeekly.xml 
        //do Crawl Editor tự động sinh ra
        public string getTopNewsWeekly()
        {
            //int datetimeid=Uti.datetimeid();
            //var rs = (from p in db.titles where p.datetimeid == datetimeid select new { p.name, p.ranking,p.link,p.image }).Distinct().OrderByDescending(o=>o.ranking).Take(5);
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "TopNewsWeekly.xml", 0, "", 0, 5);
            string json = JsonConvert.SerializeObject(Rs.arrItem);
            Rs.Clear();
            return json;
        }
        //Hiện nay function này không dùng ở Views nào nữa
        public string getFreshNews()
        {
            //int datetimeid=Uti.datetimeid();
            //var rs = (from p in db.titles where p.datetimeid == datetimeid select new { title=p.name,p.id, p.ranking, p.link, p.image, p.datetime,p.hasContent }).Distinct().OrderByDescending(o => o.datetime).Take(20);            
            //string json = JsonConvert.SerializeObject(rs.ToList());           
            //return json;
            Rss Rs = null;
            Rs = new Rss(HttpRuntime.AppDomainAppPath + "New.xml", 0, "", 0, 6);
            string json = JsonConvert.SerializeObject(Rs.arrItem);
            Rs.Clear();
            return json;
        }
        [HttpPost]
        //Lấy ra các tin liên quan cung chuyen muc
        public string getRelatedCat(int catid)
        {
            int datetimeid = Uti.datetimeid();
            //var rs = (from p in db.titles where p.datetimeid >= datetimeid && p.catid == catid select p);
            //try
            //{
            //    if (rs.ToList().Count <= 0)//.ToList()[0].id
            //    {
            //        var rs2 = (from p2 in db.titlesSearches where p2.catid == catid select p2);
            //        rs2 = rs2.OrderByDescending(o=>o.id).Take(20);
            //        string json2 = JsonConvert.SerializeObject(rs2.ToList());
            //        return json2;
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            //rs = rs.OrderByDescending(o=>o.id).Take(20);
            //string json = JsonConvert.SerializeObject(rs.ToList());
            ////json = Server.HtmlDecode(json);
            //return json;
            string query = "select top 10 name as title,id,image,maindomain,catid from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid="+catid+" order by datetimeid desc,id desc";
            var rs = db.Database.SqlQuery<viewNewsManager>(query).Distinct();
            string json = JsonConvert.SerializeObject(rs.ToList());
            //json = Server.HtmlDecode(json);
            return json;
        }

        //[HttpPost]
        //Lấy ra các tin liên quan cung chu de
        public string getRelated(int topicid)
        {
            int datetimeid = Uti.datetimeidByDay(-2);
            var rs = (from p in db.titles where p.datetimeid>=datetimeid && p.topicid ==topicid select p);
            try
            {
                if (rs.ToList().Count<=0)//.ToList()[0].id
                {
                    var rs2 = (from p2 in db.titlesSearches where p2.topicid == topicid select p2);
                    rs2 = rs2.OrderByDescending(o => o.id).Take(20);
                    string json2 = JsonConvert.SerializeObject(rs2.ToList());
                    return json2;
                }
            }catch(Exception ex){

            }
            rs = rs.OrderByDescending(o=>o.id).Take(20);
            string json = JsonConvert.SerializeObject(rs.ToList());
            //json = Server.HtmlDecode(json);
            return json;
        }
        [HttpPost]
        //Lấy ra 10 tin nóng
        public string getTopNews()
        {
            int datetimeid = Uti.datetimeid();
            var rs = (from p in db.titles where p.datetimeid>=datetimeid && p.isHot==1 orderby p.ranking descending select p).Distinct();//.OrderByDescending(o=>o.ranking);
            try
            {
                if (rs.ToList().Count <= 0)//.ToList()[0].id
                {
                    var rs2 = (from p2 in db.titlesSearches where p2.datetimeid >= datetimeid && p2.isHot == 1 orderby p2.ranking descending select p2).Distinct();//.OrderByDescending(o => o.ranking);
                    rs2 = rs2.Take(4);
                    string json2 = JsonConvert.SerializeObject(rs2.ToList());
                    return json2;
                }
            }
            catch (Exception ex)
            {

            }
            rs = rs.Take(4);
            string json = JsonConvert.SerializeObject(rs.ToList());
            return json;
        }
        //Lấy ra các từ khóa từ bảng dailykeyword là các xu hướng đọc tin
        public string getTrends(string datetime, int orderby = 0)
        {
            int datetimeid = Uti.datetimeidfromdate(datetime);
            var rs = (from p in db.dailykeywords where p.status==1 select new {id=p.id, title=p.title,keyword = p.keyword, ranking = p.ranking, datetimeid = p.datetimeid, date = p.datetime, status = p.status }).Distinct();

            if (orderby == 0)
            {
                rs = rs.OrderByDescending(o => o.datetimeid).ThenByDescending(o => o.ranking).ThenByDescending(o => o.date);
            }
            else
            {
                rs = rs.OrderByDescending(o => o.date);
            }
            rs = rs.Take(20);
            string json = JsonConvert.SerializeObject(rs.ToList());
            return json;
        }
        private void RunBatch() { 
            
        }
        public ActionResult About()
        {
            ViewBag.Message = "Giới thiệu";
            ViewBag.url = "diemtinvietnam.vn";
            ViewBag.title = "Giới thiệu Điểm tin Việt Nam- Tin tức Việt Nam tổng hợp";
            ViewBag.des = "Dự án của nhóm chuyên gia công nghệ Công ty cổ phần công nghệ Thật Việt Nam";
            ViewBag.image = "";
            return View();
        }

        public ActionResult Contact()
        {
            
            ViewBag.Message = "Liên hệ vnnvh80@gmail.com";
            ViewBag.url = "diemtinvietnam.vn";
            ViewBag.title = "Giới thiệu Điểm tin Việt Nam - Tin tức Việt Nam tổng hợp - Liên hệ vnnvh80@gmail.com";
            ViewBag.des = "Dự án của nhóm chuyên gia công nghệ Công ty cổ phần công nghệ Thật Việt Nam";
            ViewBag.image = "";
            return View();
        }
        //
        // genarate captcha Image
        public ActionResult CaptchaImage(bool noisy = false)
        {
            string[] character = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
            var rand = new Random((int)DateTime.Now.Ticks);
            //generate new question 
            int a1 = rand.Next(0, 9);
            int a2 = rand.Next(0, 9);
            string b1 = character[a1];           

            string captcha = a1 + b1 + a2;

            //store answer 
            Session["Captcha"] = captcha;

            //image stream 
            FileContentResult img = null;

            using (var mem = new MemoryStream())
            using (var bmp = new Bitmap(130, 30))
            using (var gfx = Graphics.FromImage((Image)bmp))
            {
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

                //add noise 
                if (noisy)
                {
                    int i, r, x, y;
                    var pen = new Pen(Color.Yellow);
                    for (i = 1; i < 10; i++)
                    {
                        pen.Color = Color.FromArgb(
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)),
                        (rand.Next(0, 255)));

                        r = rand.Next(0, (130 / 3));
                        x = rand.Next(0, 130);
                        y = rand.Next(0, 30);

                        gfx.DrawEllipse(pen, x - r, y - r, r, r);
                    }
                }

                //add question 
                gfx.DrawString(captcha, new Font("Tahoma", 15), Brushes.Gray, 2, 3);

                //render as Jpeg 
                bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);
                img = this.File(mem.GetBuffer(), "image/Jpeg");
            }

            return img;
        }
        [HttpGet]
        //Tăng hoặc giảm điểm cho tin có id là id dành cho Mobile
        //type là kiểu tăng hay giảm điểm cho tin
        //Ghi lại cookie để check không cho bình chọn liên tiếp-> Cần cải tiến thêm
        public string voteNewsMobile(int id, int type)
        {
            string cookie = "";
            if (System.Web.HttpContext.Current.Request.Cookies["vote_" + id.ToString()] != null)
            {
                cookie = (string)System.Web.HttpContext.Current.Request.Cookies["vote_" + id.ToString()].Value;
            }
            if (string.IsNullOrEmpty(cookie))
            {
                var title = db.titles.Find(id);
                int uplikes = (int)title.uplikes;
                int downlikes = (int)title.downlikes;
                //Tăng điểm
                if (type == 1)
                {
                    title.uplikes = uplikes + 1;

                    title.ranking = title.ranking + Config.PointLike;
                }
                else
                {//giảm điểm
                    title.downlikes = downlikes + 1;
                    title.ranking = title.ranking - Config.PointLike;

                }

                db.Entry(title).State = EntityState.Modified;
                db.SaveChanges();
                //Add cookie
                try
                {
                    HttpCookie MyCookie = new HttpCookie("vote_" + id.ToString());
                    DateTime now = DateTime.Now;
                    MyCookie.Value = "1";
                    MyCookie.Expires = now.AddDays(1);
                    Response.Cookies.Add(MyCookie);
                }
                catch (Exception ex2)
                {
                    return "Enabled cookies";
                }
                //Đếm lại số điểm sau khi đã tăng hoặc giảm điểm
                if (type == 1)
                {
                    var c = (from p in db.titles where p.id == id select p.uplikes).First();
                    return c.ToString();
                }
                else
                {
                    var c = (from p in db.titles where p.id == id select p.downlikes).First();
                    return c.ToString();
                }

            }
            return "0";
        }
        [HttpPost]
        //Tăng hoặc giảm điểm cho tin có id là id
        //type là kiểu tăng hay giảm điểm cho tin
        //Ghi lại cookie để check không cho bình chọn liên tiếp-> Cần cải tiến thêm
        public string voteNews(int id,int type) {
            string cookie="";
            if (System.Web.HttpContext.Current.Request.Cookies["vote_" + id.ToString()] != null)
            {
                cookie = (string)System.Web.HttpContext.Current.Request.Cookies["vote_" + id.ToString()].Value;
            }
            if (string.IsNullOrEmpty(cookie)) {
                var title = db.titles.Find(id);
                int uplikes = (int)title.uplikes;
                int downlikes = (int)title.downlikes;
                //Tăng điểm
                if (type == 1)
                {
                    title.uplikes = uplikes + 1;
                    
                    title.ranking = title.ranking + 1;
                }
                else {//giảm điểm
                    title.downlikes = downlikes + 1;
                    title.ranking = title.ranking - 1;
                   
                }
                
                db.Entry(title).State = EntityState.Modified;
                db.SaveChanges();
                //Add cookie
                try
                {
                    HttpCookie MyCookie = new HttpCookie("vote_" + id.ToString());
                    DateTime now = DateTime.Now;
                    MyCookie.Value = "1";
                    MyCookie.Expires = now.AddDays(1);
                    Response.Cookies.Add(MyCookie);
                }
                catch (Exception ex2) {
                    return "Enabled cookies";
                }
                //Đếm lại số điểm sau khi đã tăng hoặc giảm điểm
                if (type == 1)
                {
                    var c = (from p in db.titles where p.id == id select p.uplikes).First();
                    return c.ToString();
                }
                else {
                    var c = (from p in db.titles where p.id == id select p.downlikes).First();
                    return c.ToString();
                }
                
            } 
            return "0";
        }
        [HttpPost]
        //Tăng hoặc giảm điểm cho tin có id là id
        //type là kiểu tăng hay giảm điểm cho tin
        //Ghi lại cookie để check không cho bình chọn liên tiếp-> Cần cải tiến thêm
        public string voteNewsRead(int id, int type, int PointView)
        {
           
                var title = db.titles.Find(id);
                if (title == null) return "0";
                int uplikes = (int)title.uplikes;
                int downlikes = (int)title.downlikes;
                //Tăng điểm
                if (type == 1)
                {
                    title.uplikes = uplikes + 1;
                    title.ranking = title.ranking + PointView;
                    title.fixRanking = title.fixRanking +PointView;
                    title.totalviews = title.totalviews + 1;
                }
                else
                {//giảm điểm
                    title.downlikes = downlikes + 1;
                    title.ranking = title.ranking - PointView;
                    title.fixRanking = title.fixRanking - PointView;
                    title.totalviews = title.totalviews - 1;
                }

                db.Entry(title).State = EntityState.Modified;
                db.SaveChanges();
                
                //Đếm lại số điểm sau khi đã tăng hoặc giảm điểm
                if (type == 1)
                {
                    var c = (from p in db.titles where p.id == id select p.uplikes).First();
                    return c.ToString();
                }
                else
                {
                    var c = (from p in db.titles where p.id == id select p.downlikes).First();
                    return c.ToString();
                }
            return "0";
        }
        [HttpPost]
        //Hiện function này không dùng nữa.
        public string submitVoteNews(string title, int type, hotkeyword hot, filterkeyword filter)
        {
            if (Session["Captcha"].ToString() != Request.Form["idCaptcha"].ToString()) {
                return "0";
            }
            if (type == 1){
                int oldRanking = 0;
                int datetimeid = Uti.datetimeid();
                var rs = (from p in db.hotkeywords where p.datetimeid == datetimeid && p.keyword.Contains(title.Trim()) select p);
                foreach (var item in rs)
                {
                    oldRanking = item.ranking;
                    db.hotkeywords.Remove(item);
                }
                db.SaveChanges();
                    //hotkeyword hot=null;
                    hot.datetimeid = Uti.datetimeid();
                    hot.keyword = title.Trim();
                    if (Session["userinfo"] == null)
                    {
                        hot.ranking = oldRanking+1;
                    }
                    else
                    {
                        hot.ranking = oldRanking+2;
                    }
                    db.hotkeywords.Add(hot);
                    db.SaveChanges();
                    Session.Remove("Captcha");
                
            }
            else {
                int oldRanking = 0;
                int datetimeid=Uti.datetimeid();
                var rs = (from p in db.filterkeywords where p.datetimeid == datetimeid && p.keyword.Contains(title.Trim()) select p);
                foreach (var item in rs)
                {
                    oldRanking = item.ranking;
                    db.filterkeywords.Remove(item);
                }
                db.SaveChanges();
                filter.datetimeid = Uti.datetimeid();
                filter.keyword = title.Trim();
                if (Session["userinfo"] == null)
                {
                    filter.ranking = oldRanking+1;
                }
                else
                {
                    filter.ranking = oldRanking+2;
                }
                db.filterkeywords.Add(filter);
                db.SaveChanges();
                Session.Remove("Captcha");
            }
            return "1";
        }
    }
}
