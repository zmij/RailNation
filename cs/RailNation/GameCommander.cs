using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace RailNation
{
    public class GameCommander
    {
        private User owner_;
        private WebBrowser browser_;
        private string key_;
        private string checksum_;
        private string api_;
        private string referer_;

        private CommandBody body_;

        public GameCommander(User owner, WebBrowser browser, string key,
            string checksum, string api)
        {
            owner_ = owner;
            browser_ = browser;
            key_ = key;
            checksum_ = checksum;
            api_ = api + "flash.php";
            referer_ = api.Replace("rpc", "assets") + checksum
                + "/Railnation.swf";

            body_ = new CommandBody
            {
                checksum = checksum
            };
        }

        public T exec<T>( string iface, string method, params object[] data ) where T : JToken
        {
            string url = String.Format("{0}?interface={1}&method={2}", api_, iface, method);

            body_.parameters = new List<object>(data);
            JObject responce = JObject.Parse(browser_.post(url, body_.ToString(), referer: referer_));

            if (responce["Errorcode"].ToString() == "0")
            {
                JToken body = responce["Body"];

                if (body is T)
                    return body as T;
            }

            Trace.TraceError("{0} Unexpected responce to {1}::{2}:", owner_.UserName, iface, method);
            Trace.Indent();
            Trace.Write(responce.ToString());
            Trace.Unindent();
            return null;
        }

        public User Owner
        {
            get
            {
                return owner_;
            }
        }
    }
}
