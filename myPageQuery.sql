
use timeline


select top 10 title,link,linktoken,datetime,contents from
(select C.name as title,C.link as link,C.tokenid as linktoken,B.idContent as idContent from
(select * from members where userToken='Tmd1eWVuVmlldEh1eTJfdHdpdHRlcg==') as A
left join (select userToken,linktoken,max(id) as idContent from 
comments group by userToken,linktoken) as B on A.userToken=B.usertoken 
left join (select * from titles) as C on B.linktoken=C.tokenid
) as D left join
(select id,contents,datetime,datetimeid from comments) as E on D.idContent=E.id
order by datetime desc


