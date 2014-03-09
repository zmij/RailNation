using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace RailNation
{
    using Rails = Dictionary<string, HashSet<string>>;
    using TrainList = BindingSource<Train>;
    using WaggonList = BindingSource<WaggonCount>;
    
    using Path = List<string>;
    using System.ComponentModel;
    using System.Threading;

    [DebuggerDisplay("{Email} {AvatarName}")]
    public class User : INotifyPropertyChanged
    {
        private Regex isFacebook = new Regex("^fb:(.*)");

        private string sam_session_;

        private string email_;
        private string password_;
        private string avatarName_;

        private string id_;

        private WebBrowser browser_ = new WebBrowser();
        private GameCommander commander_ = null;

        private int cash_;
        private int gold_;
        private int prestige_;
        private int lab_;

        private string corpName_;
        private string routeSummary_;

        private TrainList trains_ = new TrainList();
        private RailGraph rails_ = new RailGraph();
        private WaggonList waggons_ = new WaggonList();

        private HashSet<int> technologies_ = new HashSet<int>();

        public enum State
        {
            NotLoggedIn = 0,
            LoggingIn,
            LoggedIn
        }

        private State loginState_;

        public User(string email, string password)
        {
            email_ = email;
            password_ = password;

            foreach (ProductType pt in Enum.GetValues(typeof(ProductType)))
            {
                if (pt == ProductType.UNDEFINED)
                    continue;
                waggons_.Add(new WaggonCount(this, pt));
            }
        }

        #region Properties
        [Browsable(false)]
        public string Id
        {
            get
            {
                return id_;
            }
        }

        [Browsable(false)]
        public bool LoggedToTravian
        {
            get
            {
                return !String.IsNullOrEmpty(sam_session_);
            }
        }

        [Browsable(false)]
        public string Email
        {
            get
            {
                return email_;
            }
        }

        [Browsable(false)]
        public string AvatarName
        {
            get
            {
                return avatarName_;
            }
            private set
            {
                avatarName_ = value;
                OnPropertyChanged("AvatarName");
                OnPropertyChanged("UserName");
            }
        }

        public string UserName
        {
            get
            {
                return String.IsNullOrEmpty(avatarName_) ? email_ : avatarName_;
            }
        }

        public State LoginState
        {
            get
            {
                return loginState_;
            }
            private set
            {
                loginState_ = value;
                OnPropertyChanged("LoginState");
            }
        }

        public string Corporation
        {
            get
            {
                return corpName_;
            }
            private set
            {
                if (corpName_ != value)
                {
                    corpName_ = value;
                    OnPropertyChanged("Corporation");
                }
            }
        }

        public string RouteSummary
        {
            get
            {
                return routeSummary_;
            }
            private set
            {
                if (routeSummary_ != value)
                {
                    routeSummary_ = value;
                    OnPropertyChanged("RouteSummary");
                }
            }
        }

        public int Cash
        {
            get
            {
                return cash_;
            }
            private set
            {
                if (cash_ != value)
                {
                    cash_ = value;
                    OnPropertyChanged("Cash");
                }
            }
        }

        public int Gold
        {
            get
            {
                return gold_;
            }
            private set
            {
                if (gold_ != value)
                {
                    gold_ = value;
                    OnPropertyChanged("Gold");
                }
            }
        }

        public int Prestige
        {
            get
            {
                return prestige_;
            }
            private set
            {
                if (prestige_ != value)
                {
                    prestige_ = value;
                    OnPropertyChanged("Prestige");
                }
            }
        }

        public int Lab
        {
            get
            {
                return lab_;
            }
            private set
            {
                if (lab_ != value)
                {
                    lab_ = value;
                    OnPropertyChanged("Lab");
                }
            }
        }

        public double AvgReliability
        {
            get
            {
                if (trains_.Count == 0)
                    return 0;
                return trains_.Average(t => t.Reliability);
            }
        }


        [Browsable(false)]
        public GameCommander Commander
        {
            get
            {
                return commander_;
            }
        }

        [Browsable(false)]
        public Rails.KeyCollection ConnectedLocations
        {
            get
            {
                //if (rails_.Empty)
                //    refreshRails();
                return rails_.ConnectedLocations;
            }
        }

        public TrainList TrainList
        {
            get
            {
                //if (trains_.Count == 0)
                //    refreshTrains();
                return trains_;
            }
        }

        public WaggonList Waggons
        {
            get
            {
                return waggons_;
            }
        }

        #endregion

        private string getLoginResp()
        {
            try
            {
                browser_.get(Config.START_URL + "#login");
                string loginPage = browser_.get(Config.LoginUrl);

                if (isFacebook.IsMatch(email_))
                {
                    Match m = isFacebook.Match(email_);
                    string actualEmail = m.Groups[1].Value;

                    // 1. Login to facebook
                    string fbPage = browser_.get("http://www.facebook.com/");
                    Regex formRe = new Regex("<form\\s+id=\"login_form\".*?action=\"([^\"]+)\"[^>]+>(.*?)</form>");
                    m = formRe.Match(fbPage);
                    if (m.Success)
                    {
                        string actionUrl = m.Groups[1].Value;
                        string inputs = m.Groups[2].Value;

                        Regex appRe = new Regex("app_id=(.*?)&");
                        m = appRe.Match(fbPage);
                        string app_id = m.Groups[1].Value;

                        // Parse hidden inputs
                        Regex inputRe = new Regex("input\\s+type=\"hidden\".*?name=\"([^\"]+)\".*?value=\"([^\"]+)\"", RegexOptions.Compiled);

                        Dictionary<string, string> formData = new Dictionary<string, string> { 
                                {"email", actualEmail},
                                {"pass", password_},
                                {"persistent", "0"},
                                {"login", "Log In"},
                            };
                        foreach (Match input in inputRe.Matches(inputs))
                        {
                            string name = input.Groups[1].Value;
                            if (!formData.ContainsKey(name))
                                formData.Add(name, input.Groups[2].Value);
                            else
                                Debug.Print("Form data already contains value {0}={1} (new value = {2})",
                                    name, formData[name], input.Groups[2].Value);
                        }

                        fbPage = browser_.post(actionUrl, formData);
                        // TODO Parse from login page
                        fbPage = browser_.get(
                            "https://railnation-sam.traviangames.com//iframe/fb-login/consumer/railnation-ru-meta/applicationLanguage/ru-RU");
                        Regex redirectRe = new Regex("top.location.href='([^']+)'");
                        m = redirectRe.Match(fbPage);
                        if (m.Success)
                        {
                            return browser_.get(m.Groups[1].Value);
                        }
                    }
                }
                else
                {
                    Regex loginRe = new Regex("id=\"loginForm\".*?action=\"([^\"]+)\"");
                    Match m = loginRe.Match(loginPage);

                    if (m.Success)
                    {
                        string actionUrl = m.Groups[1].Value;
                        return browser_.post(actionUrl,
                            new Dictionary<string, string> { 
                            {"className", "login"},
                            {"email", email_},
                            {"password", password_},
                            {"remember_me", "0"},
                            {"submit", "Вход"}
                        });
                    }
                    else
                    {
                        Trace.TraceError("{0} Failed to parse login form", email_);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Exception while obtaining login page: {1}", email_, e);
            }
            return "";
        }

        public bool login()
        {
            try
            {
                LoginState = State.LoggingIn;

                string loginResp = getLoginResp();

                browser_.get(Config.START_URL + "#login");
                string loginPage = browser_.get(Config.LoginUrl);
                Regex sessionRe = new Regex("session\\s*=\\s*'([^']+)'");
                Match m = sessionRe.Match(loginResp);
                if (m.Success)
                {
                    sam_session_ = m.Groups[1].Value;
                    Trace.TraceInformation("{0} successfully obtained travian session", email_);
                    return LoggedToTravian;
                }
                else
                {
                    Trace.TraceError("{0} Failed to get SAM session", email_);
                }
            }
            catch(Exception e)
            {
                Trace.TraceError("{0} Exception while trying to log in: {1}", email_, e);
            }
            Trace.TraceError("{0} Failed to login", email_);
            return false;
        }
        public bool selectWorld(string serverName)
        {
            try
            {
                if (!LoggedToTravian)
                {
                    login();
                }
                if (LoggedToTravian)
                {
                    Trace.TraceInformation("{0} loading initial data", email_);
                    browser_.get(Config.START_URL + "#world");
                    string page = browser_.get(Config.WorldListUrl);
                    Regex re = new Regex("var\\s+server\\s*=\\s*(.*?);");
                    Match m = re.Match(page);
                    if (m.Success)
                    {
                        string src = m.Groups[1].Value;
                        JObject srvList = JObject.Parse(src);
                        JToken avatars = srvList[Config.REGION_ID][Config.COUNTRY_ID]["avatars"];
                        if (avatars != null)
                        {
                            JToken srv = avatars[Config.SERVER_ID];
                            re = new Regex("class=\"loginAvatarForm\".*?action=\"([^\"]+)\"");
                            m = re.Match(page);
                            if (m.Success)
                            {
                                string actionUrl = m.Groups[1].Value;
                                page = browser_.post(actionUrl,
                                    new Dictionary<string, string> { 
                                    {"world", srv["consumers_id"].ToString()},
                                    {"sam_sess", sam_session_}
                                });
                                avatarName_ = srv["avatar"]["avatar_name"].ToString();
                                re = new Regex("document.location.href=\"([^\"]+\\?key=([^\"]+))");
                                m = re.Match(page);
                                if (m.Success)
                                {
                                    string gameUrl = m.Groups[1].Value;
                                    string gameKey = m.Groups[2].Value;

                                    page = browser_.get(gameUrl);

                                    re = new Regex("load_swf\\([^)]*\"assets\\/([^\"]+)\",\\s+\"[^\"]+\",\\s+\"([^\"]+)\",\\s+\"([^\"]+)");
                                    m = re.Match(page);
                                    if (m.Success)
                                    {
                                        string checksum = m.Groups[1].Value;
                                        string api = m.Groups[3].Value;
                                        commander_ = new GameCommander(this, browser_, gameKey, checksum, api);
                                        JValue tok = commander_.exec<JValue>("AccountInterface", "is_logged_in", gameKey);
                                        if (tok != null)
                                        {
                                            id_ = tok.ToString();

                                            LoginState = State.LoggedIn;
                                            Trace.TraceInformation("{0} logged into RN game with avatar {1}", email_, avatarName_);
                                            refresh(true);
                                            return true;
                                        }
                                        else
                                        {
                                            refreshData(true);
                                            if (!String.IsNullOrEmpty(id_))
                                            {
                                                LoginState = State.LoggedIn;
                                                Trace.TraceInformation("{0} logged into RN game with avatar {1}", email_, avatarName_);
                                                refresh();
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Trace.TraceWarning("{0} Can't select world: not logged in", email_);
                }
                Trace.TraceError("{0} Failed to load game initial data", email_);
                LoginState = State.NotLoggedIn;
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Exception while trying to load game initial data: {1}", email_, e);
            }
            return false;
        }

        public void Clear()
        {
            lock (this)
            {
                Trace.TraceInformation("Reset login data for {0}", UserName);
                browser_ = new WebBrowser();
                commander_ = null;
                sam_session_ = "";
                id_ = "";

                trains_.Clear();
                waggons_.Clear();
                rails_.Clear();

                LoginState = State.NotLoggedIn;
            }
        }

        public void refresh(bool firstRun = false)
        {
            try
            {
                refreshData(firstRun);
                refreshRails();
                refreshTrains();
            }
            catch (Exception e)
            {
                Trace.TraceError("{0} Failed to refresh data: {1}", UserName, e);
            }
        }

        public void refreshData(bool firstRun = false)
        {
            lock (this)
            {
                JObject tok = null;
                if (!String.IsNullOrEmpty(id_) && !firstRun)
                {
                    tok = commander_.exec<JObject>("GUIInterface", "get_gui", new List<object>() { id_ });
                }
                else
                {
                    tok = commander_.exec<JObject>("GUIInterface", "get_initial_gui");
                }
                if (tok != null)
                {
                    if (tok["resources"] != null)
                    {
                        if (String.IsNullOrEmpty(id_))
                            id_ = tok["resources"]["1"]["user_id"].ToString();
                        Cash = int.Parse(tok["resources"]["1"]["amount"].ToString());
                        Gold = int.Parse(tok["resources"]["2"]["amount"].ToString());
                        Prestige = int.Parse(tok["resources"]["3"]["amount"].ToString());
                        Lab = int.Parse(tok["resources"]["4"]["amount"].ToString());
                    }
                    if (tok["technologies"] != null)
                    {
                        JEnumerable<JProperty> techlist = tok["technologies"]["techs"].Children<JProperty>();
                        Regex is_number = new Regex("^\\d+$");
                        foreach (JProperty tech in techlist)
                        {
                            if (is_number.IsMatch(tech.Name) && tech.Value["finished"].ToObject<bool>())
                            {
                                int techId = int.Parse(tech.Name);
                                technologies_.Add(techId);
                            }
                        }
                    }
                    if (tok["corporation"] != null && tok["corporation"] is JObject && tok["corporation"]["name"] != null)
                    {
                        Corporation = tok["corporation"]["name"].ToString();
                    }
                }
            }
        }

        public void refreshTrains()
        {
            lock (this)
            {
                if (!String.IsNullOrEmpty(id_))
                {
                    JArray tok = commander_.exec<JArray>("TrainInterface", "getTrains", true, id_);

                    if (tok != null)
                    {

                        JEnumerable<JToken> children = tok.Children();
                        HashSet<string> seen = new HashSet<string>();
                        foreach (JToken child in children)
                        {
                            if (child is JObject)
                            {
                                string id = child["ID"].ToString();
                                seen.Add(id);
                                Train train = null;
                                if (trains_.Any(t => t.Id == id))
                                {
                                    train = trains_.First(t => t.Id == id);
                                    train.refresh(child);
                                }
                                else
                                {
                                    train = new Train(commander_, child);
                                    trains_.Add(train);
                                }
                            }
                            else
                            {
                                Debug.Print("Unexpected data in get all trains responce:\n{0}", child.ToString());
                            }
                        }
                        IEnumerable<Train> deleted = trains_.Where(t => !seen.Contains(t.Id));
                        //Debug.Print("Removed trains count {0}", deleted.Count());
                        foreach (Train old in deleted)
                        {
                            trains_.Remove(old);
                        }

                        Dictionary<string, int> routes = new Dictionary<string, int>();

                        foreach (Train train in trains_)
                        {
                            string loadSummary = train.Load.ToString();
                            if (routes.ContainsKey(loadSummary))
                            {
                                routes[loadSummary] += 1;
                            }
                            else
                            {
                                routes.Add(loadSummary, 1);
                            }
                        }

                        StringBuilder summary = new StringBuilder();
                        foreach (KeyValuePair<string, int> rt in routes)
                        {
                            if (summary.Length > 0)
                                summary.Append("\n");
                            summary.Append(String.Format("{0} - {1}", rt.Key, rt.Value));
                        }

                        RouteSummary = summary.ToString();

                        OnPropertyChanged("TrainList");
                        OnPropertyChanged("AvgReliability");
                        refreshWaggons();
                    }
                }
            }
        }

        public void refreshRails()
        {
            lock (this)
            {
                if (!String.IsNullOrEmpty(id_))
                {
                    JArray tok = commander_.exec<JArray>("RailInterface", "get", id_);
                    rails_.refresh(tok);
                    Debug.Print("{0} has {1} connected locations", avatarName_, rails_.ConnectedLocations.Count);
                }
            }
        }

        public void refreshWaggons()
        {
            if (!String.IsNullOrEmpty(id_))
            {
                JArray tok = commander_.exec<JArray>("DepotInterface", "getAllWaggon", id_);

                if (tok != null)
                {
                    Dictionary<ProductType, int> waggons = new Dictionary<ProductType, int>();

                    JEnumerable<JToken> children = tok.Children();
                    foreach (JToken child in children)
                    {
                        ProductType type = (ProductType)int.Parse(child["type"].ToString());
                        int cnt = int.Parse(child["amount"].ToString());
                        if (waggons.ContainsKey(type))
                        {
                            waggons[type] += cnt;
                        }
                        else
                        {
                            waggons.Add(type, cnt);
                        }
                    }

                    foreach (WaggonCount wc in waggons_)
                    {
                        if (waggons.ContainsKey(wc.Type))
                        {
                            wc.Count = waggons[wc.Type];
                        }
                        else
                        {
                            wc.Count = 0;
                        }
                        //wc.MaxHaul = totalHaulCapacity(wc.Type.era());
                    }
                }
                OnPropertyChanged("Waggons");
            }
        }

        public void repairTrains( int minEra = 0, float maxReliability = 100 )
        {
            lock (this)
            {
                foreach (Train train in trains_.Where(t => t.Era >= minEra && t.Reliability < 100 && t.Reliability <= maxReliability))
                {
                    train.repair();
                }
            }
        }

        public void boostTrains(int minEra = 0, int hours = 1)
        {
            lock (this)
            {
                foreach (Train train in trains_.Where(t => t.Era >= minEra))
                {
                    train.boost(hours);
                }
            }
        }

        public void buyMechanic(int minEra = 0)
        {
            lock (this)
            {
                foreach (Train train in trains_.Where(t => t.Era >= minEra))
                {
                    train.mechanic();
                }
            }
        }

        public bool hasTechnology(int techId)
        {
            return technologies_.Contains(techId);
        }

        public bool hasCouplingFor(int era)
        {
            if (era > 1)
            {
                TrainUpgrade tu = TrainUpgrade.getCouplingTech(era - 1);
                if (tu != null)
                {
                    return hasTechnology(tu.Id);
                }
            }
            return false;
        }

        public int totalHaulCapacity(int era)
        {
            if (hasCouplingFor(era))
                --era;
            return trains_.Where(t => t.Era >= era).Sum(t => t.MaxWaggons);
        }

        public int waggonCount(ProductType pt)
        {
            return waggons_.First(w => w.Type == pt).Count;
        }

        public int missingWaggons(ProductType pt)
        {
            if (waggons_.Any(w => w.Type == pt))
            {
                WaggonCount wc = waggons_.First(w => w.Type == pt);
                if (wc.MaxHaul > wc.Count)
                    return wc.MaxHaul - wc.Count;
            }
            return 0;
        }

        public void setRoute(Train.Condition condition, params string[] stops)
        {
            refreshTrains();
            if (stops.Length < 2)
                return;

            // 1. Dermine min era
            int min_engine_era = 0;

            List<RouteStop> route = new List<RouteStop>();
            // 2. Calculate route
            RouteStop firstStop = null;
            RouteStop lastStop = null;
            foreach (string stopId in stops)
            {
                RouteStop currStop = new RouteStop
                {
                    dest_id = stopId,
                    scheduleType = 1,
                    wait = 0,
                };
                if (firstStop == null)
                    firstStop = currStop;

                LocationInfo loc = World.getLocation(stopId);
                if (!isLocationConnected(stopId))
                {
                    Trace.TraceWarning("{0} Location {2} is not connected", UserName, loc.Name);
                    return;
                }
                if (loc is Factory)
                {
                    Factory factory = loc as Factory;
                    currStop.loadType = factory.ProductType;

                    if (totalHaulCapacity(currStop.loadType.era()) > waggonCount(currStop.loadType))
                        Trace.TraceWarning("{0} Not enough waggons for {1} (needs {2} has {3})", UserName, 
                            currStop.loadType, totalHaulCapacity(currStop.loadType.era()), waggonCount(currStop.loadType));

                    if (currStop.loadType.era() > min_engine_era)
                        min_engine_era = currStop.loadType.era();
                }
                if (lastStop != null)
                {
                    currStop.unloadType = lastStop.loadType;
                    // Find path
                    Path p = findPath(lastStop.dest_id, currStop.dest_id);
                    // Add all path locations to route
                    if (p.Count < 2)
                        // TODO Throw an exception
                        return;
                    p.RemoveAt(0); // Start point
                    p.RemoveAt(p.Count - 1); // Goal point
                    foreach (string node in p)
                    {
                        route.Add(new RouteStop {
                            dest_id = node,
                            scheduleType = 1
                        });
                    }
                }
                // Add current location to the route
                lastStop = currStop;
                route.Add(lastStop);
            }
            // We don't load anythin at the last stop
            lastStop.loadType = ProductType.UNDEFINED;
            // Find path from last stop to the first one
            Path back = findPath(lastStop.dest_id, firstStop.dest_id);
            back.RemoveAt(0); // Start point
            back.RemoveAt(back.Count - 1); // Goal point
            // Add all path locations to route
            foreach (string node in back)
            {
                route.Add(new RouteStop
                {
                    dest_id = node,
                    scheduleType = 1
                });
            }

            //waggons_.Where(w => w.Type);
            lock (this)
            {
                if (hasCouplingFor(min_engine_era))
                    --min_engine_era;
                Debug.Print("Route length for user {0} is {1}", UserName, route.Count);
                IEnumerable<Train> trains = trains_.Where(t => t.Era >= min_engine_era && (condition == null || condition(t))).
                    OrderByDescending(t => t.MaxWaggons);
                foreach (Train train in trains)
                {
                    List<RouteStop> trainRoute = new List<RouteStop>();
                    string routeStart = train.routeStart();
                    // 3. For each filtered train calculate path to route start
                    Path p = findPath(routeStart, firstStop.dest_id);
                    if (p.Count < 2)
                        continue;
                    p.RemoveAt(p.Count - 1);

                    foreach (string node in p)
                    {
                        trainRoute.Add(new RouteStop
                        {
                            dest_id = node,
                        });
                    }
                    foreach (RouteStop stop in route)
                    {
                        trainRoute.Add(stop.copy(train.MaxWaggons));
                    }
                    // 4. Set route
                    train.setRoute(trainRoute);
                }
            }
        }

        public void parkTrains(Train.Condition condition, string locId)
        {
            lock (this)
            {
                //Debug.Print("Route length for user {0} is {1}", UserName, route.Count);
                IEnumerable<Train> trains = trains_.Where(t => condition == null || condition(t)).
                    OrderByDescending(t => t.MaxWaggons);
                foreach (Train train in trains)
                {
                    List<RouteStop> trainRoute = new List<RouteStop>();
                    string routeStart = train.routeStart();
                    // 3. For each filtered train calculate path to route start
                    Path p = findPath(routeStart, locId);
                    if (p.Count < 2)
                        continue;

                    foreach (string node in p)
                    {
                        trainRoute.Add(new RouteStop
                        {
                            dest_id = node,
                        });
                    }
                    // 4. Set route
                    train.setRoute(trainRoute);
                }
            }
        }

        public HashSet<string> connectionsFrom(string from)
        {
            return rails_.connectionsFrom(from);
        }

        public bool isLocationConnected(string locId)
        {
            return rails_.isLocationConnected(locId);
        }

        public Path findPath(string from, string to)
        {
            return rails_.findPath(from, to);
        }


        public int buyWaggon(ProductType pt, int count)
        {
            if (count > 0)
            {
                Trace.TraceInformation("{0} Buy {1} waggons of {2}", UserName, count, pt);
                JArray reply = commander_.exec<JArray>("WaggonInterface", "buyWaggon", (int)pt, count);
                if (reply != null)
                {
                    int bougtCount = reply.Count;
                    if (bougtCount == count)
                        Trace.TraceInformation("{0} Succesfully bought {1} waggons of {2}", UserName, count, pt);
                    else
                        Trace.TraceWarning("{0} Bought {1} waggons of {2} instead of requested {3}", UserName, bougtCount, pt, count);
                    refreshData();
                    refreshWaggons();
                    return bougtCount;
                }
                else
                {
                    Trace.TraceError("{0} Failed to buy {1} waggons of {2}", UserName, count, pt);
                }
            }
            return 0;
        }

        public void sellWaggon(ProductType pt, int count)
        {
            if (count > 0 && count <= waggonCount(pt))
            {
                Trace.TraceInformation("{0} Sell {1} waggons of {2}", UserName, count, pt);
                JValue reply = commander_.exec<JValue>("WaggonInterface", "sellWaggons", (int)pt, count);
                try
                {
                    bool success = Boolean.Parse(reply.ToString());
                    Trace.TraceInformation("{0} Sell {1} waggons of {2}: {3}", UserName, count, pt, success ? "SUCCESS" : "FAIL");
                }
                catch
                {
                }
                refreshData();
                refreshWaggons();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                if (Thread.CurrentThread.IsBackground)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
