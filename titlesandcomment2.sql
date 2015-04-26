use timeline
if exists(select * from titles  where tokenid=N'IGh0dHA6Ly9kYW50cmkuY29tLnZuL3hhLWhvaS9uZ29pLXNhby10aGlldC1rZS12aWV0LW5hbS1jdW9jLWNoaWVuLW5nYW0tZ2l1YS1naWFtLWtoYW8tdmEtbmhhLWRhdS10dS04MDgyNTYuaHRtIA==') 
begin 
	if ((select ranking from titles  where tokenid=N'IGh0dHA6Ly9kYW50cmkuY29tLnZuL3hhLWhvaS9uZ29pLXNhby10aGlldC1rZS12aWV0LW5hbS1jdW9jLWNoaWVuLW5nYW0tZ2l1YS1naWFtLWtoYW8tdmEtbmhhLWRhdS10dS04MDgyNTYuaHRtIA==')<72)  
	begin     
		update titles set totalComment=0,ranking=72 where tokenid=N'IGh0dHA6Ly9kYW50cmkuY29tLnZuL3hhLWhvaS9uZ29pLXNhby10aGlldC1rZS12aWV0LW5hbS1jdW9jLWNoaWVuLW5nYW0tZ2l1YS1naWFtLWtoYW8tdmEtbmhhLWRhdS10dS04MDgyNTYuaHRtIA=='  
	end     
	else   
	begin     
		update titles set totalComment=0 where tokenid=N'IGh0dHA6Ly9kYW50cmkuY29tLnZuL3hhLWhvaS9uZ29pLXNhby10aGlldC1rZS12aWV0LW5hbS1jdW9jLWNoaWVuLW5nYW0tZ2l1YS1naWFtLWtoYW8tdmEtbmhhLWRhdS10dS04MDgyNTYuaHRtIA=='  
	end  
end  
else  
begin  
	insert into titles(name,tokenid,des,link,image,domain,maindomain,catid,source,datetime,ranking,keyword,datetimeid,totalComment) values(N' Ng&#244;i sao thiết kế Việt Nam: “Cuộc chiến ngầm” giữa gi&#225;m khảo v&#224; nh&#224; đầu tư ',N'IGh0dHA6Ly9kYW50cmkuY29tLnZuL3hhLWhvaS9uZ29pLXNhby10aGlldC1rZS12aWV0LW5hbS1jdW9jLWNoaWVuLW5nYW0tZ2l1YS1naWFtLWtoYW8tdmEtbmhhLWRhdS10dS04MDgyNTYuaHRtIA==',N'(D&#226;n tr&#237;) - Nếu như những liveshow đầu, cuộc tranh luận giữa c&#225;c gi&#225;m khảo v&#224; nh&#224; đầu tư c&#243; vẻ nhẹ nh&#224;ng th&#236; những tuần gần đ&#226;y, dường như giữa hai b&#234;n đang diễn ra một “cuộc chiến ngầm” nhằm bảo vệ quan điểm của ri&#234;ng m&#236;nh.',N' http://dantri.com.vn/xa-hoi/ngoi-sao-thiet-ke-viet-nam-cuoc-chien-ngam-giua-giam-khao-va-nha-dau-tu-808256.htm ',N'http://dantri21.vcmedia.vn/zoom/130_100/YL0tNBrc4AHkZMVHR5a0/Image/2013/11/bgkndtava-88fc6.JPG',3,,N'http://dantri.com.vn',1,N',2,4,13,14,',N'11/28/2013 2:47:22 PM',72,N'việt nam,ng&#244;i sao,gi&#225;m khảo,đầu tư,nh&#224; đầu tư,',735199,0) 
end
