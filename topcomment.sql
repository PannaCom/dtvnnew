use timeline
 
  select top 5 comments.linktoken,comments.link,titles.name,comments.datetimeid,count(comments.id) as total  from comments left join 
  titles on comments.linktoken=titles.tokenid where comments.datetimeid=735183  group by comments.linktoken,comments.link,titles.name,comments.datetimeid
  order by datetimeid desc,total desc