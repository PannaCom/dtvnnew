select top 500 
A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,
token=A.tokenid,ranking=A.ranking,link=A.link,
image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,
uplikes=A.uplikes,downlikes=A.downlikes,
topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,
nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,
linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,
hasContentRelated=B.hasContentRelated from 
(select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,tokenid,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=20140707 and (topicid=id or topicid is null) order by  datetimeid desc, ranking desc, id desc) as A left join
(select datetimeid,name as nameRelated,tokenid,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated from tinviet_admin.titles where datetimeid>=20140707 and catid<>0) as B on (A.topicid=B.topicidRelated and A.topicid=A.id)

order by A.datetimeid desc,A.ranking desc,A.id desc,B.idRelated desc,B.rankingRelated desc

select * from tinviet_admin.titles where id=2723325 or id=2723308



