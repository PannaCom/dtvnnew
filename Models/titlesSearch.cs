//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace youknow.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class titlesSearch
    {
        public int id { get; set; }
        public string name { get; set; }
        public string des { get; set; }
        public string fullcontent { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public string domain { get; set; }
        public string maindomain { get; set; }
        public Nullable<int> catid { get; set; }
        public string source { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public Nullable<int> ranking { get; set; }
        public string keyword { get; set; }
        public Nullable<int> datetimeid { get; set; }
        public Nullable<int> totalcomment { get; set; }
        public Nullable<int> uplikes { get; set; }
        public Nullable<int> downlikes { get; set; }
        public Nullable<int> topicid { get; set; }
        public Nullable<byte> isHot { get; set; }
        public Nullable<short> isNew { get; set; }
        public Nullable<byte> hasContent { get; set; }
        public Nullable<int> fixRanking { get; set; }
        public Nullable<System.DateTime> lastUpdateRanking { get; set; }
        public Nullable<int> totalviews { get; set; }
    }
}
