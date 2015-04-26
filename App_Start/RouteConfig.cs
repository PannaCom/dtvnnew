using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace youknow
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region SEO router 
            // Router SEO
            routes.MapRoute(
                "daily",
                "diem-tin-ngay",
                new { controller = "Home", action = "News"}
            );
            routes.MapRoute(
                "tin nong",
                "tin-nong",
                new { controller = "Home", action = "HotNews" }
            );
            routes.MapRoute(
                "the gioi",
                "the-gioi",
                new { controller = "Home", action = "CatNews", id = 2 }
            );
            routes.MapRoute(
                "xa hoi",
                "xa-hoi",
                new { controller = "Home", action = "CatNews", id = 1 }
            );
            routes.MapRoute(
                "kinh te",
                "kinh-te",
                new { controller = "Home", action = "CatNews", id = 3 }
            );
            routes.MapRoute(
                "fap luat",
                "phap-luat",
                new { controller = "Home", action = "CatNews", id = 6 }
            );
            routes.MapRoute(
                "giao duc",
                "giao-duc",
                new { controller = "Home", action = "CatNews", id = 10 }
            );
            routes.MapRoute(
                "van hoa",
                "van-hoa",
                new { controller = "Home", action = "CatNews", id = 4 }
            );
            routes.MapRoute(
                "giai tri",
                "giai-tri",
                new { controller = "Home", action = "CatNews", id = 13 }
            );
            routes.MapRoute(
                "suc khoe",
                "suc-khoe",
                new { controller = "Home", action = "CatNews", id = 12 }
            );
            routes.MapRoute(
                "tthao",
                "the-thao",
                new { controller = "Home", action = "CatNews", id = 5 }
            );
            routes.MapRoute(
                "khoahoc",
                "khoa-hoc",
                new { controller = "Home", action = "CatNews", id = 7 }
            );
            routes.MapRoute(
                "congnghe",
                "cong-nghe",
                new { controller = "Home", action = "CatNews", id = 8 }
            );
            routes.MapRoute(
                "xee",
                "xe",
                new { controller = "Home", action = "CatNews", id = 9 }
            );

            #endregion

            //routes.MapRoute(
            //    "Doc Bao",
            //    "domain/DomainNews/{domain}/{cat}",
            //    new { controller = "domain", action = "DomainNews", domain = UrlParameter.Optional, cat = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    "Doc Bao Mobile",
            //    "domain/mDomainNews/{domain}/{cat}",
            //    new { controller = "domain", action = "mDomainNews", domain = UrlParameter.Optional, cat = UrlParameter.Optional }
            //);
            
            //routes.MapRoute(
            //    "detailMobile",
            //    "id/{name}",
            //    new { controller = "Title", action = "mDetails", id = UrlParameter.Optional, name = UrlParameter.Optional }
            //);

           
            //routes.MapRoute(
            //      name: "Default",
            //      url: "Home/HotNews",
            //      defaults: new { controller = "Home", action = "HotNews", id = UrlParameter.Optional }
            //  );

           // routes.MapRoute(
           //    name: "HotNews",
           //    url: "{controller}",
           //    defaults: new { controller = "Home", action = "HotNews" }
           //);

            routes.MapRoute(
                "detail tin tuc",
                "{name}-{id}",
                new { controller = "Title", action = "Details", id = UrlParameter.Optional, name = UrlParameter.Optional }
            );//, cat = UrlParameter.Optional, domain = UrlParameter.Optional 

            routes.MapRoute(
              name: "Default",
              url: "{controller}",
              defaults: new { controller = "Home", action = "HotNews" }
           );
            routes.MapRoute(
                 "API",
                 "Home/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional }
             );

            

           routes.MapRoute(
              "readNews",
              "{controller}/{action}/{id}/{name}",
              new { controller = "comment", action = "readNews", id = UrlParameter.Optional, name = UrlParameter.Optional }
           );
           routes.MapRoute(
              "mreadNews",
              "{controller}/{action}/{id}/{name}",
              new { controller = "comment", action = "mReadNews", id = UrlParameter.Optional, name = UrlParameter.Optional }
           );
            
           //routes.MapRoute(
           //    "Xu Huong Doc",
           //    "dailyKeyWord/{action}/{id}",
           //    new { controller = "dailyKeyWord", action = "Index", id = UrlParameter.Optional }
           //);
          // routes.MapRoute(
          //    "Nguon Tin RSS",
          //    "Rss/{action}/{id}",
          //    new { controller = "Rss", action = "Index", id = UrlParameter.Optional }
          // );
          // routes.MapRoute(
          //   "Cp",
          //   "Cp/{action}/{id}",
          //   new { controller = "Cp", action = "Index", id = UrlParameter.Optional }
          //);
          // routes.MapRoute(
          // "News",
          // "News/{action}/{id}",
          // new { controller = "News", action ="Index", id = UrlParameter.Optional }
          //  );
          // routes.MapRoute(
          //   "Admin",
          //   "Admin/{action}/{id}",
          //   new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
          //);
           routes.MapRoute(
                 name: "Default2",
                 url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "HotNews", id = UrlParameter.Optional }
             );
        }
    }
}