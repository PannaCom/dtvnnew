select top 10 A.id,A.title,A.keyword,A.status,A.datetime,A.datetimeid, count(B.idB) as ranking from 
                    (select id,title,keyword,status,ranking as rankKeyword,datetime,datetimeid from tinviet_admin.dailykeyword where  datetimeid>=20140924 and ranking>=3) as A left join 
                    (select id as idB,name,datetimeid  from tinviet_admin.titles where datetimeid>=20140924) as B on CHARINDEX(A.keyword,B.name)>0 
                    group by A.id,A.title,A.keyword,A.status,A.datetime,A.datetimeid order by datetimeid desc,ranking desc
                    