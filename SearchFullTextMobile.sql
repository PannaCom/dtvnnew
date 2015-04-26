SELECT top 100 
             FT_TBL.id,FT_TBL.name as title,FT_TBL.source,FT_TBL.datetime as date,datediff(hour,FT_TBL.datetime,getdate()) as timediff,
             FT_TBL.datetimeid as datetimeid,FT_TBL.ranking as ranking,FT_TBL.link as link, 
             FT_TBL.image as image,FT_TBL.totalcomment as totalcomment,FT_TBL.maindomain as maindomain, 
             FT_TBL.uplikes as uplikes,FT_TBL.downlikes as downlikes,FT_TBL.topicid as topicid,FT_TBL.catid as catid, 
             FT_TBL.hasContent as hasContent, KEY_TBL.RANK FROM titles AS FT_TBL INNER JOIN FREETEXTTABLE(titles, name,'Obama') AS KEY_TBL ON FT_TBL.id = KEY_TBL.[KEY] order by Rank Desc,datetimeid desc,ranking desc

			 update titles set hasContent=0 where maindomain like '%vtc.vn%' 