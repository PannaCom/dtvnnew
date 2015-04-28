using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace youknow
{
    public partial class adminYouKnow : System.Web.UI.Page
    {
        struct hotkeyword {
            public string keyword;
            public int ranking;
        }
        hotkeyword[] arrHotKeyWord = new hotkeyword[10];
        struct filterkeyword
        {
            public string keyword;
            public int ranking;
        }
        filterkeyword[] arrFilterKeyword = new filterkeyword[10];
        struct trendkeyword
        {
            public string keyword;
            public int ranking;
        }
        trendkeyword[] arrTrendKeyWord = new trendkeyword[10];
        Hashtable blockLink = new Hashtable();
        int countBlockLink = 0;
        struct tempTitle
        {
            public string link;
            public int id;
        }
        tempTitle[] arrTempTitle = new tempTitle[1000];
        int countHotKeyWord = 0;
        int countFilterKeyword = 0;
        int countTrendKeyWord = 0;
        Rss.ItemXml[] mainItemTitle = null;
        int countItemTitle = 0;
        SqlConnection con=null;
        Hashtable titleCrawled=null;
        Hashtable titleComments;
        Rss Rs = null;
        string Info = "";
        protected BatchRun CurrentBatchRun
        {
            get { return (BatchRun)this.Application["CurrentBatchRun"]; }
            set { this.Application["CurrentBatchRun"] = value; }
        }
        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Hubs.ync>();
        BatchRun currentBatch;
        protected void Page_Load(object sender, EventArgs e)
        {
            //string url = "http://media.thethao247.vn/resize_480x275/upload/2014/11/22/thumb-viet-nam-vs-indonesia.jpg";
            //string fileName = Uti.unicodeToNoMark("VIỆT NAM Indo CHỜ THẮNG LỢI ĐẦU TAY") + "-" + Uti.smDomain("http://media.thethao247.vn").Replace(".", "") + "-" + 20141122 + ".jpg";
            //string file_name = Server.MapPath(".") + "\\Images\\News\\" + fileName;
            //Uti.save_file_from_url(file_name, url);
            //string abc1 = Config.domain + "/Images/News/" + fileName;

            //Bug: http://danviet.vn/que-nha/nhung-phien-cho-xuan-dac-biet-cua-viet-nam/20130922043618597p1c29.htm
            //Bug: http://autopro.com.vn/tin-tuc/10-phien-ban-dac-biet-an-tuong-nhat-cua-ong-hoang-bugatti-veyron-20140209123017306.chn
            //Bug: http://vietnamnet.vn/vn/cong-nghe-thong-tin-vien-thong/160618/xuat-khau-phan-mem-la-the-manh-cua-viet-nam.html
            //Phiên chợ đặc biệt= phiên bản đặc biệt, cần xem lại
            //string a1="<img width=\"460\" height=\"307\" src=\"/Uploads/honghai/2014/5/1/TPHCM1.jpg\" alt=\"Bắn pháo hoa tại TPHCM nhân dịp kỷ niệm 39 năm ngày thống nhất đất nước 30/4 và Ngày Quốc tế Lao động 1/5.\">";
            //a1 = Regex.Replace(a1, "<img(.*?)src=\"(.*?)>", "<img src=\"http://hanoimoi.com.vn$2\">");
            //string a = "Đội của bầu Đức thua trận mở m&#224;n Toyoya Mekong Club Championship";
            //string b = "Đội của bầu Đức thua trận mở m&#224;n Toyota Mekong Club Championship";
            //a = System.Web.HttpUtility.HtmlDecode(a);
            //b = System.Web.HttpUtility.HtmlDecode(b);
            //string test1 = Rss.XauConChungDaiNhat(a, b);
            //int len = Rss.realLength(test1);
            //bool c = "A".Equals("A");
            //a = "Nguyễn Văn A nmlk jlkhjl jlh N M B Ăn kjdka Cơm Nhé kjhasdjksad các bạn";
            //test1 = Uti.getNounUpcaseWord(a);
            //string tukhoachung1 = Rss.XauConChungDaiNhat(a, b).Trim();
            //Response.Write(tukhoachung1);
            if (!IsPostBack)
            {
                //Thời gian bắt đầu và kết thúc Crawl, hiện thời gian này chưa chuẩn
                //Mặc dù để 1000 năm nó vẫn bị ngừng, chưa điều tra ra
                this.txtStartDate.Text = DateTime.Today.AddMonths(-1).ToShortDateString();
                this.txtEndDate.Text = DateTime.Today.AddYears(1000).ToShortDateString();                
            }
            string abc = Uti.removeHtmlTag(Server.HtmlDecode("Nhà sư và &lt;span&gt;iPhone 6&lt;/span&gt;:&nbsp;“Chỉ sờ máy lấy lộc cho cửa hàng”"));
            Response.Write(DateTime.Now.ToUniversalTime().ToString()+"<br>");
            Response.Write(DateTime.Now.ToString());
            //Response.Write("Thu, 05 Jun 2014 09:29:00 GMT is date? "+Uti.isDate("Thu, 05 Jun 2014 09:29:00 GMT"));
            SetBatchParameterTableVisibility(false);

            if (!CurrentBatchInProgress())
            {
                CurrentBatchRun = new BatchRun();

                this.lblPercentage.Text = "0";
                this.lblTimeRemaining.Text = "Unknown";
                var ts = new ThreadStart(RunBatch);
                var thread = new Thread(ts);
                thread.Start();
            }
            else
            {
                ShowAlert("Statement Batch Already In Progress");
            }
            //timerBatchRun.Enabled = true;
            
        }
        //Hàm này load các từ khóa cần filter và cho vào mảng
        //Những từ khóa nằm trong mảng này khi xuất hiện trong các nội dung tin cần cho điểm thấp
        private void loadBlockLink()
        {
            try
            {
                blockLink = new Hashtable();
                //countBlockLink = 0;
                openConn();
                //currentBatch.getInfo = "loadAllNewsCrawled...";
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = con;
                Cmd.CommandText = "Select top 99 * from dbo.blockurl order by id desc";
                SqlDataReader DR = Cmd.ExecuteReader();
                while (DR.Read())
                {
                    if (!blockLink.ContainsKey(DR["url"].ToString()))
                    {
                        blockLink.Add(DR["url"].ToString(),"1");
                    }

                }
                DR.Close();
                closeConn();
                //currentBatch.getInfo = "done loadAllNewsCrawled...";
            }
            catch (Exception ex)
            {

                closeConn();
                //currentBatch.getInfo = "error in loadAllNewsCrawled..." + ex.ToString();
            }
        }      
        //Hàm này load các từ khóa cần filter và cho vào mảng
        //Những từ khóa nằm trong mảng này khi xuất hiện trong các nội dung tin cần cho điểm thấp
        private void loadFilterKeyword()
        {
            try
            {
                arrFilterKeyword = new filterkeyword[501];
                countFilterKeyword = 0;
                openConn();
                //currentBatch.getInfo = "loadAllNewsCrawled...";
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = con;
                Cmd.CommandText = "Select top 500 * from tinviet_admin.filterkeyword where datetimeid=" + Uti.datetimeid() + " order by ranking desc";
                SqlDataReader DR = Cmd.ExecuteReader();
                while (DR.Read())
                {

                    arrFilterKeyword[countFilterKeyword].keyword = DR["keyword"].ToString();
                    arrFilterKeyword[countFilterKeyword].ranking = int.Parse(DR["ranking"].ToString());
                    countFilterKeyword++;

                }
                DR.Close();
                closeConn();
                //currentBatch.getInfo = "done loadAllNewsCrawled...";
            }
            catch (Exception ex)
            {
                
                closeConn();
                //currentBatch.getInfo = "error in loadAllNewsCrawled..." + ex.ToString();
            }
        }        
        //Load các từ khóa nóng vào mảng để sau cộng điểm cho các tin có chứa từ khóa nóng.
        private void loadHotKeyword()
        {
            try
            {
                arrHotKeyWord = new hotkeyword[501];
                countHotKeyWord = 0;
                openConn();
                //currentBatch.getInfo = "loadAllNewsCrawled...";
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = con;
                Cmd.CommandText = "Select top 500 * from tinviet_admin.hotkeyword where datetimeid=" + Uti.datetimeid() + " order by ranking desc";
                SqlDataReader DR = Cmd.ExecuteReader(); 
                while (DR.Read())
                {

                    arrHotKeyWord[countHotKeyWord].keyword = DR["keyword"].ToString();
                    arrHotKeyWord[countHotKeyWord].ranking = int.Parse(DR["ranking"].ToString());
                    countHotKeyWord++;
                    
                }
                DR.Close();
                closeConn();
                //currentBatch.getInfo = "done loadAllNewsCrawled...";
            }
            catch (Exception ex)
            {
               
                closeConn();
                //currentBatch.getInfo = "error in loadAllNewsCrawled..." + ex.ToString();
            }
        }
        //Load các từ khóa có xu hướng đọc nhiều để nhóm các tin cùng chủ đề
        private void loadTrendKeyword()
        {
            try
            {
                arrTrendKeyWord = new trendkeyword[501];
                countTrendKeyWord = 0;
                openConn();
                //currentBatch.getInfo = "loadAllNewsCrawled...";
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = con;
                Cmd.CommandText = "Select top 20 * from tinviet_admin.dailykeyword where datetimeid>=" + Uti.datetimeid() + " and status=1 order by datetimeid desc,ranking desc";
                SqlDataReader DR = Cmd.ExecuteReader();
                while (DR.Read())
                {

                    arrTrendKeyWord[countTrendKeyWord].keyword = DR["keyword"].ToString();
                    arrTrendKeyWord[countTrendKeyWord].ranking = int.Parse(DR["ranking"].ToString());
                    countTrendKeyWord++;

                }
                DR.Close();
                closeConn();
                //currentBatch.getInfo = "done loadAllNewsCrawled...";
            }
            catch (Exception ex)
            {

                closeConn();
                //currentBatch.getInfo = "error in loadAllNewsCrawled..." + ex.ToString();
            }
        }
        //Cập nhật số bình luận của từng tin chủ đề vào mảng
        // để đến hàm InsertData thì cập nhật số bình luận cho tin
        private void loadTitleComment()
        {
            try
            {
                titleComments = new Hashtable();
                arrHotKeyWord = new hotkeyword[501];
                countFilterKeyword = 0;
                openConn();
                //currentBatch.getInfo = "load titles comments...";
                
                SqlCommand Cmd = new SqlCommand();
                string query = "select A.link,A.catid,count(B.id) as totalcomment from ";
                query += " (select link,catid,name,id as idnews from tinviet_admin.titles where datetimeid>=" + Uti.datetimeid() + "-3) as A left join ";//where datetimeid="+Uti.datetimeid()+"
                query += " (select id,idnews from tinviet_admin.comments) as B on A.idNews=B.idnews ";
                query += " group by A.link,A.catid ";
                Cmd.Connection = con;
                Cmd.CommandText = query;
                SqlDataReader DR = Cmd.ExecuteReader();
                while (DR.Read())
                {

                    if (!titleComments.ContainsKey(DR["link"].ToString().Trim() + DR["catid"].ToString()))
                    {
                        titleComments.Add(DR["link"].ToString().Trim() + DR["catid"].ToString(), DR["totalcomment"].ToString());
                    }

                }
                DR.Close();
                closeConn();
                //currentBatch.getInfo = "done loadAllNewsCrawled...";
            }
            catch (Exception ex)
            {
                
                closeConn();
                //currentBatch.getInfo = "error in loadAllNewsCrawled..." + ex.ToString();
            }
        }
        private string toLinkId(string catid,string datetimeid, string domain, string title)
        {
            return catid + "_" + datetimeid + "_" + domain + "_" + title.Replace(" ", "_");
        }
        //Load các tin đã crawl vào mảng, Để sau này gặp lại tin này thì không crawl về nữa.
        private void loadAllNewsCrawled() {
            try
            {
                if (titleCrawled == null)
                {
                    titleCrawled = new Hashtable();
                }
                else {
                    titleCrawled.Clear();
                    titleCrawled = new Hashtable();
                }
                
                string linkId = "";
                openConn();
                //currentBatch.getInfo = "loadAllNewsCrawled...";
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = con;
                Cmd.CommandText = "Select * from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-3) + " order by datetimeid desc,id desc";
                countItemTitle = 0;
                SqlDataReader DR = Cmd.ExecuteReader(); 
                while (DR.Read())
                {
                    linkId = DR["link"].ToString().Trim();// toLinkId(DR["catid"].ToString().Trim(), DR["datetimeid"].ToString().Trim(), DR["domain"].ToString().Trim(), DR["name"].ToString().Trim());
                    if (!titleCrawled.ContainsKey(linkId))
                    {
                        titleCrawled.Add(linkId, DR["datetime"].ToString().Trim());
                        //countItemTitle++;
                    }
                    //linkId = DR["tokenid"].ToString().Trim();
                    //if (!titleCrawled.ContainsKey(linkId))
                    //{
                    //    titleCrawled.Add(linkId, "1");
                    //    //countItemTitle++;
                    //}
                    
                }
                DR.Close();
                closeConn();
                //currentBatch.getInfo = "done loadAllNewsCrawled...";
            }
            catch (Exception ex) {
                
                closeConn();
                //currentBatch.getInfo = "error in loadAllNewsCrawled..." + ex.ToString();
            }
        }
        //Bắt đầu chạy Crawl
        protected void btnRunBatch_Click(object sender, EventArgs e)
        {
            SetBatchParameterTableVisibility(false);

            if (!CurrentBatchInProgress())
            {
                CurrentBatchRun = new BatchRun();

                this.lblPercentage.Text = "0";
                this.lblTimeRemaining.Text = "Unknown";                
                var ts = new ThreadStart(RunBatch);
                var thread = new Thread(ts);
                thread.Start();
            }
            else
            {
                ShowAlert("Statement Batch Already In Progress");
            }
            //timerBatchRun.Enabled = true;
        }
        //Chạy Crawl trong vòng lặp
        private void RunBatch()
        {
            DateTime startDate = DateTime.Parse(this.txtStartDate.Text);
            DateTime endDate = DateTime.Parse(this.txtEndDate.Text);

            var tempDate = startDate;
            int i = 0;
            while (tempDate < endDate)
            {
                i++;
                tempDate = tempDate.AddHours(1);
            }

            CurrentBatchRun.TotalNumberOfItems = i;
            CurrentBatchRun.Start();
            CurrentBatchRun.getInfo = "Tong so vong lap la" + CurrentBatchRun.TotalNumberOfItems;
            tempDate = startDate;
            while (tempDate < endDate)
            {
                //Hàm này chạy liên tục để crawl tin tức
                CurrentBatchRun.getInfo = "Dang chay vong lap " +CurrentBatchRun.ItemsCompleted+"/"+ CurrentBatchRun.TotalNumberOfItems;
                DoSomething(new Random().Next(1, 5));
                tempDate = tempDate.AddHours(1);
                if (CurrentBatchRun == null || CurrentBatchRun.ShouldStop)
                {
                    break;
                }
                CurrentBatchRun.IncrementItemsCompleted();
            }
        }
       
        private string vN(string n){
            return "N'" + System.Web.HttpUtility.HtmlEncode(n) + "'";
        }
        private string removeSpecialChar(string input)
        {
            input = input.Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "");
            return input;
        }
        private void openConn(){
            try
            {
                if (con==null || (con != null && con.State.ToString() == "Closed"))
                {
                    if (con == null)
                    {
                        con = new SqlConnection();
                        con.ConnectionString = "Server=42.112.31.145;Database=tinviet_vietnam;User Id=sa;Password=Huynguyenviet13071980;";
                        
                            //"Server=Huynv\\Local;Database=diemtinvietnam;User Id=sa;Password=111111;";
                            //"Server=SQL5006.myWindowsHosting.com;Database=DB_9ACF33_diemtin;User Id=DB_9ACF33_diemtin_admin;Password=chanhniem;";
                    }
                    //"Server=Huynv\\Local;Database=timeline;User Id=sa;Password=111111;";
                    //"Server=SQL5004.myWindowsHosting.com;Database=DB_9AAE99_timeline;User Id=DB_9AAE99_timeline_admin;Password=Huynguyenviet1;";
                    //"Server=SQL5004.myWindowsHosting.com;Database=DB_9AAE99_timeline;User Id=DB_9AAE99_timeline_admin;Password=Huynguyenviet1;";
                
                    con.Open();
                }
                //CurrentBatchRun.getInfo = "Open Sql OK";
            }
            catch (Exception ex) {
                //CurrentBatchRun.getInfo = "Open Sql not OK" + ex.ToString();
            }
        }
        private void closeConn(){
            try
            {
                if (con != null && con.State.ToString() == "Open")
                {
                    con.Close();
                    //con = null;
                }
            }
            catch (Exception ex) { 
            }
        }

        private void insertData(ref Rss.ItemXml[] arr,int L) {
            //Chen vao du lieu            
            openConn();
            string query = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            int datetimeid = Uti.datetimeid();
            DateTime datetimenow = DateTime.Now;
            int minRaking = 0;// Uti.getMinRanking();
            for (int i = 0; i < L; i++)            
            {
                //if (titleCrawled.ContainsKey(arr[i].link)) continue;
                if (arr[i].ranking >= minRaking && arr[i].title != "")// && !titleCrawled.ContainsKey(arr[i].link)
                {
                    if (arr[i].link.Contains("vietnamnet.vnhttp://thethao.vietnamnet")) continue;
                    if (blockLink.ContainsKey(arr[i].link)) continue;
                    //if (!arr[i].maindomain.Contains("autopro")) continue;
                    int totalComment = 0;
                    if (titleComments.ContainsKey(arr[i].link.Trim() + arr[i].catid))
                    {
                        try
                        {
                            totalComment = int.Parse(titleComments[arr[i].link.Trim() + arr[i].catid].ToString());
                        }
                        catch (Exception ex) { 

                        }
                    }
                    //Crawl fullContent
                    arr[i].fullContent="";
                    string fullContent = "";
                    if (titleCrawled.ContainsKey(arr[i].link.Trim()) && (Uti.getTotalTimeFromNow(titleCrawled[arr[i].link.Trim()].ToString()) <= 2 && arr[i].datetimeid >= Uti.datetimeidByDay(-1)) || (!titleCrawled.ContainsKey(arr[i].link.Trim()) && arr[i].datetimeid >= Uti.datetimeidByDay(-1)))// || (!titleCrawled.ContainsKey(arr[i].link) && arr[i].catid==0)
                    {
                        fullContent = Config.getFullContent(arr[i].link);
                    }
                    else {

                        continue;
                    }    

                    //Lay anh trong content
                    //string tempImage = arr[i].image;
                    //arr[i].image = Uti.getImageSrc(fullContent);
                    try
                    {
                        if (arr[i].image.Equals("") || arr[i].image.Trim() == "")
                        {
                            arr[i].image = Uti.getImageSrc(fullContent);
                        }
                    }
                    catch (Exception eximg)
                    { 
                    }

                    //if (arr[i].maindomain.Contains("http://www.thanhnien"))
                    //{
                        if (arr[i].image.StartsWith("http://thanhnien.com.vn"))
                        {
                            arr[i].image = arr[i].image.Replace("http://", "http://www.");
                        }else
                        if (arr[i].image.StartsWith("thanhnien.com.vn"))
                        {
                            arr[i].image = "http://www." + arr[i].image;
                        }
                    //}
                    //else continue;

                    //if (!Uti.isImage(arr[i].image)) continue;
                    //    string a = arr[i].image;

                    //}
                    //Bỏ hết các link
                    //fullContent = Regex.Replace(fullContent, "href=\"(.*?)\"", "");
                    //if (fullContent.ToLower().Contains(Config.notCrawl.ToLower())) continue;//Neu co noi dung nhung khong hien thi duoc thi khong chen
                    if (fullContent.ToLower().Contains(Config.notCrawl.ToLower()) || fullContent.ToLower().Contains(Config.notFoundErr.ToLower()) || fullContent.ToUpper().Contains("404 NOT FOUND") || fullContent.Contains("404 not found") || fullContent.Contains("404 NOT FOUND"))
                    {
                        fullContent = "";
                        string delete = "delete from tinviet_admin.titles where link=N'" + arr[i].link + "';delete from tinviet_admin.titlesSearch where link=N'" + arr[i].link + "';";
                        try { 
                            cmd.CommandText = delete;
                            cmd.ExecuteNonQuery();
                        }catch(Exception ex){
                        }
                        continue;
                    }
                    
                    arr[i].fullContent = fullContent;
                    //Nếu Des không có thì lấy ở content;
                    try
                    {
                        if (arr[i].des.Trim().Equals("") && !fullContent.Trim().Equals(""))
                        {
                            arr[i].des = Uti.smoothDes(Uti.innerHtmlText(fullContent));
                        
                        }
                        if (arr[i].maindomain.Contains("http://tuoitre.vn"))
                        {
                            arr[i].title = Uti.removeHtmlTag(Server.HtmlDecode(arr[i].title));
                        }
                    }
                    catch (Exception ex111)
                    {
                    }
                    try
                    {
                        if ((arr[i].title.ToLower().Replace(":", "").Contains("video ") || arr[i].title.ToLower().Replace(":", "").Contains("clip ")))//&& (arr[i].maindomain.Contains("doisongphapluat.com") || arr[i].maindomain.Contains("vtc.vn"))
                        {
                            continue;
                        }
                    }
                    catch (Exception ex333) { 
                    }
                    byte hasContent = 0;
                    if (!fullContent.Trim().Equals("")) {
                        hasContent = 1;
                    }
                    if (hasContent == 0) { arr[i].ranking = 1; continue; }
                    if (arr[i].image.Equals("")) arr[i].ranking = arr[i].ranking/2;

                    if (Uti.isImage(arr[i].image)) {
                        try
                        {
                            string url = arr[i].image;
                            if (arr[i].maindomain.Contains("vtc.vn")) url = url.Replace(".jpg", ".JPG");
                            string fileName = Uti.unicodeToNoMark(arr[i].title) + "-" + Uti.smDomain(arr[i].maindomain).Replace(".", "") + "-" + arr[i].datetimeid + ".jpg";
                            string file_name = Server.MapPath(".") + "\\Images\\News\\" + fileName;
                            Uti.save_file_from_url(file_name, url);
                            arr[i].image = Config.domain + "/Images/News/" + fileName;
                        }
                        catch (Exception dlimage) { }
                    }
                    
                    string condition = "";// where datetimeid=" + datetimeid + " and (name like " + vN(arr[i].title) + " or link=" + vN(arr[i].link) + ")";
                    string condition1 = " where tokenid=" + vN(arr[i].token.Trim());
                    string condition2 = " where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid=" + arr[i].catid+ " and domain=" + arr[i].domain + " and name=" + vN(arr[i].title.Trim()) + ")";
                    query = "";
                    //Nếu đã tồn tại tin trong DB thì kiểm tra điều kiện cập nhật, cập nhật nếu thỏa mãn (ranking mới >ranking cũ), nếu không thì chèn mới
                    if (arr[i].catid != 0)
                    {
                        condition = " where catid<>0 and link=" + vN(arr[i].link.Trim()) + " ";
                        if (titleCrawled.ContainsKey(arr[i].link.Trim()))
                        {
                        //    if (youknow.Uti.isImage(arr[i].image))
                        //    {
                        //        query = "    update tinviet_admin.titles set image=" + vN(arr[i].image) + ",totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        //    }
                        //    else
                        //    {
                        //        query = "    update tinviet_admin.titles set totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        //    }
                        }
                        else {
                            titleCrawled.Add(arr[i].link.Trim(), arr[i].date);
                        //    query = " insert into tinviet_admin.titles(name,des,fullContent,hasContent,link,image,domain,maindomain,catid,source,datetime,lastUpdateRanking,ranking,fixranking,keyword,datetimeid,totalComment,uplikes,downlikes) values(N'" + arr[i].title.Trim() + "',N'" + arr[i].des + "'," + vN(arr[i].fullContent) + "," + hasContent + "," + vN(arr[i].link) + "," + vN(arr[i].image) + "," + arr[i].domain + "," + vN(arr[i].maindomain) + "," + arr[i].catid + "," + vN(arr[i].source) + "," + vN(arr[i].date) + "," + vN(arr[i].date) + "," + arr[i].ranking + "," + arr[i].ranking + "," + vN(arr[i].keyword) + "," + arr[i].datetimeid + "," + totalComment + ",0,0)";
                        }
                        //Chỉ cập nhật tin khi ranking mới cao hơn ranking cũ trong Database.
                        query = "if exists(select * from tinviet_admin.titles " + condition + ")";
                        query += " begin ";
                        query += "if ((select ranking from tinviet_admin.titles " + condition + ")<" + arr[i].ranking + ")";//Neu co ranking cao thi cap nhat
                        query += "  begin ";
                        if (youknow.Uti.isImage(arr[i].image))
                        {
                            query += "    update tinviet_admin.titles set image=" + vN(arr[i].image) + ",totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        else
                        {
                            query += "    update tinviet_admin.titles set totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        query += "  end ";
                        query += "    else ";//Nếu ranking mới không cao hơn cũ thì chỉ cập nhật content và ảnh nếu có.
                        query += "  begin ";
                        if (youknow.Uti.isImage(arr[i].image))
                        {
                            query += "    update tinviet_admin.titles set image=" + vN(arr[i].image) + ",totalComment=" + totalComment + ",hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        else
                        {
                            query += "    update tinviet_admin.titles set totalComment=" + totalComment + ",hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        query += "  end ";
                        query += " end ";
                        query += " else ";
                        query += " begin ";
                        query += " insert into tinviet_admin.titles(name,des,fullContent,hasContent,link,image,domain,maindomain,catid,source,datetime,lastUpdateRanking,ranking,fixranking,keyword,datetimeid,totalComment,uplikes,downlikes) values(N'" + arr[i].title.Trim() + "',N'" + arr[i].des + "'," + vN(arr[i].fullContent) + "," + hasContent + "," + vN(arr[i].link) + "," + vN(arr[i].image) + "," + arr[i].domain + "," + vN(arr[i].maindomain) + "," + arr[i].catid + "," + vN(arr[i].source) + "," + vN(arr[i].date) + "," + vN(arr[i].date) + "," + arr[i].ranking + "," + arr[i].ranking + "," + vN(arr[i].keyword) + "," + arr[i].datetimeid + "," + totalComment + ",0,0)";
                        query += "end ";
                    }
                    else {
                        condition = " where catid=0 and link=" + vN(arr[i].link.Trim()) + " ";
                        //Chỉ cập nhật tin khi ranking mới cao hơn ranking cũ trong Database.
                        query = "if exists(select * from tinviet_admin.titles " + condition + ")";
                        query += " begin ";
                        query += "if ((select ranking from tinviet_admin.titles " + condition + ")<" + arr[i].ranking + ")";//Neu co ranking cao thi cap nhat
                        query += "  begin ";
                        if (youknow.Uti.isImage(arr[i].image))
                        {
                            query += "    update tinviet_admin.titles set image=" + vN(arr[i].image) + ",totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        else
                        {
                            query += "    update tinviet_admin.titles set totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        query += "  end ";
                        query += "    else ";//Nếu ranking mới không cao hơn cũ thì chỉ cập nhật content và ảnh nếu có.
                        query += "  begin ";
                        if (youknow.Uti.isImage(arr[i].image))
                        {
                            query += "    update tinviet_admin.titles set image=" + vN(arr[i].image) + ",totalComment=" + totalComment + ",hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        else
                        {
                            query += "    update tinviet_admin.titles set totalComment=" + totalComment + ",hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + ", name=N'" + arr[i].title.Trim() + "', des=N'" + arr[i].des.Trim() + "' " + condition;
                        }
                        query += "  end ";
                        query += " end ";
                        query += " else ";
                        query += " begin ";
                        query += " insert into tinviet_admin.titles(name,des,fullContent,hasContent,link,image,domain,maindomain,catid,source,datetime,lastUpdateRanking,ranking,fixranking,keyword,datetimeid,totalComment,uplikes,downlikes) values(N'" + arr[i].title.Trim() + "',N'" + arr[i].des + "'," + vN(arr[i].fullContent) + "," + hasContent + "," + vN(arr[i].link) + "," + vN(arr[i].image) + "," + arr[i].domain + "," + vN(arr[i].maindomain) + "," + arr[i].catid + "," + vN(arr[i].source) + "," + vN(arr[i].date) + "," + vN(arr[i].date) + "," + arr[i].ranking + "," + arr[i].ranking + "," + vN(arr[i].keyword) + "," + arr[i].datetimeid + "," + totalComment + ",0,0)";
                        query += "end ";
                    }
                    if (query == "") continue;
                    
                    // SELECT SCOPE_IDENTITY()

                    //string linkId2 = toLinkId(arr[i].catid.ToString(), arr[i].datetimeid.ToString(), arr[i].domain.ToString(), System.Web.HttpUtility.HtmlEncode(arr[i].title.Trim()));
                    ////string linkId1 = System.Web.HttpUtility.HtmlEncode(arr[i].token.Trim());

                    //if (titleCrawled.ContainsKey(linkId2))//titleCrawled.ContainsKey(linkId1) || 
                    //{
                    //    //if (titleCrawled.ContainsKey(linkId1))
                    //    //{
                    //    //    query = "update titles set totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + condition1;
                    //    //}
                    //    //else
                    //    //{
                    //    //if (titleCrawled.ContainsKey(linkId2)) 
                    //        query = "update titles set totalComment=" + totalComment + ",ranking=" + arr[i].ranking + ", hasContent=" + hasContent + ", fullContent=" + vN(fullContent) + condition2;
                    //    //}
                    //}
                    //else
                    //{
                    //    query = " insert into titles(name,tokenid,des,fullContent,hasContent,link,image,domain,maindomain,catid,source,datetime,lastUpdateRanking,ranking,fixranking,keyword,datetimeid,totalComment,uplikes,downlikes) values(" + vN(arr[i].title.Trim()) + "," + vN(arr[i].token.Trim()) + "," + vN(arr[i].des) + "," + vN(arr[i].fullContent) + "," + hasContent + "," + vN(arr[i].link) + "," + vN(arr[i].image) + "," + arr[i].domain + "," + vN(arr[i].maindomain) + "," + arr[i].catid + "," + vN(arr[i].source) + "," + vN(arr[i].date) + "," + vN(arr[i].date) + "," + arr[i].ranking + "," + arr[i].ranking + "," + vN(arr[i].keyword) + "," + Uti.convertToDateTimeId(arr[i].date) + "," + totalComment + ",0,0)";

                    //}
                    //////if (!titleCrawled.ContainsKey(linkId1)) titleCrawled.Add(linkId1, "1");

                    //if (!titleCrawled.ContainsKey(linkId2)) titleCrawled.Add(linkId2, "1");

                    ////if (arr[i].link.Contains("http://vnexpress.net/tin-tuc/thoi-su/duong-pho-ha-noi-ngon-ngang-sau-mua-giong-3000178.html")){
                    ////    int abc=0;
                    ////}

                    try
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();

                    }
                    catch(Exception ex1) {
                        //try{
                        //    cmd.CommandText = "insert into log(err,datetime) values(" + vN("insertData():" + query) + "," + vN(DateTime.Now.ToString()) + ")";
                        //    cmd.ExecuteNonQuery();
                        //}
                        //catch (Exception ex2) { 

                        //}
                        //int abc = 0;
                        //string a = query;
                    }
                    //query = "SELECT SCOPE_IDENTITY()";
                    //cmd.CommandText = query;
                    //arr[i].id = (long)(decimal)cmd.ExecuteScalar();
                    CurrentBatchRun.getInfo = "Đang chèn tin " + i + "/" + L;
                    showProcessCrawl();
                }
            }
            //update total comment
            query="update tinviet_admin.titles set totalcomment=(";
		    query+="select totalcomment from (select A.newsid,count(B.id) as totalcomment ";
            query+= " from (select link,name,id as newsid from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-2)+") as A ";
		    query+=" left join(select idnews,id from tinviet_admin.comments) as B on A.newsid=B.idnews  group by A.newsid) as C ";
            query+=" where C.newsid=tinviet_admin.titles.id)";
            CurrentBatchRun.getInfo = "Cập nhật comments!";
            try
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex2)
            {

            }            
            //Chèn vào bảng backup để search và lưu trữ cho Google tìm thấy
            int maxid = 0;
            query = "select max(id) as maxid from tinviet_admin.titlesSearch";
            try
            {
                cmd.CommandText = query;
                SqlDataReader DR = cmd.ExecuteReader();
                if (DR.Read()) {
                    maxid =int.Parse(DR["maxid"].ToString());
                }
                DR.Close();
            }
            catch (Exception ex3)
            {

            }
            query = "insert into tinviet_admin.titlesSearch select * from tinviet_admin.titles where id>"+maxid;
            try
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex4)
            {

            }
            query = "delete from tinviet_admin.titles where datetimeid<" + Uti.datetimeidByDay(-7);
            try
            {
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex2)
            {

            }
            cmd.Dispose();
            closeConn();
        }
        //Đánh dấu đâu là tin nóng trong Database
        private void updateHotNews()
        {
            try
            {
                //Chen vao du lieu            
                openConn();
                //string query = "";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                int datetimeid = Uti.datetimeid();
                int datetimeid2 = Uti.datetimeidByDay(-1);
                DateTime datetimenow = DateTime.Now;
                //int minRaking = 0;// Uti.getMinRanking();
                //reset hot
                cmd.CommandText = "update tinviet_admin.titles set isHot=0 where isHot<2 and datetimeid>=" + datetimeid;
                cmd.ExecuteNonQuery();
                //Cap nhat tin nao dung dau chu de
                cmd.CommandText = "update tinviet_admin.titles set isHot=1 where isHot<2 and datetimeid>=" + datetimeid2 + " and topicid=id and datediff(hour,datetime,getdate())<=" + Config.minHourHotNews;
                cmd.ExecuteNonQuery();
                //Cap nhat tin nao khong thuoc chu de nao ca
                cmd.CommandText = "update tinviet_admin.titles set isHot=1 where isHot<2 and datetimeid>=" + datetimeid2 + " and topicid is null and datediff(hour,datetime,getdate())<=" + Config.minHourHotNews;
                cmd.ExecuteNonQuery();

                //Cap nhat tin trong vong 3 tieng
                cmd.CommandText = "update tinviet_admin.titles set isHot=1 where isHot<2 and datetimeid>=" + datetimeid2 + " and datediff(hour,datetime,getdate())<=3";
                cmd.ExecuteNonQuery();

                //Cap nhat tin co diem>=100
                cmd.CommandText = "update tinviet_admin.titles set isHot=1 where isHot<2 and datetimeid>=" + datetimeid + " and ranking>=100";
                cmd.ExecuteNonQuery();

                //Cap nhat tin nao lay tu homepage
                //cmd.CommandText = "update tinviet_admin.titles set isHot=1  where datetimeid>=" + datetimeid2 + " and hasContent=1 and catid=0 and datediff(hour,datetime,getdate())<=" + Config.minHourHotNews;//and (topicid is null or topicid=id) 
                //cmd.ExecuteNonQuery();
                cmd.CommandText = "select id,name,link,maindomain,datetime from tinviet_admin.titles where datetimeid>=" + datetimeid + " and catid=0 and hasContent=1 order by maindomain,datetime desc";
                SqlDataReader DR = cmd.ExecuteReader();
                string maindomain = "";
                string hotNewsListId="(-1,";
                while (DR.Read()) {
                    if (!maindomain.Equals(DR["maindomain"].ToString())) {
                        hotNewsListId += DR["id"].ToString()+",";
                        maindomain = DR["maindomain"].ToString();
                    }
                }
                DR.Close();
                hotNewsListId += "-1)";
                cmd.CommandText = "update tinviet_admin.titles set isHot=1  where datetimeid>=" + datetimeid + " and hasContent=1 and catid=0 and id in "+hotNewsListId+" and datediff(hour,datetime,getdate())<=" + Config.minHourHotNews;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "update tinviet_admin.titles set isHot=0,ranking=ranking-5  where isHot<2 and datetimeid>=" + datetimeid + " and hasContent=1 and catid=0 and id not in " + hotNewsListId;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                closeConn();
            }
            catch (Exception ex) { 

            }
        }
        public void UpdateTextBox(string text)
        {
           
        }
        private void appendTwoArray(ref Rss.ItemXml[] A, Rss.ItemXml[] B)
        {
            try
            {
                for (int i = 0; i < B.Length; i++)
                    if (B[i].title != null && B[i].title != "")
                    {
                        A[countItemTitle] = B[i];
                        countItemTitle++;
                    }
            }
            catch (Exception ex) { 
            }
        }
        private int lengthSource(string value) {
            string[] temp=value.Split(',');
            int count=0;
            for (int l = 0; l < temp.Length; l++) {
                if (temp[l] != null && temp[l] != "") count++;
            }
            return count;
        }
        private bool isTheSameDate(string date1, string date2) {
            try
            {
                string d1,d2;
                d1=DateTime.Parse(date1).ToUniversalTime().ToShortDateString();
                d2=DateTime.Parse(date2).ToUniversalTime().ToShortDateString();
                if (d1==d2)
                     return true;
                else 
                     return false;
            }
            catch (Exception ex) {
                return true;
            }
            return true;
        }
        //Hàm này chạy liên tục để crawl tin tức
        private void DoSomething(int seconds)
        {
            DateTime nowDate = DateTime.Now;
            Thread.Sleep(seconds * 5000);
            try
            {
                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now;
                string howLong = "";
                //generateDailyHotKeyword();
                //generateTrends();
                //return;
                ////Rs = new Rss("http://www.thanhnien.com.vn/", 16, "thanhnien.com.vn", 0, 11);
                ////appendTwoArray(ref mainItemTitle, Rs.arrItem);
                ////Rs.Clear();
                //Rs = new Rss("http://www.techz.vn/", 16, "http://www.techz.vn/", 0, 11);
                //appendTwoArray(ref mainItemTitle, Rs.arrItem);
                //Rs.Clear();
                //CurrentBatchRun.getInfo = DateTime.Now.ToString();
                //generateSiteMap();
                //return;
                //CurrentBatchRun.getInfo = "OK" + DateTime.Now.ToString();
                //return;                
                //setNewsSameTopic();
                updateHotNews();
                //return;
                generateHotNew();
                generateAllNew();
                generateCategoryNews();                
                //return;
                //generateCategoryNews();                
                //generateAllNew();
                //realTimeNews();
                //generateDailyHotKeyword();
                //generateTrends();
                //return;
                
                //return;
                //updateRankingByTime();
                //generateSiteMap();
                //return;
                //setNewsSameTopic();                
                //generateDailyHotKeyword();
                //generateTopNewWeekly();
                //return;
                //CurrentBatchRun.getInfo = "Cap nhat tin tuc trong tuan!";
                //generateTopNewWeekly();
                //return;
                //CurrentBatchRun.getInfo = "Cập nhật ranking theo giờ cho các tin";
                //updateRankingByTime();
                //return;
                //loadTrendKeyword();
                //CurrentBatchRun.getInfo = "Cập nhật Tin cùng chủ đề";
                //setNewsSameTopic();
                //CurrentBatchRun.getInfo = "Cap nhat tin tuc trong tuan!";
                //generateTopNewWeekly();
                //return;
                //CurrentBatchRun.getInfo = "Viết ra từ khóa nóng hàng ngày daily hot keyword ... ";
                ////Viết ra từ khóa nóng hàng ngày daily hot keyword
                ////chèn vào bảng dailykeyword
                //generateDailyHotKeyword();
                //return;
                //CurrentBatchRun.getInfo = "Crawl for Dantri";
                //crawlContentFromUrl("thanhnien");
                //return;
                howLong += "Started at: " + DateTime.Now.ToString() + ".\r\n";
                loadBlockLink();
                loadTitleComment();
                loadAllNewsCrawled();
                loadHotKeyword();
                loadFilterKeyword();
                loadTrendKeyword();
                //realtime đẩy ra trang chủ 1 số tin mới nhất
                //realTimeNews();
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Loading all.\r\n";
                startTime = DateTime.Now;

                CurrentBatchRun.getInfo = "Bắt đầu crawl tin tức, đọc rss từ các báo!..";
                showProcessCrawl();
                mainItemTitle = null;
                mainItemTitle = new Rss.ItemXml[5000];
                countItemTitle = 0;

                openConn();
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = con;
                Cmd.CommandText = "select a.name as name,url,catid,domain,b.name as maindomain from tinviet_admin.rss as a left join tinviet_admin.domain as b on a.domain=b.id where catid<>0";
                SqlDataReader DR = Cmd.ExecuteReader();
                //string maindomain = "";
                while (DR.Read())
                {
                    
                    lblInfo.Text = "crawl: " + DR["name"].ToString()+"....";
                    try
                    {
                        //Đọc các tin rss từ các url và domain, gán tạm vào mảng
                        Rs = new Rss(DR["url"].ToString(), int.Parse(DR["domain"].ToString()), DR["name"].ToString(), int.Parse(DR["catid"].ToString()), 1);
                        //if (DR["url"].ToString().Contains("http://vnexpress.net/rss/thoi-su.rss")) {
                        //    int abc = 0;
                        //}
                    }
                    catch {
                        Rs.Clear();
                        continue;
                    }
                    
                    if (Rs.Length > 0)
                    {   
                        //Nếu đọc được tin từ trang báo url ở trên, thì cộng thêm vào mảng tin tức để sau này xử lý
                        appendTwoArray(ref mainItemTitle, Rs.arrItem);
                    }                    
                    Rs.Clear();
                }
                DR.Close();
                closeConn();
                

                CurrentBatchRun.getInfo = "Đã lấy hết các tin từ các báo về, với số tin là " + countItemTitle+", Đang Phân tích tin và xếp hạng tin tức!";
                showProcessCrawl();
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Loading all Rss news to array.\r\n";
                startTime = DateTime.Now;
                int countHidden = 0;
                //Giải thuật, cho điểm ranking các tin 
                //Dựa trên hai điều chủ yếu sau đây:
                    //+Tin nào có chứa từ khóa mà nhiều báo đăng=> Cần xem lại, hoặc bỏ đi
                            //Tin đó hot, cho điểm cao, độ dài của từ khóa đó càng dài chứng tỏ tin đó càng hot. 
                            //Vì nó có thể chứa Tên riêng, Địa danh, sự kiện nào đó.
                    //+Tin nào càng nhiều báo đăng càng hot--> làm yếu tố chính
                for (int i = 0; i < countItemTitle; i++) {
                    mainItemTitle[i].source = ",";
                    mainItemTitle[i].show = true;
                    mainItemTitle[i].keyword = "";
                    if (Uti.isTheSameDate(nowDate.ToString(), mainItemTitle[i].date)) {
                        mainItemTitle[i].ranking = Uti.getHourRanking(mainItemTitle[i].date);
                    }
                   
                    if (mainItemTitle[i].image!="" && !mainItemTitle[i].image.StartsWith("http") && !mainItemTitle[i].image.StartsWith("www"))
                    {
                        mainItemTitle[i].image = mainItemTitle[i].maindomain + mainItemTitle[i].image;
                    }
                }
                
                for (int i = 0; i < countItemTitle; i++) {
                    bool found = false;                    
                    //mainItemTitle[i].ranking = 0;
                    for (int j = i + 1; j < countItemTitle; j++) {
                        if (mainItemTitle[i].show && mainItemTitle[j].show && mainItemTitle[i].domain != mainItemTitle[j].domain) {
                            string cd = Rss.getThePieceOfTwoString(System.Web.HttpUtility.HtmlDecode(mainItemTitle[i].title), System.Web.HttpUtility.HtmlDecode(mainItemTitle[j].title));
                            //Lấy ra xâu con chung dài nhất của hai tin để biết độ dài từ khóa chung là gì
                            //Kiểm tra xem độ dài từ khóa đó có đủ dài hay không
                            if (Rss.realLength(cd) >= 2)
                            {
                                //try{
                                //    if (mainItemTitle[i].datetimeid > mainItemTitle[j].datetimeid)
                                //    {
                                //        mainItemTitle[j].date = mainItemTitle[i].date;
                                //    }
                                //    else mainItemTitle[i].date = mainItemTitle[j].date;
                                //}catch(Exception ex){

                                //}
                                
                                found = true;
                                
                                    if (mainItemTitle[i].source != null && !mainItemTitle[i].source.Contains("," + mainItemTitle[j].domain + ",")) mainItemTitle[i].source += mainItemTitle[j].domain + ",";
                                    if (mainItemTitle[j].source != null && !mainItemTitle[j].source.Contains("," + mainItemTitle[i].domain + ",")) mainItemTitle[j].source += mainItemTitle[i].domain + ",";
                                    found = false;
                                    if (mainItemTitle[i].keyword != null && !mainItemTitle[i].keyword.Contains(cd + ","))
                                    {
                                        mainItemTitle[i].keyword += cd + ",";
                                        found = true;
                                    }
                                    //Cho điểm ranking cho tin i này, dựa trên tiêu chí ở trên.
                                    if (found) mainItemTitle[i].ranking += cd.Length + lengthSource(mainItemTitle[i].keyword); else mainItemTitle[i].ranking += 1;
                                    found = false;
                                    if (mainItemTitle[j].keyword != null && !mainItemTitle[j].keyword.Contains(cd + ","))
                                    { 
                                        mainItemTitle[j].keyword += cd + ",";
                                        found = true;
                                    }
                                    //Cho điểm ranking cho tin j này, dựa trên tiêu chí ở trên.
                                    if (found) mainItemTitle[j].ranking += cd.Length + lengthSource(mainItemTitle[j].keyword); else mainItemTitle[j].ranking += 1;                                                                       
                                    
                                
                                countHidden++;
                            }//tin trung nhau
                        }//neu thoa man dieu kien
                        
                    }//j
                    try
                    {
                        //Tăng ranking với từ khóa nóng: (Do admin tự định nghĩa tại đia chỉ /hotKeyWord, và mỗi lần user vote lên thì
                        //cái title tin đó âm thầm cũng cập nhật thành từ khóa nóng(ở đoạn vote tin))
                        for (int l = 0; l < countHotKeyWord; l++)
                        {
                            if (arrHotKeyWord[l].keyword != null && mainItemTitle[i].title.ToLower().IndexOf(arrHotKeyWord[l].keyword.ToLower()) >= 0)
                            {
                                mainItemTitle[i].ranking += arrHotKeyWord[l].ranking;
                            }
                        }
                        //Ngược lại với tăng ranking là giảm ranking với từ khóa filter, Do admin dinh nghia tại địa chỉ /filterKeyword, va user vote down
                        for (int l = 0; l < countFilterKeyword; l++)
                        {
                            if (arrFilterKeyword[l].keyword != null && mainItemTitle[i].title.ToLower().IndexOf(arrFilterKeyword[l].keyword.ToLower()) >= 0)
                            {
                                mainItemTitle[i].ranking -= arrFilterKeyword[l].ranking;
                            }
                        }
                    }
                    catch (Exception exFilter) { 
                    }
                }//i
                //Lấy ra từ HomePage
                Rs = new Rss("http://news.zing.vn", 1, "zing.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://vnexpress.net", 2, "vnexpress.net", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://laodong.com.vn", 10, "laodong.com.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://vietnamnet.vn", 6, "vietnamnet.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://ngoisao.net", 5, "ngoisao.net", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://thethao247.vn", 16, "thethao247.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                //Rs = new Rss("http://www.bongda.com.vn", 39, "bongda.com.vn", 0, 11);
                //appendTwoArray(ref mainItemTitle, Rs.arrItem);
                //Rs.Clear();
                Rs = new Rss("http://www.techz.vn", 38, "techz.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://www.thanhnien.com.vn/pages/default.aspx", 12, "http://www.thanhnien.com.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://vtc.vn", 9, "vtc.vn", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                Rs = new Rss("http://www.doisongphapluat.com", 29, "doisongphapluat.com", 0, 11);
                appendTwoArray(ref mainItemTitle, Rs.arrItem);
                Rs.Clear();
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " set ranking of all news\r\n";
                startTime = DateTime.Now;

                //Chèn vào cơ sở dữ liệu các tin vừa crawl được ở mảng
                CurrentBatchRun.getInfo = "Chèn tin mới vào cơ sở dữ liệu! ";
                CurrentBatchRun.getInfo = "Dang chay vong lap " + CurrentBatchRun.ItemsCompleted + "/" + CurrentBatchRun.TotalNumberOfItems;
                insertData(ref mainItemTitle, countItemTitle);
                CurrentBatchRun.getInfo = "Dang chay vong lap " + CurrentBatchRun.ItemsCompleted + "/" + CurrentBatchRun.TotalNumberOfItems;
                CurrentBatchRun.getInfo = "Đã chèn tin mới vào cơ sở dữ liệu! ";
                CurrentBatchRun.getInfo = "Cập nhật ranking theo giờ cho các tin";
                showProcessCrawl();
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Inserted all news to database\r\n";
                startTime = DateTime.Now;
                ////Trừ điểm tin nào tuy là hot nhưng do thời gian cũ nên giảm dần
                ////So với lần cập nhật cuối lastUpdateRanking, cứ mỗi giờ giảm 5 điểm.                
                updateRankingByTime();
                //endTime = DateTime.Now;
                //howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Updated ranking by time\r\n";
                //startTime = DateTime.Now;
                //Rss.ItemXml tempp;
                
                //// Cập nhật những tin nóng mới nhất, cho đến thời điểm hiện tại! Khác với Điểm tin trong ngày
                ////Đoạn này cần nghiên cứu thế nào là tin nóng, nếu bỏ đi thì bỏ đoạn code này đi.
                //// Những tin nóng là những tin:  có ranking>0, đồng thời có keyword từ 2 từ khóa trở lên
                //CurrentBatchRun.getInfo = "Lọc ra tin nóng riêng";
                
                //Rss.ItemXml[] tempMainItemTitle= null;
                //tempMainItemTitle = new Rss.ItemXml[50000];
                //int tempCountItemTitle = 0;
                //for (int i = 0; i < countItemTitle; i++) {
                //    if (isTheSameDate(mainItemTitle[i].date, nowDate.ToString()) && mainItemTitle[i].ranking != 0 && lengthSource(mainItemTitle[i].keyword) >= 2)//2 domain tro len
                //    { 
                //        tempMainItemTitle[tempCountItemTitle] = mainItemTitle[i];
                //        tempCountItemTitle++;
                //    }
                //}
                //CurrentBatchRun.getInfo = "Sắp xếp tin nóng theo ranking giảm dần";
                ////Sap xep theo chieu giam dan cua ranking                
                //for (int i = 0; i < tempCountItemTitle - 1; i++)
                //{
                //    for (int j = i + 1; j < tempCountItemTitle; j++)
                //    {
                //        if (tempMainItemTitle[j].ranking > tempMainItemTitle[i].ranking)
                //        {
                //            tempp = tempMainItemTitle[i];
                //            tempMainItemTitle[i] = tempMainItemTitle[j];
                //            tempMainItemTitle[j] = tempp;
                //        }
                //    }
                //}
                CurrentBatchRun.getInfo = "Cập nhật Tin cùng chủ đề";
                showProcessCrawl();
                setNewsSameTopic();
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Updated updateRankingByTime and news to trends news same topic\r\n";
                startTime = DateTime.Now;

                CurrentBatchRun.getInfo = "Cập nhật các tin nóng vào Database:";
                showProcessCrawl();
                //Cập nhật đánh dấu đâu là tin nóng                             
                updateHotNews();
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Updated Hot News\r\n";
                startTime = DateTime.Now;

                CurrentBatchRun.getInfo = "Cập nhật Tin Nóng trong ngày HotNew.xml";
                generateHotNew();
                CurrentBatchRun.getInfo = "Cập nhật Điểm tin trong ngày AllNew.xml";
                generateAllNew();
                CurrentBatchRun.getInfo = "Cập nhật Điểm tin Các chuyên mục Cat_*.xml";
                generateCategoryNews();
                CurrentBatchRun.getInfo = "Cập nhật Tin mới nhất";
                realTimeNews();
                CurrentBatchRun.getInfo = "Cập nhật Xu hướng đọc";
                generateTrends();

                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Generated all xml file\r\n";
                startTime = DateTime.Now;

                CurrentBatchRun.getInfo = "Viết ra từ khóa nóng hàng ngày daily hot keyword ... ";
                //Viết ra từ khóa nóng hàng ngày daily hot keyword
                //chèn vào bảng dailykeyword
                generateDailyHotKeyword();

                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Generated all hot daily keyword\r\n";
                startTime = DateTime.Now;
                //Tin nóng trong tuần!
                //CurrentBatchRun.getInfo = "Cập nhật tin nóng trong tuần, ghi ra file xml";
                //showProcessCrawl();
                //generateTopNewWeekly();

                //endTime = DateTime.Now;
                //howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Generated top weekly\r\n";
                //startTime = DateTime.Now;
                CurrentBatchRun.getInfo = "Cập nhật sitemap.xml";
                //showProcessCrawl();
                generateSiteMap();
                string callService = "";
                try
                {
                    string abc=Rss.getContent("http://hayhayvuivui.com/crawlImage.aspx");
                    if (abc.Contains("Batch Runner Example")) {
                        callService = "call service hayhayvuivui ok";
                    }
                    abc = Rss.getContent("http://thetopnews.net/crawl.aspx");
                    if (abc.Contains("Batch Runner Example"))
                    {
                        callService += ", call service thetopnews ok";
                    }
                    abc = Rss.getContent("http://tingting.vn/crawlImage.aspx");
                    if (abc.Contains("Batch Runner Example"))
                    {
                        callService += ", call service tingting.vn ok";
                    }
                }
                catch (Exception ex) { 

                }
                endTime = DateTime.Now;
                howLong += Uti.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " generateSiteMap(); "+callService+"\r\n";
                startTime = DateTime.Now;
                howLong += "End at: " + DateTime.Now.ToString() + ".\r\n";
                //DateTime endTime = DateTime.Now;
                //string howLong = Uti.getDiffTimeMinuteFromTwoDate(startTime,endTime);
                StreamWriter SW = new StreamWriter(HttpRuntime.AppDomainAppPath + "HowLong.txt");
                SW.WriteLine(howLong);
                SW.Close();
                CurrentBatchRun.getInfo = "Đã xong và chạy lại!";
                showProcessCrawl();

            }
            catch (Exception ex) {
                CurrentBatchRun.getInfo = "Do SomeThing error.." + ex.ToString();
                
            }
        }
        //So với lần cập nhật cuối lastUpdateRanking, cứ mỗi giờ giảm 5 điểm.
        private void updateRankingByTime() {
            try
            {
                int dateTimeId = Uti.datetimeid();
                openConn();
                SqlCommand SC = new SqlCommand();
                SC.Connection = con;
                SC.CommandText = "update tinviet_admin.titles set ranking=ranking-datediff(hour,lastUpdateRanking,getdate())*" + Config.PointUpdateRanking + ",downlikes=downlikes+datediff(hour,lastUpdateRanking,getdate())*" + Config.PointUpdateRanking + " where isHot>=1 and datetimeid>=" + dateTimeId;
                SC.ExecuteNonQuery();
                SC.CommandText = "update tinviet_admin.titles set lastUpdateRanking=getdate() where isHot>=1 and datetimeid>=" + dateTimeId + " and datediff(hour,lastUpdateRanking,getdate())>=1";
                SC.ExecuteNonQuery();
                SC.CommandText = "update tinviet_admin.titles set ranking=" + Config.PointUpdateRanking + " where datetimeid>=" + dateTimeId + " and ranking<0";
                SC.ExecuteNonQuery();
                closeConn();
            }
            catch (Exception ex) { 
            }
        }
        //Lấy ra các tin mới nhất ở Data, Push realtime ra trang chủ
        private void realTimeNews()
        {
            try
            {
                CurrentBatchRun.getInfo = "Real time to homepage";
                showProcessCrawl();
                openConn();
                SqlCommand SC = new SqlCommand();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                SqlDataReader DR = null;
                string preTitle = "";
                string query = "select top 250 ";
                query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,";
                query += " ranking=A.ranking,link=A.link,";
                query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
                query += " uplikes=A.uplikes,downlikes=A.downlikes,";
                query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
                query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
                query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";

                query += " (select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,link,isNull(image,'') as image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " order by datetime desc,ranking desc,id desc) as A left join ";
                query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeid() + " and id=-1) as B on (A.id=B.topicidRelated and A.maindomain<>B.maindomainRelated) or (A.id=B.topicidRelated and A.id=A.topicid and A.maindomain=B.maindomainRelated) or (A.id=B.topicidRelated and A.datetimeid>B.datetimeid and A.maindomain=B.maindomainRelated) ";

                query += " order by A.datetime desc,A.ranking desc,A.id desc,B.idRelated desc,B.rankingRelated desc";

                SC.Connection = con;
                SC.CommandText = query;
                DR = SC.ExecuteReader();


                string xmlDoc = HttpRuntime.AppDomainAppPath + "New.xml";
                string des = "";
                using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("channel");
                    while (DR.Read())
                    {
                        if (DR["title"].ToString().Trim().ToLower().Equals(preTitle.ToLower())) continue;                        
                        preTitle = DR["title"].ToString().Trim().ToLower();
                        try
                        {
                            writer.WriteStartElement("item");
                            writer.WriteElementString("id", DR["id"].ToString());
                            writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                            try
                            {
                                des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                                des = XmlConvert.VerifyXmlChars(des);
                                string re = @"\u0008";
                                des = Regex.Replace(des, re, "");
                                writer.WriteElementString("des", des);
                            }
                            catch (Exception desex)
                            {
                                //writer.WriteElementString("des", "&nbsp;");
                            }
                            writer.WriteElementString("source", DR["source"].ToString());
                            writer.WriteElementString("date", DR["date"].ToString());
                            writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                            //writer.WriteElementString("token", DR["token"].ToString());
                            writer.WriteElementString("ranking", DR["ranking"].ToString());
                            writer.WriteElementString("link", System.Web.HttpUtility.HtmlDecode(DR["link"].ToString()));
                            writer.WriteElementString("image", DR["image"].ToString());
                            writer.WriteElementString("totalcomment", DR["totalcomment"].ToString());
                            writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                            writer.WriteElementString("uplikes", DR["uplikes"].ToString());
                            writer.WriteElementString("downlikes", DR["downlikes"].ToString());
                            writer.WriteElementString("topicid", DR["topicid"].ToString());
                            writer.WriteElementString("catid", DR["catid"].ToString());
                            writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                            if (int.Parse(DR["timediff"].ToString()) >= 0)
                            {
                                writer.WriteElementString("timediff", DR["timediff"].ToString());
                            }
                            else
                            {
                                writer.WriteElementString("timediff", "6");
                            }
                            writer.WriteElementString("nameRelated", System.Web.HttpUtility.HtmlDecode(DR["nameRelated"].ToString()));
                            writer.WriteElementString("topicidRelated", DR["topicidRelated"].ToString());
                            writer.WriteElementString("idRelated", DR["idRelated"].ToString());
                            writer.WriteElementString("linkRelated", DR["linkRelated"].ToString());
                            writer.WriteElementString("maindomainRelated", DR["maindomainRelated"].ToString());
                            writer.WriteElementString("rankingRelated", DR["rankingRelated"].ToString());
                            writer.WriteElementString("hasContentRelated", DR["hasContentRelated"].ToString());
                            writer.WriteElementString("imageRelated", DR["imageRelated"].ToString());
                            writer.WriteEndElement();
                        }
                        catch (Exception ex) { 
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                DR.Close();
                //For mobile app
                //query = "Select top 100 id,name as title,des,hasContent,datetime as date,ranking,link,isNull(image,'') as image,maindomain,topicid,catid,datediff(minute,datetime,getdate()) as timediff from titles where datetimeid>=" + Uti.datetimeidByDay(-1) + "  order by datetime desc,id desc";


                ////SC.Connection = con;
                //SC.CommandText = query;
                //DR = SC.ExecuteReader();
                //settings = new XmlWriterSettings();
                //settings.Indent = true;
                //settings.Encoding = Encoding.UTF8;

                //xmlDoc = HttpRuntime.AppDomainAppPath + "mNew.xml";

                //using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                //{
                //    writer.WriteStartDocument();
                //    writer.WriteStartElement("channel");
                //    while (DR.Read())
                //    {

                //        writer.WriteStartElement("item");
                //        writer.WriteElementString("id", DR["id"].ToString());
                //        writer.WriteElementString("title", DR["title"].ToString());
                //        //writer.WriteElementString("des", DR["des"].ToString());
                //        writer.WriteElementString("date", DR["date"].ToString());
                //        writer.WriteElementString("ranking", DR["ranking"].ToString());
                //        writer.WriteElementString("link", DR["link"].ToString());
                //        writer.WriteElementString("image", DR["image"].ToString());
                //        writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                //        writer.WriteElementString("topicid", DR["topicid"].ToString());
                //        writer.WriteElementString("catid", DR["catid"].ToString());
                //        writer.WriteElementString("timediff", DR["timediff"].ToString());
                //        writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                //        writer.WriteEndElement();
                //    }
                //    writer.WriteEndElement();
                //    writer.WriteEndDocument();
                //}
                //DR.Close();
                //context.Clients.All.noticeNews("broadCast");
                closeConn();
            }
            catch (Exception ex) { 
            }
            
        }
        //Lấy ra các tin hot nhất ở Data, Ghi ra HotNew.xml
        private void generateHotNew()
        {
            try
            {
                CurrentBatchRun.getInfo = "Ghi ra 100 tin nóng nhất ra file xml HotNew";
                showProcessCrawl();
                openConn();
                SqlCommand SC = new SqlCommand();
                SqlDataReader DR = null;
                XmlWriterSettings settings = null;
                string xmlDoc = null;
                string query = "select top 200 ";
                query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,isHot=A.isHot, ";
                query += " ranking=A.ranking,link=A.link,";
                query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
                query += " uplikes=A.uplikes,downlikes=A.downlikes,";
                query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
                query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
                query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
                query += " (select top 100 name,des,datetimeid,isHot,datetime,source,ranking,topicid,catid,id,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and isHot>=1 and (topicid=id or topicid is null or catid=0) order by  datetimeid desc, ranking desc, id desc) as A left join ";
                query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid<>0 and id=-1) as B on (A.topicid=B.topicidRelated and A.topicid=A.id) ";
                query += " order by A.datetimeid desc,A.isHot desc,A.ranking desc,A.id desc,B.idRelated desc,B.rankingRelated desc";


                SC.Connection = con;
                SC.CommandText = query;
                DR = SC.ExecuteReader();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                xmlDoc = HttpRuntime.AppDomainAppPath + "HotNew.xml";
                string des = "";
                string duplicateHotNews = "";
                using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("channel");
                    while (DR.Read())
                    {
                          
                        try
                        {
                            if (DR["catid"].ToString()=="0" )
                            {
                                if (duplicateHotNews.Contains("," + DR["maindomain"].ToString() + ",")) continue;
                                duplicateHotNews += "," + DR["maindomain"].ToString() + ",";
                            }
                            writer.WriteStartElement("item");
                            writer.WriteElementString("id", DR["id"].ToString());
                            writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                            try
                            {
                                des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                                des = XmlConvert.VerifyXmlChars(des);
                                string re = @"\u0008";
                                des = Regex.Replace(des, re, "");
                                writer.WriteElementString("des", des);
                            }
                            catch (Exception desex)
                            {
                                //writer.WriteElementString("des", "&nbsp;");
                            }
                            writer.WriteElementString("source", DR["source"].ToString());
                            writer.WriteElementString("date", DR["date"].ToString());
                            writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                            //writer.WriteElementString("token", DR["token"].ToString());
                            writer.WriteElementString("ranking", DR["ranking"].ToString());
                            writer.WriteElementString("link", DR["link"].ToString());
                            writer.WriteElementString("image", DR["image"].ToString());
                            writer.WriteElementString("totalcomment", DR["totalcomment"].ToString());
                            writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                            writer.WriteElementString("uplikes", DR["uplikes"].ToString());
                            writer.WriteElementString("downlikes", DR["downlikes"].ToString());
                            writer.WriteElementString("topicid", DR["topicid"].ToString());
                            writer.WriteElementString("catid", DR["catid"].ToString());
                            writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                            if (int.Parse(DR["timediff"].ToString()) >= 0)
                            {
                                writer.WriteElementString("timediff", DR["timediff"].ToString());
                            }
                            else
                            {
                                writer.WriteElementString("timediff", "6");
                            }
                            writer.WriteElementString("nameRelated", System.Web.HttpUtility.HtmlDecode(DR["nameRelated"].ToString()));
                            writer.WriteElementString("topicidRelated", DR["topicidRelated"].ToString());
                            writer.WriteElementString("idRelated", DR["idRelated"].ToString());
                            writer.WriteElementString("linkRelated", DR["linkRelated"].ToString());
                            writer.WriteElementString("maindomainRelated", DR["maindomainRelated"].ToString());
                            writer.WriteElementString("rankingRelated", DR["rankingRelated"].ToString());
                            writer.WriteElementString("hasContentRelated", DR["hasContentRelated"].ToString());
                            writer.WriteElementString("imageRelated", DR["imageRelated"].ToString());
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2) { 

                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                DR.Close();
                query = "select top 10 ";
                query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,isHot=A.isHot, ";
                query += " ranking=A.ranking,link=A.link,";
                query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
                query += " uplikes=A.uplikes,downlikes=A.downlikes,";
                query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
                query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
                query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
                query += " (select top 100 name,des,isHot,datetimeid,datetime,source,ranking,topicid,catid,id,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and (catid=0 or isHot>=2) and hasContent=1 order by id desc) as A left join ";
                query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid<>0 and id=-1) as B on (A.topicid=B.topicidRelated and A.topicid=A.id) ";
                query += " order by A.datetimeid desc,A.ranking desc,A.id desc";
                SC.CommandText = query;
                DR = SC.ExecuteReader();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                xmlDoc = HttpRuntime.AppDomainAppPath + "TopHotNew.xml";
                string duplicateDomainHotNews = ",-1,";
                using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("channel");
                    while (DR.Read())
                    {

                        try
                        {
                            if (duplicateDomainHotNews.Contains("," + DR["maindomain"].ToString()+",")) {                                
                                continue;
                            }
                            duplicateDomainHotNews += "," + DR["maindomain"].ToString() + ",";
                            writer.WriteStartElement("item");
                            writer.WriteElementString("id", DR["id"].ToString());
                            writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                            try
                            {
                                des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                                des = XmlConvert.VerifyXmlChars(des);
                                string re = @"\u0008";
                                des = Regex.Replace(des, re, "");
                                writer.WriteElementString("des", des);
                            }
                            catch (Exception desex)
                            {
                                //writer.WriteElementString("des", "&nbsp;");
                            }
                            writer.WriteElementString("source", DR["source"].ToString());
                            writer.WriteElementString("date", DR["date"].ToString());
                            writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                            //writer.WriteElementString("token", DR["token"].ToString());
                            writer.WriteElementString("ranking", DR["ranking"].ToString());
                            writer.WriteElementString("link", DR["link"].ToString());
                            writer.WriteElementString("image", DR["image"].ToString());
                            writer.WriteElementString("totalcomment", DR["totalcomment"].ToString());
                            writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                            writer.WriteElementString("uplikes", DR["uplikes"].ToString());
                            writer.WriteElementString("downlikes", DR["downlikes"].ToString());
                            writer.WriteElementString("topicid", DR["topicid"].ToString());
                            writer.WriteElementString("catid", DR["catid"].ToString());
                            writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                            if (int.Parse(DR["timediff"].ToString()) >= 0)
                            {
                                writer.WriteElementString("timediff", DR["timediff"].ToString());
                            }
                            else
                            {
                                writer.WriteElementString("timediff", "6");
                            }
                            writer.WriteElementString("nameRelated", System.Web.HttpUtility.HtmlDecode(DR["nameRelated"].ToString()));
                            writer.WriteElementString("topicidRelated", DR["topicidRelated"].ToString());
                            writer.WriteElementString("idRelated", DR["idRelated"].ToString());
                            writer.WriteElementString("linkRelated", DR["linkRelated"].ToString());
                            writer.WriteElementString("maindomainRelated", DR["maindomainRelated"].ToString());
                            writer.WriteElementString("rankingRelated", DR["rankingRelated"].ToString());
                            writer.WriteElementString("hasContentRelated", DR["hasContentRelated"].ToString());
                            writer.WriteElementString("imageRelated", DR["imageRelated"].ToString());
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {

                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                DR.Close();
                closeConn();
            }
            catch (Exception ex) { 

            }
        }
        //Lấy ra các tin ở Data, Ghi ra SiteMap.xml
        private void generateSiteMap()
        {
            try
            {
                CurrentBatchRun.getInfo = "Ghi ra siteMap";
                //showProcessCrawl();
                openConn();
                SqlCommand SC = new SqlCommand();
                SqlDataReader DR = null;
                XmlWriterSettings settings = null;
                string xmlDoc = null;
                string query = "select top 300 ";
                query = " select * from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-7) + " and hasContent=1 order by id desc";


                SC.Connection = con;
                SC.CommandText = query;
                DR = SC.ExecuteReader();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";
                float percent = 0.85f;
                int datetimeid = Uti.datetimeid();
                string urllink = "";
                using (XmlTextWriter writer = new XmlTextWriter(xmlDoc, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://diemtinvietnam.vn/tin-nong");
                    //writer.WriteElementString("lastmod", DateTime.Now.ToUniversalTime().ToString());
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "1");
                    writer.WriteEndElement();
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://diemtinvietnam.vn/diem-tin-ngay");
                    //writer.WriteElementString("lastmod", DateTime.Now.ToUniversalTime().ToString());
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "0.98");
                    writer.WriteEndElement();
                    for (int i = 1; i <= 13; i++)
                        if (i != 11)
                        {
                            writer.WriteStartElement("url");
                            writer.WriteElementString("loc", "http://diemtinvietnam.vn/" + Uti.getCatNameFromIdNoMark(i));
                            //writer.WriteElementString("lastmod", DateTime.Now.ToUniversalTime().ToString());
                            writer.WriteElementString("changefreq", "always");
                            writer.WriteElementString("priority", "0.95");
                            writer.WriteEndElement();
                        }
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://diemtinvietnam.vn/Home/DanhBa");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "0.94");
                    writer.WriteEndElement();
                    //writer.WriteStartElement("url");
                    //writer.WriteElementString("loc", "http://covua.diemtinvietnam.vn");
                    //writer.WriteElementString("changefreq", "hourly");
                    //writer.WriteElementString("priority", "0.93");
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("url");
                    //writer.WriteElementString("loc", "http://giaitri.diemtinvietnam.vn/Home/Video");
                    //writer.WriteElementString("changefreq", "always");
                    //writer.WriteElementString("priority", "0.92");
                    //writer.WriteEndElement();
                    //writer.WriteStartElement("url");
                    //writer.WriteElementString("loc", "http://giaitri.diemtinvietnam.vn/lmht");
                    //writer.WriteElementString("changefreq", "always");
                    //writer.WriteElementString("priority", "0.92");
                    //writer.WriteEndElement();
                    while (DR.Read())
                    {
                        //string token = DR["tokenid"].ToString();
                        //Guid.NewGuid().ToString();
                        //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                        //writer.WriteAttributeString("id", "p3"); 
                        try
                        {
                            writer.WriteStartElement("url");
                            urllink = "http://diemtinvietnam.vn/" + Uti.unicodeToNoMark(System.Web.HttpUtility.HtmlDecode(DR["name"].ToString()) + " " + Uti.smDomainNew(DR["maindomain"].ToString()) + " " + Uti.getCatNameFromId(int.Parse(DR["catid"].ToString()))) + "-" + DR["id"].ToString();
                            writer.WriteElementString("loc", urllink);//"http://diemtinvietnam.vn/" + Uti.unicodeToNoMark(Server.HtmlDecode(DR["name"].ToString())) + "-" + DR["id"].ToString()
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try
                            {
                                if (Uti.datetimeid() - int.Parse(DR["datetimeid"].ToString()) <= 1)
                                {
                                    writer.WriteElementString("changefreq", "hourly");
                                    percent = 0.85f;
                                }
                                else
                                {
                                    writer.WriteElementString("changefreq", "monthly");
                                    percent = 0.70f;
                                }
                            }
                            catch (Exception ex1)
                            {
                            }
                            //try
                            //{
                            //    //percent = 0.85f;
                            //    //percent = percent + (int.Parse(DR["datetimeid"].ToString()) - datetimeid) * 0.01f;
                            //}
                            //catch (Exception ex)
                            //{
                            //}
                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                DR.Close();
                closeConn();
            }
            catch (Exception extry)
            {
                CurrentBatchRun.getInfo = "Ghi ra siteMap, bi loi " + extry.ToString();
            }
        }
        //Lấy ra các tin hot nhất ở Data, Ghi ra HotNew.xml
        private void generateAllNew()
        {
            try
            {
                CurrentBatchRun.getInfo = "Ghi ra 100 điểm tin trong ngày AllNew";
                showProcessCrawl();
                openConn();
                SqlCommand SC = null;
                SqlDataReader DR = null;
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                string query = "select top 200 ";
                query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,";
                query += " ranking=A.ranking,link=A.link,";
                query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
                query += " uplikes=A.uplikes,downlikes=A.downlikes,";
                query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
                query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
                query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
                query += " (select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-2) + " and (topicid=id or topicid is null or isHot=2 or (catid=0 and hasContent=1 and (topicid=id or topicid is null))) order by  datetimeid desc, ranking desc, id desc) as A left join ";
                query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-2) + " and catid<>0 and id=-1) as B on (A.topicid=B.topicidRelated and A.topicid=A.id) ";//on (A.id=B.topicidRelated and A.maindomain<>B.maindomainRelated) or (A.id=B.topicidRelated and A.id=A.topicid and A.maindomain=B.maindomainRelated) or (A.id=B.topicidRelated and A.datetimeid>B.datetimeid and A.maindomain=B.maindomainRelated) 
                query += " order by A.datetimeid desc,A.ranking desc,A.id desc,B.idRelated desc,B.rankingRelated desc";

                SC = new SqlCommand();
                SC.Connection = con;
                SC.CommandText = query;
                DR = SC.ExecuteReader();

                string xmlDoc = HttpRuntime.AppDomainAppPath + "AllNew.xml";
                string des = "";
                using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("channel");
                    while (DR.Read())
                    {
                        //string token = DR["tokenid"].ToString();
                        //Guid.NewGuid().ToString();
                        //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                        //writer.WriteAttributeString("id", "p3");   
                        try
                        {
                            writer.WriteStartElement("item");
                            writer.WriteElementString("id", DR["id"].ToString());
                            writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                            try
                            {
                                des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                                des = XmlConvert.VerifyXmlChars(des);
                                string re = @"\u0008";
                                des = Regex.Replace(des, re, "");
                                writer.WriteElementString("des", des);
                            }
                            catch (Exception desex)
                            {
                                //writer.WriteElementString("des", "&nbsp;");
                            }
                            writer.WriteElementString("source", DR["source"].ToString());
                            writer.WriteElementString("date", DR["date"].ToString());
                            writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                            //writer.WriteElementString("token", DR["token"].ToString());
                            writer.WriteElementString("ranking", DR["ranking"].ToString());
                            writer.WriteElementString("link", DR["link"].ToString());
                            writer.WriteElementString("image", DR["image"].ToString());
                            writer.WriteElementString("totalcomment", DR["totalcomment"].ToString());
                            writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                            writer.WriteElementString("uplikes", DR["uplikes"].ToString());
                            writer.WriteElementString("downlikes", DR["downlikes"].ToString());
                            writer.WriteElementString("topicid", DR["topicid"].ToString());
                            writer.WriteElementString("catid", DR["catid"].ToString());
                            writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                            if (int.Parse(DR["timediff"].ToString()) >= 0)
                            {
                                writer.WriteElementString("timediff", DR["timediff"].ToString());
                            }
                            else
                            {
                                writer.WriteElementString("timediff", "6");
                            }
                            writer.WriteElementString("nameRelated", System.Web.HttpUtility.HtmlDecode(DR["nameRelated"].ToString()));
                            writer.WriteElementString("topicidRelated", DR["topicidRelated"].ToString());
                            writer.WriteElementString("idRelated", DR["idRelated"].ToString());
                            writer.WriteElementString("linkRelated", DR["linkRelated"].ToString());
                            writer.WriteElementString("maindomainRelated", DR["maindomainRelated"].ToString());
                            writer.WriteElementString("rankingRelated", DR["rankingRelated"].ToString());
                            writer.WriteElementString("hasContentRelated", DR["hasContentRelated"].ToString());
                            writer.WriteElementString("imageRelated", DR["imageRelated"].ToString());
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2) { 
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                DR.Close();
                //For mobile app
                //query = "select top 100 id,name as title,des,hasContent,datetime as date,ranking,link,image,maindomain,topicid,catid,datediff(minute,datetime,getdate()) as timediff from titles";
                //query += " where datetimeid>=" + +Uti.datetimeidByDay(-2) + " and (topicid is null or topicid=id) order by datetimeid desc,ranking desc,id desc";


                ////SC.Connection = con;
                //SC.CommandText = query;
                //DR = SC.ExecuteReader();
                //settings = new XmlWriterSettings();
                //settings.Indent = true;
                //settings.Encoding = Encoding.UTF8;

                //xmlDoc = HttpRuntime.AppDomainAppPath + "mAllNew.xml";

                //using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                //{
                //    writer.WriteStartDocument();
                //    writer.WriteStartElement("channel");
                //    while (DR.Read())
                //    {

                //        writer.WriteStartElement("item");
                //        writer.WriteElementString("id", DR["id"].ToString());
                //        writer.WriteElementString("title", DR["title"].ToString());
                //        //writer.WriteElementString("des", DR["des"].ToString());
                //        writer.WriteElementString("date", DR["date"].ToString());
                //        writer.WriteElementString("ranking", DR["ranking"].ToString());
                //        writer.WriteElementString("link", DR["link"].ToString());
                //        writer.WriteElementString("image", DR["image"].ToString());
                //        writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                //        writer.WriteElementString("topicid", DR["topicid"].ToString());
                //        writer.WriteElementString("catid", DR["catid"].ToString());
                //        writer.WriteElementString("timediff", DR["timediff"].ToString());
                //        writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                //        writer.WriteEndElement();
                //    }
                //    writer.WriteEndElement();
                //    writer.WriteEndDocument();
                //}
                //DR.Close();

                //context.Clients.All.noticeNews("broadCast");
                closeConn();
            }
            catch (Exception ex) { 

            }
        }
        struct sameTopic {
            public int id;
            public string keyword;
            public string des;
            public string title;
            public int topicid;
            public int domain;
            public int catid;
            public int datetimeid;
        }
        sameTopic[] arrSameTopic=new sameTopic[4000];
        int countSameTopic = 0;
        //Tự động phân tích và gán các tin cùng từ khóa vào 1 chủ đề
        //Phân tích các từ khóa chung giữa title của 2 tin, hoặc des để biết hai tin này có liên quan hay không
        //Ví dụ nếu 2 tin có 3,4 từ khóa liên tiếp giống nhau, hoặc 4,5,6 từ khóa rời giống nhau thì có thể kết luận là có liên quan
        //Cho lại điểm ranking nếu một tin có nhiều tin khác liên quan, ví dụ 1 tin có 19 tin khác liên quan thì điểm của nó là 19*70=1330 điểm
        private void setNewsSameTopic()
        {
            try
            {
                int today = Uti.datetimeid();
                int yesterday = Uti.datetimeidByDay(-1);
                arrSameTopic = new sameTopic[9000];
                countSameTopic = 0;
                CurrentBatchRun.getInfo = "Tự động phân tích và gán các tin cùng từ khóa vào 1 chủ đề";
                openConn();
                SqlCommand SC = new SqlCommand();
                SC.Connection = con;
                SC.CommandText = "Select id,keyword,name,des,domain,catid,datetimeid from tinviet_admin.titles where datetimeid>=" + today + "  order by ranking desc,datetime desc";
                SqlDataReader DR = SC.ExecuteReader();
                while (DR.Read())
                {
                    arrSameTopic[countSameTopic].id = int.Parse(DR["id"].ToString());
                    arrSameTopic[countSameTopic].keyword = System.Web.HttpUtility.HtmlDecode(DR["keyword"].ToString().Replace(",", " "));
                    arrSameTopic[countSameTopic].des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                    arrSameTopic[countSameTopic].title = System.Web.HttpUtility.HtmlDecode(DR["name"].ToString());
                    arrSameTopic[countSameTopic].topicid = 0;//Lúc đầu chưa set vào đâu cả
                    arrSameTopic[countSameTopic].domain = int.Parse(DR["domain"].ToString());
                    arrSameTopic[countSameTopic].catid = int.Parse(DR["catid"].ToString());
                    arrSameTopic[countSameTopic].datetimeid = int.Parse(DR["datetimeid"].ToString());
                    countSameTopic++;

                }
                DR.Close();
                Array.Resize(ref arrSameTopic, countSameTopic);
                //reset all
                //SC.CommandText = "update tinviet_admin.titles set topicid=null where id=" + arrSameTopic[i].id;
                //SC.ExecuteNonQuery();
                bool found = false;
                for (int i = 0; i < countSameTopic - 1; i++)
                {
                    if (arrSameTopic[i].topicid == 0)
                    {
                        found = false;
                        for (int j = i + 1; j < countSameTopic; j++)
                            if (arrSameTopic[j].topicid == 0)
                            //if (arrSameTopic[i].domain != arrSameTopic[j].domain && arrSameTopic[i].catid == arrSameTopic[j].catid)
                            {
                                //bool condition = (arrSameTopic[i].domain != arrSameTopic[j].domain && arrSameTopic[i].catid == arrSameTopic[j].catid);
                                //condition = condition || (arrSameTopic[i].domain == arrSameTopic[j].domain && (arrSameTopic[i].catid == 0 || arrSameTopic[j].catid == 0));
                                //condition = condition || (arrSameTopic[i].domain == arrSameTopic[j].domain && arrSameTopic[i].catid == arrSameTopic[j].catid);
                                //condition = condition || (arrSameTopic[i].domain != arrSameTopic[j].domain && (arrSameTopic[i].catid == 0 || arrSameTopic[j].catid == 0));
                                //condition = condition || (arrSameTopic[i].title.ToLower().Equals(arrSameTopic[j].title.ToLower()));
                                //if (!condition) continue;//Neu cung domain va 2 catid<>0 va khac nhau, hoac khac domain nhung ca hai catid deu <>0 va khac nhau
                                CurrentBatchRun.getInfo = countSameTopic + ") so sánh phân tích cặp " + i + ":" + arrSameTopic[i].title + "," + j + ":" + arrSameTopic[j].title;
                                showProcessCrawl();
                                //Có chung từ khóa của title, des giống nhau với độ dài chấp nhận được
                                bool isTheSameTopic = false;
                                string tukhoachung1 = "";
                                string tukhoachung2 = "";
                                string tukhoachung3 = "";
                                //string tukhoachung4 = "";

                                if (!arrSameTopic[i].title.ToLower().Equals(arrSameTopic[j].title.ToLower()))
                                {
                                    tukhoachung1 = Rss.XauConChungDaiNhat(arrSameTopic[i].title, arrSameTopic[j].title).Trim();
                                    // tukhoachung2 = Rss.XauConChungDaiNhat(arrSameTopic[i].des, arrSameTopic[j].des).Trim();


                                    //Nếu title có các xâu con chung dài nhất>4 
                                    isTheSameTopic = (Rss.realLength(tukhoachung1) >= 6);// && (Rss.realLength(tukhoachung2) >= 4);// && (Rss.realLength(tukhoachung3.Split(' ')) >=2);                        

                                    if (!isTheSameTopic)
                                    {
                                        tukhoachung2 = Rss.getThePieceOfTwoString(arrSameTopic[i].title, arrSameTopic[j].title).Trim();
                                        //Hoặc 2 title của 2 tin này có 3 từ khóa liên tiếp giống nhau 
                                        isTheSameTopic = isTheSameTopic || (Rss.realLength(tukhoachung2) >= 4);// && Rss.realLength(tukhoachung1) >= 4);
                                    }

                                    if (!isTheSameTopic)
                                    {
                                        tukhoachung3 = Rss.XauConChungDaiNhat(arrSameTopic[i].keyword, arrSameTopic[j].keyword).Trim();
                                        isTheSameTopic = isTheSameTopic || (Rss.realLength(tukhoachung3) >= 10 && Rss.realLength(tukhoachung2) >= 3);//có 3 từ khóa giống nhau.
                                    }

                                    //Hoặc 2 keyword của 2 tin này có 4 từ khóa chung.
                                    //isTheSameTopic = isTheSameTopic || (Rss.realLength(tukhoachung4) >= 4 && Rss.realLength(tukhoachung1) >= 4);

                                    //Hoặc 2 tin này có chung từ khóa Trends
                                    if (!isTheSameTopic)
                                    {
                                        bool isTheSameTrendsKeyword = false;
                                        for (int l = 0; l < countTrendKeyWord; l++)
                                        {
                                            if (arrSameTopic[i].title.Contains(arrTrendKeyWord[l].keyword) && arrSameTopic[j].title.Contains(arrTrendKeyWord[l].keyword))
                                            {
                                                isTheSameTrendsKeyword = true;
                                                break;
                                            }
                                        }
                                        isTheSameTopic = isTheSameTopic || isTheSameTrendsKeyword;
                                    }

                                }
                                else isTheSameTopic = true;
                                isTheSameTopic = isTheSameTopic || arrSameTopic[i].title.ToLower().Equals(arrSameTopic[j].title.ToLower());
                                if (isTheSameTopic)
                                {
                                    arrSameTopic[i].topicid = arrSameTopic[i].id;
                                    arrSameTopic[j].topicid = arrSameTopic[i].id;
                                    found = true;
                                    SC.CommandText = "update tinviet_admin.titles set topicid=" + arrSameTopic[i].id + " where id=" + arrSameTopic[j].id;
                                    SC.ExecuteNonQuery();
                                }
                            }//for j
                        if (found)
                        {
                            SC.CommandText = "update tinviet_admin.titles set topicid=" + arrSameTopic[i].id + " where id=" + arrSameTopic[i].id;
                            SC.ExecuteNonQuery();
                        }
                        else
                        {
                            SC.CommandText = "update tinviet_admin.titles set topicid=null where id=" + arrSameTopic[i].id + " and topicid<>0";
                            SC.ExecuteNonQuery();
                        }
                    }//if
                }//for
                //countSameTopic = 0;
                //Set lại raking cho các tin có nhiều tin liên quan, chi xet rieng cac tin hom nay
                //SC.CommandText = "select topicid,count(*) as dem from tinviet_admin.titles where datetimeid>=" + today + " and topicid is not null group by topicid order by dem desc";
                //DR = SC.ExecuteReader();
                //while (DR.Read())
                //{
                //    //if (int.Parse(DR["dem"].ToString()) <= 2) continue;
                //    arrSameTopic[countSameTopic].id = int.Parse(DR["topicid"].ToString());
                //    arrSameTopic[countSameTopic].topicid = int.Parse(DR["dem"].ToString());//dùng tạm để đếm xem id này có bao nhiêu tin liên quan
                //    countSameTopic++;

                //}
                //DR.Close();
                //Array.Resize(ref arrSameTopic, countSameTopic);
                //for (var k = 0; k < countSameTopic; k++)
                //{
                //    try
                //    {
                //        int newranking = arrSameTopic[k].topicid * Config.BonusCountRelated;
                //        SC.CommandText = "update tinviet_admin.titles set ranking=" + newranking + ",fixRanking=" + newranking + " where id=" + arrSameTopic[k].id + " and ranking<" + newranking;
                //        SC.ExecuteNonQuery();
                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //}
                //Đối với những tin không lấy được nội dung thì giảm điểm
                try
                {
                    SC.CommandText = "update tinviet_admin.titles set ranking=1,fixRanking=1 where datetimeid>=" + yesterday + " and hasContent=0";
                    SC.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                }
                closeConn();
            }
            catch (Exception ex) { 
            }

        }
        //Đếm xem giữa hai tin có bao nhiêu từ khóa chung
        private int getLengthCommonKeyword(string A,string B) {
            int c = 0;
            string[] aA = A.Split(',');
            string[] aB = B.Split(',');
            for (int i = 0; i < aA.Length - 1; i++) {
                for (int j = 0; j < aB.Length - 1; j++)
                {
                    if (aA[i].Trim().Equals(aB[j].Trim())) {
                        c++;
                        break;
                    }
                }
            }
            return c;
        }
        private void crawlContentFromUrl(string domain)
        {
            int count = 0;
            int datetimeid = 20140212;
            arrTempTitle=new tempTitle[12000];
            openConn();
            SqlCommand SC = new SqlCommand();
            SC.Connection = con;
            SC.CommandText = "Select id,link from titles where datetimeid>=" + datetimeid + " and maindomain like '%" + domain + "%\' order by datetimeid desc,ranking desc";
            SqlDataReader DR = SC.ExecuteReader();
            while (DR.Read())
            {
                arrTempTitle[count].link=DR["link"].ToString();
                arrTempTitle[count].id=int.Parse(DR["id"].ToString());
                count++;
            }
            DR.Close();
            //string html;
            //string query="";
            //for(int i=0;i<count;i++){
            //    html=Rss.getContent(arrTempTitle[i].link);
            //    Regex titRegex = Config.getTitRegex();
            //        //new Regex(@"<div class=(.)fon34 mt3 mr2 fon43(.)>(?s).*news-tag", RegexOptions.IgnoreCase);
            //    Match titm = titRegex.Match(html);
            //    if (titm.Success)
            //    {
            //        html = titm.Value;
            //    }
            //    //html = html.Replace("<span class=\"news-tagname\">Xem thêm :</span>", string.Empty);
            //    query = "update titles set fullcontent=" + vN(html) + ",hasContent=1 where id=" + arrTempTitle[i].id;
            //    //try
            //    //{
            //        SC.CommandText = query;
            //        SC.ExecuteNonQuery();
            //    //}
            //    //catch (Exception ex1) { 

            //    //}
            //        CurrentBatchRun.getInfo = i+"/"+count;
            //}
            closeConn();
        }
        //Lay ra các tin chuyên mục
        private void generateCategoryNews()
        {
            try
            {
                openConn();
                SqlCommand SC = new SqlCommand();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                SC.Connection = con;
                for (int catid = 1; catid <= 13; catid++)
                {
                    try
                    {
                        CurrentBatchRun.getInfo = "Ghi ra 100 tin nóng nhất ra file xml của chuyên mục " + Uti.getCatNameFromId(catid);
                        showProcessCrawl();
                        string query = "select top 200 ";
                        query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,";
                        query += " ranking=A.ranking,link=A.link,";
                        query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
                        query += " uplikes=A.uplikes,downlikes=A.downlikes,";
                        query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff, ";
                        query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
                        query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
                        query += " (select top 100 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-4) + " and catid=" + catid + " and (topicid=id or topicid is null) order by  datetimeid desc, ranking desc, id desc) as A left join ";
                        query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated,catid as catidRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-4) + " and id=-1) as B on (A.topicid=B.topicidRelated and A.topicid=A.id and A.catid=B.catidRelated) ";//(A.id=B.topicidRelated and A.maindomain<>B.maindomainRelated) or (A.id=B.topicidRelated and A.id=A.topicid and A.maindomain=B.maindomainRelated) or (A.id=B.topicidRelated and A.datetimeid>B.datetimeid and A.maindomain=B.maindomainRelated) or 
                        query += " order by A.datetimeid desc,A.datetime desc,B.idRelated desc,B.rankingRelated desc";//A.ranking desc,

                        //SC = new SqlCommand();
                        SC.CommandText = query;
                        SqlDataReader DR = SC.ExecuteReader();
                        if (!DR.HasRows || DR == null)
                        {
                            DR.Close();
                            continue;
                        }
                        string xmlDoc = HttpRuntime.AppDomainAppPath + "Cat_" + catid + ".xml";
                        string des = "";
                        using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("channel");
                            while (DR.Read())
                            {
                                //string token = DR["tokenid"].ToString();
                                //Guid.NewGuid().ToString();
                                //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                                //writer.WriteAttributeString("id", "p3");
                                try
                                {
                                    writer.WriteStartElement("item");
                                    writer.WriteElementString("id", DR["id"].ToString());
                                    writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                                    try
                                    {
                                        des=System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                                        des = XmlConvert.VerifyXmlChars(des);
                                        string re = @"\u0008";
                                        des=Regex.Replace(des, re, ""); 
                                        writer.WriteElementString("des", des);
                                    }
                                    catch (Exception desex)
                                    {
                                        //writer.WriteElementString("des", "&nbsp;");
                                    }
                                    writer.WriteElementString("source", DR["source"].ToString());
                                    writer.WriteElementString("date", DR["date"].ToString());
                                    writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                                    //writer.WriteElementString("token", DR["token"].ToString());
                                    writer.WriteElementString("ranking", DR["ranking"].ToString());
                                    writer.WriteElementString("link", DR["link"].ToString());
                                    writer.WriteElementString("image", DR["image"].ToString());
                                    writer.WriteElementString("totalcomment", DR["totalcomment"].ToString());
                                    writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                                    writer.WriteElementString("uplikes", DR["uplikes"].ToString());
                                    writer.WriteElementString("downlikes", DR["downlikes"].ToString());
                                    writer.WriteElementString("topicid", DR["topicid"].ToString());
                                    writer.WriteElementString("catid", DR["catid"].ToString());
                                    writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                                    if (int.Parse(DR["timediff"].ToString()) >= 0)
                                    {
                                        writer.WriteElementString("timediff", DR["timediff"].ToString());
                                    }
                                    else
                                    {
                                        writer.WriteElementString("timediff", "6");
                                    }
                                    writer.WriteElementString("nameRelated", System.Web.HttpUtility.HtmlDecode(DR["nameRelated"].ToString()));
                                    writer.WriteElementString("topicidRelated", DR["topicidRelated"].ToString());
                                    writer.WriteElementString("idRelated", DR["idRelated"].ToString());
                                    writer.WriteElementString("linkRelated", DR["linkRelated"].ToString());
                                    writer.WriteElementString("maindomainRelated", DR["maindomainRelated"].ToString());
                                    writer.WriteElementString("rankingRelated", DR["rankingRelated"].ToString());
                                    writer.WriteElementString("hasContentRelated", DR["hasContentRelated"].ToString());
                                    writer.WriteElementString("imageRelated", DR["imageRelated"].ToString());
                                    writer.WriteEndElement();
                                }
                                catch (Exception itemcat) { 
                                }
                            }
                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                        }
                        DR.Close();
                    }
                    catch (Exception exCat) {
                        string e = exCat.ToString();
                    }
                }
                //Ghi ra 4 tin moi nhat moi chuyen muc
                for (int catid = 1; catid <= 13; catid++)
                {
                    try
                    {
                        CurrentBatchRun.getInfo = "Ghi ra 4 tin mới nhất ra file xml của chuyên mục " + Uti.getCatNameFromId(catid);
                        showProcessCrawl();
                        string query = "select top 4 id,catid,name as title,link,image,datetime,maindomain,hasContent from tinviet_admin.titles where catid=" + catid + " and hasContent=1 and (topicid=id or topicid is null) order by datetimeid desc,datetime desc";


                        //SC = new SqlCommand();
                        SC.CommandText = query;
                        SqlDataReader DR = SC.ExecuteReader();
                        if (!DR.HasRows || DR == null)
                        {
                            DR.Close();
                            continue;
                        }
                        string xmlDoc = HttpRuntime.AppDomainAppPath + "Cat_" + catid + "_New.xml";
                        using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("channel");
                            while (DR.Read())
                            {

                                writer.WriteStartElement("item");
                                writer.WriteElementString("id", DR["id"].ToString());
                                writer.WriteElementString("catid", DR["catid"].ToString());
                                writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                                writer.WriteElementString("link", DR["link"].ToString());
                                writer.WriteElementString("image", DR["image"].ToString());
                                writer.WriteElementString("datetime", DR["datetime"].ToString());
                                writer.WriteElementString("maindomain", DR["maindomain"].ToString());
                                writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                        }
                        DR.Close();
                    }
                    catch (Exception exCatNew) { 
                    }
                }//for
                closeConn();
            }
            catch (Exception ex) { 

            }
        }
        //Lay ra các xu huong doc
        private void generateTrends()
        {
            int datetimeid = Uti.datetimeid();// Uti.datetimeidByDay(-1);
            int datetimeid2 = Uti.datetimeidByDay(-1);
            openConn();
                
                CurrentBatchRun.getInfo = "Ghi ra xu huong doc Trends.xml";
                showProcessCrawl();
                string query = "";// "select top 200 A.title,A.keyword,A.rankKeyword,A.datetimeid,B.id,B.name,B.link,B.image,B.hasContent,B.maindomain,B.datetime,B.topicid from ";
                //query += "(select title,keyword,status,ranking as rankKeyword,datetimeid from tinviet_admin.dailykeyword where status=1 and datetimeid>=" + datetimeid + ") as A left join ";
                //query += "(select id,link,hasContent,name,ranking,datetime,datetimeid,maindomain,isNull(image,'') as image,topicid from tinviet_admin.titles where datetimeid>=" + datetimeid + ") as B on CHARINDEX(A.keyword,B.name)>0 ";
                //query += " order by A.keyword,A.datetimeid desc,A.rankKeyword desc,B.id desc";

                SqlCommand SC = new SqlCommand();
                SC.Connection = con;
                //SC.CommandText = query;
                SqlDataReader DR = null;// SC.ExecuteReader();
                //if (!DR.HasRows || DR == null)
                //{
                //    DR.Close();
                //    closeConn();
                //    return;
                //}
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                
                string xmlDoc = HttpRuntime.AppDomainAppPath + "Trends.xml";
                string preName = "";

                //using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                //{
                //    writer.WriteStartDocument();
                //    writer.WriteStartElement("channel");
                //    while (DR.Read())
                //    {
                //        if (preName.ToLower().Equals(DR["name"].ToString().ToLower()))
                //        {
                //            continue;
                //        }
                //        else {
                //            preName = DR["name"].ToString();
                //        }
                //        //string token = DR["tokenid"].ToString();
                //        //Guid.NewGuid().ToString();
                //        //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                //        //writer.WriteAttributeString("id", "p3");        
                //        writer.WriteStartElement("item");
                //        writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()).ToUpperInvariant());
                //        writer.WriteElementString("keyword", System.Web.HttpUtility.HtmlDecode(DR["keyword"].ToString()).ToUpperInvariant());                       
                //        //writer.WriteElementString("rankKeyword", DR["rankKeyword"].ToString());
                //        writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                //        writer.WriteElementString("id", DR["id"].ToString());
                //        writer.WriteElementString("name", System.Web.HttpUtility.HtmlDecode(DR["name"].ToString()));
                //        writer.WriteElementString("link", DR["link"].ToString());
                //        writer.WriteElementString("image", DR["image"].ToString());
                //        writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                //        writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                //        writer.WriteElementString("topicid", DR["topicid"].ToString());
                //        writer.WriteEndElement();
                //    }
                //    writer.WriteEndElement();
                //    writer.WriteEndDocument();
                //}
                //DR.Close();
            // Tin doc nhieu nhat

                query = "select top 10 ";
                query += " A.id,title=A.name,des=A.des,source=A.source,date=A.datetime,datetimeid=A.datetimeid,";
                query += " ranking=A.ranking,link=A.link,";
                query += " image=A.image,totalcomment=A.totalcomment,maindomain=A.maindomain,";
                query += " uplikes=A.uplikes,downlikes=A.downlikes,";
                query += " topicid=A.topicid,catid=A.catid,hasContent=A.hasContent,datediff(minute,A.datetime,GETDATE()) as timediff,totalviews=A.totalviews, ";
                query += " nameRelated=B.nameRelated,topicidRelated=B.topicidRelated,idRelated=B.idRelated,";
                query += " linkRelated=B.linkRelated,maindomainRelated=B.maindomainRelated,rankingRelated=B.rankingRelated,hasContentRelated=B.hasContentRelated,imageRelated=B.imageRelated from ";
                query += " (select top 10 name,des,datetimeid,datetime,source,ranking,topicid,catid,id,link,image,totalcomment,maindomain,uplikes,downlikes,hasContent,totalviews from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and totalviews>0 order by  datetimeid desc,totalviews desc) as A left join ";
                query += " (select datetimeid,name as nameRelated,topicid as topicidRelated,id as idRelated,link as linkRelated,maindomain as maindomainRelated,ranking as rankingRelated,hasContent as hasContentRelated,image as imageRelated from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and catid<>0 and id=-1) as B on (A.topicid=B.topicidRelated and A.topicid=A.id) ";
                query += " order by A.datetimeid desc,A.totalviews desc";


                //SC.Connection = con;
                SC.CommandText = query;
                DR = SC.ExecuteReader();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                xmlDoc = HttpRuntime.AppDomainAppPath + "TopReadNew.xml";
                string des = "";
                using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("channel");
                    while (DR.Read())
                    {
                        //string token = DR["tokenid"].ToString();
                        //Guid.NewGuid().ToString();
                        //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                        //writer.WriteAttributeString("id", "p3");     
                        try
                        {
                            writer.WriteStartElement("item");
                            writer.WriteElementString("id", DR["id"].ToString());
                            writer.WriteElementString("title", System.Web.HttpUtility.HtmlDecode(DR["title"].ToString()));
                            try
                            {
                                des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                                des = XmlConvert.VerifyXmlChars(des);
                                string re = @"\u0008";
                                des = Regex.Replace(des, re, "");
                                writer.WriteElementString("des", des);
                            }
                            catch (Exception desex)
                            {
                                //writer.WriteElementString("des", "&nbsp;");
                            }
                            writer.WriteElementString("source", DR["source"].ToString());
                            writer.WriteElementString("date", DR["date"].ToString());
                            writer.WriteElementString("datetimeid", DR["datetimeid"].ToString());
                            //writer.WriteElementString("token", DR["token"].ToString());
                            writer.WriteElementString("ranking", DR["ranking"].ToString());
                            writer.WriteElementString("link", DR["link"].ToString());
                            writer.WriteElementString("image", DR["image"].ToString());
                            writer.WriteElementString("totalcomment", DR["totalcomment"].ToString());
                            writer.WriteElementString("maindomain", Uti.smDomain(DR["maindomain"].ToString()));
                            writer.WriteElementString("uplikes", DR["uplikes"].ToString());
                            writer.WriteElementString("downlikes", DR["downlikes"].ToString());
                            writer.WriteElementString("topicid", DR["topicid"].ToString());
                            writer.WriteElementString("catid", DR["catid"].ToString());
                            writer.WriteElementString("hasContent", DR["hasContent"].ToString());
                            if (int.Parse(DR["timediff"].ToString()) >= 0)
                            {
                                writer.WriteElementString("timediff", DR["timediff"].ToString());
                            }
                            else
                            {
                                writer.WriteElementString("timediff", "6");
                            }
                            writer.WriteElementString("nameRelated", System.Web.HttpUtility.HtmlDecode(DR["nameRelated"].ToString()));
                            writer.WriteElementString("topicidRelated", DR["topicidRelated"].ToString());
                            writer.WriteElementString("idRelated", DR["idRelated"].ToString());
                            writer.WriteElementString("linkRelated", DR["linkRelated"].ToString());
                            writer.WriteElementString("maindomainRelated", DR["maindomainRelated"].ToString());
                            writer.WriteElementString("rankingRelated", DR["rankingRelated"].ToString());
                            writer.WriteElementString("hasContentRelated", DR["hasContentRelated"].ToString());
                            writer.WriteElementString("imageRelated", DR["imageRelated"].ToString());
                            writer.WriteEndElement();
                        }
                        catch (Exception ex3)
                        {

                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                DR.Close();
            
            closeConn();
        }
        //Viết ra từ khóa nóng hàng ngày daily hot keyword
        //chèn vào bảng dailykeyword, dựa vào đó để nhóm các tin cùng chủ đề với nhau dựa trên từ khóa
        
        private void generateDailyHotKeyword(){
            Hashtable dailyHotKeyword = new Hashtable();
            
            try
            {
                int datetimeid = Uti.datetimeid();
                arrSameTopic = new sameTopic[5000];
                countSameTopic = 0;
                CurrentBatchRun.getInfo = "Tự động phân tích và lấy ra các danh từ riêng viết hoa";
                showProcessCrawl();
                openConn();
                SqlCommand SC = new SqlCommand();
                SC.Connection = con;
                SC.CommandText = "Select id,keyword,name,des,datetimeid from tinviet_admin.titles where datetimeid>=" + datetimeid + "  order by ranking desc,datetime desc";
                SqlDataReader DR = SC.ExecuteReader();
                while (DR.Read())
                {
                    if (countSameTopic >= arrSameTopic.Length) break;
                    arrSameTopic[countSameTopic].id = int.Parse(DR["id"].ToString());
                    arrSameTopic[countSameTopic].keyword = System.Web.HttpUtility.HtmlDecode(DR["keyword"].ToString());
                    arrSameTopic[countSameTopic].des = System.Web.HttpUtility.HtmlDecode(DR["des"].ToString());
                    arrSameTopic[countSameTopic].title = System.Web.HttpUtility.HtmlDecode(DR["name"].ToString());
                    arrSameTopic[countSameTopic].topicid = 0;//Lúc đầu chưa set vào đâu cả
                    arrSameTopic[countSameTopic].datetimeid = int.Parse(DR["datetimeid"].ToString());
                    countSameTopic++;

                }
                DR.Close();
                Array.Resize(ref arrSameTopic, countSameTopic);
                SC.CommandText = "delete from tinviet_admin.dailykeyword where status=0";
                SC.ExecuteNonQuery();
                string temp = "";
                for (int i = 0; i < countSameTopic; i++)
                {
                    if (arrSameTopic[i].title != null && arrSameTopic[i].title.Length > 0)
                    {
                        //Lấy ra các danh từ riêng, viết hoa
                        string[] tempKw = Uti.getNounUpcaseWord(arrSameTopic[i].title).Split(',');// + " " + arrSameTopic[i].des
                        for (int j = 0; j < tempKw.Length; j++)
                        {
                            temp=tempKw[j].Trim();
                            if (temp != "" && temp.Length > 4)
                            {
                                if (dailyHotKeyword.ContainsKey(temp))
                                {
                                    int count = int.Parse(dailyHotKeyword[temp].ToString());
                                    count++;
                                    dailyHotKeyword.Remove(temp.Trim());
                                    dailyHotKeyword.Add(temp, count);
                                }
                                else dailyHotKeyword.Add(temp, 1);
                            }
                        }
                        //Lấy ra các từ khóa chung có độ dài lớn hơn 3
                        tempKw = arrSameTopic[i].keyword.Split(',');// + " " + arrSameTopic[i].des
                        for (int j = 0; j < tempKw.Length; j++)
                        {
                            temp = tempKw[j].Trim();
                            if (temp != "" && Rss.realLength(temp) >= 3)
                            {
                                if (dailyHotKeyword.ContainsKey(temp))
                                {
                                    int count = int.Parse(dailyHotKeyword[temp].ToString());
                                    count++;
                                    dailyHotKeyword.Remove(temp);
                                    dailyHotKeyword.Add(temp, count);
                                }
                                else dailyHotKeyword.Add(temp, 1);
                            }
                        }

                    }
                }
                
                string query = "";
                
                // sau khi da co keyword va so luong thi chen vao database
                // Đây là bảng chứa keyword định hướng của trang, có editor biên tập riêng
                foreach (DictionaryEntry entry in dailyHotKeyword)
                {
                    //if (int.Parse(entry.Value.ToString()) < 100) continue;
                    query = "IF EXISTS (SELECT * FROM tinviet_admin.dailykeyword WHERE keyword=" + vN(entry.Key.ToString()) + " and datetimeid=" + Uti.datetimeid() + ")";
                    query += " UPDATE tinviet_admin.dailykeyword SET ranking=ranking+" + entry.Value + " WHERE keyword=" + vN(entry.Key.ToString()) + " and datetimeid=" + Uti.datetimeid();
                    query += " ELSE ";
                    query += "INSERT INTO tinviet_admin.dailykeyword(keyword,length,ranking,status,datetime,datetimeid) VALUES(" + vN(entry.Key.ToString()) + "," + entry.Key.ToString().Length + "," + entry.Value + ",0," + vN(DateTime.Now.ToString()) + "," + Uti.datetimeid().ToString() + ")";
                    try{
                        SC.CommandText = query;
                        SC.ExecuteNonQuery();
                    }
                    catch (Exception ex1) {
                        
                    }                    
                }
                //Kiểm tra xem từ khóa này có phải là xu hướng đọc không?
                try
                {
                    int datetimeid2 = Uti.datetimeid();
                    string[] mainDailyKeyword = new string[30];
                    int lengMainDailyKeyword = 0;
                    string query2 = "select top 20 A.id,A.title,A.keyword,A.status,A.datetime,A.datetimeid, count(B.idB) as ranking from ";
                    query2 += " (select id,title,keyword,status,ranking as rankKeyword,datetime,datetimeid from tinviet_admin.dailykeyword where  datetimeid>=" + datetimeid2 + " and ranking>=3) as A left join ";
                    query2 += " (select id as idB,name,datetimeid  from tinviet_admin.titles where datetimeid>=" + datetimeid2 + ") as B on CHARINDEX(A.keyword,B.name)>0 ";
                    query2 += " group by A.id,A.title,A.keyword,A.status,A.datetime,A.datetimeid order by datetimeid desc,ranking desc";
                    SC.CommandText = query2;//"Select top 20 * from tinviet_admin.dailykeyword where datetimeid>=" + datetimeid + " and ranking>="+Config.MinPointRankingDailyKeyword+" order by ranking desc,datetime desc";
                    DR = SC.ExecuteReader();
                    lengMainDailyKeyword = 0;
                    while (DR.Read())
                    {
                        if (int.Parse(DR["ranking"].ToString()) >= Config.MinPointRankingDailyKeyword)
                        {
                            mainDailyKeyword[lengMainDailyKeyword] = HttpUtility.HtmlDecode(DR["keyword"].ToString());
                            lengMainDailyKeyword++;
                        }

                    }
                    DR.Close();
                    for (int k = 0; k < lengMainDailyKeyword;k++)
                    {
                        string word = mainDailyKeyword[k].Trim();
                        bool found = false;
                        if (Uti.isDailyKeyword(ref word))
                        {
                            string tempword = " " + word + " ";
                            for (int l = 0; l < k - 1; l++)
                            {
                                if (tempword.Contains(" " + mainDailyKeyword[l] + " "))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                if (!word.Equals(mainDailyKeyword[k]))
                                {
                                    query = " UPDATE tinviet_admin.dailykeyword SET status=1,title=" + vN(word) + " WHERE keyword=" + vN(mainDailyKeyword[k]) + " and datetimeid>=" + Uti.datetimeid();
                                }
                                else {
                                    query = " UPDATE tinviet_admin.dailykeyword SET status=1 WHERE keyword=" + vN(mainDailyKeyword[k]) + " and datetimeid>=" + Uti.datetimeid();
                                }
                                SC.CommandText = query;
                                SC.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {

                }
                SC.CommandText = "delete from tinviet_admin.dailykeyword where datetimeid<" + Uti.datetimeidByDay(-1);
                SC.ExecuteNonQuery();
                //SC.CommandText = "delete from dailykeyword where ranking<100";
                //SC.ExecuteNonQuery();
                closeConn();
            }
            catch (Exception ex) {
                closeConn();
            }
        }
        
       
        
        private void SetBatchParameterTableVisibility(bool visible)
        {
            this.tblBatchParameters.Visible = visible;
            this.tblBatchProgress.Visible = !visible;
        }

        private bool CurrentBatchInProgress()
        {
            var batchRun = CurrentBatchRun;
            if (batchRun == null || batchRun.HasNotBegun)
            {
                return false;
            }
            return !batchRun.IsCompletedOrExpired;
        }
        
        
        private void showProcessCrawl() {
            currentBatch = CurrentBatchRun;
            if (currentBatch != null && currentBatch.HasNotBegun)
            {
                return;
            }
            if (currentBatch == null)
            {
                SetBatchParameterTableVisibility(true);
                return;
            }
            if (currentBatch.IsCompletedOrExpired)
            {
                if (currentBatch.ShouldStop)
                {
                    //ShowAlert(String.Format("Statement Run Completed at {0}, but was cancelled.", currentBatch.LastUpdatedTime));
                    this.lblInfo.Text = "Statement Run Completed at "+currentBatch.LastUpdatedTime+", but was cancelled.<br>";
                }
                else
                {
                    //ShowAlert(String.Format("Statement Run Completed at {0}.", currentBatch.LastUpdatedTime));
                    this.lblInfo.Text = "Statement Run Completed at " + currentBatch.LastUpdatedTime + ".<br>";
                }
                //timerBatchRun.Enabled = false;
                SetBatchParameterTableVisibility(true);
            }
            else
            {
                this.lblPercentage.Text = currentBatch.PercentDone.ToString();
                this.lblTimeRemaining.Text = String.Format("{0:f2}", currentBatch.EstimatedTimeRemaining.TotalMinutes);
                this.lblInfo.Text = currentBatch.getInfo + "<br>";
                if (this.lblInfo.Text.Contains("Đã xong và chạy lại")) this.lblInfo.Text = "";
                //var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.ync>();
                //context.Clients.All.addNewAll("xahoi", "Xin chao cac ban ok");  
            }
        }
        private void ShowAlert(string alert)
        {
            if (currentBatch != null) currentBatch.getInfo = alert;
            //Response.Flush();
            showProcessCrawl();

        }
        protected void btnStop_Click(object sender, EventArgs e)
        {
            ShowAlert("Batch Run Stopped");
            if (CurrentBatchRun != null)
            {
                CurrentBatchRun.ShouldStop = true;
                //this.timerBatchRun.Enabled = false;
                SetBatchParameterTableVisibility(true);
            }
        }
    }
}