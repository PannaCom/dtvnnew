 
 use tinviet_vietnam

 select * from tinviet_admin.titles where datetimeid=20140709 order by ranking desc
select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,tokenid,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=20140707 and (topicid=id or topicid is null) order by  datetimeid desc, ranking desc, id desc 


 select id,title,source,date,datetimeid,token,ranking,link,image,totalcomment,maindomain,uplikes,downlikes,topicid,catid,hasContent,timediff 
 ,nameRelated=D.nameRelated,topicidRelated=D.topicidRelated,idRelated=D.idRelated,linkRelated=D.linkRelated,maindomainRelated=D.maindomainRelated,rankingRelated=D.rankingRelated,hasContentRelated=D.hasContentRelated,imageRelated=D.imageRelated from
 (
 select  top 10 B.id,title=B.name,source=B.source,date=B.datetime,datetimeid=A.datetimeid,token=B.tokenid,ranking=B.ranking,link=B.link,image=B.image,totalcomment=B.totalcomment,maindomain=B.maindomain,uplikes=B.uplikes,downlikes=B.downlikes,topicid=B.topicid,catid=B.catid,hasContent=B.hasContent,datediff(minute,B.datetime,GETDATE()) as timediff from 
            (select  datetimeid,max(ranking) as ranking from tinviet_admin.titles where datetimeid>=20140701 and (topicid=id or topicid is null)
            group by  datetimeid) as A join tinviet_admin.titles as B on A.ranking=B.ranking and A.datetimeid=B.datetimeid 
            order by datetimeid desc 
) as C left join
(select datetimeid as datetimeid2,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where catid<>0) as D on (C.id=D.topicidRelated and C.maindomain<>D.maindomainRelated) or (C.id=D.topicidRelated and C.id=C.topicid and C.maindomain=D.maindomainRelated) or (C.id=D.topicidRelated and C.datetimeid>D.datetimeid2 and C.maindomain=D.maindomainRelated) or (C.topicid=D.topicidRelated and C.id<>D.idRelated)
order by datetimeid desc,ranking desc,id desc,idRelated desc,rankingRelated desc


select * from tinviet_admin.titles where id=2715034 or topicid=2715034


