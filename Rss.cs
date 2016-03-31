using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;
using System.IO;
using youknow.Views;
using HtmlAgilityPack;
namespace youknow
{
    class Rss
    {
        public string Url;
        public int catid;
        public int domain;
        public string maindomain;
        public static int _Max_Topic_KeyWord_Length_ = 4;
        
        public struct ItemXml
        {
            public long id;//id của tin đó
            public string title;//Tên của tin tức
            public string link;//link của tin
            public string des;//tóm tắt tin
            public string date;//ngày đăng tin
            public string image;//ảnh của tin
            public int catid;//tin thuộc chuyên mục nào
            public int domain;//tin lấy từ domain nào?
            public string maindomain;//domain chính của tin này là gì? lấy ở đâu.
            public string token;//Mã hóa link của tin,base64
            public bool show;//
            public string source;//Nguồn lấy tin
            public int ranking;//Điểm của tin 
            public string keyword;//Các từ khóa của tin
            public int datetimeid;//Convert datetime sang dạng integer để dễ sắp xếp.
            public int totalcomment;//Tổng số comment của tin
            public int topicid;//Tin thuộc chủ đề nào.
            public string fullContent;//Nội dung tin
            public int hasContent;//Nội dung tin
            public string timediff;//Khoang cach thoi gian
        }
        
        public ItemXml[] arrItem = null;
        public viewNewsManager[] arrNewsManager = null;
        public viewNewsDomainCat[] arrNewsDomainCat = null;
        public viewNewsMobileAppManager[] arrNewsMobileAppManager = null;
        public viewTrendsManager[] arrTrendsManager = null;
        public int Length = 0;
        public viewCatNewsLatestManager[] arrCatNewsLatestManager = null;
        public int LengthCatNewsLatest=0;
        
              
        public Rss(string url,int domain,string maindomain,int catid,int type)
        {
            this.Url = url;
            this.catid = catid;
            this.domain = domain;
            this.maindomain = maindomain;
            if (!this.maindomain.StartsWith("http://")) {
                this.maindomain = "http://" + this.maindomain;
            }
            Length = 0;
            if (type == 1)
            {               
                arrItem = new ItemXml[1000];
                getAllItem(url);
            }
            else if (type == 2)
            {
                arrItem = new ItemXml[2000];
                readXml(url);
            }
            else if (type == 3)
            {
                arrItem = new ItemXml[1000];
                readTopNewsXml(url);
            }
            else if (type == 4)
            {
                arrItem = new ItemXml[1000];
                readTopCommentXml(url);
            }
            else if (type == 5)
            {
                arrItem = new ItemXml[1000];
                readTopNewWeekly(url);
            }
            else if (type == 6)
            {
                arrItem = new ItemXml[1000];
                readNewestNew(url);
            }
            else if (type == 7) readNewsManager(url);
            else if (type == 8) readTrends(url);
            else if (type == 9) readNewsMobileAppManager(url);
            else if (type == 10) readNewsDomainCat(url);
            else if (type == 11) {
                arrItem = new ItemXml[10];
                getHotNewsHomePage(url);
            }
        }
        
        public void Clear()
        {
            arrItem = null;
            Length = 0;
        }
        public static string getImageFromLink(string link)
        {
            //string matchString = Regex.Match(content, "<img.*?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;

            //return matchString;
            //string matchString = Regex.Match(content, "<img.*?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;
            string content = getContent(link);
            Regex titRegex;
            Match titm;
            content = content.ToLower();
           
                titRegex = new Regex("<meta(.*?)property=\"og:image\"(.*?)content=\"(.*?)\"(.*?)/>", RegexOptions.IgnoreCase);
                titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Value;
                    titRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
                    titm = titRegex.Match(content);
                    return titm.Value.Replace("content=", "").Replace("\"", "");
                }
            

            titRegex = new Regex("<meta(.*?)content=\"(.*?).jpg\"(.*?)>", RegexOptions.IgnoreCase);
            titm = titRegex.Match(content);
            if (titm.Success)
            {
                content = titm.Value;
                titRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
                titm = titRegex.Match(content);
                return titm.Value.Replace("content=", "");
            }
            else
            {
                titRegex = new Regex("<meta(.*?)content=\"(.*?).gif\"(.*?)>", RegexOptions.IgnoreCase);
                titm = titRegex.Match(content);
                if (titm.Success)
                {
                    content = titm.Value;
                    titRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
                    titm = titRegex.Match(content);
                    return titm.Value.Replace("content=", "");
                }
                else
                {
                    titRegex = new Regex("<meta(.*?)content=\"(.*?).png\"(.*?)>", RegexOptions.IgnoreCase);
                    titm = titRegex.Match(content);
                    if (titm.Success)
                    {
                        content = titm.Value;
                        titRegex = new Regex("content=\"(.*?)\"", RegexOptions.IgnoreCase);
                        titm = titRegex.Match(content);
                        return titm.Value.Replace("content=", "");
                    }
                }
            }
            return "";
        }
        public string getImageSrc(string content)
        {
            string matchString = Regex.Match(content, "<img.*?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;

            return matchString;
        }
        public String removeHtmlTag(String s)
        {
            return Regex.Replace(s, "\\<[^>]*>", "");
            //s = s.Replace("\\<[^>]*>", "");

        }
        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”","");
            return input;
        }
        public string removeBr(string s)
        {
            s = s.Replace("\r\n", " ");
            s = s.Replace("  ", " ");
            s = s.Trim();
            return s;
        }
        
        //Doc Rss
        private void readNewsDomainCat(string Url)
        {
            string nowDate = DateTime.Now.ToString();
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            //long oldTime = DateTime.Now.Ticks;
            //String HtmlContent = getContent(Url);
            //long newTime = DateTime.Now.Ticks;
            //long diff1=0,diff2 = 0;
            //diff1 = newTime - oldTime;
            Length = 0;
            arrNewsDomainCat = new viewNewsDomainCat[200];
            if (Url.Contains("zing.vn"))
            {
                getZingLink(Url, 1);
                return;
            }
            if (Url.Contains("http://tuoitre.vn"))
            {
                getTuoitreLink(Url, 1);
                return;
            }
            //if (Url.Contains("tinmoi.vn"))
            //{
            //    int abc = 0;
            //}
            //if (Url.Contains("nld.com.vn"))
            //{
            //    int abc2 = 0;
            //}
            string allLinked = "";
            try
            {
                //oldTime = DateTime.Now.Ticks;
                //if (!Url.Contains("laodong"))
                //{
                     RSSXml.Load(Url);
                //}
                //else
                //{
                //    String HtmlContent = getContent(Url);
                //    RSSXml.LoadXml(HtmlContent);
                //}
                //newTime = DateTime.Now.Ticks;
                //diff2 = newTime - oldTime;
                //RSSXml.LoadXml(HtmlContent);
            }
            catch (Exception ex)
            {
                try
                {
                    String HtmlContent = getContent(Url);
                    RSSXml.LoadXml(HtmlContent);
                }
                catch (Exception ex2)
                {
                    Array.Resize(ref arrNewsDomainCat, Length);
                    return;
                }
                //Array.Resize(ref arrNewsDomainCat, Length);
                //return;
            }

            //if (Url.Contains("nld.com.vn"))
            //{
            //    int abc = 0;
            //}
            XmlNodeList RSSNodeList = RSSXml.SelectNodes("rss/channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("rss/channel/title");

            //StringBuilder sb = new StringBuilder();
            try
            {
                foreach (XmlNode RSSNode in RSSNodeList)
                {
                    //if (Url.Contains("hanoimoi.com.vn") || Url.Contains("nld.com.vn") || Url.Contains("http://vietnamnet.vn")) {
                    //    int abc = 0;
                    //}
                    //if (Url.Contains("nld.com.vn"))
                    //{
                    //    int abc = 0;
                    //}
                    try
                    {
                        XmlNode RSSSubNode;
                        RSSSubNode = RSSNode.SelectSingleNode("title");
                        string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        RSSSubNode = RSSNode.SelectSingleNode("link");
                        string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        RSSSubNode = RSSNode.SelectSingleNode("description");
                        string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        RSSSubNode = RSSNode.SelectSingleNode("pubDate");
                        if (Url.Contains("hanoimoi.com.vn"))
                        {
                            RSSSubNode = RSSNode.LastChild;//.SelectSingleNode("a10:updated");
                        }
                        string date = RSSSubNode != null ? RSSSubNode.InnerText : "";
                        // Kiểm tra nếu ngày gửi quá lâu thì không lấy

                        if (link.Contains("thanhnien.com.vn"))
                        {
                            string date1 = Uti.datetimeid().ToString();
                            string date2 = Uti.datetimeidByDay(-1).ToString();
                            string date3 = Uti.datetimeidByDay(-2).ToString();
                            string date4 = Uti.datetimeidByDay(-3).ToString();
                            if (link.Contains(date1) || link.Contains(date2) || link.Contains(date3) || link.Contains(date4))
                            {
                                if (link.Contains(date1))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                    if (link.Contains(date2))
                                    {
                                        date = DateTime.Now.AddDays(-1).ToString();
                                    }
                                    else
                                        if (link.Contains(date3))
                                        {
                                            date = DateTime.Now.AddDays(-2).ToString();
                                        }
                                        else
                                            if (link.Contains(date4))
                                            {
                                                date = DateTime.Now.AddDays(-3).ToString();
                                            }
                            }
                            else continue;
                        }
                        if (link.Contains("thethao.tuoitre.vn"))
                        {
                            string date1 = Uti.datetimeid().ToString();
                            string date2 = Uti.datetimeidByDay(-1).ToString();
                            string tempLink = link.Replace("/", "");
                            if (tempLink.Contains(date1) || tempLink.Contains(date2))
                            {
                                if (tempLink.Contains(date1))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }
                            }
                            else continue;
                        }
                        if (link.Contains("vietnamnet.vn"))
                        {
                            string date1 = DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Day.ToString("00") + "/" + DateTime.Now.Year.ToString();
                            string date11 = DateTime.Now.Day.ToString("00") + "/" + DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year.ToString();
                            DateTime yesterday = DateTime.Now.AddDays(-1);
                            string date2 = yesterday.Month.ToString("00") + "/" + yesterday.Day.ToString("00") + "/" + yesterday.Year.ToString();
                            string date22 = yesterday.Day.ToString("00") + "/" + yesterday.Month.ToString("00") + "/" + yesterday.Year.ToString();
                            if (date.Contains(date1) || date.Contains(date2) || date.Contains(date11) || date.Contains(date22))
                            {
                                if (date.Contains(date1) || date.Contains(date11))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (Url.Contains("http://nld.com.vn"))
                        {
                            string date1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                            DateTime yesterday = DateTime.Now.AddDays(-1);
                            string date2 = yesterday.Month.ToString() + "/" + yesterday.Day.ToString() + "/" + yesterday.Year.ToString();
                            if (date.Contains(date1) || date.Contains(date2))
                            {
                                if (date.Contains(date1))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }
                            }
                            else continue;
                        }

                        if (date == null || date == "")// || Uti.dateDiff(date, nowDate) == 0
                        {
                            date = DateTime.Now.ToString();
                        }
                        else
                        {
                            date = Uti.isDate(date);
                            if (date.Equals(""))
                            {
                                date = DateTime.Now.ToString();
                            }
                        }
                        if (!date.Equals(""))
                        {
                            if (Uti.dateDiff(date, nowDate) > 4) continue;
                        }

                        //RSSSubNode = RSSNode.SelectSingleNode("domain");
                        //string domain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        //RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                        //string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        //RSSSubNode = RSSNode.SelectSingleNode("catid");
                        //string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";


                        if (title != null && !title.Equals(""))
                        {
                            link = link.Trim();
                            //if (link.Contains("thethao.vietnamnet.vn")) link = link.Replace("http://vietnamnet.vn","");
                            title = title.Trim();
                            if (!allLinked.Contains("," + link + ","))
                            {
                                allLinked += "," + link + ",";
                            }
                            else continue;
                            //Guid.NewGuid().ToString();
                            arrNewsDomainCat[Length] = new viewNewsDomainCat();
                            arrNewsDomainCat[Length].title = title.Trim();
                            arrNewsDomainCat[Length].link = link.Trim();
                            string token = Config.EncodeStr(link.Trim());
                            arrNewsDomainCat[Length].image = getImageSrc(desc);
                            if (Url.Contains("laodong.com.vn"))
                            {
                                arrNewsDomainCat[Length].image = arrNewsDomainCat[Length].image.ToLower();
                                if (arrNewsDomainCat[Length].image.Contains(".jpg"))
                                    arrNewsDomainCat[Length].image = arrNewsDomainCat[Length].image.Substring(0, arrNewsDomainCat[Length].image.LastIndexOf(".jpg") + 4);
                                else
                                    if (arrNewsDomainCat[Length].image.Contains(".bmp")) arrNewsDomainCat[Length].image = arrNewsDomainCat[Length].image.Substring(0, arrNewsDomainCat[Length].image.LastIndexOf(".bmp") + 4);

                            }
                            if (Url.Contains("24h"))
                            {
                                RSSSubNode = RSSNode.SelectSingleNode("summaryImg");
                                string imgsrc = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                arrNewsDomainCat[Length].image = this.maindomain + imgsrc;
                            }
                            if (arrNewsDomainCat[Length].image.Contains("http://hanoimoi.com.vn")) arrNewsDomainCat[Length].image = arrNewsDomainCat[Length].image.Replace("http://admin.hanoimoi.com.vn", "");// http://admin.hanoimoi.com.vnhttp://hanoimoi.com.vn/Uploads/honghai/2014/4/22/DUNG-1-52edb.jpg)
                            if (arrNewsDomainCat[Length].image.Contains("autopro2.vcmedia.vn")) arrNewsDomainCat[Length].image = arrNewsDomainCat[Length].image.Replace("autopro2.vcmedia.vn", "autopro2.vcmedia.vn/");
                            if (!arrNewsDomainCat[Length].image.StartsWith("http://") && !arrNewsDomainCat[Length].image.StartsWith("www"))
                            {
                                arrNewsDomainCat[Length].image = this.maindomain + arrNewsDomainCat[Length].image;
                            }
                            arrNewsDomainCat[Length].des = removeBr(removeHtmlTag(desc));
                            arrNewsDomainCat[Length].catid = this.catid;
                            if (date == null || date == "")// || Uti.dateDiff(date, nowDate) == 0
                            {
                                arrNewsDomainCat[Length].date = DateTime.Now.ToString();
                            }
                            else
                            {
                                //string tempdate = Uti.isDate(date);
                                //if (!tempdate.Equals(""))
                                //{
                                //    arrItem[Length].date = tempdate;
                                //}
                                //else
                                //{
                                //    arrItem[Length].date = DateTime.Now.ToString();
                                //}//DateTime.Now.ToString();
                                //Uti.toUTCTime(date);
                                arrNewsDomainCat[Length].date = date;
                            }
                            arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                            arrNewsDomainCat[Length].domain = this.domain;
                            arrNewsDomainCat[Length].maindomain = this.maindomain;
                            arrNewsDomainCat[Length].token = token;//sinh ngay nhien token
                            arrNewsDomainCat[Length].show = true;
                            Length++;
                        }
                        else continue;
                    }
                    catch (Exception exInFor)
                    {
                        //int abc = 0;
                        //Array.Resize(ref arrItem, Length);
                    }
                }//for node
            }
            catch (Exception exTryFor)
            {
                //int abc = 0;
            }
            Array.Resize(ref arrNewsDomainCat, Length);
        }//void
        private void getVnexpressHotNewsLink(string Url, int type)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'box_hot_news')]"))
                {
                    string stitle = link.SelectSingleNode(".//div[contains(@class,'block_news_big')]").SelectSingleNode(".//img[contains(@class,'')]").Attributes["alt"].Value;
                    stitle = stitle.Replace("\n", "");
                    string slink = link.SelectSingleNode(".//div[contains(@class,'block_news_big')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//div[contains(@class,'block_news_big')]").SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        sdes = link.SelectSingleNode(".//div[contains(@class,'news_lead')]").InnerText;                        
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;                    
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();                    
                    
                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour*5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }                   
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getNgoiSaoHotNewsLink(string Url, int type)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'news')]"))
                {
                    string stitle = link.SelectSingleNode(".//div[contains(@class,'txw')]").SelectSingleNode(".//h3[contains(@class,'')]").InnerText;
                    stitle = stitle.Replace("\n", "");
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//a[contains(@class,'')]").SelectSingleNode(".//img[contains(@class,'imgnews')]").Attributes["src"].Value;
                        sdes = link.SelectSingleNode(".//div[contains(@class,'txw')]").SelectSingleNode(".//p[contains(@class,'leadnews')]").InnerText;
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getThethao247HotNewsLink(string Url, int type)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//ul[contains(@class,'slide_main')]"))
                {
                    string stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                    stitle = stitle.Replace("\n", "");
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        sdes = link.SelectSingleNode(".//div[contains(@class,'sapo_slide_show')]").InnerText;
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getTechzHotNewsLink(string Url, int type)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'ot-slider-layer first')]"))
                {
                    string stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                    stitle = stitle.Replace("\n", "");
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["style"].Value;
                        simage = simage.Replace("background-image:url(", "").Replace(");", "");
                        //simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        sdes = "";
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getLaoDongHotNewsLink(string Url, int type)
        {
            try
            {
               
                var doc = new HtmlDocument();                
                
                    string html;
                    using (var wc = new GZipWebClient())
                        html = wc.DownloadString(Url);
                    //html = DecodeFromUtf8(html);
                    doc.LoadHtml(html);
                
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'CoverStory1')]"))
                {
                    string stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                    stitle = stitle.Replace("\n", "");
                    stitle = stitle.ToString();
                    stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    slink = "http://laodong.com.vn/" + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        simage = "http://static.laodong.com.vn/" + simage;
                        simage = HttpUtility.HtmlDecode(simage);
                        sdes = link.SelectSingleNode(".//div[contains(@class,'summary')]").InnerText;
                        sdes = sdes.Replace("\n", "");
                        sdes = DecodeFromUtf8(sdes);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        public static string DecodeFromUtf8(string utf16String)
        {
            byte[] bytes = Encoding.Default.GetBytes(utf16String);
            string myString = Encoding.UTF8.GetString(bytes);
            return myString;
        }
        private void getVietNamNetHotNewsLink(string Url, int type)
        {
            try
            {
                string html;
                WebClient client = new WebClient();
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(html);
                }
                catch (Exception ex2) {
                    HtmlWeb hw = new HtmlWeb();
                    doc = hw.Load(Url);
                }

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'topNewsLastestFirst')]"))
                {
                    string stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                    stitle = stitle.Replace("\n", "");
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    slink = "http://vietnamnet.vn" + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        sdes = link.SelectSingleNode(".//p[contains(@class,'lead')]").InnerText;
                        sdes = sdes.Replace("\n", "");
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getZingHotNewsLink(string Url, int type)
        {
            try
            {
                var doc = new HtmlDocument();
                string html;
                using (var wc = new GZipWebClient())
                    html = wc.DownloadString(Url);
                //html = DecodeFromUtf8(html);
                doc.LoadHtml(html);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//section[contains(@class,'featured')]"))
                {
                    string stitle = link.SelectSingleNode(".//h1[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").InnerText;
                    stitle = stitle.Replace("\n", "");
                    stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//h1[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    slink = Url + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//div[contains(@class,'cover')]").Attributes["style"].Value;
                        simage = simage.Replace("background-image: url(", "").Replace(");","");
                        sdes = link.SelectSingleNode(".//p[contains(@class,'summary')]").InnerText;
                        sdes = DecodeFromUtf8(sdes);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getThanhNienHotNewsLink(string Url, int type)
        {
            try
            {
                string html;
                WebClient client = new WebClient();
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(html);
                }
                catch (Exception ex2)
                {
                    HtmlWeb hw = new HtmlWeb();
                    doc = hw.Load(Url);
                }

                foreach (HtmlNode link in doc.DocumentNode.SelectSingleNode("//div[contains(@class,'tinnb')]").SelectNodes(".//div[contains(@id,'tnao1')]"))
                {
                    string stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                    stitle = stitle.Replace("\n", "");
                    //stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    slink = this.maindomain + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        simage = this.maindomain + simage;
                        sdes = link.SelectSingleNode(".//span[contains(@class,'tt-des')]").InnerText;
                        //sdes = DecodeFromUtf8(sdes);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getVtcHotNewsLink(string Url, int type)
        {
            try
            {
                string html;
                WebClient client = new WebClient();
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(html);
                }
                catch (Exception ex2)
                {
                    HtmlWeb hw = new HtmlWeb();
                    doc = hw.Load(Url);
                }

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'p1_tin_pro')]"))
                {
                    string stitle = link.SelectSingleNode(".//div[contains(@class,'p1_tin_nen')]").SelectSingleNode(".//h1[contains(@class,'')]").InnerText;
                    stitle = stitle.Replace("\n", "");
                    //stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    if (!slink.StartsWith("http://")) slink = this.maindomain + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        if (!simage.StartsWith("http://")) simage = this.maindomain + simage;
                        sdes = link.SelectSingleNode(".//p[contains(@class,'instructions')]").InnerText;
                        //sdes = DecodeFromUtf8(sdes);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getDsplHotNewsLink(string Url, int type)
        {
            try
            {
                string html;
                WebClient client = new WebClient();
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(html);
                }
                catch (Exception ex2)
                {
                    HtmlWeb hw = new HtmlWeb();
                    doc = hw.Load(Url);
                }

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'home-feature fl-left')]"))
                {
                    string stitle = link.SelectSingleNode(".//h4[contains(@class,'')]").InnerText;
                    stitle = stitle.Replace("\n", "");
                    //stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    if (!slink.StartsWith("http://")) slink = this.maindomain + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        if (!simage.StartsWith("http://")) simage = this.maindomain + simage;
                        sdes = link.SelectSingleNode(".//p[contains(@class,'desc')]").InnerText;
                        sdes = sdes.Replace("\"", "");
                        //sdes = DecodeFromUtf8(sdes);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getBongDaHotNewsLink(string Url, int type)
        {
            //Chua xong
            try
            {
                string html;
                WebClient client = new WebClient();
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                try
                {
                    doc.LoadHtml(html);
                }
                catch (Exception ex2)
                {
                    HtmlWeb hw = new HtmlWeb();
                    doc = hw.Load(Url);
                }

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@id,'topNewLeft')]"))
                {
                    string stitle = link.SelectSingleNode(".//div[contains(@class,'hometitle')]").InnerText;
                    stitle = stitle.Replace("\r\n", "");
                    stitle = stitle.Replace("\n", "");
                    stitle = stitle.Replace("\t", "");
                    //stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//div[contains(@class,'hometitle')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    if (!slink.StartsWith("http://")) slink = this.Url + slink;
                    string simage = "";
                    string sdatetime = "";
                    string sdes = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        if (!simage.StartsWith("http://")) simage = this.Url + simage;
                        sdes = link.SelectSingleNode(".//p[contains(@class,'desc')]").InnerText;
                        sdes = sdes.Replace("\"", "");
                        //sdes = DecodeFromUtf8(sdes);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    //sdatetime = Uti.isDate(sdatetime);
                    //if (sdatetime.Equals("")) 
                    sdatetime = DateTime.Now.ToString();

                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = sdes;
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].ranking = 500 + DateTime.Now.Hour * 5;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Array.Resize(ref arrItem, Length);
        }
        private void getZingLink(string Url,int type) {
            try
            {
                var doc = new HtmlDocument();
                string html;
                using (var wc = new GZipWebClient())
                    html = wc.DownloadString(Url);
                //html = DecodeFromUtf8(html);
                doc.LoadHtml(html);
                string allLinked = "";
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//article[contains(@class,'')]"))
                {
                    string stitle = link.SelectSingleNode(".//h1[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").InnerText;
                    stitle = stitle.Replace("\n", "");
                    stitle = DecodeFromUtf8(stitle);
                    string slink = link.SelectSingleNode(".//h1[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                    string simage = "";
                    string sdatetime = "";
                    try
                    {
                        simage = link.SelectSingleNode(".//div[contains(@class,'cover')]").Attributes["style"].Value;
                        sdatetime = link.SelectSingleNode(".//time[contains(@class,'')]").Attributes["datetime"].Value;
                    }
                    catch (Exception ex)
                    {

                    }
                    if (stitle.Equals("") || slink.Equals("") || simage.Equals("")) continue;
                    simage = simage.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                    simage = simage.Replace("background-image:", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                    slink = "http://news.zing.vn" + slink;
                    if (!allLinked.Contains("," + slink + ","))
                    {
                        allLinked += "," + slink + ",";
                    }
                    else continue;
                    sdatetime = Uti.isDate(sdatetime);
                    if (sdatetime.Equals("")) sdatetime = DateTime.Now.ToString();
                    //if (stitle.Contains("Ronaldo")) {
                    //    int abc = 0;
                    //}
                    if (Uti.dateDiff(sdatetime, DateTime.Now.ToString()) > 4)
                    {
                        continue;
                    }
                    if (type == 0)
                    {
                        arrItem[Length].title = stitle;
                        arrItem[Length].link = slink;
                        arrItem[Length].image = simage;
                        arrItem[Length].des = "";
                        arrItem[Length].catid = this.catid;
                        arrItem[Length].date = sdatetime;
                        arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                        arrItem[Length].domain = this.domain;
                        arrItem[Length].maindomain = this.maindomain;
                        arrItem[Length].token = Config.EncodeStr(slink.Trim());
                        arrItem[Length].show = true;
                        Length++;
                    }
                    else {
                        arrNewsDomainCat[Length] = new viewNewsDomainCat();
                        arrNewsDomainCat[Length].title = stitle;
                        arrNewsDomainCat[Length].link = slink;
                        arrNewsDomainCat[Length].image = simage;
                        arrNewsDomainCat[Length].des = "";
                        arrNewsDomainCat[Length].catid = this.catid;
                        arrNewsDomainCat[Length].date = sdatetime;
                        arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                        arrNewsDomainCat[Length].domain = this.domain;
                        arrNewsDomainCat[Length].maindomain = this.maindomain;
                        arrNewsDomainCat[Length].token = Config.EncodeStr(slink.Trim());
                        arrNewsDomainCat[Length].show = true;
                        Length++;
                    }
                }
            }
            catch (Exception ex) { 
            }
            if (type == 0)
            {
                Array.Resize(ref arrItem, Length);
            }
            else {
                Array.Resize(ref arrNewsDomainCat, Length);
            }
        }
        private void getTuoitreLink(string Url,int type)
        {          
            try
            {                
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);
                string allLinked = "";
                //Tuoi Tre Trang Chu
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//section[contains(@class,'content')]"))
                {
                    foreach (HtmlNode link2 in link.SelectNodes(".//a[contains(@class,'')]"))
                    {
                        if (link2.InnerHtml.Contains("img"))
                        {
                            string slink = link2.Attributes["href"].Value;
                            string stitle = link2.SelectSingleNode(".//img[contains(@class,'')]").Attributes["alt"].Value;
                            string simage = "";
                            string sdate = "";
                            try
                            {
                                simage = link2.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                simage = link2.SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                            }
                            catch (Exception ex)
                            {

                            }
                            if (stitle.Equals("") || slink.Equals("")) continue;
                            if (!allLinked.Contains("," + slink + ","))
                            {
                                allLinked += "," + slink + ",";
                            }
                            else continue;
                            if (simage.Equals("")) continue;
                            if (Length >= 10) continue;
                            //image = image.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                            //string link = link.SelectSingleNode(".//li[contains(@class,'nameAuPost')]").InnerText;
                            if (type == 0)
                            {
                                arrItem[Length].title = System.Web.HttpUtility.HtmlDecode(stitle);
                                arrItem[Length].title = Uti.removeHtmlTag(arrItem[Length].title);
                                arrItem[Length].link = slink;
                                arrItem[Length].image = simage;
                                arrItem[Length].des = "";
                                arrItem[Length].catid = this.catid;
                                arrItem[Length].date = DateTime.Now.ToString();
                                arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                                arrItem[Length].domain = this.domain;
                                arrItem[Length].maindomain = this.maindomain;
                                arrItem[Length].token = Config.EncodeStr(slink.Trim());
                                arrItem[Length].show = true;
                                Length++;
                            }
                             else {
                                arrNewsDomainCat[Length] = new viewNewsDomainCat();
                                arrNewsDomainCat[Length].title = System.Web.HttpUtility.HtmlDecode(stitle);
                                arrNewsDomainCat[Length].link = slink;
                                arrNewsDomainCat[Length].image = simage;
                                arrNewsDomainCat[Length].des = "";
                                arrNewsDomainCat[Length].catid = this.catid;
                                arrNewsDomainCat[Length].date = DateTime.Now.ToString();
                                arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                                arrNewsDomainCat[Length].domain = this.domain;
                                arrNewsDomainCat[Length].maindomain = this.maindomain;
                                arrNewsDomainCat[Length].token = Config.EncodeStr(slink.Trim());
                                arrNewsDomainCat[Length].show = true;
                                Length++;                            
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            if (type == 0)
            {
                Array.Resize(ref arrItem, Length);
            }
            else {
                Array.Resize(ref arrNewsDomainCat, Length);
            }
        }
        private void getVuiVietLink(string Url, int type)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);
                string allLinked = "";
                string slink="";
                string simage="";
                string stitle="";
                string sdes="";
                //Tuoi Tre Trang Chu
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'article')]"))
                {
                    try
                    {
                        slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                        simage = link.SelectSingleNode(".//a[contains(@class,'')]").SelectSingleNode(".//div[contains(@class,'thumb')]").Attributes["style"].Value;
                        stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                        simage = simage.Replace("background-image: url(", "").Replace(")", "").Replace("amp;", "");
                        try
                        {
                            if (!Url.Contains("http://vuiviet.vn/category/video-clip"))
                            {
                                sdes = link.SelectSingleNode(".//div[contains(@class,'summary')]").InnerText;
                            }
                        }
                        catch (Exception exDes)
                        {
                        }
                        if (!allLinked.Contains("," + slink + ","))
                        {
                            allLinked += "," + slink + ",";
                        }
                        else continue;
                        //if (image.Equals("")) continue;
                        //image = image.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                        //string link = link.SelectSingleNode(".//li[contains(@class,'nameAuPost')]").InnerText;
                        if (type == 0)
                        {
                            arrItem[Length].title = stitle;
                            arrItem[Length].link = slink;
                            arrItem[Length].image = simage;
                            arrItem[Length].des = sdes;
                            arrItem[Length].catid = this.catid;
                            arrItem[Length].date = DateTime.Now.ToString();
                            arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                            arrItem[Length].domain = this.domain;
                            arrItem[Length].maindomain = this.maindomain;
                            arrItem[Length].token = Config.EncodeStr(slink.Trim());
                            arrItem[Length].show = true;
                            Length++;
                        }
                        else
                        {
                            arrNewsDomainCat[Length] = new viewNewsDomainCat();
                            arrNewsDomainCat[Length].title = stitle;
                            arrNewsDomainCat[Length].link = slink;
                            arrNewsDomainCat[Length].image = simage;
                            arrNewsDomainCat[Length].des = sdes;
                            arrNewsDomainCat[Length].catid = this.catid;
                            arrNewsDomainCat[Length].date = DateTime.Now.ToString();
                            arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                            arrNewsDomainCat[Length].domain = this.domain;
                            arrNewsDomainCat[Length].maindomain = this.maindomain;
                            arrNewsDomainCat[Length].token = Config.EncodeStr(slink.Trim());
                            arrNewsDomainCat[Length].show = true;
                            Length++;
                        }
                    }
                    catch (Exception exinFor) { 
                    }
                }//for
            }
            catch (Exception ex)
            {

            }
            if (type == 0)
            {
                Array.Resize(ref arrItem, Length);
            }
            else
            {
                Array.Resize(ref arrNewsDomainCat, Length);
            }
        }
        private void getClipVnLink(string Url, int type)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(Url);
                string allLinked = "";
                string slink = "";
                string simage = "";
                string stitle = "";
                string sdes = "";
                //Tuoi Tre Trang Chu
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//li[contains(@class,'vitem vsmall')]"))
                {
                    try
                    {
                        slink = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                        simage = link.SelectSingleNode(".//a[contains(@class,'')]").SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        stitle = link.SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                        
                        if (!allLinked.Contains("," + slink + ","))
                        {
                            allLinked += "," + slink + ",";
                        }
                        else continue;
                        //if (image.Equals("")) continue;
                        //image = image.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                        //string link = link.SelectSingleNode(".//li[contains(@class,'nameAuPost')]").InnerText;
                        if (type == 0)
                        {
                            arrItem[Length].title = stitle;
                            arrItem[Length].link = slink;
                            arrItem[Length].image = simage;
                            arrItem[Length].des = sdes;
                            arrItem[Length].catid = this.catid;
                            arrItem[Length].date = DateTime.Now.ToString();
                            arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                            arrItem[Length].domain = this.domain;
                            arrItem[Length].maindomain = this.maindomain;
                            arrItem[Length].token = Config.EncodeStr(slink.Trim());
                            arrItem[Length].show = true;
                            Length++;
                        }
                        else
                        {
                            arrNewsDomainCat[Length] = new viewNewsDomainCat();
                            arrNewsDomainCat[Length].title = stitle;
                            arrNewsDomainCat[Length].link = slink;
                            arrNewsDomainCat[Length].image = simage;
                            arrNewsDomainCat[Length].des = sdes;
                            arrNewsDomainCat[Length].catid = this.catid;
                            arrNewsDomainCat[Length].date = DateTime.Now.ToString();
                            arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                            arrNewsDomainCat[Length].domain = this.domain;
                            arrNewsDomainCat[Length].maindomain = this.maindomain;
                            arrNewsDomainCat[Length].token = Config.EncodeStr(slink.Trim());
                            arrNewsDomainCat[Length].show = true;
                            Length++;
                        }
                    }
                    catch (Exception exinFor)
                    {
                    }
                }//for
            }
            catch (Exception ex)
            {

            }
            if (type == 0)
            {
                Array.Resize(ref arrItem, Length);
            }
            else
            {
                Array.Resize(ref arrNewsDomainCat, Length);
            }
        }
        private void getGiaoducNetVnLink(string Url, int type)
        {
            try
            {
                WebClient client = new WebClient();
                string html = "";
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                string allLinked = "";
                string slink = "";
                string stitle = "";
                string simage = "";
                string sdate = "";
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'content-wrap')]"))
                {
                    //HtmlAttribute att = link.Attributes["href"];
                    foreach (HtmlNode link2 in link.SelectNodes(".//article[contains(@class,'featured ')]"))
                    {
                        
                            slink = link2.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                            simage = link2.SelectSingleNode(".//a[contains(@class,'')]").SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                            stitle = link2.SelectSingleNode(".//img[contains(@class,'')]").Attributes["alt"].Value;
                            sdate = "";
                            try
                            {
                                sdate = link2.SelectSingleNode(".//time[contains(@class,'')]").InnerText;
                            }
                            catch (Exception ex)
                            {
                            }
                            if (stitle.Equals("") || slink.Equals("")) continue;
                            if (!allLinked.Contains("," + slink + ","))
                            {
                                allLinked += "," + slink + ",";
                            }
                            else continue;
                            if (!slink.Contains("http://giaoduc.net.vn/")) slink = "http://giaoduc.net.vn/" + slink;
                            //if (image.Equals("")) continue;
                            //image = image.Replace("background-image: ", "").Replace(")", "").Replace("url(", "").Replace(";", "");
                            //string link = link.SelectSingleNode(".//li[contains(@class,'nameAuPost')]").InnerText;
                            if (!sdate.Equals(""))
                            {
                                sdate = sdate.Substring(3, 2) + "/" + sdate.Substring(0, 2) + "/" + DateTime.Now.Year.ToString();
                                sdate = Uti.isDate(sdate);
                                if (sdate.Equals(""))
                                {
                                    sdate = DateTime.Now.ToString();
                                }

                            }
                            if (type == 0)
                            {
                                arrItem[Length].title = stitle;
                                arrItem[Length].link = slink;
                                arrItem[Length].image = simage;
                                arrItem[Length].des = "";
                                arrItem[Length].catid = this.catid;                                
                                arrItem[Length].date = sdate;
                                arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                                arrItem[Length].domain = this.domain;
                                arrItem[Length].maindomain = this.maindomain;
                                arrItem[Length].token = Config.EncodeStr(slink.Trim());
                                arrItem[Length].show = true;
                                Length++;
                            }
                            else
                            {
                                arrNewsDomainCat[Length] = new viewNewsDomainCat();
                                arrNewsDomainCat[Length].title = stitle;
                                arrNewsDomainCat[Length].link = slink;
                                arrNewsDomainCat[Length].image = simage;
                                arrNewsDomainCat[Length].des = "";
                                arrNewsDomainCat[Length].catid = this.catid;
                                arrNewsDomainCat[Length].date = sdate;
                                arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                                arrNewsDomainCat[Length].domain = this.domain;
                                arrNewsDomainCat[Length].maindomain = this.maindomain;
                                arrNewsDomainCat[Length].token = Config.EncodeStr(slink.Trim());
                                arrNewsDomainCat[Length].show = true;
                                Length++;
                            }
                        
                    }

                }
            }
            catch (Exception ex)
            {

            }
            if (type == 0)
            {
                Array.Resize(ref arrItem, Length);
            }
            else
            {
                Array.Resize(ref arrNewsDomainCat, Length);
            }
        }
        private void getNldLink(string Url, int type)
        {
            try
            {
                WebClient client = new WebClient();
                string html = "";
                var data = client.DownloadData(Url);
                html = Encoding.UTF8.GetString(data);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                //string allLinked = "";
                string slink = "";
                string stitle = "";
                string simage = "";
                string sdate = "";
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[contains(@class,'shadow2 ml2')]"))
                {
                    //HtmlAttribute att = link.Attributes["href"];
                    foreach (HtmlNode link2 in link.SelectNodes(".//div[contains(@class,'clearfix')]"))
                    {

                        slink = "";
                        slink = link2.SelectSingleNode(".//a[contains(@class,'')]").Attributes["href"].Value;
                        try
                        {
                            simage = link2.SelectSingleNode(".//a[contains(@class,'')]").SelectSingleNode(".//img[contains(@class,'')]").Attributes["src"].Value;
                        }
                        catch (Exception eximage)
                        {
                        }
                        stitle = link2.SelectSingleNode(".//h3[contains(@class,'')]").SelectSingleNode(".//a[contains(@class,'')]").Attributes["title"].Value;
                        sdate = "";
                        try
                        {
                            sdate = link2.SelectSingleNode(".//p[contains(@class,'datetime')]").InnerText;
                        }
                        catch (Exception ex)
                        {
                        }
                        if (stitle.Equals("") || slink.Equals("")) continue;

                        if (!slink.Contains("http://nld.com.vn")) slink = "http://nld.com.vn" + slink;
                      
                        if (!sdate.Equals(""))
                        {
                            sdate = sdate.Substring(3, 2) + "/" + sdate.Substring(0, 2) + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.ToShortTimeString();
                            sdate = Uti.isDate(sdate);
                            if (sdate.Equals(""))
                            {
                                sdate = DateTime.Now.ToString();
                            }

                        }
                        if (type == 0)
                        {
                            arrItem[Length].title = System.Web.HttpUtility.HtmlDecode(stitle);
                            arrItem[Length].link = slink;
                            arrItem[Length].image = simage;
                            arrItem[Length].des = "";
                            arrItem[Length].catid = this.catid;
                            arrItem[Length].date = sdate;
                            arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                            arrItem[Length].domain = this.domain;
                            arrItem[Length].maindomain = this.maindomain;
                            arrItem[Length].token = Config.EncodeStr(slink.Trim());
                            arrItem[Length].show = true;
                            Length++;
                        }
                        else
                        {
                            arrNewsDomainCat[Length] = new viewNewsDomainCat();
                            arrNewsDomainCat[Length].title = System.Web.HttpUtility.HtmlDecode(stitle);
                            arrNewsDomainCat[Length].link = slink;
                            arrNewsDomainCat[Length].image = simage;
                            arrNewsDomainCat[Length].des = "";
                            arrNewsDomainCat[Length].catid = this.catid;
                            arrNewsDomainCat[Length].date = sdate;
                            arrNewsDomainCat[Length].datetimeid = Uti.datetimeidfromdate(arrNewsDomainCat[Length].date);
                            arrNewsDomainCat[Length].domain = this.domain;
                            arrNewsDomainCat[Length].maindomain = this.maindomain;
                            arrNewsDomainCat[Length].token = Config.EncodeStr(slink.Trim());
                            arrNewsDomainCat[Length].show = true;
                            Length++;
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }
            if (type == 0)
            {
                Array.Resize(ref arrItem, Length);
            }
            else
            {
                Array.Resize(ref arrNewsDomainCat, Length);
            }
        }
        private void getHotNewsHomePage(string Url)
        {
            string nowDate = DateTime.Now.ToString();
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            //long oldTime = DateTime.Now.Ticks;
            //String HtmlContent = getContent(Url);
            //long newTime = DateTime.Now.Ticks;
            //long diff1=0,diff2 = 0;
            //diff1 = newTime - oldTime;
            Length = 0;
            arrItem = new ItemXml[10];
            if (Url.Contains("vnexpress"))
            {
                getVnexpressHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("laodong"))
            {
                getLaoDongHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("vietnamnet"))
            {
                getVietNamNetHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("ngoisao.net"))
            {
                getNgoiSaoHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("thethao247.vn"))
            {
                getThethao247HotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("techz.vn"))
            {
                getTechzHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("zing.vn"))
            {
                getZingHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("thanhnien.com.vn"))
            {
                getThanhNienHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("vtc.vn"))
            {
                getVtcHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("doisongphapluat.com"))
            {
                getDsplHotNewsLink(Url, 0);
                return;
            }
            if (Url.Contains("bongda.com.vn"))
            {
                getBongDaHotNewsLink(Url, 0);
                return;
            }
            
        }
        //Doc Rss
        private void getAllItem(string Url)
        {
            string nowDate=DateTime.Now.ToString();
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            //long oldTime = DateTime.Now.Ticks;
            //String HtmlContent = getContent(Url);
            //long newTime = DateTime.Now.Ticks;
            //long diff1=0,diff2 = 0;
            //diff1 = newTime - oldTime;
            Length = 0;
            arrItem = new ItemXml[1000];
            if (Url.Contains("zing.vn")) {
                getZingLink(Url,0);
                return;
            }
            if (Url.Contains("nld.com.vn"))
            {
                getNldLink(Url, 0);
                return;
            }
            if (Url.Contains("http://tuoitre.vn"))
            {
                getTuoitreLink(Url,0);
                return;
            }
            if (Url.Contains("http://giaoduc.net.vn/"))
            {
                getGiaoducNetVnLink(Url, 0);
                return;
            }
            if (Url.Contains("vuiviet.vn"))
            {
                getVuiVietLink(Url, 0);
                return;
            }
            if (Url.Contains("clip.vn"))
            {
                getClipVnLink(Url, 0);
                return;
            }
            //if (Url.Contains("http://autopro.com.vn/"))
            //{
            //    int abc = 0;
            //}
            //if (Url.Contains("nld.com.vn"))
            //{
            //    int abc2 = 0;
            //}
            string allLinked = "";
            try
            {
                //oldTime = DateTime.Now.Ticks;
                //if (!Url.Contains("laodong"))
                //{
                    RSSXml.Load(Url);
                //}
                //else
                //{
                //    String HtmlContent = getContent(Url);
                //    RSSXml.LoadXml(HtmlContent);
                //}
                //newTime = DateTime.Now.Ticks;
                //diff2 = newTime - oldTime;
                //RSSXml.LoadXml(HtmlContent);
            }
            catch (Exception ex) {
                try
                {
                    String HtmlContent = getContent(Url);
                    RSSXml.LoadXml(HtmlContent);
                }
                catch (Exception ex2)
                {
                    Array.Resize(ref arrNewsDomainCat, Length);
                    return;
                }
            }

            //if (Url.Contains("nld.com.vn"))
            //{
            //    int abc = 0;
            //}
            XmlNodeList RSSNodeList = RSSXml.SelectNodes("rss/channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("rss/channel/title");

            //StringBuilder sb = new StringBuilder();
            try
            {
                foreach (XmlNode RSSNode in RSSNodeList)
                {
                    //if (Url.Contains("hanoimoi.com.vn") || Url.Contains("nld.com.vn") || Url.Contains("http://vietnamnet.vn")) {
                    //    int abc = 0;
                    //}
                    //if (Url.Contains("nld.com.vn"))
                    //{
                    //    int abc = 0;
                    //}
                    try
                    {
                        XmlNode RSSSubNode;
                        RSSSubNode = RSSNode.SelectSingleNode("title");
                        string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        RSSSubNode = RSSNode.SelectSingleNode("link");
                        string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        RSSSubNode = RSSNode.SelectSingleNode("description");
                        string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        RSSSubNode = RSSNode.SelectSingleNode("pubDate");
                        if (Url.Contains("hanoimoi.com.vn"))
                        {
                            RSSSubNode = RSSNode.LastChild;//.SelectSingleNode("a10:updated");
                        }
                        string date = RSSSubNode != null ? RSSSubNode.InnerText : "";
                        // Kiểm tra nếu ngày gửi quá lâu thì không lấy

                        if (link.Contains("thanhnien.com.vn"))
                        {
                            string date1 = Uti.datetimeid().ToString();
                            string date2 = Uti.datetimeidByDay(-1).ToString();
                            string date3 = Uti.datetimeidByDay(-2).ToString();
                            string date4 = Uti.datetimeidByDay(-3).ToString();
                            if (link.Contains(date1) || link.Contains(date2) || link.Contains(date3) || link.Contains(date4))
                            {
                                if (link.Contains(date1))
                                {
                                    date = DateTime.Now.ToString();
                                }else
                                if (link.Contains(date2))                                
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }else
                                if (link.Contains(date3))                                
                                {
                                    date = DateTime.Now.AddDays(-2).ToString();
                                }else 
                                if (link.Contains(date4))                                
                                {
                                    date = DateTime.Now.AddDays(-3).ToString();
                                }
                            }
                            else continue;
                        }
                        if (link.Contains("thethao.tuoitre.vn"))
                        {
                            string date1 = Uti.datetimeid().ToString();
                            string date2 = Uti.datetimeidByDay(-1).ToString();
                            string tempLink = link.Replace("/", "");
                            if (tempLink.Contains(date1) || tempLink.Contains(date2))
                            {
                                if (tempLink.Contains(date1))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }
                            }
                            else continue;
                        }
                        if (link.Contains("vietnamnet.vn"))
                        {
                            string date1 = DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Day.ToString("00") + "/" + DateTime.Now.Year.ToString();
                            string date11 = DateTime.Now.Day.ToString("00") + "/" + DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year.ToString();
                            DateTime yesterday = DateTime.Now.AddDays(-1);
                            string date2 = yesterday.Month.ToString("00") + "/" + yesterday.Day.ToString("00") + "/" + yesterday.Year.ToString();
                            string date22 = yesterday.Day.ToString("00") + "/" + yesterday.Month.ToString("00") + "/" + yesterday.Year.ToString();
                            if (date.Contains(date1) || date.Contains(date2) || date.Contains(date11) || date.Contains(date22))
                            {
                                if (date.Contains(date1) || date.Contains(date11))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (Url.Contains("http://nld.com.vn"))
                        {
                            
                            string date1 = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                            DateTime yesterday = DateTime.Now.AddDays(-1);
                            string date2 = yesterday.Month.ToString() + "/" + yesterday.Day.ToString() + "/" + yesterday.Year.ToString();
                            if (date.Contains(date1) || date.Contains(date2))
                            {
                                if (date.Contains(date1))
                                {
                                    date = DateTime.Now.ToString();
                                }
                                else
                                {
                                    date = DateTime.Now.AddDays(-1).ToString();
                                }
                            }
                            else continue;
                        }

                        if (date == null || date == "")// || Uti.dateDiff(date, nowDate) == 0
                        {
                            date = DateTime.Now.ToString();
                        }
                        else
                        {
                            date = Uti.isDate(date);
                            if (date.Equals(""))
                            {
                                date = DateTime.Now.ToString();
                            }
                        }
                        if (!date.Equals(""))
                        {
                            if (Uti.dateDiff(date, nowDate) > 4) continue;
                        }

                        //RSSSubNode = RSSNode.SelectSingleNode("domain");
                        //string domain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        //RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                        //string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        //RSSSubNode = RSSNode.SelectSingleNode("catid");
                        //string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";


                        if (title != null && !title.Equals("") && Length<arrItem.Length)
                        {
                            link = link.Trim();
                            if (link.Contains("http://nld.com.vnhttp://")) link = link.Replace("http://nld.com.vn","");
                            //if (link.Contains("thethao.vietnamnet.vn")) link = link.Replace("http://vietnamnet.vn","");
                            title = title.Trim();
                            if (!allLinked.Contains("," + link + ","))
                            {
                                allLinked += "," + link + ",";
                            }
                            else continue;
                            //Guid.NewGuid().ToString();
                            arrItem[Length].title = title.Trim();
                            arrItem[Length].link = link.Trim();                            
                            string token = Config.EncodeStr(link.Trim());
                            arrItem[Length].image = getImageSrc(desc);
                            if (Url.Contains("laodong.com.vn"))
                            {
                                arrItem[Length].image = arrItem[Length].image.ToLower();
                                if (arrItem[Length].image.Contains(".jpg"))
                                    arrItem[Length].image = arrItem[Length].image.Substring(0, arrItem[Length].image.LastIndexOf(".jpg") + 4);
                                else
                                    if (arrItem[Length].image.Contains(".bmp")) arrItem[Length].image = arrItem[Length].image.Substring(0, arrItem[Length].image.LastIndexOf(".bmp") + 4);

                            }                            
                            if (Url.Contains("24h")) {
                                RSSSubNode = RSSNode.SelectSingleNode("summaryImg");
                                string imgsrc = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                arrItem[Length].image = "http://24h-img.24hstatic.com" + imgsrc;
                            }
                            if (Url.Contains("vcmedia.vn") && Url.StartsWith("autopro.com.vn")) {
                                arrItem[Length].image = arrItem[Length].image.Replace("autopro.com.vn", "");
                            }
                            if (arrItem[Length].image.Contains("http://hanoimoi.com.vn")) arrItem[Length].image = arrItem[Length].image.Replace("http://admin.hanoimoi.com.vn", "");// http://admin.hanoimoi.com.vnhttp://hanoimoi.com.vn/Uploads/honghai/2014/4/22/DUNG-1-52edb.jpg)
                            if (arrItem[Length].image.Contains("autopro2.vcmedia.vn")) arrItem[Length].image = arrItem[Length].image.Replace("autopro2.vcmedia.vn", "autopro2.vcmedia.vn/");
                            if (!arrItem[Length].image.StartsWith("http://") && !arrItem[Length].image.StartsWith("www"))
                            {
                                arrItem[Length].image = this.maindomain + arrItem[Length].image;
                            }
                             if (Url.Contains("http://kienthuc.net.vn")) {
                                 arrItem[Length].image = arrItem[Length].image.Replace(".ashx?width=80", "");
                            }                            
                            if (!Uti.isImage(arrItem[Length].image)) arrItem[Length].image = "";
                            arrItem[Length].des = removeBr(removeHtmlTag(desc));
                            arrItem[Length].catid = this.catid;
                            if (date == null || date == "")// || Uti.dateDiff(date, nowDate) == 0
                            {
                                arrItem[Length].date = DateTime.Now.ToString();
                            }
                            else
                            {
                                //string tempdate = Uti.isDate(date);
                                //if (!tempdate.Equals(""))
                                //{
                                //    arrItem[Length].date = tempdate;
                                //}
                                //else
                                //{
                                //    arrItem[Length].date = DateTime.Now.ToString();
                                //}//DateTime.Now.ToString();
                                //Uti.toUTCTime(date);
                                arrItem[Length].date = date;
                            }
                            arrItem[Length].datetimeid = Uti.datetimeidfromdate(arrItem[Length].date);
                            arrItem[Length].domain = this.domain;
                            arrItem[Length].maindomain = this.maindomain;
                            arrItem[Length].token = token;//sinh ngay nhien token
                            arrItem[Length].show = true;
                            Length++;
                        }
                        else continue; 
                    }
                    catch (Exception exInFor)
                    {
                        //int abc = 0;
                        //Array.Resize(ref arrItem, Length);
                    }
                }//for node
            }
            catch (Exception exTryFor) {
                //int abc = 0;
            }
            Array.Resize(ref arrItem, Length);
        }//void
        private string encode(string val) { 
            //return Convert.ToBase64String(
            return "";
        }
        //doc tu file 1.xml, 2.xml
        private void readXml(string Url)
        {
            Length = 0;
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex) {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();

            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;
                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("description");
                string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("source");
                string source = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("ranking");
                string ranking = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("keyword");
                string keyword = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("date");
                string date = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("token");
                string token = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("totalcomment");
                string totalcomment = RSSSubNode != null ? RSSSubNode.InnerText : "0";
                if (totalcomment == null || totalcomment == "") totalcomment = "0";
                //RSSSubNode = RSSNode.SelectSingleNode("catid");
                //string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (title != null && title != "")
                {
                    arrItem[Length].title = title;
                    arrItem[Length].link = link;
                    arrItem[Length].des = desc;
                    if (date == null || date == "") 
                                 arrItem[Length].date = DateTime.Now.ToString(); 
                            else 
                                 arrItem[Length].date = date;
                    //if (catid!="") arrItem[Length].catid = int.Parse(catid);
                    //if (domain != "")  arrItem[Length].domain = int.Parse(domain);
                    if (image != "") arrItem[Length].image = image;
                    arrItem[Length].catid = this.catid;
                    arrItem[Length].domain = this.domain;
                    arrItem[Length].maindomain = maindomain;
                    arrItem[Length].source = source;
                    arrItem[Length].token = token;
                    arrItem[Length].show = true;
                    if (ranking!="") arrItem[Length].ranking = Int32.Parse(ranking);
                    arrItem[Length].keyword = keyword;
                    arrItem[Length].totalcomment = Int32.Parse(totalcomment);
                    Length++;
                }
            }
            Array.Resize(ref arrItem, Length);
        }//void
        public void writeToXml(ItemXml[] arr,int length,int domain,string maindomain, int catid,string filename) {
            //string physicalPath = HttpContext.Server.MapPath("");
            Hashtable removeDuplicateLink = new Hashtable();
            Hashtable removeDuplicateTitle = new Hashtable();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            int minRaking = 0;// Uti.getMinRanking();
            string xmlDoc = filename;

            using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
            {   
                writer.WriteStartDocument();
                writer.WriteStartElement("channel");
                for (int i = 0; i < length; i++)
                    if (arr[i].ranking >= minRaking && arr[i].title != null && arr[i].title.Trim() != "" && !removeDuplicateLink.ContainsKey(arr[i].link.Trim()) && !removeDuplicateTitle.ContainsKey(arr[i].title.Trim()))
                {
                    string token = Config.EncodeStr(arr[i].link);
                        //Guid.NewGuid().ToString();
                    //writer.WriteElementString("title", catid.ToString()); //writer.WriteEndElement();
                    //writer.WriteAttributeString("id", "p3");
                    writer.WriteStartElement("item");
                    writer.WriteElementString("title", arr[i].title);
                    writer.WriteElementString("description", arr[i].des);
                    writer.WriteElementString("image", arr[i].image);
                    writer.WriteElementString("link", arr[i].link.Trim());
                    writer.WriteElementString("totalcomment", arr[i].totalcomment.ToString());
                    string date;
                    if (arr[i].date != "") {
                        try
                        {
                            date = DateTime.Parse(arr[i].date).ToString();//ToUniversalTime().
                        }
                        catch (Exception ex) {
                            date = DateTime.Now.ToString();//ToUniversalTime().
                        }
                    }
                    else date = DateTime.Now.ToString();//ToUniversalTime().
                    writer.WriteElementString("date", date);
                    writer.WriteElementString("domain", arr[i].domain.ToString());
                    writer.WriteElementString("maindomain", arr[i].maindomain);
                    writer.WriteElementString("catid", catid.ToString());
                    writer.WriteElementString("token", arr[i].token);
                    writer.WriteElementString("source", arr[i].source);
                    writer.WriteElementString("ranking", arr[i].ranking.ToString());
                    writer.WriteElementString("keyword", arr[i].keyword.ToString());
                    writer.WriteEndElement();                    
                    removeDuplicateLink.Add(arr[i].link.Trim(), "1");
                    removeDuplicateTitle.Add(arr[i].title.Trim(), "1");
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        //public void writeFullContentNewsToXml(ItemXml arr, string filename)
        //{
        //    //string physicalPath = HttpContext.Server.MapPath("");
        //    Hashtable removeDuplicateLink = new Hashtable();
        //    Hashtable removeDuplicateTitle = new Hashtable();
        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    settings.Indent = true;
        //    settings.Encoding = Encoding.UTF8;
        //    int minRaking = 0;// Uti.getMinRanking();
        //    string xmlDoc = filename;

        //    using (XmlWriter writer = XmlWriter.Create(xmlDoc, settings))
        //    {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("channel");
                
        //                writer.WriteStartElement("item");
        //                writer.WriteElementString("title", arr.title);
        //                writer.WriteElementString("content", System.Web.HttpUtility.HtmlEncode(arr.fullContent));
        //                writer.WriteElementString("image", arr.image);
        //                writer.WriteElementString("link", arr.link.Trim());                        
        //                //string date;
        //                //if (arr.date != "")
        //                //{
        //                //    try
        //                //    {
        //                //        date = DateTime.Parse(arr.date).ToString();//ToUniversalTime().
        //                //    }
        //                //    catch (Exception ex)
        //                //    {
        //                //        date = DateTime.Now.ToString();//ToUniversalTime().
        //                //    }
        //                //}
        //                //else date = DateTime.Now.ToString();//ToUniversalTime().
        //                writer.WriteElementString("date", arr.date);                       
        //                writer.WriteEndElement();
        //                removeDuplicateLink.Add(arr[i].link.Trim(), "1");
        //                removeDuplicateTitle.Add(arr[i].title.Trim(), "1");
                    
        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
            
        //}
        //doc tu file topComment.xml
        private void readTopCommentXml(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();

            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : "";


                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("total");
                string total = RSSSubNode != null ? RSSSubNode.InnerText : "0";
                

                RSSSubNode = RSSNode.SelectSingleNode("token");
                string token = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (title != null && title != "")
                {
                    arrItem[Length].id = int.Parse(id);
                    arrItem[Length].title = title;
                    arrItem[Length].link = link;
                    arrItem[Length].token = token;                    
                    arrItem[Length].totalcomment = int.Parse(total);
                    arrItem[Length].maindomain = maindomain;
                    Length++;
                }
            }
        }//void
        //doc tu file TopNewWeekly.xml
        private void readTopNewWeekly(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();

            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : "";


                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("ranking");
                string ranking = RSSSubNode != null ? RSSSubNode.InnerText : "0";


                RSSSubNode = RSSNode.SelectSingleNode("datetimeid");
                string datetimeid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("topicid");
                string topicid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("hasContent");
                string hasContent = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (title != null && title != "")
                {
                    arrItem[Length].id = int.Parse(id);
                    arrItem[Length].title = title;
                    arrItem[Length].link = link;
                    arrItem[Length].ranking = int.Parse(ranking);
                    arrItem[Length].maindomain = maindomain;
                    arrItem[Length].datetimeid = int.Parse(datetimeid);
                    if (topicid != "")
                    {
                        arrItem[Length].topicid = int.Parse(topicid);
                    }
                    if (hasContent != "")
                    {
                        arrItem[Length].hasContent = int.Parse(hasContent);
                    }  
                    Length++;
                }
            }
            Array.Resize(ref arrItem, Length);
        }//void
        //doc tu file TopNewWeekly.xml
        private void readNewestNew(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();

            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("ranking");
                string ranking = RSSSubNode != null ? RSSSubNode.InnerText : "0";


                RSSSubNode = RSSNode.SelectSingleNode("datetime");
                string datetime = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("timediff");
                string timediff = RSSSubNode != null ? RSSSubNode.InnerText : "";
               

                RSSSubNode = RSSNode.SelectSingleNode("hasContent");
                string hasContent = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("catid");
                string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (title != null && title != "")
                {
                    arrItem[Length].id = int.Parse(id);
                    arrItem[Length].title = title;
                    arrItem[Length].link = link;
                    arrItem[Length].ranking = int.Parse(ranking);                   
                    arrItem[Length].date = datetime;
                    arrItem[Length].timediff = timediff;
                    arrItem[Length].image = image;  
                    if (hasContent != "")
                    {
                        arrItem[Length].hasContent = int.Parse(hasContent);
                    }
                    if (catid != "")
                    {
                        arrItem[Length].catid = int.Parse(catid);
                    }
                    arrItem[Length].maindomain = maindomain;
                    Length++;
                }
            }
            Array.Resize(ref arrItem, Length);
        }//void
        //doc tu file HotNew.xml, New.xml, Cat*.Xml
        private void readNewsManager(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();
            Length = 0;
            arrNewsManager = new viewNewsManager[1000];
            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("des");
                string des = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("source");
                string source = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("date");
                string date = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("datetimeid");
                string datetimeid = RSSSubNode != null ? RSSSubNode.InnerText : DateTime.Now.ToString();


                RSSSubNode = RSSNode.SelectSingleNode("ranking");
                string ranking = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";


                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("totalcomment");
                string totalcomment = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("uplikes");
                string uplikes = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("downlikes");
                string downlikes = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("topicid");
                string topicid = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("catid");
                string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("hasContent");
                string hasContent = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("timediff");
                string timediff = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("nameRelated");
                string nameRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("topicidRelated");
                string topicidRelated = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("idRelated");
                string idRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("linkRelated");
                string linkRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomainRelated");
                string maindomainRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("rankingRelated");
                string rankingRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("hasContentRelated");
                string hasContentRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("imageRelated");
                string imageRelated = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (id != null && id != "")
                {
                    arrNewsManager[Length] = new viewNewsManager();
                    arrNewsManager[Length].id = Int32.Parse(id);
                    arrNewsManager[Length].title = title;
                    arrNewsManager[Length].des = des;
                    arrNewsManager[Length].source = source;
                    arrNewsManager[Length].date = DateTime.Parse(date);
                    arrNewsManager[Length].datetimeid = Int32.Parse(datetimeid);
                    arrNewsManager[Length].ranking = Int32.Parse(ranking);
                    arrNewsManager[Length].link = link;
                    arrNewsManager[Length].image = image;
                    if (totalcomment != "") arrNewsManager[Length].totalcomment = Int32.Parse(totalcomment);
                    arrNewsManager[Length].image = image;
                    arrNewsManager[Length].maindomain = maindomain;
                    arrNewsManager[Length].uplikes = Int32.Parse(uplikes);
                    arrNewsManager[Length].downlikes = Int32.Parse(downlikes);
                    if (topicid!="") arrNewsManager[Length].topicid = Int32.Parse(topicid);
                    arrNewsManager[Length].catid = Int32.Parse(catid);
                    arrNewsManager[Length].hasContent = Byte.Parse(hasContent);
                    arrNewsManager[Length].timediff = Int32.Parse(timediff);
                    arrNewsManager[Length].nameRelated = nameRelated;
                    if (topicidRelated != "") arrNewsManager[Length].topicidRelated = Int32.Parse(topicidRelated);
                    if (idRelated != "") arrNewsManager[Length].idRelated = Int32.Parse(idRelated);
                    arrNewsManager[Length].linkRelated = linkRelated;
                    arrNewsManager[Length].maindomainRelated = maindomainRelated;
                    if (rankingRelated != "") arrNewsManager[Length].rankingRelated = Int32.Parse(rankingRelated);
                    if (hasContentRelated != "") arrNewsManager[Length].hasContentRelated = Byte.Parse(hasContentRelated);
                    arrNewsManager[Length].imageRelated = imageRelated;
                    Length++;
                }
            }
            Array.Resize(ref arrNewsManager, Length);
        }//void
        //doc tu file Cat_*_New.Xml
        public void readCatNewsLatest(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();
            LengthCatNewsLatest = 0;
            arrCatNewsLatestManager = new viewCatNewsLatestManager[6];
            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("catid");
                string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("datetime");
                string datetime = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : DateTime.Now.ToString();


                RSSSubNode = RSSNode.SelectSingleNode("hasContent");
                string hasContent = RSSSubNode != null ? RSSSubNode.InnerText : "0";
                               
                if (id != null && id != "")
                {
                    arrCatNewsLatestManager[LengthCatNewsLatest] = new viewCatNewsLatestManager();
                    arrCatNewsLatestManager[LengthCatNewsLatest].id = Int32.Parse(id);
                    arrCatNewsLatestManager[LengthCatNewsLatest].catid = Int32.Parse(catid);
                    arrCatNewsLatestManager[LengthCatNewsLatest].title = title;
                    arrCatNewsLatestManager[LengthCatNewsLatest].link = link;
                    arrCatNewsLatestManager[LengthCatNewsLatest].image = image;
                    arrCatNewsLatestManager[LengthCatNewsLatest].datetime = DateTime.Parse(datetime);
                    arrCatNewsLatestManager[LengthCatNewsLatest].maindomain = maindomain;
                    arrCatNewsLatestManager[LengthCatNewsLatest].hasContent = Int32.Parse(hasContent);
                    LengthCatNewsLatest++;
                }
            }
            Array.Resize(ref arrCatNewsLatestManager, LengthCatNewsLatest);
        }//void
        //doc tu file mHotNew.xml, mNew.xml, mCat*.Xml
        private void readNewsMobileAppManager(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();
            Length = 0;
            arrNewsMobileAppManager = new viewNewsMobileAppManager[10000];
            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("date");
                string date = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("ranking");
                string ranking = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";


                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";
                

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";
                

                RSSSubNode = RSSNode.SelectSingleNode("topicid");
                string topicid = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("catid");
                string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("timediff");
                string timediff = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("hasContent");
                string hasContent = RSSSubNode != null ? RSSSubNode.InnerText : "";

                //RSSSubNode = RSSNode.SelectSingleNode("des");
                //string des = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (id != null && id != "")
                {
                    arrNewsMobileAppManager[Length] = new viewNewsMobileAppManager();
                    arrNewsMobileAppManager[Length].id = Int32.Parse(id);
                    arrNewsMobileAppManager[Length].title = title;

                    arrNewsMobileAppManager[Length].date = DateTime.Parse(date);

                    arrNewsMobileAppManager[Length].ranking = Int32.Parse(ranking);
                    arrNewsMobileAppManager[Length].link = link;
                    arrNewsMobileAppManager[Length].image = image;

                    arrNewsMobileAppManager[Length].image = image;
                    arrNewsMobileAppManager[Length].maindomain = maindomain;

                    if (topicid != "") arrNewsMobileAppManager[Length].topicid = Int32.Parse(topicid);
                    arrNewsMobileAppManager[Length].catid = Int32.Parse(catid);
                    arrNewsMobileAppManager[Length].timediff = Int32.Parse(timediff);
                    arrNewsMobileAppManager[Length].hasContent = hasContent;
                    //arrNewsMobileAppManager[Length].des = des;
                    Length++;
                }
            }
            Array.Resize(ref arrNewsMobileAppManager, Length);
        }//void
        //doc tu file Trends.Xml
        public void readTrends(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();
            Length = 0;
            arrTrendsManager = new viewTrendsManager[500];
            foreach (XmlNode RSSNode in RSSNodeList)
            {
                if (Length >= arrTrendsManager.Length) break;
                XmlNode RSSSubNode;

                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("keyword");
                string keyword = RSSSubNode != null ? RSSSubNode.InnerText : "";
                

                RSSSubNode = RSSNode.SelectSingleNode("datetimeid");
                string datetimeid = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("id");
                string id = RSSSubNode != null ? RSSSubNode.InnerText : DateTime.Now.ToString();


                RSSSubNode = RSSNode.SelectSingleNode("name");
                string name = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";


                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";               

                RSSSubNode = RSSNode.SelectSingleNode("hasContent");
                string hasContent = RSSSubNode != null ? RSSSubNode.InnerText : "0";

                RSSSubNode = RSSNode.SelectSingleNode("topicid");
                string topicid = RSSSubNode != null ? RSSSubNode.InnerText : "-1";

                


                if (id != null && id != "")
                {
                    arrTrendsManager[Length] = new viewTrendsManager();
                    arrTrendsManager[Length].keyword = keyword;
                    arrTrendsManager[Length].title = title;
                    arrTrendsManager[Length].datetimeid = Int32.Parse(datetimeid);
                    arrTrendsManager[Length].id = Int32.Parse(id);
                    arrTrendsManager[Length].name = name;
                    arrTrendsManager[Length].link = link;
                    arrTrendsManager[Length].image = image;
                    arrTrendsManager[Length].maindomain = maindomain;
                    if (hasContent != null && hasContent!="") arrTrendsManager[Length].hasContent = Byte.Parse(hasContent);
                    //if (topicid != null && topicid != "") arrTrendsManager[Length].topicid = Byte.Parse(topicid); 
                    Length++;
                }
            }
            Array.Resize(ref arrTrendsManager, Length);
        }//void
        //doc tu file dailyHot.xml
        private void readTopNewsXml(string Url)
        {
            //Fetch the subscribed RSS Feed
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(Url);
            }
            catch (Exception ex)
            {
                return;
            }

            XmlNodeList RSSNodeList = RSSXml.SelectNodes("channel/item");
            XmlNode RSSDesc = RSSXml.SelectSingleNode("channel/title");

            StringBuilder sb = new StringBuilder();

            foreach (XmlNode RSSNode in RSSNodeList)
            {
                XmlNode RSSSubNode;
                RSSSubNode = RSSNode.SelectSingleNode("title");
                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("link");
                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("image");
                string image = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("ranking");
                string ranking = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("description");
                string description = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("token");
                string token = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("date");
                string date = RSSSubNode != null ? RSSSubNode.InnerText : "";

                RSSSubNode = RSSNode.SelectSingleNode("totalcomment");
                string totalcomment = RSSSubNode != null ? RSSSubNode.InnerText : "0";
                if (totalcomment == null || totalcomment == "") totalcomment = "0";

                RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                if (title != null && title != "")
                {
                    arrItem[Length].title = title;
                    arrItem[Length].link = link;
                    arrItem[Length].des = description;
                    arrItem[Length].date = date;
                    if (image != "") arrItem[Length].image = image;
                    arrItem[Length].token = token;
                    if (ranking != "") arrItem[Length].ranking = Int32.Parse(ranking);
                    arrItem[Length].totalcomment = Int32.Parse(totalcomment);
                    arrItem[Length].maindomain = maindomain;
                    Length++;
                }
            }
            Array.Resize(ref arrItem, Length);
        }//void
        //return true if at least one word is related of 2 words
        public static bool Related2Word(string s1, string s2)
        {
            string[] arr = s1.Split(' ');
            int c = 0;
            s2 = " " + s2 + " ";
            for (int k = 0; k < arr.Length; k++)
                if (s2.IndexOf(" " + arr[k] + " ") >= 0)
                {
                    c = c + 1;
                }
            if (c >= 2) return true;
            return false;
        }
        private bool isTheRealKeyWord(string input)
        {
            return ",AT,ON,IN,THE,THIS,THERE,THAT,OF,HOW,AND,WHAT,DO YOU,HOW TO,ARE YOU,AND THE,AND MORE,AND A,AND IN,AT THE,AT,BACK AT,DO THE,OF THE,OF A,OR A,IN THE,IN A,IN,ON,ON THE,ON A,OF OTHER,IS A,IS THE,IS NOT,IS THIS,FOR THE,FOR A,FOR,FROM THE,HAVE THE,TO BE,TO THE,TO GET,WITH A,WITH THE,WHAT A,WHAT TO,".IndexOf("," + input.ToUpper().Trim() + ",") < 0;
        }
       
        public static string getThePieceOfTwoString(string s1, string s2)
        {
            s1 = removeSpecialChar(s1).ToLower().Trim();
            s2 = removeSpecialChar(s2).ToLower().Trim();
            
            if (s1.Length > s2.Length)
            {
                string tp = s1;
                s1 = s2;
                s2 = tp;
            }
            s1 = s1.Trim();
            s2 = s2.Trim();
            if (!Related2Word(s1, s2)) return "";
            int L = s1.Length;
            int N = s2.Length;
            if (L == 0 || N == 0) return "";
            string result = "";
            int minLengOfWord = _Max_Topic_KeyWord_Length_;
            string[] arrTemp = null;
            s2 = " " + s2 + " ";
            while (minLengOfWord >= 2)
            {
                arrTemp = s1.Split(' ');
                result = "";
                // Lay tung tu ra mot roi so sanh
                for (int k = 0; k <= arrTemp.Length - minLengOfWord; k++)
                {
                    result = "";
                    for (int l = k; l < k + minLengOfWord; l++) result += arrTemp[l] + " ";
                    if (realLength(result) >= 2 && s2.IndexOf(" " + result.Trim() + " ") >= 0)
                    {
                        return result.Trim();
                    }
                    else result = "";
                }
                minLengOfWord--;
            }
            return "";
        }
        public static string XauConChungDaiNhat(string s1, string s2)
        {
            s1 = removeSpecialChar(s1).ToLower().Trim();
            s2 = removeSpecialChar(s2).ToLower().Trim();

            //if (s1.Length > s2.Length)
            //{
            //    string tp = s1;
            //    s1 = s2;
            //    s2 = tp;
            //}
            //s1 = s1.Trim();
            //s2 = s2.Trim();
            string[] xau1 = s1.Split(' ');
            string[] xau2 = s2.Split(' ');
            int L = xau1.Length;
            int N = xau2.Length;
            if (L == 0 || N == 0) return "";            
            int[,] max=new int[L+1,N+1];
            int Len=L>N?N:L;
            int i=0, j = 0;
            for(i=0;i<L;i++){
                for(j=0;j<N;j++){
                    if (xau1[i]==xau2[j]){
                        if (i==0 || j==0) max[i,j]=1; else max[i,j]=max[i-1,j-1]+1;
                    }else{
                        if (i == 0 || j == 0) max[i, j] = 0; else max[i, j] = max[i - 1, j] > max[i, j - 1] ? max[i - 1, j] : max[i, j - 1];
                    }
                }
            }
            i=L-1;
            j=N-1;
            string rs="";
            while(i>0 && j>0){
                if (max[i,j]==max[i-1,j-1]+1 && xau1[i]==xau2[j]){                    
                        rs=xau1[i]+" "+rs;
                        i--;
                        j--;
                    
                }else{
                    if (max[i,j]==max[i,j-1]) j--; else
                        if (max[i, j] == max[i - 1, j]) i--;
                        else if (max[i, j] == max[i - 1, j-1]) {i--;j--;}
                }
            }
            if (xau1[i] == xau2[j])
            {
                rs = xau1[i] + " " + rs;
            }
            return rs.Trim();
        }
        public static int realLength(string input)
        {
            string[] arr = input.Split(' ');
            int count = 0;
            for (int k = 0; k < arr.Length; k++)
                if (!arr[k].Trim().Equals("")) count++;
            return count;
        }
        //getCOntent
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
                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                        myRequest.Method = "GET";
                        myRequest.Timeout = 15000;
                        myRequest.UserAgent = myAngent[randomNumber];
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
        public class GZipWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
                
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                //request.TransferEncoding = "UTF-8";
                return request;
            }
        }
    }//class
}
