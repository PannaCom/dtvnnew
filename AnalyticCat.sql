
select * from tinviet_admin.rss where catid=3

select catid,count(id) from tinviet_admin.rss group by catid

select * from tinviet_admin.rss order by  name

select maindomain,catid,count(id) from tinviet_admin.titles where datetimeid>=20140603 group by maindomain,catid

select maindomain,count(id) as dem from tinviet_admin.titles where datetimeid>=20140620 group by maindomain order by dem desc

select  * from tinviet_admin.titles where catid=9 and datetimeid>=20140414

select count(*) as dem,datetimeid from tinviet_admin.titles group by datetimeid order by datetimeid desc

delete from tinviet_admin.titles where datetimeid<=20140520

select catid,count(id) from tinviet_admin.titles where datetimeid>=20140526 group by catid
 
