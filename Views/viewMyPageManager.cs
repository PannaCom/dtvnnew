using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace youknow.Views
{
    public class viewMyPageManager
    {
        public int idnews { get; set; }    
        public string link { get; set; }
        public string linktoken { get; set; }
        public string title { get; set; }
        public DateTime? datetime { get; set; }
        public string contents { get; set; }
    }
    public class ModelClassViewMyPage {
        public IEnumerable<viewMyPageManager> ieViewMyPage { get; set; }
    }
    
    public class viewNewsMobileAppManager
    {
        public int id  { get; set; }  
        public string title { get; set; }
        //public string des { get; set; }
        public DateTime? date { get; set; }
        public int timediff { get; set; }
        //public string token { get; set; }
        public int? ranking { get; set; }
        public string link { get; set; }
        public string image { get; set; }       
        public string maindomain { get; set; }         
        public int? topicid { get; set; }
        public int? catid { get; set; }
        public string hasContent { get; set; }
    }
    public class viewNewsManager
    {
        public int id  { get; set; }  
        public string title { get; set; }
        public string des  { get; set; }
        public string source { get; set; }
        public DateTime? date { get; set; }
        public int? datetimeid { get; set; }
        //public string token { get; set; }
        public int? ranking { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public int?  totalcomment { get; set; }
        public string maindomain { get; set; } 
        public int? uplikes { get; set; } 
        public int? downlikes {get; set;}
        public int? topicid { get; set; }
        public int? catid { get; set; }
        public byte? hasContent { get; set; }
        public int? timediff { get; set; }
        public string nameRelated { get; set; }
        public int? topicidRelated { get; set; }
        public int? idRelated { get; set; }
        public string linkRelated { get; set; } 
        public string maindomainRelated { get; set; }
        public int? rankingRelated { get; set; }
        public byte? hasContentRelated { get; set; }
        public string imageRelated { get; set; }
    }
  
    public class ModelClassViewNewsManager
    {
        public IEnumerable<viewNewsManager> ieViewNews { get; set; }
    }
    public class viewNewsDomainCat
    {
        public long id { get; set; }//id của tin đó
        public string title { get; set; }//Tên của tin tức
        public string link { get; set; }//link của tin
        public string des { get; set; }//tóm tắt tin
        public string date { get; set; }//ngày đăng tin
        public string image { get; set; }//ảnh của tin
        public int catid { get; set; }//tin thuộc chuyên mục nào
        public int domain { get; set; }//tin lấy từ domain nào?
        public string maindomain { get; set; }//domain chính của tin này là gì? lấy ở đâu.
        public string token { get; set; }//Mã hóa link của tin,base64
        public bool show { get; set; }//
        public string source { get; set; }//Nguồn lấy tin
        public int ranking { get; set; }//Điểm của tin 
        public string keyword { get; set; }//Các từ khóa của tin
        public int datetimeid { get; set; }//Convert datetime sang dạng integer để dễ sắp xếp.
        public int totalcomment { get; set; }//Tổng số comment của tin
        public int topicid { get; set; }//Tin thuộc chủ đề nào.
        public string fullContent { get; set; }//Nội dung tin
        public int hasContent { get; set; }//Nội dung tin
        public string timediff { get; set; }//Khoang cach thoi gian
    }
    public class ModelClassViewNewsDomainCat
    {
        public IEnumerable<viewNewsDomainCat> ieViewNewsDomainCat { get; set; }
    }
    public class viewNewsSearchManager
    {
        public int id { get; set; }
        public string title { get; set; }
        //public string des  { get; set; }
        public string source { get; set; }
        public DateTime? date { get; set; }
        public int timediff { get; set; }
        public int? datetimeid { get; set; }
        //public string token { get; set; }
        public int? ranking { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public int? totalcomment { get; set; }
        public string maindomain { get; set; }
        public int? uplikes { get; set; }
        public int? downlikes { get; set; }
        public int? topicid { get; set; }
        public int? catid { get; set; }
        public byte? hasContent { get; set; }       
    }
    public class ModelClassViewNewsSearchManager
    {
        public IEnumerable<viewNewsSearchManager> ieViewNews { get; set; }
    }
    public class viewNewsAlexa
    {       
        public string site { get; set; }        
        public string des { get; set; }
        public int rank { get; set; }       
    }
    public class ModelClassViewNewsAlexa
    {
        public IEnumerable<viewNewsAlexa> ieViewNewsAlexa { get; set; }
    }
    public class viewTrendsManager
    {
       
        public string title { get; set; }
        public string keyword { get; set; }
        //public int? rankKeyword { get; set; }
        public int? datetimeid { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public byte hasContent { get; set; }
        public string maindomain { get; set; }
        public int topicid { get; set; }
      
    }
    public class ModelClassViewTrendsManager
    {
        public IEnumerable<viewTrendsManager> ieViewTrends { get; set; }
    }

    public class viewDailyKeywordManager
    {
        public int id { get; set; }
        public string keyword { get; set; }
        public string title { get; set; }
        public int? ranking { get; set; }
        public byte? status { get; set; }
        public DateTime? datetime { get; set; }
        public int? datetimeid { get; set; }
    }
    public class ModelClassViewDailyKeywordManager
    {
        public IEnumerable<viewDailyKeywordManager> ieViewDailyKeyword { get; set; }
    }
    public class viewCatNewsLatestManager
    {
        public int id { get; set; }
        public int catid { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public DateTime? datetime { get; set; }
        public string maindomain { get; set; }
        //public string token { get; set; }
        public int? hasContent { get; set; }       
    }
}
