﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
namespace youknow.Controllers
{
    public class OChuController : Controller
    {
        //
        // GET: /OChu/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public string getOchu(int rows, int cols) {
            OChu O = new OChu(rows, cols);
            string json = JsonConvert.SerializeObject(OChu.getOChu().ToList());
            return json;
            //return json;
        }
        public ActionResult Play() {
            return View();
        }
    }
}
