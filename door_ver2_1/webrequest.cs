using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
namespace door_ver2_1
{
    public class webrequest
    {
        private static readonly Lazy<webrequest> web = new Lazy<webrequest>(() => new webrequest());
        public static webrequest Instance
        {
            get
            {
                return web.Value;
            }
        }
        public string getsession(CookieContainer cookie, string url, string refer)
        {
            string strSessionInfo = String.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            WebHeaderCollection webHeader = req.Headers;

            req.Accept = "*/*";
            req.ProtocolVersion = HttpVersion.Version11;
            req.KeepAlive = true;
            req.ContentType = "application/x-www-form-urlencoded";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36";
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            req.Host = "door.deu.ac.kr";
            req.Method = WebRequestMethods.Http.Get;
            req.Referer = refer;
            req.CookieContainer = cookie;
            //쿠키 정보 출력
            //if (req.CookieContainer.Count > 0)
            //{
            //    foreach (System.Net.Cookie cook in req.CookieContainer.GetCookies(new Uri(url)))
            //    {
            //        string c = String.Empty;
            //        c += "Cookie:\n" + $"{cook.Name} = {cook.Value}\n" + $"Domain: {cook.Domain}\n" + $"Path: {cook.Path}\n" +
            //            $"Secure: {cook.Secure}\n" +
            //            $"When issued: {cook.TimeStamp}\n" +
            //            $"Expires: {cook.Expires} (expired? {cook.Expired})\n" +
            //            $"Don't save: {cook.Discard}\n" +
            //            $"Comment: {cook.Comment}\n" +
            //            $"Uri for comments: {cook.CommentUri}\n" +
            //            $"Version: RFC {(cook.Version == 1 ? 2109 : 2965)}" +
            //            $"String: {cook}";
            //        MessageBox.Show(c);
            //    }
            //}
            try
            {
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                    string strResult = sr.ReadToEnd();
                    Console.WriteLine(res.ResponseUri);

                    try
                    {
                        strSessionInfo = strResult;
                    }
                    finally
                    {
                        if (sr != null)
                            sr.Close();
                        //res.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

            strSessionInfo = strSessionInfo.Replace("\r\n", "");
            strSessionInfo = strSessionInfo.Replace("\t", "");

            return strSessionInfo;
        }

    }
}
