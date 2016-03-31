using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;
using System.Security.Cryptography;
namespace youknow
{
    public class Config
    {
        
        public static Regex titRegex = new Regex(@"<td class=(.)tt-left-content(.)(?s).*<!--- Edit mode ---->", RegexOptions.IgnoreCase);
        
        public const string connectStringWF = "Server=SQL5004.myWindowsHosting.com;Database=DB_9AAE99_timeline;User Id=DB_9AAE99_timeline_admin;Password=Huynguyenviet1;";
            //"Server=Huynv\\Local;Database=timeline;User Id=sa;Password=111111;";
            //"Data Source=SQL5004.myWindowsHosting.com;Initial Catalog=DB_9AAE99_timeline;User Id=DB_9AAE99_timeline_admin;Password=Huynguyenviet1;";
            //"Server=Huynv\\Local;Database=timeline;User Id=sa;Password=111111;";
        
        public const int BonusCountRelated = 50;//tăng 50 ranking mỗi khi có tin liên quan, chứng tỏ nó được nhiều báo đưa
        public const int maxDesLength = 150;
        public const int PageSizeLoad = 20;
        public const int PointLike = 30;//Tăng 30 điểm nếu user vote
        public const int PointView = 5;//Tăng 5 ranking mỗi lượt xem thường
        public const int PointViewFacebook = 20;//Tăng 20 ranking mỗi lượt xem từ facebook
        public const int PointViewGoogle = 10;//Tăng 10 ranking mỗi lượt xem từ google
        public const int MinPointRankingDailyKeyword = 8;//Tăng 10 ranking mỗi lượt xem từ google
        public static String NewsImagePath = "/Images/News/";
        public struct CatLink
        {
            public CatLinkItem[] item;
            public string domain;
        }
        public struct CatLinkItem
        {
            public string catname;
            public string link;
        }
        public static CatLink[] domainLink = null; 

        public const string domain = "http://diemtinvietnam.vn";
        public const string notFoundErr = "404 NOT FOUND";
        public const string notCrawl = "404 NOT Crawl";//khong craw tin nay 
        public const byte minHourHotNews = 6;
        public const byte PointUpdateRanking = 30;

        public static void loadDomainCatLink()
        {
            if (domainLink == null)
            {
                domainLink = new CatLink[13];
                domainLink[0].domain = "http://vnexpress.net";
                domainLink[0].item = new CatLinkItem[15];
                domainLink[0].item[0].catname="Mới nhất";
                domainLink[0].item[0].link="http://vnexpress.net/rss/tin-moi-nhat.rss";
                domainLink[0].item[1].catname = "Thời sự";
                domainLink[0].item[1].link = "http://vnexpress.net/rss/thoi-su.rss";
                domainLink[0].item[2].catname = "Đời sống";
                domainLink[0].item[2].link = "http://vnexpress.net/rss/doi-song.rss";
                domainLink[0].item[3].catname = "Thế giới";
                domainLink[0].item[3].link = "http://vnexpress.net/rss/the-gioi.rss";                
                domainLink[0].item[4].catname = "Kinh doanh";
                domainLink[0].item[4].link = "http://vnexpress.net/rss/kinh-doanh.rss";
                domainLink[0].item[5].catname = "Giải trí";
                domainLink[0].item[5].link = "http://vnexpress.net/rss/giai-tri.rss";
                domainLink[0].item[6].catname = "Thể thao";
                domainLink[0].item[6].link = "http://vnexpress.net/rss/the-thao.rss";
                domainLink[0].item[7].catname = "Pháp luật";
                domainLink[0].item[7].link = "http://vnexpress.net/rss/phap-luat.rss";
                domainLink[0].item[8].catname = "Du lịch";
                domainLink[0].item[8].link = "http://vnexpress.net/rss/du-lich.rss";
                domainLink[0].item[9].catname = "Khoa học";
                domainLink[0].item[9].link = "http://vnexpress.net/rss/khoa-hoc.rss";
                domainLink[0].item[10].catname = "Số hóa";
                domainLink[0].item[10].link = "http://vnexpress.net/rss/so-hoa.rss";
                domainLink[0].item[11].catname = "Xe";
                domainLink[0].item[11].link = "http://vnexpress.net/rss/oto-xe-may.rss";
                domainLink[0].item[12].catname = "Cộng đồng";
                domainLink[0].item[12].link = "http://vnexpress.net/rss/cong-dong.rss";
                domainLink[0].item[13].catname = "Tâm sự";
                domainLink[0].item[13].link = "http://vnexpress.net/rss/tam-su.rss";
                domainLink[0].item[14].catname = "Cười";
                domainLink[0].item[14].link = "http://vnexpress.net/rss/cuoi.rss";


                domainLink[1].domain = "http://dantri.com.vn";
                domainLink[1].item = new CatLinkItem[20];
                domainLink[1].item[0].catname = "Trang chủ";
                domainLink[1].item[0].link = "http://dantri.com.vn/trangchu.rss";
                domainLink[1].item[1].catname = "Xã Hội";
                domainLink[1].item[1].link = "http://dantri.com.vn/xa-hoi.rss";
                domainLink[1].item[2].catname = "Thế Giới";
                domainLink[1].item[2].link = "http://dantri.com.vn/Thegioi.rss";
                domainLink[1].item[3].catname = "Thể thao";
                domainLink[1].item[3].link = "http://dantri.com.vn/The-Thao.rss";                
                domainLink[1].item[4].catname = "Giáo dục";
                domainLink[1].item[4].link = "http://dantri.com.vn/giaoduc-khuyenhoc.rss";
                domainLink[1].item[5].catname = "Tấm lòng nhân ái";
                domainLink[1].item[5].link = "http://dantri.com.vn/tamlongnhanai.rss";
                domainLink[1].item[6].catname = "Kinh doanh";
                domainLink[1].item[6].link = "http://dantri.com.vn/kinhdoanh.rss";
                domainLink[1].item[7].catname = "Văn hóa";
                domainLink[1].item[7].link = "http://dantri.com.vn/van-hoa.rss";
                domainLink[1].item[8].catname = "Giải trí";
                domainLink[1].item[8].link = "http://dantri.com.vn/giaitri.rss";
                domainLink[1].item[9].catname = "Pháp luật";
                domainLink[1].item[9].link = "http://dantri.com.vn/skphapluat.rss";
                domainLink[1].item[10].catname = "Nhịp sống trẻ";
                domainLink[1].item[10].link = "http://dantri.com.vn/nhipsongtre.rss";
                domainLink[1].item[11].catname = "Tình yêu giới tính";
                domainLink[1].item[11].link = "http://dantri.com.vn/tinhyeu-gioitinh.rss";
                domainLink[1].item[12].catname = "Sức khỏe";
                domainLink[1].item[12].link = "http://dantri.com.vn/suckhoe.rss";               
                domainLink[1].item[13].catname = "Sức mạnh số";
                domainLink[1].item[13].link = "http://dantri.com.vn/cong-nghe.rss";  
                domainLink[1].item[14].catname = "Ô tô xe máy";
                domainLink[1].item[14].link = "http://dantri.com.vn/otoxemay.rss"; 
                domainLink[1].item[15].catname = "Bạn đọc";
                domainLink[1].item[15].link = "http://dantri.com.vn/diendan-bandoc.rss";                
                domainLink[1].item[16].catname = "Du lịch";
                domainLink[1].item[16].link = "http://dantri.com.vn/du-lich.rss";
                domainLink[1].item[17].catname = "Chuyện lạ";
                domainLink[1].item[17].link = "http://dantri.com.vn/chuyen-la.rss";
                domainLink[1].item[18].catname = "Tuyển sinh";
                domainLink[1].item[18].link = "http://dantri.com.vn/tuyen-sinh.rss";                
                domainLink[1].item[19].catname = "Blog";
                domainLink[1].item[19].link = "http://dantri.com.vn/blog.rss";                

                domainLink[2].domain = "http://news.zing.vn";
                domainLink[2].item = new CatLinkItem[15];
                domainLink[2].item[0].catname = "Trang chủ";
                domainLink[2].item[0].link = "http://news.zing.vn/";
                domainLink[2].item[1].catname = "Xã Hội";
                domainLink[2].item[1].link = "http://news.zing.vn/xa-hoi.html";
                domainLink[2].item[2].catname = "Thế Giới";
                domainLink[2].item[2].link = "http://news.zing.vn/the-gioi.html";
                domainLink[2].item[3].catname = "Thị trường";
                domainLink[2].item[3].link = "http://news.zing.vn/thi-truong.html";
                domainLink[2].item[4].catname = "Thể thao";
                domainLink[2].item[4].link = "http://news.zing.vn/the-thao.html";
                domainLink[2].item[5].catname = "Sống trẻ";
                domainLink[2].item[5].link = "http://news.zing.vn/song-tre.html";
                domainLink[2].item[6].catname = "Pháp luật";
                domainLink[2].item[6].link = "http://news.zing.vn/phap-luat.html";
                domainLink[2].item[7].catname = "Tin sách";
                domainLink[2].item[7].link = "http://news.zing.vn/the-gioi-sach.html";
                domainLink[2].item[8].catname = "Giải trí";
                domainLink[2].item[8].link = "http://news.zing.vn/giai-tri.html";
                domainLink[2].item[9].catname = "Âm nhạc";
                domainLink[2].item[9].link = "http://news.zing.vn/am-nhac.html";
                domainLink[2].item[10].catname = "Phim ảnh";
                domainLink[2].item[10].link = "http://news.zing.vn/phim-anh.html";
                domainLink[2].item[11].catname = "Thời trang";
                domainLink[2].item[11].link = "http://news.zing.vn/thoi-trang.html";
                domainLink[2].item[12].catname = "Công nghệ";
                domainLink[2].item[12].link = "http://news.zing.vn/cong-nghe.html";
                domainLink[2].item[13].catname = "Xe 360";
                domainLink[2].item[13].link = "http://news.zing.vn/oto-xe-may.html";
                domainLink[2].item[14].catname = "Nhịp sống";
                domainLink[2].item[14].link = "http://news.zing.vn/nhip-song.html";

                domainLink[3].domain = "http://www.24h.com.vn";
                domainLink[3].item = new CatLinkItem[19];
                //domainLink[3].item[0].catname = "Trang chủ";
                //domainLink[3].item[0].link = "http://www.24h.com.vn/upload/rss/tintuctrongngay.rss";
                domainLink[3].item[0].catname = "Bóng đá";
                domainLink[3].item[0].link = "http://www.24h.com.vn/upload/rss/bongda.rss";
                domainLink[3].item[1].catname = "An ninh hình sự";
                domainLink[3].item[1].link = "http://www.24h.com.vn/upload/rss/anninhhinhsu.rss";
                domainLink[3].item[2].catname = "Thời trang";
                domainLink[3].item[2].link = "http://www.24h.com.vn/upload/rss/thoitrang.rss";
                domainLink[3].item[3].catname = "Thời trang Hitech";
                domainLink[3].item[3].link = "http://www.24h.com.vn/upload/rss/thoitranghitech.rss";
                domainLink[3].item[4].catname = "Tài chính - Bất động sản";
                domainLink[3].item[4].link = "http://www.24h.com.vn/upload/rss/taichinhbatdongsan.rss";
                domainLink[3].item[5].catname = "Ẩm thực";
                domainLink[3].item[5].link = "http://www.24h.com.vn/upload/rss/amthuc.rss";
                domainLink[3].item[6].catname = "Làm đẹp";
                domainLink[3].item[6].link = "http://www.24h.com.vn/upload/rss/lamdep.rss";
                domainLink[3].item[7].catname = "Phim";
                domainLink[3].item[7].link = "http://www.24h.com.vn/upload/rss/phim.rss";
                domainLink[3].item[8].catname = "Giáo dục - du học";
                domainLink[3].item[8].link = "http://www.24h.com.vn/upload/rss/giaoducduhoc.rss";
                domainLink[3].item[9].catname = "Bạn trẻ và cuộc sống";
                domainLink[3].item[9].link = "http://www.24h.com.vn/upload/rss/bantrecuocsong.rss";
                domainLink[3].item[10].catname = "Ca nhạc MTV";
                domainLink[3].item[10].link = "http://www.24h.com.vn/upload/rss/canhacmtv.rss";
                domainLink[3].item[11].catname = "Thể thao";
                domainLink[3].item[11].link = "http://www.24h.com.vn/upload/rss/thethao.rss";
                domainLink[3].item[12].catname = "Phi thường kỳ quặc";
                domainLink[3].item[12].link = "http://www.24h.com.vn/upload/rss/phithuongkyquac.rss";
                domainLink[3].item[13].catname = "Công nghệ thông tin";
                domainLink[3].item[13].link = "http://www.24h.com.vn/upload/rss/congnghethongtin.rss";
                domainLink[3].item[14].catname = "Ô tô xe máy";
                domainLink[3].item[14].link = "http://www.24h.com.vn/upload/rss/otoxemay.rss";
                domainLink[3].item[15].catname = "Thị trường tiêu dùng";
                domainLink[3].item[15].link = "http://www.24h.com.vn/upload/rss/thitruongtieudung.rss";
                domainLink[3].item[16].catname = "Du lịch";
                domainLink[3].item[16].link = "http://www.24h.com.vn/upload/rss/dulich.rss";
                domainLink[3].item[17].catname = "Sức khỏe đời sống";
                domainLink[3].item[17].link = "http://www.24h.com.vn/upload/rss/suckhoedoisong.rss";
                domainLink[3].item[18].catname = "Cười";
                domainLink[3].item[18].link = "http://www.24h.com.vn/upload/rss/cuoi24h.rss";

                domainLink[4].domain = "http://kenh14.vn";
                domainLink[4].item = new CatLinkItem[18];
                domainLink[4].item[0].catname = "Trang chủ";
                domainLink[4].item[0].link = "http://kenh14.vn/home.rss";
                domainLink[4].item[1].catname = "Star";
                domainLink[4].item[1].link = "http://kenh14.vn/star.rss";
                domainLink[4].item[2].catname = "Music";
                domainLink[4].item[2].link = "http://kenh14.vn/musik.rss";
                domainLink[4].item[3].catname = "Cine";
                domainLink[4].item[3].link = "http://kenh14.vn/cine.rss";
                domainLink[4].item[4].catname = "Fashion";
                domainLink[4].item[4].link = "http://kenh14.vn/fashion.rss";
                domainLink[4].item[5].catname = "Đời sống";
                domainLink[4].item[5].link = "http://kenh14.vn/doi-song.rss";
                domainLink[4].item[6].catname = "Xã hội";
                domainLink[4].item[6].link = "http://kenh14.vn/xa-hoi.rss";
                domainLink[4].item[7].catname = "Sức khỏe giới tính";
                domainLink[4].item[7].link = "http://kenh14.vn/suc-khoe-gioi-tinh.rss";                
                domainLink[4].item[8].catname = "Made by me";
                domainLink[4].item[8].link = "http://kenh14.vn/made-by-me.rss";
                domainLink[4].item[9].catname = "Lạ";
                domainLink[4].item[9].link = "http://kenh14.vn/la.rss";
                domainLink[4].item[10].catname = "Sport";
                domainLink[4].item[10].link = "http://kenh14.vn/sport.rss";
                domainLink[4].item[11].catname = "Funny";
                domainLink[4].item[11].link = "http://kenh14.vn/funny.rss";
                domainLink[4].item[12].catname = "Khám phá";
                domainLink[4].item[12].link = "http://kenh14.vn/kham-pha.rss";
                domainLink[4].item[13].catname = "2 Tek";
                domainLink[4].item[13].link = "http://kenh14.vn/2-tek.rss";
                domainLink[4].item[14].catname = "Cool";
                domainLink[4].item[14].link = "http://kenh14.vn/cool.rss";
                domainLink[4].item[15].catname = "Góc trái tim";
                domainLink[4].item[15].link = "http://kenh14.vn/goc-trai-tim.rss";
                domainLink[4].item[16].catname = "Học đường";
                domainLink[4].item[16].link = "http://kenh14.vn/hoc-duong.rss";


                domainLink[5].domain = "http://www.doisongphapluat.com";
                domainLink[5].item = new CatLinkItem[11];
                domainLink[5].item[0].catname = "Trang chủ";
                domainLink[5].item[0].link = "http://www.doisongphapluat.com/trang-chu.rss";
                domainLink[5].item[1].catname = "Tin tức";
                domainLink[5].item[1].link = "http://www.doisongphapluat.com/rss/tin-tuc.rss";
                domainLink[5].item[2].catname = "Pháp luật";
                domainLink[5].item[2].link = "http://www.doisongphapluat.com/rss/phap-luat.rss";
                domainLink[5].item[3].catname = "Đời sống";
                domainLink[5].item[3].link = "http://www.doisongphapluat.com/rss/doi-song.rss";
                domainLink[5].item[4].catname = "Thế giới";
                domainLink[5].item[4].link = "http://www.doisongphapluat.com/rss/the-gioi.rss";
                domainLink[5].item[5].catname = "Kinh doanh";
                domainLink[5].item[5].link = "http://www.doisongphapluat.com/rss/kinh-doanh.rss";
                domainLink[5].item[6].catname = "Giải trí";
                domainLink[5].item[6].link = "http://www.doisongphapluat.com/rss/giai-tri.rss";
                domainLink[5].item[7].catname = "Giáo dục";
                domainLink[5].item[7].link = "http://www.doisongphapluat.com/rss/giao-duc.rss";
                domainLink[5].item[8].catname = "Công nghệ";
                domainLink[5].item[8].link = "http://www.doisongphapluat.com/rss/cong-nghe.rss";
                domainLink[5].item[9].catname = "Thể thao";
                domainLink[5].item[9].link = "http://www.doisongphapluat.com/rss/the-thao.rss";
                domainLink[5].item[10].catname = "Ô tô xe máy";
                domainLink[5].item[10].link = "http://www.doisongphapluat.com/rss/oto-xemay.rss";

                domainLink[6].domain = "http://thethao247.vn";
                domainLink[6].item = new CatLinkItem[6];
                domainLink[6].item[0].catname = "Trang chủ";
                domainLink[6].item[0].link = "http://thethao247.vn/home.rss";
                domainLink[6].item[1].catname = "Bóng đá Việt Nam";
                domainLink[6].item[1].link = "http://thethao247.vn/bong-da-viet-nam-c1.rss";
                domainLink[6].item[2].catname = "Bóng đá Quốc tế";
                domainLink[6].item[2].link = "http://thethao247.vn/bong-da-quoc-te-c2.rss";
                domainLink[6].item[3].catname = "Quần vợt";
                domainLink[6].item[3].link = "http://thethao247.vn/quan-vot-c4.rss";
                domainLink[6].item[4].catname = "Hậu trường";
                domainLink[6].item[4].link = "http://thethao247.vn/hau-truong-c49.rss";
                domainLink[6].item[5].catname = "Thể thao tổng hợp";
                domainLink[6].item[5].link = "http://thethao247.vn/tin-the-thao-tong-hop-c5.rss";
                //domainLink[6].item[6].catname = "Lịch thi đấu";
                //domainLink[6].item[6].link = "http://thethao247.vn/lich-thi-dau-ket-qua-c53/";

                domainLink[7].domain = "http://ngoisao.net";
                domainLink[7].item = new CatLinkItem[18];
                domainLink[7].item[0].catname = "Hậu trường";
                domainLink[7].item[0].link = "http://ngoisao.net/rss/hau-truong.rss";
                domainLink[7].item[1].catname = "Showbiz Việt";
                domainLink[7].item[1].link = "http://ngoisao.net/rss/showbiz-viet.rss";
                domainLink[7].item[2].catname = "Châu Á";
                domainLink[7].item[2].link = "http://ngoisao.net/rss/chau-a.rss";
                domainLink[7].item[3].catname = "Hollywood";
                domainLink[7].item[3].link = "http://ngoisao.net/rss/hollywood.rss";
                domainLink[7].item[4].catname = "Bên lề";
                domainLink[7].item[4].link = "http://ngoisao.net/rss/ben-le.rss";
                domainLink[7].item[5].catname = "Khỏe đẹp";
                domainLink[7].item[5].link = "http://ngoisao.net/rss/khoe-dep.rss";
                domainLink[7].item[6].catname = "Thời cuộc";
                domainLink[7].item[6].link = "http://ngoisao.net/rss/thoi-cuoc.rss";
                domainLink[7].item[7].catname = "Phong cách";
                domainLink[7].item[7].link = "http://ngoisao.net/rss/phong-cach.rss";
                domainLink[7].item[8].catname = "Thời trang";
                domainLink[7].item[8].link = "http://ngoisao.net/rss/thoi-trang.rss";
                domainLink[7].item[9].catname = "Tâm tình";
                domainLink[7].item[9].link = "http://ngoisao.net/rss/tam-tinh.rss";
                domainLink[7].item[10].catname = "Làm đẹp";
                domainLink[7].item[10].link = "http://ngoisao.net/rss/lam-dep.rss";
                domainLink[7].item[11].catname = "Ăn chơi";
                domainLink[7].item[11].link = "http://ngoisao.net/rss/an-choi.rss";
                domainLink[7].item[12].catname = "Dân chơi";
                domainLink[7].item[12].link = "http://ngoisao.net/rss/dan-choi.rss";           
                domainLink[7].item[13].catname = "Thư giãn";
                domainLink[7].item[13].link = "http://ngoisao.net/rss/thu-gian.rss";
                domainLink[7].item[14].catname = "Góc độc giả";
                domainLink[7].item[14].link = "http://ngoisao.net/rss/goc-doc-gia.rss";
                domainLink[7].item[15].catname = "Cưới hỏi";
                domainLink[7].item[15].link = "http://ngoisao.net/rss/cuoi-hoi.rss";
                domainLink[7].item[16].catname = "Ảnh cưới";
                domainLink[7].item[16].link = "http://ngoisao.net/rss/anh-cuoi.rss";
                domainLink[7].item[17].catname = "Cô dâu";
                domainLink[7].item[17].link = "http://ngoisao.net/rss/co-dau.rss";

                domainLink[8].domain = "http://vietnamnet.vn";
                domainLink[8].item = new CatLinkItem[12];
                domainLink[8].item[0].catname = "Trang chủ";
                domainLink[8].item[0].link = "http://vietnamnet.vn/rss/home.rss";
                domainLink[8].item[1].catname = "Xã hội";
                domainLink[8].item[1].link = "http://vietnamnet.vn/rss/xa-hoi.rss";
                domainLink[8].item[2].catname = "Giáo dục";
                domainLink[8].item[2].link = "http://vietnamnet.vn/rss/giao-duc.rss";
                domainLink[8].item[3].catname = "Chính trị";
                domainLink[8].item[3].link = "http://vietnamnet.vn/rss/chinh-tri.rss";
                domainLink[8].item[4].catname = "Tuần Việt Nam";
                domainLink[8].item[4].link = "http://vietnamnet.vn/rss/tuanvietnam.rss";
                domainLink[8].item[5].catname = "Đời sống";
                domainLink[8].item[5].link = "http://vietnamnet.vn/rss/doi-song.rss";
                domainLink[8].item[6].catname = "Kinh tế";
                domainLink[8].item[6].link = "http://vietnamnet.vn/rss/kinh-te.rss";
                domainLink[8].item[7].catname = "Quốc tế";
                domainLink[8].item[7].link = "http://vietnamnet.vn/rss/quoc-te.rss";
                domainLink[8].item[8].catname = "Văn hóa";
                domainLink[8].item[8].link = "http://vietnamnet.vn/rss/van-hoa.rss";
                domainLink[8].item[9].catname = "Khoa học";
                domainLink[8].item[9].link = "http://vietnamnet.vn/rss/khoa-hoc.rss";
                domainLink[8].item[10].catname = "Công nghệ thông tin";
                domainLink[8].item[10].link = "http://vietnamnet.vn/rss/cong-nghe-thong-tin-vien-thong.rss";
                domainLink[8].item[11].catname = "Bạn đọc";
                domainLink[8].item[11].link = "http://vietnamnet.vn/rss/ban-doc-phap-luat.rss";

                domainLink[9].domain = "http://laodong.com.vn";
                domainLink[9].item = new CatLinkItem[10];
                //domainLink[9].item[0].catname = "Trang chủ";
                //domainLink[9].item[0].link = "http://laodong.com.vn/rss/home.rss";
                domainLink[9].item[0].catname = "Chính trị";
                domainLink[9].item[0].link = "http://laodong.com.vn/rss/chinh-tri-57.rss";
                domainLink[9].item[1].catname = "Công đoàn";
                domainLink[9].item[1].link = "http://laodong.com.vn/rss/cong-doan-58.rss";
                domainLink[9].item[2].catname = "Thế giới";
                domainLink[9].item[2].link = "http://laodong.com.vn/rss/the-gioi-62.rss";
                domainLink[9].item[3].catname = "Xã hội";
                domainLink[9].item[3].link = "http://laodong.com.vn/rss/xa-hoi-59.rss";
                domainLink[9].item[4].catname = "Kinh tế";
                domainLink[9].item[4].link = "http://laodong.com.vn/rss/kinh-te-63.rss";
                domainLink[9].item[5].catname = "Pháp luật";
                domainLink[9].item[5].link = "http://laodong.com.vn/rss/phap-luat-65.rss";
                domainLink[9].item[6].catname = "Thể thao";
                domainLink[9].item[6].link = "http://laodong.com.vn/rss/the-thao-60.rss";
                domainLink[9].item[7].catname = "Văn hóa";
                domainLink[9].item[7].link = "http://laodong.com.vn/rss/van-hoa-61.rss";
                domainLink[9].item[8].catname = "Công nghệ";
                domainLink[9].item[8].link = "http://laodong.com.vn/rss/cong-nghe-66.rss";
                domainLink[9].item[9].catname = "Diễn đàn";
                domainLink[9].item[9].link = "http://laodong.com.vn/rss/dien-dan-75.rss";

                domainLink[10].domain = "http://afamily.vn";
                domainLink[10].item = new CatLinkItem[13];
                domainLink[10].item[0].catname = "Trang chủ";
                domainLink[10].item[0].link = "http://afamily.vn/trang-chu.rss";
                domainLink[10].item[1].catname = "Đẹp";
                domainLink[10].item[1].link = "http://afamily.vn/dep.rss";
                domainLink[10].item[2].catname = "Đời sống";
                domainLink[10].item[2].link = "http://afamily.vn/doi-song.rss";
                domainLink[10].item[3].catname = "Công sở";
                domainLink[10].item[3].link = "http://afamily.vn/cong-so.rss";
                domainLink[10].item[4].catname = "Ăn ngon";
                domainLink[10].item[4].link = "http://afamily.vn/an-ngon.rss";
                domainLink[10].item[5].catname = "Tình yêu hôn nhân";
                domainLink[10].item[5].link = "http://afamily.vn/tinh-yeu-hon-nhan.rss";
                domainLink[10].item[6].catname = "Sức khỏe";
                domainLink[10].item[6].link = "http://afamily.vn/suc-khoe.rss";
                domainLink[10].item[7].catname = "Tâm sự";
                domainLink[10].item[7].link = "http://afamily.vn/tam-su.rss";
                domainLink[10].item[8].catname = "Mẹ và bé";
                domainLink[10].item[8].link = "http://afamily.vn/me-va-be.rss";
                domainLink[10].item[9].catname = "Nhà hay";
                domainLink[10].item[9].link = "http://afamily.vn/nha-hay.rss";
                domainLink[10].item[10].catname = "Hậu trường";
                domainLink[10].item[10].link = "http://afamily.vn/hau-truong.rss";
                domainLink[10].item[11].catname = "Giải trí";
                domainLink[10].item[11].link = "http://afamily.vn/giai-tri.rss";
                domainLink[10].item[12].catname = "Chuyện lạ";
                domainLink[10].item[12].link = "http://afamily.vn/chuyen-la.rss";
            

                domainLink[11].domain = "http://tuoitre.vn";
                domainLink[11].item = new CatLinkItem[9];
                domainLink[11].item[0].catname = "Chính trị xã hội";
                domainLink[11].item[0].link = "http://tuoitre.vn/Chinh-tri-Xa-hoi";
                domainLink[11].item[1].catname = "Pháp luật";
                domainLink[11].item[1].link = "http://tuoitre.vn/Chinh-tri-Xa-hoi/Phap-luat";
                domainLink[11].item[2].catname = "Thế giới";
                domainLink[11].item[2].link = "http://tuoitre.vn/The-gioi";
                domainLink[11].item[3].catname = "Kinh tế";
                domainLink[11].item[3].link = "http://tuoitre.vn/Kinh-te";
                domainLink[11].item[4].catname = "Giáo dục";
                domainLink[11].item[4].link = "http://tuoitre.vn/Giao-duc";
                domainLink[11].item[5].catname = "Nhịp sống trẻ";
                domainLink[11].item[5].link = "http://tuoitre.vn/Nhip-song-tre";
                domainLink[11].item[6].catname = "Văn hóa giải trí";
                domainLink[11].item[6].link = "http://tuoitre.vn/Van-hoa-Giai-tri";
                domainLink[11].item[7].catname = "Thế giới xe";
                domainLink[11].item[7].link = " http://tuoitre.vn/kinh-te/The-gioi-xe";           
                domainLink[11].item[8].catname = "Bạn đọc";
                domainLink[11].item[8].link = "http://tuoitre.vn/Ban-doc";

                domainLink[12].domain = "http://www.thanhnien.com.vn";
                domainLink[12].item = new CatLinkItem[11];
                domainLink[12].item[0].catname = "Trang chủ";
                domainLink[12].item[0].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx";
                domainLink[12].item[1].catname = "Chính trị xã hội";
                domainLink[12].item[1].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=Ch%C3%ADnh+tr%E1%BB%8B+-+X%C3%A3+h%E1%BB%99i";
                domainLink[12].item[2].catname = "Thế giới trẻ";
                domainLink[12].item[2].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=Th%E1%BA%BF+gi%E1%BB%9Bi+tr%E1%BA%BB";
                domainLink[12].item[3].catname = "Kinh tế";
                domainLink[12].item[3].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=Kinh+t%E1%BA%BF";
                domainLink[12].item[4].catname = "Thế giới";
                domainLink[12].item[4].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=Th%E1%BA%BF+gi%E1%BB%9Bi";
                domainLink[12].item[5].catname = "Văn hóa nghệ thuật";
                domainLink[12].item[5].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=V%C4%83n+h%C3%B3a+-+Ngh%E1%BB%87+thu%E1%BA%ADt";
                domainLink[12].item[6].catname = "Giáo dục";
                domainLink[12].item[6].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=Gi%C3%A1o+d%E1%BB%A5c";
                domainLink[12].item[7].catname = "Công nghệ thông tin";
                domainLink[12].item[7].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=C%C3%B4ng+ngh%E1%BB%87+th%C3%B4ng+tin";
                domainLink[12].item[8].catname = "Khoa học";
                domainLink[12].item[8].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=Khoa+h%E1%BB%8Dc";
                domainLink[12].item[9].catname = "Sức khỏe";
                domainLink[12].item[9].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=S%E1%BB%A9c+kh%E1%BB%8Fe";
                domainLink[12].item[10].catname = "Đời sống";
                domainLink[12].item[10].link = "http://www.thanhnien.com.vn/_layouts/newsrss.aspx?Channel=%C4%90%E1%BB%9Di+s%E1%BB%91ng";

            }
            //domainLink.Add("");
        }

        public static string getTemplateContent(string regex, string url)
        {
            
            try
            {
                if (url.Contains("zing.vn"))
                {
                    return getZing(url);
                }
                else if (url.Contains("kenh14.vn"))
                {
                    return getKenh14(url);
                }
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                try
                {
                    foreach (HtmlNode link1 in doc.DocumentNode.SelectNodes("//div[contains(@class,'" + regex + "')]"))
                    {
                        string att1 = link1.InnerHtml;
                        //string att2 = link.InnerText;
                        if (att1 != "" && att1 != null) return att1;
                    }
                }
                catch (Exception ex) {
                    doc = null;
                    hw = null;
                    hw = new HtmlWeb();
                    doc = hw.Load(url);
                    foreach (HtmlNode link2 in doc.DocumentNode.SelectNodes("//div[contains(@id,'" + regex + "')]"))
                    {
                        string att2 = link2.InnerHtml;
                        //string att2 = link.InnerText;
                        if (att2 != "" && att2 != null) return att2;
                    }

                }
                
            }
            catch (Exception ex2)
            {
                return notFoundErr;
            }
            return notFoundErr;
        }

        public static string getThanhNien(string url) {
            string fullContent = "";
            string domain = "http://www.thanhnien.com.vn/";
            if (url.Contains("http://thethao.thanhnien.com.vn/"))
            {
                domain="http://thethao.thanhnien.com.vn/";
            }
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'print-news')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'article-content article-content03')]").InnerHtml;
                    //return fullContent;
                    //bo cac the tin lien quan di
                    string span = "";
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@class,'article-content article-content03')]").SelectNodes(".//a[contains(@style,'text-decoration:none;')]"))
                        {
                            try
                            {
                                span = linkspan.InnerText;
                                fullContent = fullContent.Replace(span,"");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }
                    fullContent = removeAllRelatedLink(fullContent);
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = Regex.Replace(fullContent, @"<td class=(.)tt-left-content(.)(?s).*<div class=(.)article-content ", "");
            fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"[^http](.*?)>", "<img src=\"" + domain + "$2\">");
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\""+domain+"$2\">");
            //fullContent = Regex.Replace(fullContent, "<p style=\"text-align:left\">(?s).*<!--- Edit mode ---->", "");
            //fullContent = fullContent.Replace("article-content03\">", "");
            //fullContent = Regex.Replace(fullContent, "<p style=(.)text-align:right(.)>(?s).*<!--- Edit mode ---->", "");
                       
            return fullContent;
        }
        public static string getDantri(string url)
        {
            //return "";
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'ctl00_IDContent_ctl00_divContent')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@id,'divNewsContent')]").InnerHtml;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            string domain = "http://dantri.com.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getGiaoDucNetVn(string url)
        {
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'content-wrap')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'body')]").InnerHtml;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            string domain = "http://giaoduc.net.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getTechz(string url)
        {
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'primary left')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@id,'abdf')]").InnerHtml;
                    string span = "";
                    //Remove tin lien quan
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@id,'abdf')]").SelectNodes(".//div[contains(@class,'news-relation-top-detail')]"))
                        {
                            try
                            {
                                span = linkspan.InnerHtml;
                                fullContent = fullContent.Replace(span, "");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            string domain = "http://www.techz.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getKienThucNetVn(string url)
        {
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//section[contains(@class,'main-article clearfix')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'text')]").InnerHtml;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<div class=\"tag\">(?s).*</div>", "");
            fullContent = Regex.Replace(fullContent, "<div class=\"article-relate-bottom clearfix hzol-clear\" style=\"font-size: 16px;\">(?s).*</div>", "");
            string domain = "http://kienthuc.net.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            //fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            return fullContent;
        }
        public static string getVuiViet(string url)
        {
            //if (url.Contains("http://vuiviet.vn/category/video-clip"))
            //{
            //    return getVuiVietVideo(url);
            //}
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//article[contains(@class,'post-content')]"))
                {
                    try
                    {
                        //if (link.InnerHtml.Contains("post-video")) return link.InnerHtml;
                        fullContent = link.SelectSingleNode(".//div[contains(@class,'post-text')]").InnerHtml;
                    }
                    catch (Exception ex) {
                        //fullContent = link.InnerHtml;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<script async=\"\" src=\"//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js\"></script>", "");
            //fullContent = Regex.Replace(fullContent, "<div class=\"article-relate-bottom clearfix hzol-clear\" style=\"font-size: 16px;\">(?s).*</div>", "");

            //fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            return fullContent;
        }
        public static string getClipVn(string url)
        {
            //if (url.Contains("http://vuiviet.vn/category/video-clip"))
            //{
            //    return getVuiVietVideo(url);
            //}
            return notFoundErr;
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'container watch-container')]"))
                {
                    try
                    {
                        //if (link.InnerHtml.Contains("post-video")) return link.InnerHtml;
                        fullContent = link.SelectSingleNode(".//div[contains(@id,'watch-region')]").InnerHtml;
                    }
                    catch (Exception ex)
                    {
                        //fullContent = link.InnerHtml;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<script async=\"\" src=\"//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js\"></script>", "");
            //fullContent = Regex.Replace(fullContent, "<div class=\"article-relate-bottom clearfix hzol-clear\" style=\"font-size: 16px;\">(?s).*</div>", "");

            //fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            return fullContent;
        }
        public static string getVuiVietVideo(string url)
        {
            string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //hw.OverrideEncoding = Encoding.UTF8;
                //HtmlDocument doc = hw.Load(url);
                //WebProxy wp = new WebProxy("42.112.31.43");
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                //client.Proxy = wp;
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//article[contains(@class,'post-content')]"))
                {
                    fullContent = link.InnerHtml;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<script async=\"\" src=\"//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js\"></script>", "");
            //fullContent = Regex.Replace(fullContent, "<div class=\"article-relate-bottom clearfix hzol-clear\" style=\"font-size: 16px;\">(?s).*</div>", "");

            //fullContent = fullContent.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            return fullContent;
        }
        public static string getLaodong(string url)
        {
            string fullContent = "";
            string domain = "http://laodong.com.vn/";
            try
            {

                var doc = new HtmlDocument();
                string html;
                using (var wc = new GZipWebClient())
                    html = wc.DownloadString(url);
                //html = DecodeFromUtf8(html);
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'article-content')]"))
                {
                    //Lay ra content chinh.
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'content')]").InnerHtml;
                    //Remove tin lien quan
                    string span = "";
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@class,'main-contents')]").SelectNodes(".//ul[contains(@class,'article-relate clearfix')]"))
                        {
                            try
                            {
                                span = linkspan.InnerHtml;
                                fullContent = fullContent.Replace(span, "");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }
                    //Remove tin facebook va cac option
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@class,'main-contents')]").SelectNodes(".//ul[contains(@class,'story-tools clearfix')]"))
                        {
                            try
                            {
                                span = linkspan.InnerHtml;
                                fullContent = fullContent.Replace(span, "");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }
                    //Thoat
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = DecodeFromUtf8(fullContent);
            fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"[^http](.*?)>", "<img src=\"http://laodong.com.vn/$2\">");
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            fullContent = Regex.Replace(fullContent, "<param value=(.)file=(.*?)>", "<param value=\"file=http://laodong.com.vn$2\">");
            fullContent = Regex.Replace(fullContent, "<embed flashvars=(.)file=(.*?)>", "<embed flashvars=\"file=http://laodong.com.vn$2\">");
            fullContent = Regex.Replace(fullContent, "<div class=(.)stories-box-horizontal(.)>(?s).*", "");
            fullContent = Regex.Replace(fullContent, "<ul class=(.)story-tools clearfix(.)(?s).*<div id=(.)ctl00_mainContent_divAvatar(.)", "<div id=\"ctl00_mainContent_divAvatar\"");
            if (fullContent.Contains("class=\"galleria-images\"") || fullContent.Contains("id=\"galleria\""))
            {
                fullContent = notCrawl;
            }
            return fullContent;
        }
        public static string DecodeFromUtf8(string utf16String)
        {
            byte[] bytes = Encoding.Default.GetBytes(utf16String);
            string myString = Encoding.UTF8.GetString(bytes);
            return myString;
        }
        public static string getNgoisao(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            string domain = "http://ngoisao.net/";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'detailCT')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'fck_detail')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = Regex.Replace(fullContent, "<div id=(.)top-social(.)(?s).*<h1 class=(.)title(.)>", "<h1 class=\"title\">");
            fullContent = Regex.Replace(fullContent, "<table(.*?)width=\"1\">", "<table width=\"90%\">");
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getTuoiTreTheThao(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            string domain = "http://thethao.tuoitre.vn/";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'content_left')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'one_new')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            //fullContent = Regex.Replace(fullContent, "<div id=(.)top-social(.)(?s).*<h1 class=(.)title(.)>", "<h1 class=\"title\">");

            return fullContent;
        }
        public static string getTinMoi(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'tm-content')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'one_new')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<div id=\"add_\" class=\"box-news-top\">(?s).*</div>", "");
            fullContent = Regex.Replace(fullContent, "<iframe(?s).*>(?s).*</iframe>", "");
            
            return fullContent;
        }
        public static string getTuoitre(string url)
        {
            string fullContent = "";
            string domain = "http://tuoitre.vn/";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                //Phải lấy 2 vòng div bởi trong div bao quanh bài viết có thể có thêm các thẻ ảnh,...cùng với thẻ chính chứa
                //nội dung
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'detail-content')]"))
                {
                    //try
                    //{
                    //    fullContent = link.SelectSingleNode(".//div[contains(@class,'fck')]").InnerHtml;
                    //}
                    //catch (Exception ex)//Lay anh
                    //{ 
                    //}
                    //try
                    //{
                    //    fullContent += link.SelectSingleNode(".//div[contains(@class,'gallery')]").InnerHtml;
                    //}
                    //catch (Exception ex2)//Lay anh
                    //{ 
                    //}
                    fullContent += link.SelectSingleNode(".//div[contains(@class,'fck')]").InnerHtml;
                    //bo cac the span di
                    //string span = "";
                    //try
                    //{
                    //    foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@id,'divArticleDetailMain')]").SelectNodes(".//a[contains(@style,'TEXT-DECORATION: none;')]"))
                    //    {
                    //        try
                    //        {
                    //            span = linkspan.InnerText;
                    //            fullContent = fullContent.Replace(span, "");
                    //        }
                    //        catch (Exception spanitem) { 
                    //        }
                    //    }
                    //}
                    //catch (Exception exspan) { 
                    //}
                    
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = Regex.Replace(fullContent, "<div id=(.)top-social(.)(?s).*<h1 class=(.)title(.)>", "<h1 class=\"title\">");
            //return fullContent;
            //fullContent = Regex.Replace(fullContent, "<div class=(.)colitemdetailNewsLeft(.)>(?s).*<div class=(.)colitemDetailNewsRight(.)", "<div class=\"colitemDetailNewsRight\"");
            fullContent = Regex.Replace(fullContent, "<div class=\"boxComment\">(?s).*</div>", "");
            fullContent=fullContent.Replace("<a href=\"javascript:;\" class=\"btnSlideshow\">Phóng to</a>","");
            fullContent = Regex.Replace(fullContent, "<div class=\"bdTop bderBtom pad10\">(?s).*</div>", "");
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            //fullContent = Regex.Replace(fullContent, "<span(?s).*>(?s).*</span>", "");
            
            return fullContent;
        }
        public static string get24h(string url)
        {
            string fullContent = "";
            string domain = "http://24h.com.vn/";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'colCenter')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'text-conent')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = Regex.Replace(fullContent, "<div id=(.)top-social(.)(?s).*<h1 class=(.)title(.)>", "<h1 class=\"title\">");
            //return fullContent;
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getVietnamnet(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'ArticleDetail')]"))
                {
                    try
                    {
                        fullContent += link.SelectSingleNode(".//div[contains(@id,'ArticlePhotoSliderShow')]").InnerHtml;
                    }
                    catch (Exception ex)
                    {
                    }
                    fullContent += link.SelectSingleNode(".//div[contains(@class,'ArticleContent')]").InnerHtml;
                    fullContent = fullContent.Replace("TIN BÀI LIÊN QUAN:", "");
                    if (fullContent.Contains("http://res.vietnamnet.vn/jscore/player/player.swf")) return notFoundErr;
                    fullContent = removeAllRelatedLink(fullContent);
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            if (fullContent.Contains("ArticlePhotoSliderShow-Head"))
            {
                //fullContent = Regex.Replace(fullContent,"(.)w=57&amp;h=57\" width=\"57\" height=\"57\"","\"");
                fullContent = notFoundErr;
                //fullContent += "<script>";
                //fullContent +=" var widthImage = $(window).width()*60/100;";     
                //fullContent +=" $(\"img\").css(\"width\", widthImage);";
                //fullContent +="$(\"img\").css(\"height\", \"auto\");";
                //fullContent += "</script>";
                //$("img").css("maxWidth",widthImage);
                //arr[i].hasContent = 0;
            }
            fullContent=Regex.Replace(fullContent, "<img alt=\"\" src=\"http://imgs.vietnamnet.vn/logo.gif\">", "");
            fullContent = Regex.Replace(fullContent, "<img alt=\"\" src=\"http://imgs.vietnamnet.vn/logo/vef.gif\">", "");
            string domain = "http://vietnamnet.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
        
            //fullContent = Regex.Replace(fullContent, "<p><strong>(?s).*</strong></p>", "");
            return fullContent;
        }
        public static string getVtc(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                try
                {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'pageContent')]"))
                    {
                        fullContent = link.InnerHtml;//SelectSingleNode(".//div[contains(@id,'pageContent')]")
                        //return fullContent;
                        //bo cac the tin lien quan di
                        string span = "";
                        try
                        {
                            foreach (HtmlNode linkspan in link.SelectNodes(".//a[contains(@class,'related_articles_container')]"))
                            {
                                try
                                {
                                    span = linkspan.InnerText;
                                    fullContent = fullContent.Replace(span, "");
                                }
                                catch (Exception spanitem)
                                {
                                }
                            }
                            fullContent = fullContent.Replace("Video đang được xem nhiều", "");
                        }
                        catch (Exception exspan)
                        {
                        }
                        break;
                    }
                }
                catch (Exception ex2) {
                    fullContent = "";
                }
                //Neu co album image thi bo di khong lay ve
                if (fullContent.Equals("")) {                    
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'pictureGallery')]"))
                    {
                        //fullContent = link.SelectSingleNode(".//div[contains(@itemprop,'articleBody')]").InnerHtml;
                        //fullContent += link.SelectSingleNode(".//div[contains(@class,'sl_relate_article')]").InnerHtml; 
                        fullContent = notCrawl;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //if (fullContent.Contains("id=\"pictureGallery\""))
            //{
            //    fullContent = notCrawl;
            //}
            //fullContent = Regex.Replace(fullContent, "<script>(?s).*</script>", "");
            //fullContent = Regex.Replace(fullContent, "<a(.*?)class=\"tinlienquan\"(.*?)>(?s).*</a>", "");
            //fullContent = Regex.Replace(fullContent, "<div class=(.)shareseo_liked(.)(?s).*", "");
            string domain = "http://vtc.vn/";
            //string domain2 = "http://res.vtc.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            fullContent = Regex.Replace(fullContent,".jpg\"",".JPG\"");
            //fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"[^http](.*?)>", "<img src=\"" + domain2 + "$2\">");
            return fullContent;
        }
        public static string getSoha(string fullContent)
        {
            Regex titRegex = new Regex(@"<div id=(.)ctl00_cphMainContent(?s).*popup-poll-bg", RegexOptions.IgnoreCase);
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            fullContent = Regex.Replace(fullContent, "<div class=(.)fl mgr20 linkhay(.)(?s).*</iframe>", "");
            return fullContent;
        }
        public static string getCafeBiz(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'divnewsdetail')]"))
                {
                    try
                    {
                        fullContent += link.SelectSingleNode(".//div[contains(@class,'f_l w100 t_a_c newsimage')]").InnerHtml;
                    }
                    catch (Exception ex)
                    {
                    }
                    fullContent += link.SelectSingleNode(".//div[contains(@class,'contentbody')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = Regex.Replace(fullContent, "<div class=(.)tlqitems(.)>(?s).*<div id=(.)contentv2(.)", "<div id=\"contentv2\"");
            //fullContent = Regex.Replace(fullContent, "<span class=(.)tagname(.)(?s).*<div style=(.)clear: both;(.)>", "");
            string domain = "http://cafebiz.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getCafeF(string url)
        {
            //return notCrawl;
            string fullContent = "";
            //string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //HtmlDocument doc = hw.Load(url);
                string html;              
                WebClient client = new WebClient();
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'newscontent_right')]"))
                {
                    //try
                    //{
                    //    fullContent += link.SelectSingleNode(".//div[contains(@id,'ctl00_contentMain_ucNewsContent1_divBigImage')]").InnerHtml;
                    //}
                    //catch (Exception ex) { 
                    //}
                    fullContent += link.InnerHtml;//SelectSingleNode(".//div[contains(@id,'contentv2')]").
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            //fullContent = Regex.Replace(fullContent, "<div class=(.)tlqitems(.)>(?s).*<div id=(.)contentv2(.)", "<div id=\"contentv2\"");
            //fullContent = Regex.Replace(fullContent, "<span class=(.)tagname(.)(?s).*<div style=(.)clear: both;(.)>", "");
            string domain = "http://cafef.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getAutopro(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'news-details')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@id,'content-id')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<div class=\"tags\">", "");
            string domain = "http://autopro.com.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            //fullContent = Regex.Replace(fullContent, "<div class=(.)box-news(.)>(?s).*</iframe>", "");
            //fullContent = Regex.Replace(fullContent, "http://autopro4.vcmedia.vn/images/logo2.jpg", "");
            return fullContent;
        }
        public static string getThethao247(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//details[contains(@class,'pkg')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@id,'main-detail')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<div id=\"add\" class=\"box-news-suggest box-news-suggest-hor style1\">(?s).*</div>", "");
            string domain = "http://thethao247.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string get2sao(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                try
                {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'vmccontent')]"))
                    {
                        fullContent = link.InnerHtml;//SelectSingleNode(".//div[contains(@class,'detail_content')]").
                        //return fullContent;
                        break;
                    }
                }
                catch (Exception ex1) { 
                }
                //Nếu vẫn chưa lấy được
                try
                {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'detailcolleft')]"))
                    {
                        fullContent = link.SelectSingleNode(".//div[contains(@class,'detail_content')]").InnerHtml;
                        //return fullContent;
                        break;
                    }
                }
                catch (Exception ex2)
                {
                }

            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<div class=\"tags\">(?s).*</div>", "");
            string domain = "http://2sao.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getBaohay(string fullContent)
        {
            return notFoundErr;
            Regex titRegex = new Regex(@"<div class=(.)itemHeader(.)>(?s).*<div class=(.)itemFacebookButton(.)>", RegexOptions.IgnoreCase);
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            fullContent = Regex.Replace(fullContent, "src=\"(.*?)\"", "");
            fullContent = Regex.Replace(fullContent, "<img(.*?)pagespeed_lazy_src=\"(.*?)>", "<img src=\"$2\">");
            return fullContent;
        }
        public static string getCongan(string fullContent)
        {
            Regex titRegex = new Regex(@"Expires(?s).*Pragma", RegexOptions.IgnoreCase);
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            return fullContent;
        }
        public static string getGdtd(string fullContent)
        {
            //http://gdtd.vn/ template
            Regex titRegex = new Regex(@"<div class=(.)article(?s).*<div class=(.)article-ads clearfix(.)>", RegexOptions.IgnoreCase);
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            return fullContent;
        }
        public static string getDanviet(string fullContent)
        {
            //http://danviet.vn/ template
            Regex titRegex = new Regex(@"<h1 class=(.)ntitle(.)(?s).*<ul class=(.)share(.)>", RegexOptions.IgnoreCase);            
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            return fullContent;
        }
        public static string getOtozine(string fullContent)
        {
            //otozine template
            Regex titRegex = new Regex(@"<h2 class=(.)titleNewsDetail(.)(?s).*<div class=(.)tabLDBottom(.)></div>", RegexOptions.IgnoreCase);
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"(.*?)>", "<img src=\"http://otozine.net//$2\">");
            fullContent = Regex.Replace(fullContent, "</h2>(?s).*div class=(.)direction(.)>", "</h2>");
            return fullContent;
        }
        public static string getBizlive(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'ctl00_main_pnNormal')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'page-detail')]").InnerHtml;
                    //return fullContent;
                    string span = "";
                    //Remove tin facebook va cac option khhac
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@class,'page-detail')]").SelectNodes(".//div[contains(@class,'summary clearfix')]"))
                        {
                            try
                            {
                                span = linkspan.InnerHtml;
                                fullContent = fullContent.Replace(span, "");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }
                    //Remove tin lien quan
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@class,'page-detail')]").SelectNodes(".//ul[contains(@class,'internal-links')]"))
                        {
                            try
                            {
                                span = linkspan.InnerHtml;
                                fullContent = fullContent.Replace(span, "");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }
                    
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"[^http](.*?)>", "<img src=\"http://bizlive.vn//$2\">");
            fullContent = Regex.Replace(fullContent, "</h2>(?s).*div class=(.)direction(.)>", "</h2>");
            fullContent = Regex.Replace(fullContent, "<div id=(.)adsctl00_main_AdsHomeTop(.)(?s).*</div>", "");
            string domain = "http://bizlive.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getKhoahocCom(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'rPanel1')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@id,'divContent')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"[^http](.*?)>", "<img src=\"http://www.khoahoc.com.vn//$2\">");
            fullContent = Regex.Replace(fullContent, "</h2>(?s).*div class=(.)direction(.)>", "</h2>");
            string domain = "http://www.khoahoc.com.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getThethaovanhoa(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            if (url.Contains("http://worldcup.thethaovanhoa.vn/")) return notCrawl;
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'clearfix chitiet')]"))
                {
                    try
                    {
                        fullContent += link.SelectSingleNode(".//div[contains(@class,'ovh chitiet_one')]").InnerHtml;
                    }
                    catch (Exception ex) { 
                    }
                    fullContent += link.SelectSingleNode(".//div[contains(@class,'clearfix cont')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            return fullContent;
        }
        public static string getVnexpress(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'w670 left')]"))
                {
                    try
                    {
                        fullContent = link.SelectSingleNode(".//div[contains(@class,'block_content_slide_showdetail')]").InnerHtml;                        
                    }
                    catch (Exception ex2) {
                        try
                        {
                            fullContent = link.SelectSingleNode(".//div[contains(@class,'fck_detail width_common')]").InnerHtml;                            
                        }
                        catch (Exception ex3) {
                            fullContent = link.SelectSingleNode(".//div[contains(@id,'container_tab_live')]").InnerHtml;
                            //fullContent = Regex.Replace(fullContent, "<div class=\"item_social hidden_320\">(?s).*</div>", "");
                            
                        }
                    }
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            string domain = "http://vnexpress.net/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getKenh14(string url)
        {            
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'knd-content')]"))
                {
                    string content = link.InnerHtml;//SelectSingleNode(".//div[contains(@class,'content')]").
                    return content;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }

            return notFoundErr;
        }
        public static string getAfamily(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'detail fl')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'detail_content fl mgt15')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            string domain = "http://afamily.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }        
        public static string getHanoimoi(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'NewsGallery')]"))
                {
                    fullContent = link.InnerHtml;//SelectSingleNode(".//div[contains(@class,'AD_body')]").
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"[^http](.*?)>", "<img src=\"http://hanoimoi.com.vn/$2\">");
            string domain = "http://hanoimoi.com.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            //tit = Regex.Replace(tit, "<ul class=(.)detail_other(.)(?s).*</ul>", "");

            return fullContent;
        }
        public static string getDoiSongPhapLuat(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(url);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'alpha fl-left')]"))
                {                    
                    try
                    {
                        fullContent += link.SelectSingleNode(".//div[contains(@id,'main-detail')]").InnerHtml;
                    }
                    catch (Exception ex2) {
                        fullContent = "";
                    }
                    //return fullContent;
                    break;
                }
                if (fullContent == "") {
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//main[contains(@class,'main pkg')]"))
                    {
                        try
                        {
                            fullContent += link.SelectSingleNode(".//article[contains(@class,'detail-photo')]").InnerHtml;
                            //fullContent = Regex.Replace(fullContent, "<div class=\"share-detail\">(?s).*</div>", "");
                            fullContent = Regex.Replace(fullContent, "<button id=\"view_full_slide\" class=\"btn-group\">Xem full</button>", "");
                            
                        }
                        catch (Exception ex1)
                        {
                        }
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            fullContent = fullContent.Replace("Phóng to</a>","</a>");
            //fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"(.*?)>", "<img src=\"http://hanoimoi.com.vn$2\">");
            //tit = Regex.Replace(tit, "<ul class=(.)detail_other(.)(?s).*</ul>", "");
            fullContent = Regex.Replace(fullContent, "<div style=\"margin-bottom: 10px !important;margin-top: 40px !important;\">(?s).*</div>", "");
            if (fullContent.Contains("http://www.doisongphapluat.com/images/404.jpg")) {
                fullContent = "";
            }
            string domain = "http://www.doisongphapluat.com/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getVietbao(string fullContent)
        {
            //getAfamily
            Regex titRegex = new Regex(@"<div class=(.)postby clearfix(.)>(?s).*<div class=(.)bl_nd(.)>", RegexOptions.IgnoreCase);//<h1 class=(.)d-title(.)>(?s).*<div class=(.)share fl(.)>
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            //fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"(.*?)>", "<img src=\"http://hanoimoi.com.vn$2\">");
            //tit = Regex.Replace(tit, "<ul class=(.)detail_other(.)(?s).*</ul>", "");

            return fullContent;
        }
        public static string getNld(string url)
        {
            string fullContent = "";
            //string fullContent = "";
            try
            {
                //HtmlWeb hw = new HtmlWeb();
                //HtmlDocument doc = hw.Load(url);
                string html;
                //using (var wc = new GZipWebClient())
                //    html = wc.DownloadString(url);
                WebClient client = new WebClient();
                var data = client.DownloadData(url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'box10')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@id,'divNewsContent')]").InnerHtml;
                    //return fullContent;
                    break;
                }
            }
            catch (Exception ex)
            {
                return notFoundErr;
            }
            string domain = "http://nld.com.vn/";
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"" + domain + "$2\">");
            return fullContent;
        }
        public static string getZing(string url)
        {
            string fullContent = "";
            string domain = "http://news.zing.vn";
            try
            {
                var doc = new HtmlDocument();
                string html;
                using (var wc = new GZipWebClient())
                    html = wc.DownloadString(url);
                //html = DecodeFromUtf8(html);
                doc.LoadHtml(html);
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//section[contains(@class,'main')]"))
                {
                    fullContent = link.SelectSingleNode(".//div[contains(@class,'the-article-body')]").InnerHtml;
                    string span = "";
                    //Remove tin lien quan
                    try
                    {
                        foreach (HtmlNode linkspan in link.SelectSingleNode(".//div[contains(@class,'main')]").SelectNodes(".//div[contains(@class,'inner-article')]"))
                        {
                            try
                            {
                                span = linkspan.InnerHtml;
                                fullContent = fullContent.Replace(span, "");
                            }
                            catch (Exception spanitem)
                            {
                            }
                        }
                    }
                    catch (Exception exspan)
                    {
                    }
                    break;
                }
            }
            catch (Exception ex) {
                return notFoundErr;
            }
            fullContent = DecodeFromUtf8(fullContent);
            fullContent = Regex.Replace(fullContent, "<a(.*?)href=\"[^http](.*?)>", "<a href=\"http://news.zing.vn/$2\">");
            return fullContent;
        }
        public static string getRegex(string regex, string fullContent)
        {
            Regex titRegex = new Regex(System.Web.HttpUtility.HtmlDecode(regex), RegexOptions.IgnoreCase);
            Match titm = titRegex.Match(fullContent);
            if (titm.Success)
            {
                fullContent = titm.Value;
            }
            else return notFoundErr;
            //fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"(.*?)>", "<img src=\"http://laodong.com.vn$2\">");
            //fullContent = Regex.Replace(fullContent, "<param value=(.)file=(.*?)>", "<param value=\"file=http://laodong.com.vn$2\">");
            //fullContent = Regex.Replace(fullContent, "<embed flashvars=(.)file=(.*?)>", "<embed flashvars=\"file=http://laodong.com.vn$2\">");
            //fullContent = Regex.Replace(fullContent, "<div class=(.)stories-box-horizontal(.)>(?s).*", "");
            //fullContent = Regex.Replace(fullContent, "<ul class=(.)story-tools clearfix(.)(?s).*<div id=(.)ctl00_mainContent_divAvatar(.)", "<div id=\"ctl00_mainContent_divAvatar\"");
            return fullContent;
        }
        public static string EncodeStr(string input)
        {
            if (input == null || input.Trim() == "") return "";
            input = input.Trim();
            byte[] arr = UTF8Encoding.UTF8.GetBytes(input);
                //ASCIIEncoding.ASCII.GetBytes(input);
            return Convert.ToBase64String(arr);
        }
        
        public static string DecodeStr(string input)
        {            
            if (input == null || input.Trim() == "") return "";
            input = input.Trim();
            byte[] arr = Convert.FromBase64String(input);
            return UTF8Encoding.UTF8.GetString(arr);
                //ASCIIEncoding.ASCII.GetString(arr);
        }
        public static string  getFullContent(string url){
            string fullContent = "";
            if (url.Contains("thanhnien"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getThanhNien(url);                                                
                    }                    
                    else if (url.Contains("laodong.com.vn"))
                    {
                        fullContent = getLaodong(url);
                    }
                    else if (url.Contains("dantri"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getDantri(url);
                    }
                    else if (url.Contains("ngoisao"))
                    {

                        fullContent = getNgoisao(url);
                        //fullContent = Regex.Replace(fullContent, "<div class=\"topDetail\">(?s).*<h1 class=\"title\">", "<h1 class=\"title\">");
                        //fullContent = Regex.Replace(fullContent, "<div class=\"relateNewsDetail\">(?s).*<!--Link crosssite-->", "");
                        //fullContent = "";
                        //arr[i].hasContent = 0;
                    }
                    else if (url.Contains("thethao.tuoitre.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        //fullContent = getTuoitre(fullContent);
                        fullContent = getTuoiTreTheThao(url);//getTemplateContent("one_new", url);
                    }
                    else if (url.Contains("http://tuoitre.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getTuoitre(url);
                        ///fullContent = getTemplateContent("divArticleDetailMain", url);
                        //fullContent = Regex.Replace(fullContent, "<div class=\"boxComment\">(?s).*</div>", "");

                    }
                    else if (url.Contains("tinmoi.vn"))
                    {
                        fullContent = notCrawl;
                        //fullContent = Regex.Replace(fullContent, "<div class=\"boxComment\">(?s).*</div>", "");
                    }
                    else if (url.Contains("24h.com.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = get24h(url);
                    }
                     else if (url.Contains("vietnamnet.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getVietnamnet(url);
                       
                    }
                    else if (url.Contains("vtc.vn"))
                    {
                        fullContent = getVtc(url);
                        //fullContent = getVtc(fullContent);
                    }                    
                    else if (url.Contains("cafef.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        //fullContent = getCafeF(fullContent);
                        fullContent = getCafeF(url);//getTemplateContent("contentv2", url);
                    }
                    else if (url.Contains("cafebiz.vn"))
                    {
                        fullContent = getCafeBiz(url);
                        //if (fullContent.Equals(notFoundErr))
                        //{
                        //    fullContent = Rss.getContent(url);
                        //    fullContent = getAutopro(fullContent);
                        //}
                    }
                    else if (url.Contains("autopro.com.vn"))
                    {
                        fullContent = getAutopro(url);
                        //if (fullContent.Equals(notFoundErr))
                        //{
                        //    fullContent = Rss.getContent(url);
                        //    fullContent = getAutopro(fullContent);
                        //}
                    }
                    else if (url.Contains("thethao247"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getThethao247(url);
                        //fullContent = getTemplateContent("detail_post", url);
                    }
                    else if (url.Contains("http://2sao.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = get2sao(url);
                    }
                    else if (url.Contains("thethaovanhoa"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getThethaovanhoa(url);
                    }
                    else if (url.Contains("vnexpress.net"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getVnexpress(url);
                    }
                    else if (url.Contains("kenh14"))
                    {
                        fullContent = getKenh14(url);
                    }
                    else if (url.Contains("afamily"))
                    {
                        //fullContent = Rss.getContent(url);
                        //fullContent = getAfamily(fullContent);
                        fullContent = getAfamily(url);
                    }
                    else if (url.Contains("nld.com.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getNld(url);
                    }
                    else if (url.Contains("hanoimoi.com.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getHanoimoi(url);
                        //fullContent = getTemplateContent("AD_body", url);
                        //if (fullContent.ToLower().Contains(notFoundErr.ToLower()))
                        //{
                        //    fullContent=notCrawl;
                        //}
                        //else {
                        //    fullContent = Regex.Replace(fullContent, "<img(.*?)src=\"(.*?)>", "<img src=\"http://hanoimoi.com.vn$2\">");
                        //    //fullContent = Regex.Replace(fullContent, "<div class=\"associated_news\">(?s).*<h1 class=\"title\">", "<h1 class=\"title\">");
                        //}
                    }
                    else if (url.Contains("doisongphapluat.com"))
                    {
                        //fullContent = Rss.getContent(url);
                        //fullContent = getDoiSongPhapLuat(fullContent);
                        //fullContent = getTemplateContent("main-detail", url);
                        //fullContent = Regex.Replace(fullContent, "<div style=\"margin-bottom: 10px !important;margin-top: 40px !important;\">(?s).*</div>", "");
                        fullContent = getDoiSongPhapLuat(url);
                    }
                    else if (url.Contains("vietbao.vn"))
                    {
                        fullContent = notCrawl;
                        //fullContent = Rss.getContent(url);
                        //fullContent = getVietbao(fullContent);
                        //fullContent = getTemplateContent("textnd",url);
                        //if (fullContent.ToLower().Contains(notFoundErr.ToLower()))
                        //{
                        //    fullContent=notCrawl;
                        //}
                    }
                    else if (url.Contains("bizlive.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getBizlive(url);
                    }
                    else if (url.Contains("khoahoc.com.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getKhoahocCom(url);
                    }
                    else if (url.Contains("zing"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getZing(url);
                    }
                    else if (url.Contains("http://giaoduc.net.vn/"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getGiaoDucNetVn(url);
                    }
                    else if (url.Contains("http://kienthuc.net.vn/"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getKienThucNetVn(url);
                    }
                    else if (url.Contains("vuiviet.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getVuiViet(url);
                    }
                    else if (url.Contains("clip.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getClipVn(url);
                    }
                    else if (url.Contains("techz.vn"))
                    {
                        //fullContent = Rss.getContent(url);
                        fullContent = getTechz(url);
                    }
            fullContent = removeAtributeHtml(fullContent);
            return fullContent;
        }
        
        public static string removeAtributeHtml(string fullContent)
        {
            //fullContent = Regex.Replace(fullContent, "<table(.*?)width=\"(.*?)\"(.*?)>", "<table>");
            //fullContent = Regex.Replace(fullContent, "<table(.*?)height=\"(.*?)\"(.*?)>", "<table>");
            //fullContent = Regex.Replace(fullContent, "<table(.*?)style=\"(.*?)\"(.*?)>", "<table>");
            //fullContent = Regex.Replace(fullContent, "<table(.*?)class=\"(.*?)\"(.*?)>", "<table>");
            //fullContent = Regex.Replace(fullContent, "<td(.*?)width=\"(.*?)\"(.*?)>", "<td>");
            //fullContent = Regex.Replace(fullContent, "<td(.*?)height=\"(.*?)\"(.*?)>", "<td>");
            //fullContent = Regex.Replace(fullContent, "<td(.*?)style=\"(.*?)\"(.*?)>", "<td>");
            //fullContent = Regex.Replace(fullContent, "<td(.*?)class=\"(.*?)\"(.*?)>", "<td>");
            //fullContent = Regex.Replace(fullContent, "<th(.*?)width=\"(.*?)\"(.*?)>", "<th>");
            //fullContent = Regex.Replace(fullContent, "<th(.*?)height=\"(.*?)\"(.*?)>", "<th>");
            //fullContent = Regex.Replace(fullContent, "<th(.*?)style=\"(.*?)\"(.*?)>", "<th>");
            //fullContent = Regex.Replace(fullContent, "<th(.*?)class=\"(.*?)\"(.*?)>", "<th>");
            //fullContent = Regex.Replace(fullContent, "<tr(.*?)width=\"(.*?)\"(.*?)>", "<tr>");
            //fullContent = Regex.Replace(fullContent, "<tr(.*?)height=\"(.*?)\"(.*?)>", "<tr>");
            //fullContent = Regex.Replace(fullContent, "<tr(.*?)style=\"(.*?)\"(.*?)>", "<tr>");
            //fullContent = Regex.Replace(fullContent, "<tr(.*?)class=\"(.*?)\"(.*?)>", "<tr>");
            //fullContent = Regex.Replace(fullContent, "<div(.*?)width=\"(.*?)\"(.*?)>", "<div>");
            //fullContent = Regex.Replace(fullContent, "<div(.*?)height=\"(.*?)\"(.*?)>", "<div>");
            //fullContent = Regex.Replace(fullContent, "<div(.*?)style=\"(.*?)\"(.*?)>", "<div>");
            //fullContent = Regex.Replace(fullContent, "<div(.*?)class=\"(.*?)\"(.*?)>", "<div>");
            
            //fullContent = Regex.Replace(fullContent, "<p(.*?)style=\"(.*?)\"(.*?)>", "<p>");
            //fullContent = Regex.Replace(fullContent, "<p(.*?)class=\"(.*?)\"(.*?)>", "<p>");
            //fullContent = Regex.Replace(fullContent, "<font(.*?)style=\"(.*?)\"(.*?)>", "<font>");
            //fullContent = Regex.Replace(fullContent, "<font(.*?)class=\"(.*?)\"(.*?)>", "<font>");
            //fullContent = Regex.Replace(fullContent, "<font(.*?)size=\"(.*?)\"(.*?)>", "<font>");
            //fullContent = Regex.Replace(fullContent, "<ul(.*?)style=\"(.*?)\"(.*?)>", "<ul>");
            //fullContent = Regex.Replace(fullContent, "<ul(.*?)class=\"(.*?)\"(.*?)>", "<ul>");
            //fullContent = Regex.Replace(fullContent, "<li(.*?)style=\"(.*?)\"(.*?)>", "<li>");
            //fullContent = Regex.Replace(fullContent, "<li(.*?)class=\"(.*?)\"(.*?)>", "<li>");
            //fullContent = Regex.Replace(fullContent, "<span(.*?)style=\"(.*?)\"(.*?)>", "<span>");
            //fullContent = Regex.Replace(fullContent, "<span(.*?)class=\"(.*?)\"(.*?)>", "<span>");
            //fullContent = Regex.Replace(fullContent, "href=\"(.*?)\"", "");
            //fullContent = Regex.Replace(fullContent, ">>", "");
            //string html;
            
            var doc = new HtmlDocument();
            doc.LoadHtml(fullContent);
            var elementsWithStyleAttribute = doc.DocumentNode.SelectNodes("//@style");
            if (elementsWithStyleAttribute != null)
            {
                try
                {
                    foreach (var element in elementsWithStyleAttribute)
                    {
                    
                            element.Attributes["style"].Remove();
                            //element.Attributes["width"].Remove();
                            //element.Attributes["height"].Remove();
                            //element.Attributes["display"].Remove();
                            //element.Attributes["max-width"].Remove();
                            //element.Attributes["min-width"].Remove();
                            //element.Attributes["background-color"].Remove();
                    }
                }
                catch (Exception ex1)
                {
                }
            }
            elementsWithStyleAttribute = doc.DocumentNode.SelectNodes("//@class");
            if (elementsWithStyleAttribute != null)
            {
                try
                {
                    foreach (var element in elementsWithStyleAttribute)
                    {

                        element.Attributes["class"].Remove();

                    }
                }
                catch (Exception ex1)
                {
                }
            }
            elementsWithStyleAttribute = doc.DocumentNode.SelectNodes("//@width");
            if (elementsWithStyleAttribute != null)
            {
                try
                {
                    foreach (var element in elementsWithStyleAttribute)
                    {

                        element.Attributes["width"].Remove();

                    }
                }
                catch (Exception ex1)
                {
                }
            }
            elementsWithStyleAttribute = doc.DocumentNode.SelectNodes("//@face");
            if (elementsWithStyleAttribute != null)
            {
                try
                {
                    foreach (var element in elementsWithStyleAttribute)
                    {

                        element.Attributes["face"].Remove();

                    }
                }
                catch (Exception ex1)
                {
                }
            }
            elementsWithStyleAttribute = doc.DocumentNode.SelectNodes("//@size");
            if (elementsWithStyleAttribute != null)
            {
                try
                {
                    foreach (var element in elementsWithStyleAttribute)
                    {

                        element.Attributes["size"].Remove();

                    }
                }
                catch (Exception ex1)
                {
                }
            }
            fullContent = doc.DocumentNode.InnerHtml;
            //fullContent = Regex.Replace(fullContent, "href=\"(.*?)\"", "");
            fullContent = Regex.Replace(fullContent, ">>", "");
            fullContent = Regex.Replace(fullContent, "<h2>", "");
            fullContent = Regex.Replace(fullContent, "</h2>", "");
            fullContent = Regex.Replace(fullContent, "<h1>", "");
            fullContent = Regex.Replace(fullContent, "</h1>", "");
            fullContent = fullContent.Replace("&gt;&gt; ", "");
            
            //Nếu phát hiện ra trong bài có một link nào đó quá dài, ví dụ độ dài lớn hơn 17 thì có thể đây là các tin liên quan hoặc link không cần thiết
            //Remove đi, cần nghiên cứu thêm vì có thể remove nhầm
            //string span = "";
            //try
            //{
            //    foreach (HtmlNode linkspan in doc.DocumentNode.SelectNodes("//a[contains(@class,'')]"))
            //    {
            //        try
            //        {
            //            span = linkspan.InnerText;
            //            if (span.Length >= 20)
            //            {
            //                fullContent = fullContent.Replace(span, "");
            //            }
            //        }
            //        catch (Exception spanitem)
            //        {
            //        }
            //    }
            //}
            //catch (Exception exspan)
            //{
            //}
            //fullContent = Regex.Replace(fullContent, "<a>(.*?)</a>", "$1");
            //fullContent =fullContent.Replace("<a></a>","");
            
            
            return fullContent;
        }
        public static string removeAllRelatedLink(string fullContent) {
            //Nếu phát hiện ra trong bài có một link nào đó quá dài, ví dụ độ dài lớn hơn 17 thì có thể đây là các tin liên quan hoặc link không cần thiết
            //Remove đi, cần nghiên cứu thêm vì có thể remove nhầm
            var doc = new HtmlDocument();
            doc.LoadHtml(fullContent);
            string span = "";
            try
            {
                foreach (HtmlNode linkspan in doc.DocumentNode.SelectNodes("//a[contains(@class,'')]"))
                {
                    try
                    {
                        span = linkspan.InnerText;
                        if (span.Length >= 25)
                        {
                            fullContent = fullContent.Replace(span, "");
                        }
                    }
                    catch (Exception spanitem)
                    {
                    }
                }
            }
            catch (Exception exspan)
            {
            }
            return fullContent;
        }
        public class GZipWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return request;
            }
        }
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = value;
            MyCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[v].Value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        //private void LoadHtmlWithBrowser(String url)
        //{

        //    webBrowser1.ScriptErrorsSuppressed = true;
        //    webBrowser1.Navigate(url);

        //    waitTillLoad(this.webBrowser1);

        //    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        //    var documentAsIHtmlDocument3 = (mshtml.IHTMLDocument3)webBrowser1.Document.DomDocument;
        //    StringReader sr = new StringReader(documentAsIHtmlDocument3.documentElement.outerHTML);
        //    doc.Load(sr);
        //}
        //private void waitTillLoad(WebBrowser webBrControl)
        //{
        //    WebBrowserReadyState loadStatus;
        //    int waittime = 100000;
        //    int counter = 0;
        //    while (true)
        //    {
        //        loadStatus = webBrControl.ReadyState;
        //        Application.DoEvents();
        //        if ((counter > waittime) || (loadStatus == WebBrowserReadyState.Uninitialized) || (loadStatus == WebBrowserReadyState.Loading) || (loadStatus == WebBrowserReadyState.Interactive))
        //        {
        //            break;
        //        }
        //        counter++;
        //    }

        //    counter = 0;
        //    while (true)
        //    {
        //        loadStatus = webBrControl.ReadyState;
        //        Application.DoEvents();
        //        if (loadStatus == WebBrowserReadyState.Complete && webBrControl.IsBusy != true)
        //        {
        //            break;
        //        }
        //        counter++;
        //    }
        //}
    }
}