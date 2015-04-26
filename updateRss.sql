select maindomain,count(*) as dem from [tinviet_vietnam].tinviet_admin.titles where datetimeid>=20140528 group by maindomain order by dem 

select * from [tinviet_vietnam].tinviet_admin.rss where name like '%doisongphap%'

update [tinviet_vietnam].tinviet_admin.titles set maindomain='http://hanoimoi.com.vn' where maindomain like '%hanoimoi.com.vn%'
update [tinviet_vietnam].tinviet_admin.rss set name='http://hanoimoi.com.vn' where name like '%hanoimoi.com.vn%'

select * from [tinviet_vietnam].tinviet_admin.rss order by name desc

select * from [tinviet_vietnam].tinviet_admin.rss where CHARINDEX('http://',name)<=0
update [tinviet_vietnam].tinviet_admin.rss set name='http://'+name where CHARINDEX('http://',name)<=0
update [tinviet_vietnam].tinviet_admin.titles set maindomain='http://'+maindomain where CHARINDEX('http://',maindomain)<=0
