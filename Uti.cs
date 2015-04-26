using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.IO;
using System.Net;
namespace youknow
{
    public class Uti
    {
        //public static int[] MonthDay=new int[31,28,31,30,31,30,31,31,30,31,30,31];
        public static int getMinRanking() {
            return int.Parse(WebConfigurationManager.AppSettings["minRanking"].ToString());
        }
        public static string getUserNameFromToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return "";
            string[] temp = Config.DecodeStr(token).Split('_');
            return temp[0];
        }
        public static string convertToDateTimeId(string d)
        {
            DateTime d1;
            try
            {
                d1 = DateTime.Parse(d);//ToUniversalTime();
                return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
            }
            catch (Exception ex)
            {
                d1 = DateTime.Now;                
                return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
            }
            d1 = DateTime.Now;
            return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
        }
        public static string isDate(string d) {
            if (d.Contains("GMT")) d = d.Replace("GMT", "");
            try {
                DateTime temp = DateTime.Parse(d);
                if (temp.Ticks > DateTime.Now.Ticks)
                {
                    return DateTime.Now.ToString();
                }
                return temp.ToString();
            }
            catch (Exception ex)
            {
                return "";
            } 
            return "";
        }
        public static string toUTCTime(string d) {
            string d1;
            try
            {
                d1 = DateTime.Parse(d).ToUniversalTime().ToString();
            }
            catch (Exception ex) {
                return DateTime.Now.ToUniversalTime().ToString();
            } 
            return d1;
        }
        public static string getUserNameFromInfor(string info){
            if (string.IsNullOrEmpty(info)) return "";
            string[] temp = info.Split('|');
            if (temp.Length>3) return temp[3];
            else return temp[0];
        }
        public static bool isTheSameDate(string date1, string date2)
        {
            try
            {
                string d1, d2;
                d1 = DateTime.Parse(date1).ToUniversalTime().ToShortDateString();
                d2 = DateTime.Parse(date2).ToUniversalTime().ToShortDateString();
                if (d1 == d2)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return true;
            }
            
        }
        //Nếu là tin mới gần đây thì thời gian càng mới càng nhiều điểm hơn tin cũ
        public static int getHourRanking(string date) {
            try
            {
                DateTime st = DateTime.Parse(date).ToUniversalTime();
                DateTime now = DateTime.Now.ToUniversalTime();
                TimeSpan TS = new System.TimeSpan(now.Ticks - st.Ticks);
                int ranking = (int)TS.TotalHours;
                if (ranking<=1) 
                {
                    return 100;
                }
                else 
                if (ranking < 2)
                {
                    return 70;
                }
                else
                    if (ranking < 4)
                    {
                        return 40;
                    }
                    else
                        if (ranking < 8)
                        {
                            return 30;
                        }
                        else
                        { return 100 / ranking; }
                
            }
            catch (Exception ex) {
                return 1;
            }
            return 0;
        }
        public static int datetimeidByDay(int days)
        {
            DateTime d1;
            try
            {
                //DateTime st = new DateTime(0001, 1, 1).ToUniversalTime();
                //DateTime now = DateTime.Now.ToUniversalTime();
                //TimeSpan TS = new System.TimeSpan(now.Ticks - st.Ticks);
                //return (int)TS.Days;
                d1 = DateTime.Now.AddDays(days);//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);

            }
            catch (Exception ex)
            {
                d1 = DateTime.Now.AddDays(days);//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
            //return DateTime.Now.Year * 365 + DateTime.Now.Month * 30 + DateTime.Now.Day;
        }
        public static int datetimeid()
        {
            DateTime d1;
            try
            {
                //DateTime st = new DateTime(0001, 1, 1).ToUniversalTime();
                //DateTime now = DateTime.Now.ToUniversalTime();
                //TimeSpan TS = new System.TimeSpan(now.Ticks - st.Ticks);
                //return (int)TS.Days;
                d1 = DateTime.Now;//.ToUniversalTime();
                string rs=d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);

            }
            catch (Exception ex) {
                d1 = DateTime.Now;//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
            //return DateTime.Now.Year * 365 + DateTime.Now.Month * 30 + DateTime.Now.Day;
        }
        public static int datetimeidfromdate(string date) {
            try
            {

                DateTime d1 = DateTime.Parse(date);//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
            catch (Exception ex) {
                DateTime d1 = DateTime.Now;//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
        }
        public static int dateDiff(string date1, string date2) {
            try
            {
                DateTime d1 = DateTime.Parse(date1);
                DateTime d2 = DateTime.Parse(date2);
                TimeSpan TS = new System.TimeSpan(d1.Ticks - d2.Ticks);
                return (int)Math.Abs(TS.TotalDays);
            }
            catch (Exception ex) {
                return 100;
            }
        }
        public static string smDomain(string domain) {            
            domain=domain.ToLower().Replace("http://www.","").Replace("http://","").Replace("www.","").Replace("www","");
            //domain = removeSpecialChar(domain);
            return domain;
        }
        public static string smDomainNew(string domain)
        {
            domain = domain.ToLower().Replace("http://www.", "").Replace("http://", "").Replace("www.", "").Replace("www", "");
            domain = removeSpecialChar(domain);
            return domain;
        }
        public static int getTotalTimeFromNow(string date2)
        {
            try
            {
                DateTime d1 = DateTime.Now.ToUniversalTime();
                DateTime d2 = DateTime.Parse(date2).ToUniversalTime();
                TimeSpan TS = new System.TimeSpan(d1.Ticks - d2.Ticks);
                int totalHours = Math.Abs((int)TS.TotalHours);
                return totalHours;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string getDiffTimeFromNow(string date2)
        {
            try
            {
                DateTime d1 = DateTime.Now.ToUniversalTime();
                DateTime d2 = DateTime.Parse(date2).ToUniversalTime();
                TimeSpan TS = new System.TimeSpan(d1.Ticks - d2.Ticks);
                int totalHours = (int)TS.TotalHours;
                if (totalHours >= 24*4)
                {

                    return " 30 phút trước";
                }
                if (totalHours < 0)
                {
                    //d2 = d2.ToLocalTime();
                    return "5 phút trước";
                }
                else {
                    if (totalHours >= 24)
                    {
                        int days = totalHours / 24;
                        return days + " ngày trước";
                    }
                    else if (totalHours == 0) 
                        return " 30 phút trước";
                    else
                        return totalHours.ToString() + " giờ trước";
                }                
            }
            catch (Exception ex)
            {
                return date2;
            }
        }
        public static string getDiffTimeMinuteFromNow(string date2)
        {
            try
            {
                DateTime d1 = DateTime.Now.ToUniversalTime();
                DateTime d2 = DateTime.Parse(date2).ToUniversalTime();
                TimeSpan TS = new System.TimeSpan(d1.Ticks - d2.Ticks);
                int totalHours = (int)TS.TotalMinutes;
                if (totalHours < 0)
                {
                    return "1 phút trước";
                    //d2 = d2.ToLocalTime();
                    //return d2.Day + "/" + d2.Month + "/" + d2.Year;
                }
                else
                {
                    if (totalHours >= 60)
                    {
                        int days = totalHours / 60;
                        return days + " giờ trước";
                    }
                    else return totalHours.ToString() + " phút trước";
                }
            }
            catch (Exception ex)
            {
                return date2;
            }
        }
        public static string getDiffTimeMinuteFromTwoDate(DateTime date1, DateTime date2)
        {
            try
            {
                DateTime d1 = date1;
                DateTime d2 = date2;
                TimeSpan TS = new System.TimeSpan(d2.Ticks - d1.Ticks);
                int totalHours = (int)Math.Abs(TS.TotalSeconds);
                if (totalHours < 0)
                {
                    return "1 phút trước";
                    //d2 = d2.ToLocalTime();
                    //return d2.Day + "/" + d2.Month + "/" + d2.Year;
                }
                else
                {
                    if (totalHours >= 3600)
                    {
                        int days = totalHours / 3600;
                        return days + " giờ trước";
                    }
                    else return totalHours.ToString() + " giây trước";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        //Lấy ra các danh từ viết hoa: Địa danh, tên...
        public static string getNounUpcaseWord(string input) { 
            string[] UpW=new string[]{"A","Ă","Â","B","C","D","Đ","E","F","G","H","I","J","K","L","M","N","O","Ô","Ơ","P","Q","R","S","T","U","Ư","V","W","X","Y","Z"};
            string[] temp = input.Split(' ');
            string rs = "";
            int i = 0;
            while (i < temp.Length) {
                if (temp[i]!="" && UpW.Contains(temp[i][0] + "")) {
                    int j = i+1;
                    while (j < temp.Length && temp[j] != "" && UpW.Contains(temp[j][0] + ""))
                    {
                        j = j + 1;
                    }
                    if (j > i + 1)
                    {//Nếu tồn tại danh từ riêng viết hoa
                        for (int k = i; k <= j - 1; k++) {
                            if (k < temp.Length) rs += temp[k] + " ";
                        }
                        rs = rs.Trim() + ",";
                        i = j - 1;
                    }
                }
                i++;
            }
            return rs;
        }
        //Bỏ các thẻ HTML đi.
        public static string convert_html(string html)
        {
            html = Regex.Replace(html, @"<[^>]*>(.*?)<[^>]*>", String.Empty).Trim();
            html = Regex.Replace(html, @"<[^>]*>", String.Empty).Trim();
            return html;
        }
        //Lấy tên chuyên mục từ catid
        public static string getCatNameFromId(int? id=0){
            string[] catname=new string[]{"Tin nóng", "Xã hội", "Thế giới", "Kinh tế", "Văn hóa", "Thể thao", "Pháp luật", "Khoa học", "Công nghệ", "Xe", "Giáo dục", "", "Sức khỏe","Giải trí","Bạn đọc viết"};
            if (id > catname.Length - 1) id = 0;
            return catname[(int)id];
        }
        public static string getDesMetaCatFromId(int? id = 0) {
            string[] catname = new string[15];// { "Tin nóng", "Xã hội", "Thế giới", "Kinh tế", "Văn hóa", "Thể thao", "Pháp luật", "Khoa học", "Công nghệ", "Xe", "Giáo dục", "", "Sức khỏe", "Giải trí", "Bạn đọc viết" };
            if (id > catname.Length - 1) id = 0;
            catname[0] = "Tin nóng thời sự, giải trí văn hóa xã hội thể thao trong ngày từ các báo vnexpress,zing,vietnamnet, lao động, thanh niên...";
            catname[1] = "Chuyên mục xã hội từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[2] = "Chuyên mục thế giới, quốc tế từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[3] = "Chuyên mục kinh tế, tài chính, ngân hàng, thị trường chứng khoán từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[4] = "Chuyên mục văn hóa, điện ảnh, thời trang từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[5] = "Chuyên mục thể thao, bóng đá, tenis,sopcast từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[6] = "Chuyên mục pháp luật, hình sự, an ninh từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[7] = "Chuyên mục khoa học, vũ trụ, thiên văn học, nghiên cứu từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[8] = "Chuyên mục công nghệ, điện thoại, di động từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[9] = "Chuyên mục ô tô, xe máy từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[10] = "Chuyên mục giáo dục, đào tạo, đại học, cao đẳng từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[11] = "Chuyên mục tin từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[12] = "Chuyên mục sức khỏe, y tế, vệ sinh, an toàn thực phẩm, thuốc từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[13] = "Chuyên mục giải trí, ngôi sao, showbiz, thời trang, điện ảnh từ các báo vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            return catname[(int)id];
        }
        public static string getKeywordMetaCatFromId(int? id = 0)
        {
            string[] catname = new string[15];// { "Trang chủ", "Xã hội", "Thế giới", "Kinh tế", "Văn hóa", "Thể thao", "Pháp luật", "Khoa học", "Công nghệ", "Xe", "Giáo dục", "", "Sức khỏe", "Giải trí", "Bạn đọc viết" };
            if (id > catname.Length - 1) id = 0;
            catname[0] = "đọc báo, tin nóng, tin tức, thời sự, giải trí, văn hóa, xã hội, kinh tế, thế giới, pháp luật, văn hóa, thể thao, sức khỏe,công nghệ, ô tô, xe máy";
            catname[1] = "xã hội, đời sống, pháp luật, an ninh, quốc phòng, gia đình, cuộc sống, hôn nhân, môi trường, tình yêu, giới trẻ";
            catname[2] = "thể giới, quốc tế, liên hợp quốc, quốc phòng, ngoại giao, an ninh, chuyện lạ đó đây, du lịch";
            catname[3] = "kinh tế, tài chính, ngân hàng, thị trường chứng khoán, vàng, đầu tư, bất động sản, doanh nghiệp, khởi nghiệp, thuế, doanh nhân, thương mại, thương mại điện tử, bán buôn, bán lẻ, siêu thị";
            catname[4] = "văn hóa, điện ảnh, thời trang, nếp sống, lễ hội, di tích lịch sử, danh lam thắng cảnh, kiến trúc, làng nghề,";
            catname[5] = "thể thao, bóng đá, tenis,sopcast, bóng rổ, golf, điền kinh, olympic, thế vận hội, á vận hội, đua xe, bóng chuyền, bóng bàn, cầu lông, karate, bơi lội, bắn cung";
            catname[6] = "pháp luật, hình sự, an ninh, trật tự trị an, xã hội, công an, cảnh sát, điều tra, phá án, vụ án";
            catname[7] = "khoa học, vũ trụ, thiên văn học, toán học, vật lý, công nghệ, sinh học, hóa học, nobel, y học, môi trường, thiên nhiên, rừng, thực vật, sinh vật";
            catname[8] = "công nghệ, điện thoại, di động, internet, máy tính bảng, mobile, phone, iphone, ipad, samsung galaxy, sony xperia";
            catname[9] = "ô tô, xe máy, toyota.com.vn, honda.com.vn, xe đạp điện, ford.com.vn, xe đạp, siêu xe, harley davidson, mercedes, bmw, ferrari";
            catname[10] = "giáo dục, đào tạo, đại học, cao đẳng, trung học, tốt nghiệp, du học, học bổng, sách giáo khoa, thanh niên, thiếu niên, nhi đồng, mầm non, trẻ em";
            catname[11] = "vnexpress,zing,vietnamnet,lao động, thanh niên, tuổi trẻ,đời sống pháp luật...";
            catname[12] = "sức khỏe, y tế, vệ sinh, an toàn thực phẩm, thuốc, thầy thuốc, thể dục, yoga, ăn uống, dịch bệnh, chữa bệnh, đời sống";
            catname[13] = "giải trí, ngôi sao, showbiz, thời trang, điện ảnh, sân khấu, Đan Trường, Lam Trường, Đàm Vĩnh Hưng, Hồ Ngọc Hà, Mỹ Linh, Hồng Nhung, Tăng Thanh Hà, Mỹ Tâm, Thanh Lam, Hoài Linh, Chí Trung";
            return catname[(int)id];
        }
        //Lấy tên chuyên mục không dấu từ catid
        public static string getCatNameFromIdNoMark(int? id = 0)
        {
            string[] catname = new string[] { "Tin nóng", "Xã hội", "Thế giới", "Kinh tế", "Văn hóa", "Thể thao", "Pháp luật", "Khoa học", "Công nghệ", "Xe", "Giáo dục", "", "Sức khỏe", "Giải trí", "Bạn đọc viết" };
            if (id > catname.Length - 1) id = 0;
            return unicodeToNoMark(catname[(int)id]);
        }
        //Lấy tên chuyên mục từ catid
        public static string getCatNameEditorFromId(int? id = 0)
        {
            string[] catname = new string[] {"","Xã hội","Thế giới", "Kinh tế", "Giáo dục", "Văn hóa", "Giới trẻ", "Độc lạ", "Game", "Công nghệ", "Thể thao", "Video"};
            if (id > catname.Length - 1) id = 0;
            return catname[(int)id].ToLowerInvariant();
        }
        //format dạng ngày tháng năm từ 20040131 thành 31/01/2004
        public static string getVnDate(string datetimeid) {
            try
            {
                return datetimeid.Substring(6, 2) + "/" + datetimeid.Substring(4, 2) + "/" + datetimeid.Substring(0, 4);
            }
            catch {
                return "";
            }
        }
        public static string removeQuotes(string input)
        {
            input = input.Replace("'", "").Replace("\"", "").Replace("”", "").Replace("&nbsp;", "");
            return input;
        }
        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace(".","").Replace("%","");
            return input;
        }
        
        //convert tieng viet thanh khong dau va them dau -
        public static string unicodeToNoMark(string input) {
            input = input.ToLowerInvariant();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++) {
                input = input.Replace(a_u[i],a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            input = input.Replace(" ", "-");
            return input;
        }
        //convert tieng viet thanh khong dau
        public static string unicodeToNoMarkBasic(string input)
        {
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            return input;
        }
        public static string replaceInogreCase(string sentence,string old,string newstr) {            
            return Regex.Replace(sentence,old, newstr, RegexOptions.IgnoreCase);
        }
        public static string removeSpaceCode(string text) {
            text = text.Replace("&amp;amp;nbsp;", " ");
            return text.Replace("&amp;nbsp;", "");
        }
        public static int realLength(string input)
        {
            string[] arr = input.Split(' ');
            int count = 0;
            for (int k = 0; k < arr.Length; k++)
                if (!arr[k].Trim().Equals("")) count++;
            return count;
        }
        public static string getAvatarOfUser(string userinfo,string idOA) {
            if (userinfo==null||userinfo=="") return "";
            string[] temp = userinfo.Split(',');
            if (temp[1] == "facebook") {
                return "<a href=\"http:////facebook.com//" + idOA + "\"><img src=\"http:////graph.facebook.com//" + idOA + "//picture\"></a>";
            }
            return "";
        }
        public static Boolean getContain(string A, string B) {
            return A.Contains(B);
            
        }
        public static String getDomainImage(String domain)
        {
            if (domain.Contains("24h")) return Config.domain + "/Images/Icon/24h.jpg";
            if (domain.Contains("cafef")) return Config.domain + "/Images/Icon/cafef.jpg";
            if (domain.Contains("dantri")) return Config.domain + "/Images/Icon/dantri.jpg";
            if (domain.Contains("kenh14")) return Config.domain + "/Images/Icon/kenh14.jpg";
            if (domain.Contains("laodong")) return Config.domain + "/Images/Icon/laodong.jpg";
            if (domain.Contains("ngoisao")) return Config.domain + "/Images/Icon/ngoisao.jpg";
            if (domain.Contains("thethaovanhoa")) return Config.domain + "/Images/Icon/thethaovanhoa.jpg";
            if (domain.Contains("vietnamnet")) return Config.domain + "/Images/Icon/vietnamnet.jpg";
            if (domain.Contains("vnexpress")) return Config.domain + "/Images/Icon/vnexpress.jpg";
            if (domain.Contains("vtc")) return Config.domain + "/Images/Icon/vtc.jpg";
            if (domain.Contains("thethao247")) return Config.domain + "/Images/Icon/thethao247.jpg";
            if (domain.Contains("thanhnien")) return Config.domain + "/Images/Icon/thanhnien.jpg";
            if (domain.Contains("vietbao")) return Config.domain + "/Images/Icon/vietbao.jpg";
            if (domain.Contains("hanoimoi")) return Config.domain + "/Images/Icon/hanoimoi.jpg";
            if (domain.Contains("doisongphapluat")) return Config.domain + "/Images/Icon/doisongphapluat.jpg";
            if (domain.Contains("danviet")) return Config.domain + "/Images/Icon/danviet.jpg";
            if (domain.Contains("afamily")) return Config.domain + "/Images/Icon/afamily.jpg";
            if (domain.Contains("zing")) return Config.domain + "/Images/Icon/zing.jpg";
            if (domain.Contains("tuoitre")) return Config.domain + "/Images/Icon/tuoitre.jpg";
            return Config.domain + "/Images/logo.png";
        }
        public static String getDomainImageMobile(String domain)
        {
            if (domain.Contains("24h")) return Config.domain + "/Images/Icon/24hm.jpg";
            if (domain.Contains("cafef")) return Config.domain + "/Images/Icon/cafefm.jpg";
            if (domain.Contains("dantri")) return Config.domain + "/Images/Icon/dantrim.jpg";
            if (domain.Contains("kenh14")) return Config.domain + "/Images/Icon/kenh14m.jpg";
            if (domain.Contains("laodong")) return Config.domain + "/Images/Icon/laodongm.jpg";
            if (domain.Contains("ngoisao")) return Config.domain + "/Images/Icon/ngoisaom.jpg";
            if (domain.Contains("thethaovanhoa")) return Config.domain + "/Images/Icon/thethaovanhoam.jpg";
            if (domain.Contains("vietnamnet")) return Config.domain + "/Images/Icon/vietnamnetm.jpg";
            if (domain.Contains("vnexpress")) return Config.domain + "/Images/Icon/vnexpressm.jpg";
            if (domain.Contains("vtc")) return Config.domain + "/Images/Icon/vtcm.jpg";
            if (domain.Contains("thethao247")) return Config.domain + "/Images/Icon/thethao247m.jpg";
            if (domain.Contains("thanhnien")) return Config.domain + "/Images/Icon/thanhnienm.jpg";
            if (domain.Contains("vietbao")) return Config.domain + "/Images/Icon/vietbaom.jpg";
            if (domain.Contains("hanoimoi")) return Config.domain + "/Images/Icon/hanoimoim.jpg";
            if (domain.Contains("doisongphapluat")) return Config.domain + "/Images/Icon/doisongphapluatm.jpg";
            if (domain.Contains("danviet")) return Config.domain + "/Images/Icon/danvietm.jpg";
            if (domain.Contains("afamily")) return Config.domain + "/Images/Icon/afamilym.jpg";
            if (domain.Contains("zing")) return Config.domain + "/Images/Icon/zingm.jpg";
            if (domain.Contains("tuoitre")) return Config.domain + "/Images/Icon/tuoitrem.jpg";
            return Config.domain + "/Images/logo.png";
        }
        public static string getTimeDiff(int? time)
        {
            int? hourDiff = time;
            String timeDiff = "";
            if (hourDiff <= 5)
            {
                timeDiff = "5 phút trước";
            }
            else
            {
                if (hourDiff >= 60)
                {
                    hourDiff = hourDiff / 60;
                    if (hourDiff == 1)
                    {
                        timeDiff = "1 giờ trước";
                    }
                    else
                    {
                        //hourDiff=hourDiff/24;
                        if (hourDiff >= 24)
                        {
                            hourDiff = hourDiff / 24;
                            if (hourDiff == 1)
                                timeDiff = "1 ngày trước";
                            else
                                timeDiff = hourDiff + " ngày trước";
                        }
                        else
                        {
                            timeDiff = hourDiff + " giờ trước";
                        }
                    }
                }
                else
                {
                    timeDiff = hourDiff + " phút trước";
                }
            }
            return timeDiff;
        }
        public static bool isImage(string image) {
            if (image == null || image.Trim() == "" || image.Equals("")) return false;
            image = image.ToLower();
            return image.IndexOf(".jp") > 0 || image.IndexOf(".bmp") > 0 || image.IndexOf(".png") > 0 || image.IndexOf(".gif") > 0;
        }
        public static string getImageSrc(string content)
        {
            string matchString = Regex.Match(content, "<img.*?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;

            return matchString;
        }
        public static string smoothImage(string content) {
            Regex re = new Regex(@"(<img)(.*?)(width="")(.*?)(px)("")", RegexOptions.IgnoreCase);
            string Result = re.Replace(content, "[$1$2$3$60%$6]");
            return Result;
        }
        public static bool isVn(string content)
        {
            string[] code = new string[] { "á", "ả", "ã", "â", "ă", "à", "ạ", "ú", "ủ", "ũ", "ù", "ụ", "é", "ẻ", "ẽ", "ê", "è", "ẹ", "í", "ỉ", "ĩ", "ì", "ị", "ó", "ỏ", "õ", "ô", "ơ", "ò", "ọ", "ấ", "ẩ", "ẫ", "ầ", "ậ", "ắ", "ẳ", "ẵ", "ằ", "ặ", "ế", "ể", "ễ", "ề", "ệ", "ố", "ổ", "ỗ", "ồ", "ộ", "ớ", "ở", "ỡ", "ờ", "ợ", "ú", "ủ", "ũ", "ù", "ụ", "ý", "ỷ", "ỹ", "ỳ", "ỵ" };
            int count = 0;
            for (int i = 0; i < code.Length; i++)
            {
                if (content.ToLowerInvariant().Contains(code[i]))
                {
                    count++;
                    if (count >= 2) return true;
                }
            }
            return false;
        }
        public static string smoothDes(string content)
        {
            try
            {
                //string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
                //content=Regex.Replace(content, r, "", RegexOptions.Compiled);
                string re = @"\u0008";
                content = Regex.Replace(content, re, ""); 
                if (!isVn(content)) return "";
                content = removeHtmlTag(content);
                content = content.Replace("&amp;nbsp;", " ");
                content = content.Replace("<!--", "").Replace("-->", "").Replace("<!--/", "");                  
                if (content.Length >= Config.maxDesLength) 
                    return content.Substring(0, Config.maxDesLength-2).Trim() + "...";
                else
                    return content.Trim();
            }
            catch (Exception ex) {
                return "";
            }
            return "";
        }        
        public static string removeHtmlTag(string s)
        {
            return Regex.Replace(s, "\\<[^>]*>", "");
        }
        public static bool isDailyKeyword(ref string s) {
            s = s.Trim();
            if (realLength(s) == 4) return true;
            string standardword = ",việt nam,trung quốc,hà nội,sài gòn,";
            string[] commonword = new string[] { "của","tại","đến","bị","ở","với","về","có","vào"};
            for (int i = 0; i < commonword.Length; i++) {
                if (s.StartsWith(commonword[i] + " ")) {
                    s = s.Replace(commonword[i] + " ","");
                }
                if (s.EndsWith(" "+commonword[i]))
                {
                    s = s.Replace(" "+commonword[i], "");
                }
            }
            if (standardword.Contains("," + s.ToLowerInvariant() + ",")) return false;
            if (realLength(s) == 2) return true;
            if (realLength(s) == 3)
            {
                if (realLength(getNounUpcaseWord(s)) == 3)
                    return true;
                else 
                    return false;
            }
            if (realLength(s) <= 1) return false;
            return true;
        }
        public static string innerHtmlText(string fullContent) {
            var doc = new HtmlDocument();
            doc.LoadHtml(fullContent);
            return doc.DocumentNode.InnerText.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Trim();
        }
        public static string tagKeyword(string keyword) {
            string[] arr = keyword.Split(',');
            string rs = "";
            for (int i = 0; i < arr.Length; i++)
            if (!arr[i].Equals(""))
            {
                rs += "<a href=\"/Home/SearchNews?keyword=" + arr[i] + "\" target=\"_blank\">" + arr[i] + "</a>,";
            }
            return rs;
        }
        public static void save_file_from_url(string file_name, string url)
        {
            byte[] content;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            using (BinaryReader br = new BinaryReader(stream))
            {
                content = br.ReadBytes(500000);
                br.Close();
            }
            response.Close();

            FileStream fs = new FileStream(file_name, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(content);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
        }
    }
}