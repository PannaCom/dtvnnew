using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace youknow.Views
{
    public class viewCommentManager
    {
        public long? id { get; set;}
        public int? idNews { get; set; }
        public string image { get; set; }
        public string userToken { get; set; }
        public string userName { get; set; }
        public string userTokenReply { get; set; }
        public string email { get; set; }
        public DateTime? datetime { get; set; }
        public DateTime? datetimenews { get; set; }
        public int? datetimeid { get; set; }
        public string title { get; set; }
        public string des { get; set; }
        public string fullContent { get; set; }
        public string keyword { get; set; }
        public string link { get; set; }        
        public string contents { get; set; }
        public string maindomain { get; set; }
        public int? ranking { get; set; }
        public int? uplikes { get; set; }
        public int? downlikes { get; set; }
        public string userNameReply { get; set; }
        public int? topicid { get; set; }
        public int? catid { get; set; }
    }
    public class ModelClassViewComment
    {
        public IEnumerable<viewCommentManager> ieViewComment { get; set; }
    }
    public class viewCommentManagerRealtime
    {
        
        public int? idNews { get; set; }
        public string image { get; set; }        
        public string title { get; set; }
        public string des { get; set; }
        public string link { get; set; }
        public string maindomain { get; set; }
        public int? ranking { get; set; }
        public int? uplikes { get; set; }
        public int? downlikes { get; set; }
        public DateTime? lastDateTimeComment { get; set; }
        public int? totalComment { get; set; }
        public DateTime? datetime { get; set; }
    }
    public class ModelClassViewCommentRealtime
    {
        public IEnumerable<viewCommentManagerRealtime> ieViewCommentRealtime { get; set; }
    }

}