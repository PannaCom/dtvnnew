using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
namespace youknow.Controllers
{
    public class AdvController : Controller
    {
        //
        // GET: /Adv/
        private timelineEntities db = new timelineEntities();        
        public ActionResult Index()
        {
            var p = (from q in db.zadvs select q).OrderByDescending(o => o.datetime).Take(100);
            return View(p.ToList());
        }
        public ActionResult Create() {
            return View();
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string Edit(zadv zadv)
        {
            return "ok";
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string Upload(zadv zadv, IEnumerable<HttpPostedFileBase> file)
        {
            //if (Session["username"] == null) return RedirectToAction("Index", "Adv");
            if (ModelState.IsValid)
            {
                string physicalPath = HttpContext.Server.MapPath("../Images/Adv\\");
                string ext=".jpg";
                byte type = 0;
                string nameFile ="";
                int countFile = Request.Files.Count;
                //tblImageFile tblImgFile; 
                try{
                    for (int i = 0; i < countFile; i++)
                    {
                    
                        if (Request.Files[i].FileName.IndexOf(".swf") > 0) { ext = ".swf"; type = 1; }
                        nameFile = Request.Files[i].FileName;// String.Format("{0}" + ext, Guid.NewGuid().ToString());                    
                        Request.Files[i].SaveAs(physicalPath + System.IO.Path.GetFileName(nameFile));                    
                    }
                    zadv.views = 0;
                    zadv.filepath = "/Images/Adv/" + nameFile;
                    zadv.filetype=type;
                    zadv.datetime = DateTime.Now;
                    zadv.enddate = DateTime.Now.AddDays(1000);
                    db.zadvs.Add(zadv);
                    db.SaveChanges();
                    return zadv.id.ToString();
                }catch(Exception ex){
                    return "-1";
                }
                //return "Success!";
            }

            return "-1";
        }
    }
}
