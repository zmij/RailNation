using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RailNation
{
    public class WebBrowser
    {
        CookieContainer cookies_ = new CookieContainer();
        string lastUrl_ = "";
        string lastHost_ = "";

        public string LastUrl
        {
            get
            {
                return lastUrl_;
            }
        }

        public string LastHost
        {
            get
            {
                return lastHost_;
            }
        }

        public void clear()
        {
            cookies_ = new CookieContainer();
            lastUrl_ = "";
        }
        public string get(string url, string referer = "",
            string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
        {
            HttpWebRequest req = createRequest(url, "GET", referer, accept);

            return runRequest(req, referer, accept);
        }

        /// <summary>
        /// Post url-encoded form data
        /// </summary>
        /// <param name="url">URL to post to</param>
        /// <param name="postData">Dictionary of form key-value pairs</param>
        /// <param name="referer">Request referer</param>
        /// <param name="accept">Accept header value</param>
        /// <param name="contentType">ContentType header value</param>
        /// <returns></returns>
        public string post(string url, Dictionary<string, string> postData,
            string referer = "", 
            string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
            string contentType = "application/x-www-form-urlencoded")
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> entry in postData)
            {
                if (sb.Length > 0)
                    sb.Append("&");
                sb.Append(entry.Key + "=" + HttpUtility.UrlEncode(entry.Value));
            }

            return post(url, sb.ToString(), referer, accept, contentType);
        }

        /// <summary>
        /// Post plain string data
        /// </summary>
        /// <param name="url">URL to post to</param>
        /// <param name="postData">String to post</param>
        /// <param name="referer">Request referer</param>
        /// <param name="accept">Accept header value</param>
        /// <param name="contentType">ContentType header value</param>
        /// <returns></returns>
        public string post(string url, string postData,
            string referer = "",
            string accept = "application/json",
            string contentType = "application/json")
        {
            if (!String.IsNullOrEmpty(postData))
                Debug.Print("Request body: {0}", postData);
            HttpWebRequest req = createRequest(url, "POST", referer, accept);
            if (!String.IsNullOrEmpty(contentType))
                req.ContentType = contentType;

            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            req.ContentLength = bytes.Length;
            Stream data = req.GetRequestStream();
            data.Write(bytes, 0, bytes.Length);
            data.Close();
            return runRequest(req, referer, accept);
        }

        private HttpWebRequest createRequest(string url, string method, string referer, string accept)
        {
            Uri uri = new Uri(url);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = method;
            if (referer.Length == 0)
            {
                referer = lastUrl_;
            }
            req.UserAgent = Config.USER_AGENT;
            req.CookieContainer = cookies_;

            CookieCollection cookies = cookies_.GetCookies(req.RequestUri);

            Debug.Print(req.Method + " " + req.RequestUri.ToString());
            if (!String.IsNullOrEmpty(referer))
                Debug.Print("Referer: {0}", referer);
            if (cookies.Count > 0)
            {
                Debug.Print("Sending cookies:");
                foreach (Cookie cookie in cookies)
                {
                    Debug.Print("Domain {0} Path {1} Value {2}", cookie.Domain, cookie.Domain, cookie.ToString());
                }
            }

            req.Accept = accept;
            req.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            req.Headers.Add("AcceptEncoding", "gzip, deflate");
            req.AllowAutoRedirect = true;
            req.KeepAlive = true;
            req.ProtocolVersion = HttpVersion.Version11;

            return req;
        }

        private string runRequest(HttpWebRequest req, string referer, string accept)
        {
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp.Cookies.Count > 0)
                {
                    cookies_.Add(resp.Cookies);
                    Debug.Print("Received cookies:");
                    foreach (Cookie cookie in resp.Cookies)
                    {
                        Debug.Print("Domain {0} Path {1} Value {2}", cookie.Domain, cookie.Domain, cookie.ToString());
                    }
                }
                lastUrl_ = resp.ResponseUri.ToString();
                lastHost_ = resp.ResponseUri.Host.ToString();

                StreamReader reader = new StreamReader(resp.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error running request {0} {1}: {2}", req.Method, req.RequestUri.ToString(), e.ToString());
            }
            return "";
        }
        public CookieCollection getCookies(string url)
        {
            return cookies_.GetCookies(new Uri(url));
        }
        public CookieCollection getCookies(Uri uri)
        {
            return cookies_.GetCookies(uri);
        }
    }
}
