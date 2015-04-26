use timeline

update titles set totalcomment=(
select totalcomment from 
(
select A.tokenid,count(B.id) as totalcomment from 
(select link,name,tokenid from titles) as A left join
(select linktoken,id from comments) as B on A.tokenid=B.linktoken
group by A.tokenid  
) as C where C.tokenid=titles.tokenid
)
