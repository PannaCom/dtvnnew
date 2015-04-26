select * from tinviet_admin.titles where id=2443440


select  ISNULL(B.id,-1) as id,A.idNews,A.image,B.userToken,C.userName,B.userTokenReply,C.email,ISNULL(B.datetime,getdate()) as datetime,A.datetimenews,ISNULL(B.datetimeid,0) as datetimeid,
A.title,A.des,A.fullContent,A.link,B.contents,A.maindomain,A.ranking,A.uplikes,A.downlikes,B.userNameReply from 
(select id as idNews,image,name as title,datetime as datetimenews,des,fullContent,link,maindomain,ranking,uplikes,downlikes from tinviet_admin.titles where id=2443440) as A left join
(select ISNULL(id,0) as id,fullNameReply as userNameReply,usertoken as userToken,datetime,datetimeid,contents,usertokenreplyid as userTokenReply from tinviet_admin.comments) as B on A.idNews=B.id left join
(select usertoken,fullName as userName,email from tinviet_admin.members) as C on B.usertoken=C.usertoken
order by B.id desc


		select  ISNULL(B.id,-1) as id,B.idNews,B.userToken,C.userName,B.userTokenReply,C.email,B.datetime,A.datetimenews,B.datetimeid,  A.title,A.des,A.fullContent,A.link,B.contents,A.maindomain,A.ranking,A.uplikes,A.downlikes,B.userNameReply from  (select id,image,name as title,datetime as datetimenews,des,fullContent,link,maindomain,ranking,uplikes,downlikes from tinviet_admin.titles where id=2443440) as A left join  (select ISNULL(id,0) as id,idnews,fullNameReply as userNameReply,usertoken as userToken,datetime,datetimeid,contents,usertokenreplyid as userTokenReply from tinviet_admin.comments) as B on A.id=B.idnews left join  (select usertoken,fullName as userName,email from tinviet_admin.members) as C on B.usertoken=C.usertoken  order by B.id desc 

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
        public string link { get; set; }        
        public string contents { get; set; }
        public string maindomain { get; set; }
        public int? ranking { get; set; }
        public int? uplikes { get; set; }
        public int? downlikes { get; set; }
        public string userNameReply { get; set; }

		select C.usertoken,C.userName,C.email,ISNULL(B.id,0) as id,B.idnews,B.userNameReply,B.userToken,B.datetime,B.datetimeid,B.contents,B.userTokenReply,A.image,A.title,A.des,A.fullContent,A.link,A.maindomain,A.ranking,A.uplikes,A.downlikes from (select id,image,name as title,des,fullContent,link,maindomain,ranking,uplikes,downlikes from tinviet_admin.titles where id=2443440) as A left join (select ISNULL(id,0) as id,idnews,fullNameReply as userNameReply,usertoken as userToken,datetime,datetimeid,contents,usertokenreplyid as userTokenReply from tinviet_admin.comments) as B on A.id=B.idnews left join (select usertoken,fullName as userName,email from tinviet_admin.members) as C on B.usertoken=C.usertoken order by B.id desc
