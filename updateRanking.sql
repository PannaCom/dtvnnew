
update [tinviet_vietnam].[tinviet_admin].[titles] set lastUpdateRanking='2014-01-16 15:15:36.000' where datetimeid=20140116

select datetime,lastUpdateRanking,datediff(hour,'2014-01-16 05:00:00.000',lastUpdateRanking) as h from [tinviet_vietnam].[tinviet_admin].[titles] where datetimeid=20140116

select datetime from [tinviet_vietnam].[tinviet_admin].[titles] where datetimeid=20140116 order by datetime desc

update [tinviet_vietnam].[tinviet_admin].[titles] set ranking=ranking-datediff(hour,datetime,lastUpdateRanking)*5,downlikes=datediff(hour,datetime,lastUpdateRanking)*5 where datetimeid=20140116

update [tinviet_vietnam].[tinviet_admin].[titles] set fixranking=ranking