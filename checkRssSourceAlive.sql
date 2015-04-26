 select name from tinviet_admin.rss where name not in (select maindomain as name from tinviet_admin.titles where datetimeid>=20140616 group by maindomain) group by name
 select name,url,domain from tinviet_admin.rss where domain not in (select domain from tinviet_admin.titles where datetimeid=20140615 group by domain) group by name,url,domain
 select * from tinviet_admin.rss where name like '%soha%'

 select * from tinviet_admin.titles where  datetimeid>=20140615  and maindomain like '%vietnamnet%' order by datetime desc 

 update tinviet_admin.rss set url='soha' where id=137


 select a.name as name,url,catid,domain,b.name as maindomain from tinviet_admin.rss as a left join tinviet_admin.domain as b on a.domain=b.id where catid<>0

 select catid,count(*) from tinviet_admin.titles where datetimeid>=20140616 group by catid