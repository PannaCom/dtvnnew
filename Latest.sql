

select top 100 name,datetime,datediff(minute,datetime,getdate()) as timediff,getdate() as now from titles order by datetime desc
