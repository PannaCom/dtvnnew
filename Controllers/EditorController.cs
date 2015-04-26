using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;

namespace youknow.Controllers
{
    public class EditorController : Controller
    {
        public timelineEntities db = new timelineEntities();        
        //
        // GET: /Editor/

        public ActionResult Index(string keyword)
        {
            //if (keyword != null)
            //{
            //    var p = (from q in db.contents where q.title.Contains(keyword) select q).OrderByDescending(o => o.datetime).Take(100);
            //    return View(p.ToList());
            //}
            //else {
            //    var p = (from q in db.contents select q).OrderByDescending(o => o.datetime).Take(100);
            //    return View(p.ToList());
            //}
            return View();
            
        }
        public ActionResult create() {
            return View();
        }
        public ActionResult view(int id = 0)
        {
            //var q = (from ct in db.contents where ct.id == id select ct).ToList();
            
            return View();
        }

        // FOR POST
        //public string showCats()
        //{
        //    var q = (from cats in db.cats select cats).ToList();

        //    return JsonConvert.SerializeObject(q);
        //}
        //public string showGroups()
        //{
        //    var q = (from groups in db.groups select groups).ToList();

        //    return JsonConvert.SerializeObject(q);
        //}

        [HttpPost, ValidateInput(false)]
        public string post(string title = "",string link="", string des = "", string image = "", string fullcontent = "", string catid = "", string groupid = "", string moduleid = "")
        {
            try
            {
                //content newpost = new content();
                //newpost.title = title;
                ////newpost.link = link;
                //newpost.des = des;
                //newpost.image = image;
                //newpost.fullcontent = fullcontent;
                //newpost.catid = int.Parse(catid);
                ////newpost.groupid = int.Parse(groupid);
                //newpost.datetime = DateTime.Now;
                ////newpost.moduleid = int.Parse(moduleid);

                //db.contents.Add(newpost);
                //db.SaveChanges();

                //return newpost.id.ToString();
                return "";

            }
            catch (Exception)
            {
                return "0";
            }            
            
        }
        // EDIT
        public ActionResult edit(int id = 0)
        {
            //var q = (from editpost in db.contents where editpost.id == id select editpost).ToList();

            return View(); 
        }

        [HttpPost, ValidateInput(false)]
        public string editpost(string id = "", string title = "",string link="", string des = "", string image = "", string fullcontent = "", string catid = "", string groupid = "", string moduleid = "")
        {
            try
            {
                //content newpost = db.contents.Find(int.Parse(id));
                
                //newpost.title = title;
                ////newpost.link = link;
                //newpost.des = des;
                //newpost.image = image;
                //newpost.fullcontent = fullcontent;
                //newpost.catid = int.Parse(catid);
                ////newpost.groupid = int.Parse(groupid);
                ////newpost.moduleid = int.Parse(moduleid);
                //newpost.datetime = DateTime.Now;

                //db.SaveChanges();

                return "1";

            }
            catch (Exception)
            {
                return "0";
            }

        }
        

    }
}
