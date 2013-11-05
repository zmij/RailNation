using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailNation
{
    public static class Config
    {
        public const string USER_AGENT   = "Mozilla/5.0 (Windows NT 6.1; rv:24.0) Gecko/20100101 Firefox/24.0";
        public const string START_URL    = "http://railnation.ru/";
        public const string SAM_URL      = "https://railnation-sam.traviangames.com";
        public const string APP_ID       = "railnation";
        public const string APP_INST     = "meta";
        public const string REGION_ID    = "europe";
        public const string COUNTRY_ID   = "ru";
        public const string SERVER_ID    = "railnation-ru-rn20rus4";
        private const string URL_FMT     = 
            "{0}/iframe/{1}/applicationId/{2}/applicationCountryId/{3}/applicationInstanceId/{4}/userParam/undefined/";

        public static string LoginUrl 
        {
            get
            {
                return getUrl("login");
            }
        }

        public static string WorldListUrl
        {
            get
            {
                return getUrl("external-avatar-list");
            }
        }

        private static string getUrl(string action)
        {
            return String.Format(URL_FMT, SAM_URL, action, APP_ID, COUNTRY_ID, APP_INST);
        }
    }
}
