select top 250
A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,
token=A.tokenid,ranking=A.ranking,link=A.link,
image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,
uplikes=A.uplikes,downlikes=A.downlikes,
topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETUTCDATE()) as timediff,
nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,
linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,
hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from 
(select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,tokenid,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from  tinviet_admin.titles where datetimeid>=20140530 and isHot=1 order by datetime desc,ranking desc,id desc) as A left join
(select name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from  tinviet_admin.titles where datetimeid>=20140530) as B on A.id=B.topicidRelated
order by A.datetime desc,A.ranking desc,A.id desc,B.rankingRelated desc