select A.title,A.keyword,A.rankKeyword,A.datetimeid,B.id,B.name,B.link,B.image,B.hasContent,B.maindomain,B.datetime from 
<<<<<<< .mine
                (select title,keyword,status,ranking as rankKeyword,datetimeid from tinviet_admin.dailykeyword where status=1 and datetimeid>=20140607) as A left join 
               (select distinct isNull(image,'') as image,id,link,hasContent,name,ranking,datetime,datetimeid,maindomain from tinviet_admin.titles where datetimeid>=20140605) as B on CHARINDEX(A.keyword,B.name)>0 and B.datetimeid=A.datetimeid
                     order by A.keyword,A.datetimeid desc,A.rankKeyword desc,B.datetime desc,B.name,B.maindomain
=======
                (select title,keyword,status,ranking as rankKeyword,datetimeid from tinviet_admin.dailykeyword where status=1 and datetimeid>=20140608) as A left join 
               (select distinct isNull(image,'') as image,id,link,hasContent,name,ranking,datetime,datetimeid,maindomain from tinviet_admin.titles where datetimeid>=20140606 and catid<>0) as B on CHARINDEX(A.keyword,B.name)>0 and B.datetimeid=A.datetimeid
                     order by A.keyword,A.datetimeid desc,A.rankKeyword desc,B.id desc
>>>>>>> .r271
					 
