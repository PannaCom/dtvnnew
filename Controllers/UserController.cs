using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using youknow.Models;
namespace youknow.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        private timelineEntities db = new timelineEntities();
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            return View(db.users.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(user user)
        {
            try
            {
                // TODO: Add insert logic here
                string pass = user.password;
                MD5 md5Hash = MD5.Create();
                string hash = Config.GetMd5Hash(md5Hash, pass);
                user.password = hash;
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(user user)
        {
            try
            {
                string pass = user.password;
                MD5 md5Hash = MD5.Create();
                string hash = Config.GetMd5Hash(md5Hash, pass);
                user.password = hash;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Index", "Cp");
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                user user = db.users.Find(id);
                db.users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public string Login(string name, string pass)
        {
            MD5 md5Hash = MD5.Create();
            pass = Config.GetMd5Hash(md5Hash, pass);
            var p = (from q in db.users where q.username.Contains(name) && q.password.Contains(pass) select q.username).SingleOrDefault();
            if (p != null && p != "")
            {
                //Ghi ra cookie
                Config.setCookie("logged", "logged");
                return "1";
            }
            else
            {
                return "0";
            }
        }
    }
}
