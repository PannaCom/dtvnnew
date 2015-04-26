if exists(select * from timeline.dbo.titles where name like N'Nhật Bản: Th&#234;m một &quot;đại gia&quot; ng&#226;n h&#224;ng thừa nhận cho x&#227; hội đen vay vốn' or link='http://dantri.com.vn/kinh-doanh/nhat-ban-them-mot-dai-gia-ngan-hang-thua-nhan-cho-xa-hoi-den-vay-von-797441.htm')
begin
  if ((select ranking from timeline.dbo.titles where name like N'Nhật Bản: Th&#234;m một &quot;đại gia&quot; ng&#226;n h&#224;ng thừa nhận cho x&#227; hội đen vay vốn' or link='http://dantri.com.vn/kinh-doanh/nhat-ban-them-mot-dai-gia-ngan-hang-thua-nhan-cho-xa-hoi-den-vay-von-797441.htm')>22)
    update timeline.dbo.titles set ranking=ranking+2 where id=7551
end
else
begin
    update timeline.dbo.titles set ranking=ranking+2 where id=7551
end