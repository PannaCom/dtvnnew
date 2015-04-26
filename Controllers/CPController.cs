using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace youknow.Controllers
{
    public class CPController : Controller
    {
        //
        // GET: /CP/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult checkLogin()
        {
            if (Request.Form["user"].ToString().Equals("admin") && Request.Form["pass"].ToString().Equals("solution"))
            {
                Session["username"] = "admin";
                if (Request.Browser.IsMobileDevice)
                {
                    return RedirectToAction("Index", "dailykeyword");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return RedirectToAction("Index", "CP");
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public string UploadProcess(IEnumerable<HttpPostedFileBase> file)
        {
            //string[] arrNameImage = null;
            string content = "<table>";
            if (ModelState.IsValid)
            {
               // if (Session["tokenImageId"] == null || Session["tokenImageId"] == "") Session["tokenImageId"] = Guid.NewGuid().ToString();
                string physicalPath = HttpContext.Server.MapPath("../Images\\");
                int countFile = Request.Files.Count;
                //tblImageFile tblImgFile; 
                Session["arrImageFileId"] = "(-1,";
                for (int i = 0; i < countFile; i++)
                {
                    string nameFile = String.Format("{0}.jpg", Guid.NewGuid().ToString());
                    //generateNewIdOfImage() + Path.GetExtension(Request.Files[i].FileName);
                    Request.Files[i].SaveAs(physicalPath + System.IO.Path.GetFileName(nameFile));
                    //imgFile.dateTime = DateTime.Now;
                    //imgFile.idDoing = -1;
                    //imgFile.imageName = nameFile;
                    //imgFile.tokenDoingId = Session["tokenImageId"].ToString();
                    //db.ImageStores.Add(imgFile);
                    //db.SaveChanges();
                    //long newId = imgFile.idImage;
                    //Return to a form
                    content += "<tr><td><img src=\"\\Images\\" + nameFile + "\" width=100 height=100></td>";
                   // content += "<td><input type=text value=\"\" id=\"desImage" + i + "\"></td>";
                    //content += "<td><input type=hidden value=\"" + 1 + "\" id=\"idImage" + i + "\">";
                   // content += "<input type=hidden value=\"" + nameFile + "\" id=\"nameImage" + i + "\">";
                    //content += "<a onclick=\"removeThisRow(" + 1 + ");\">remove</a></td></tr>";
                    //Session["arrImageFileId"] += newId.ToString() + ",";
                }
                content += "</table>";
                Session["arrImageFileId"] = "-1)";

            }
            return content;
            //return View(doing); Basic has
        }
        public ActionResult upload() {
            //if (Session["username"] == null) return RedirectToAction("Index", "Home");
            //if (Session["username"] != null && !Session["username"].ToString().Contains("DaoHoaDaoChu")) return RedirectToAction("Index", "Home");

            return View();
        }

    }
}
