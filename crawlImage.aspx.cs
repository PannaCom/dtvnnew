using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
//using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using HtmlAgilityPack;
namespace youknow
{
    public partial class crawlImage : System.Web.UI.Page
    {
        struct img
        {
            public string id;
            public string link;
            public string image;
        }
        img[] arrImg = new img[5000];
        int Length = 0;
        SqlConnection con = null;
        protected RunbatchImage CurrentBatchRunImage
        {
            get { return (RunbatchImage)this.Application["CurrentBatchRunImage"]; }
            set { this.Application["CurrentBatchRunImage"] = value; }
        }
        //IHubContext context = GlobalHost.ConnectionManager.GetHubContext<Hubs.ync>();
        RunbatchImage currentBatch;
        protected void Page_Load(object sender, EventArgs e)
        {
            string content = Rss.getContent("http://feedly.com/#subscription%2Ffeed%2Fhttp%3A%2F%2Ffeeds.reuters.com%2Freuters%2FbusinessNews");

            if (!IsPostBack)
            {
                //Thời gian bắt đầu và kết thúc Crawl, hiện thời gian này chưa chuẩn
                //Mặc dù để 1000 năm nó vẫn bị ngừng, chưa điều tra ra
                this.txtStartDate.Text = DateTime.Today.AddMonths(-1).ToShortDateString();
                this.txtEndDate.Text = DateTime.Today.AddYears(1000).ToShortDateString();
            }
            Response.Write(DateTime.Now.ToUniversalTime().ToString() + "<br>");
            Response.Write(DateTime.Now.ToString());
        }
        //Bắt đầu chạy Crawl
        protected void btnRunBatch_Click(object sender, EventArgs e)
        {
            SetBatchParameterTableVisibility(false);

            if (!CurrentBatchInProgress())
            {
                CurrentBatchRunImage = new RunbatchImage();

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
            timerBatchRun.Enabled = true;
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

            CurrentBatchRunImage.TotalNumberOfItems = i;
            CurrentBatchRunImage.Start();
            CurrentBatchRunImage.getInfo = "Tong so vong lap la" + CurrentBatchRunImage.TotalNumberOfItems;
            tempDate = startDate;
            while (tempDate < endDate)
            {
                //Hàm này chạy liên tục để crawl tin tức
                CurrentBatchRunImage.getInfo = "Dang chay vong lap " + CurrentBatchRunImage.ItemsCompleted + "/" + CurrentBatchRunImage.TotalNumberOfItems;
                DoSomething(new Random().Next(1, 5));
                tempDate = tempDate.AddHours(1);
                if (CurrentBatchRunImage == null || CurrentBatchRunImage.ShouldStop)
                {
                    break;
                }
                CurrentBatchRunImage.IncrementItemsCompleted();
            }
        }

        private string vN(string n)
        {
            return "N'" + System.Web.HttpUtility.HtmlEncode(n) + "'";
        }
        private string removeSpecialChar(string input)
        {
            input = input.Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "");
            return input;
        }
        //Lấy ra các tin ở Data, Ghi ra SiteMap.xml
        private void generateSiteMap()
        {
            try
            {
                CurrentBatchRunImage.getInfo = "Ghi ra siteMap";
                //showProcessCrawl();
                openConn();
                SqlCommand SC = new SqlCommand();
                SqlDataReader DR = null;
                XmlWriterSettings settings = null;
                string xmlDoc = null;
                string query = "select top 300 ";
                query = " select * from tinviet_admin.titles where datetimeid>=" + Uti.datetimeidByDay(-1) + " and hasContent=1 order by datetimeid desc,ranking desc";


                SC.Connection = con;
                SC.CommandText = query;
                DR = SC.ExecuteReader();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";
                float percent = 0.85f;
                int datetimeid = Uti.datetimeid();
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

                    while (DR.Read())
                    {
                        //string token = DR["tokenid"].ToString();
                        //Guid.NewGuid().ToString();
                        //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                        //writer.WriteAttributeString("id", "p3"); 
                        try
                        {
                            writer.WriteStartElement("url");
                            writer.WriteElementString("loc", "http://diemtinvietnam.vn/"+Uti.unicodeToNoMark(Server.HtmlDecode(DR["name"].ToString()))+"-" + DR["id"].ToString());
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try{
                                if (Uti.datetimeid()-int.Parse(DR["datetimeid"].ToString())<=1){
                                    writer.WriteElementString("changefreq", "hourly");
                                }else{
                                    writer.WriteElementString("changefreq", "monthly");
                                }
                            }catch(Exception ex1){
                            }
                            try
                            {
                                percent = 0.85f;
                                //percent = percent + (int.Parse(DR["datetimeid"].ToString()) - datetimeid) * 0.01f;
                            }
                            catch (Exception ex)
                            {
                            }
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
            catch (Exception extry) {
                CurrentBatchRunImage.getInfo = "Ghi ra siteMap, bi loi " + extry.ToString();
            }
        }
        private void openConn()
        {
            try
            {
                if (con == null || (con != null && con.State.ToString() == "Closed"))
                {
                    if (con == null)
                    {
                        con = new SqlConnection();
                        con.ConnectionString = "Server=that.vn;Database=tinviet_vietnam;User Id=sa;Password=Tvn!@#456;";

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
            catch (Exception ex)
            {
                //CurrentBatchRun.getInfo = "Open Sql not OK" + ex.ToString();
            }
        }
        private void closeConn()
        {
            if (con != null && con.State.ToString() == "Open")
            {
                con.Close();
                //con = null;
            }
        }
        //Hàm này chạy liên tục để crawl tin tức
        private void DoSomething(int seconds)
        {
            DateTime nowDate = DateTime.Now;
            openConn();
            SqlCommand SC = new SqlCommand();
            SC.Connection = con;
            //SqlDataReader DR = null;
            Thread.Sleep(seconds * 5000);
            try
            {
               
                CurrentBatchRunImage.getInfo = "Bắt đầu lấy ảnh";
                generateSiteMap();
                return;
                //SC.CommandText = "select top 5000 id,link,image from tinviet_admin.titles where datetimeid>=" + Uti.datetimeid() + " and (image='' or image is Null)";
                //DR = SC.ExecuteReader();
                //Length = 0;
                //while (DR.Read())
                //{
                //    arrImg[Length].id = DR["ID"].ToString();
                //    arrImg[Length].link = DR["link"].ToString();
                //    arrImg[Length].image = DR["image"].ToString();
                //    Length++;
                //}
                //DR.Close();
                //Array.Resize(ref arrImg, Length);
                //for (int i = 0; i < Length; i++)
                //{
                //    arrImg[i].image = Rss.getImageFromLink(arrImg[i].link);
                //    CurrentBatchRunImage.getInfo = i + "/" + Length + ", Lấy ảnh xong của link=" + arrImg[i].link + ", image=" + arrImg[i].image;
                //    if (arrImg[i].image != "")
                //    {
                //        SC.CommandText = "update tinviet_admin.titles set image=" + vN(arrImg[i].image) + " where id=" + arrImg[i].id;
                //        SC.ExecuteNonQuery();
                //    }
                //}
                //closeConn();
                CurrentBatchRunImage.getInfo = "Bắt đầu ghi ra danh bạ Việt Nam";
                for (int i = 0; i <= 20; i++)
                {
                    HtmlWeb hw = new HtmlWeb();
                    HtmlDocument doc = hw.Load("http://www.alexa.com/topsites/countries;"+i+"/VN");
                    string site = "";
                    string keyword = "";
                    string des = "";
                    string rank = "";
                    string content = "";
                    Regex titRegex;
                    Match titm;
                    foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//span[contains(@class,'inner-2col tb')]"))
                    {
                        foreach (HtmlNode link2 in link.SelectNodes(".//li[contains(@class,'site-listing')]"))
                        {
                            site = "";
                            keyword = "";
                            des = "";
                            rank = "";
                            content = "";
                            site = link2.SelectSingleNode(".//p[contains(@class,'desc-paragraph')]").InnerText.Replace("\n","");
                            if (!site.StartsWith("http://")) site = "http://" + site.ToLowerInvariant();
                            try
                            {
                                des = link2.SelectSingleNode(".//div[contains(@class,'description')]").InnerText.Replace("&nbsp;", "").Replace("... More","");
                            }
                            catch (Exception exin) { 
                            }
                            keyword = "";
                            rank = link2.SelectSingleNode(".//div[contains(@class,'count')]").InnerText;
                            if (des.Trim().Equals("")) {
                                content = getContent(site);
                                titRegex = new Regex("<meta(.*?)name=\"description\"(.*?)content=\"(.*?)\"(.*?)>", RegexOptions.IgnoreCase);
                                titm = titRegex.Match(content);
                                if (titm.Success)
                                {
                                    content = titm.Value;
                                    titRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
                                    titm = titRegex.Match(content);
                                    des = titm.Value.Replace("content=", "").Replace("\"", "");                                    
                                }
                                titRegex = new Regex("<meta(.*?)name=\"keywords\"(.*?)content=\"(.*?)\"(.*?)>", RegexOptions.IgnoreCase);
                                titm = titRegex.Match(content);
                                if (titm.Success)
                                {
                                    content = titm.Value;
                                    titRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
                                    titm = titRegex.Match(content);
                                    keyword = titm.Value.Replace("content=", "").Replace("\"", "");
                                }
                            }
                            if (des.ToLowerInvariant().Contains("sex")) continue;
                            if (isVn(des))
                            {
                                string query = "";
                                query = "if exists(select * from zalexa where site='" + site + "')";
                                query += " begin ";
                                query += "    update zalexa set rank=" + rank + ",des=N'" + des + "',keyword=N'" + keyword + "' where site='" + site + "'";
                                query += "  ";
                                query += " end ";
                                query += " else ";
                                query += " begin ";
                                query += "    insert into zalexa(site,keyword,des,rank) values(N'" + site + "',N'" + keyword + "',N'" + des + "'," + rank + ")";
                                query += "end ";
                                try
                                {
                                    SC.CommandText = query;
                                    SC.ExecuteNonQuery();
                                }
                                catch (Exception exquery)
                                {

                                }
                            }
                        }//for element 2
                    }//for element 1
                }//for i
                //CurrentBatchRunImage.getInfo = content;
                closeConn();
                generateSiteMap();
                CurrentBatchRunImage.getInfo = "Đã xong, Bắt đầu lại";
            }
            catch (Exception ex)
            {
                //DR.Close();
                closeConn();
                CurrentBatchRunImage.getInfo = "Do SomeThing error.." + ex.ToString();

            }
        }
        public static bool isVn(string content) {
            string[] code = new string[] { "á", "ả", "ã", "â", "ă", "à", "ạ", "ú", "ủ", "ũ", "ù", "ụ", "é", "ẻ", "ẽ", "ê", "è", "ẹ", "í", "ỉ", "ĩ", "ì", "ị", "ó", "ỏ", "õ", "ô", "ơ", "ò", "ọ", "ấ", "ẩ", "ẫ", "ầ", "ậ", "ắ", "ẳ", "ẵ", "ằ", "ặ", "ế", "ể", "ễ", "ề", "ệ", "ố", "ổ", "ỗ", "ồ", "ộ", "ớ", "ở", "ỡ", "ờ", "ợ", "ú", "ủ", "ũ", "ù", "ụ", "ý", "ỷ", "ỹ", "ỳ", "ỵ" };
            int count = 0;
            for(int i=0;i<code.Length;i++){
                if (content.ToLowerInvariant().Contains(code[i])) {
                    count++;
                    if (count>=2) return true;
                }
            }
            return false;
        }
        public static String getContent(String url)
        {
            String htmlCode = "";
            Random r = new Random();
            string[] myAngent = {"Chrome /36.0.1944.0",
                       "Chrome /35.0.1916.47",
                       "Firefox/29.0",                                              
                       "Chrome/32.0.1667.0",
                       "Chrome/32.0.1664.3",
                       "Chrome/28.0.1467.0",
                       "Chrome/28.0.1467.0",
                       "Firefox /31.0"};
            string[] arrProxy ={"177.47.94.66:8080",
                       "202.4.104.156:8080",
                       "190.221.160.190:3127",                                              
                       "27.147.141.194",
                       "118.97.158.26:8080",
                       "186.233.168.254:8080",
                       "79.143.188.37:3128",
                       "221.182.62.115:9999"};
            string[] arrUrl ={"http://www.kproxy.com/servlet/redirect.srv/ssb/sujxeqvjcykexcy/p1/Home/HotNews",
                              "http://www.free-web-proxy.de/@.php?u=3GZiH3A0g4E1TWkMR%2FMDlOAt%2FCKJSJhnMm5G%2BXP9FT7a&b=6",
                              "http://onlineproxyfree.com/index.php?q=aHR0cDovL2RpZW10aW52aWV0bmFtLnZuL0hvbWUvSG90TmV3cw%3D%3D",
                              "https://webproxy.vpnbook.com/browse.php?u=0FBEeZB1sMz2bWBcOG7WCpvJ5YQSW5bA7kTBvIe8FXlk&b=0"
                            };
            try
            {
                bool conti = true;
                byte stry = 0;
                WebResponse myResponse;
                StreamReader sr;
                do
                {
                    try
                    {
                        Random random = new Random();
                        int randomNumber = random.Next(0, myAngent.Length);
                        conti = false;
                        int randomUrl=random.Next(0,arrUrl.Length);
                        //string proxyAddress = arrProxy[random.Next(0, arrProxy.Length)];
                        //Uri newUri = new Uri(proxyAddress);
                        // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                        //myProxy.Address = newUri;
                        //WebProxy myProxy = new WebProxy(proxyAddress);
                        //url=arrUrl[randomUrl];
                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                        myRequest.Method = "GET";
                        myRequest.Timeout = 15000;
                        myRequest.UserAgent = myAngent[randomNumber];
                        //myRequest.Proxy = myProxy;
                        //myRequest.TransferEncoding = "UTF-8";
                        myResponse = myRequest.GetResponse();
                        sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);//.UTF8
                        htmlCode = sr.ReadToEnd();
                        sr.Close();
                        myResponse.Close();

                    }
                    catch (Exception err1)
                    {
                        conti = true;
                        stry++;
                    }
                } while (conti && stry < 3);
            }
            catch (Exception err)
            {
                return "";
            }

            return htmlCode;
        }
        private void SetBatchParameterTableVisibility(bool visible)
        {
            this.tblBatchParameters.Visible = visible;
            this.tblBatchProgress.Visible = !visible;
        }

        private bool CurrentBatchInProgress()
        {
            var RunbatchImage = CurrentBatchRunImage;
            if (RunbatchImage == null || RunbatchImage.HasNotBegun)
            {
                return false;
            }
            return !RunbatchImage.IsCompletedOrExpired;
        }

        protected void timerBatchRun_Tick(object sender, EventArgs e)
        {
            currentBatch = CurrentBatchRunImage;
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
                    ShowAlert(String.Format("Statement Run Completed at {0}, but was cancelled.", currentBatch.LastUpdatedTime));
                }
                else
                {
                    ShowAlert(String.Format("Statement Run Completed at {0}.", currentBatch.LastUpdatedTime));
                }
                timerBatchRun.Enabled = false;
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
            Response.Write("<script>alert('" + alert + "')</script>");
        }
        protected void btnStop_Click(object sender, EventArgs e)
        {
            ShowAlert("Batch Run Stopped");
            if (CurrentBatchRunImage != null)
            {
                CurrentBatchRunImage.ShouldStop = true;
                this.timerBatchRun.Enabled = false;
                SetBatchParameterTableVisibility(true);
            }
        }
    }
}