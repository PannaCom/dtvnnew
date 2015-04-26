use tinviet_vietnam

select * from titles order by datetime desc,datetimeid desc,fixranking desc,ranking desc


update titles set datetime=CONVERT(datetime, 
               SWITCHOFFSET(CONVERT(datetimeoffset, 
                                    datetime), 
                            DATENAME(TzOffset, SYSDATETIMEOFFSET())))

update titles set datetimeid=Cast(CONVERT(varchar(8), datetime, 112) as int)  