using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace RailNation
{
    public class Train : INotifyPropertyChanged
    {
        private GameCommander commander_;

        private string id_;
        private EngineType type_;

        private int baseNumWaggons_ = 0;
        private int maxWaggons_ = 0;

        private RouteLoad load_ = new RouteLoad();

        private float reliability_;
        private int profitLastHour_;
        private int profitToday_;

        private string currentLocation_;
        private string nextLocation_;
        private string nextVisibleLocation_;

        private DateTime mechanicEnd_ = DateTime.Now;
        private DateTime boostEnd_ = DateTime.Now;

        private HashSet<int> appliedUpgrades_ = new HashSet<int>();
        private TrainStats stats_;

        public delegate bool Condition(Train t);

        public Train(GameCommander commander, JToken tok)
        {
            commander_ = commander;
            id_ = tok["ID"].ToString();
            type_ = (EngineType)int.Parse(tok["type"].ToString());
            baseNumWaggons_ = int.Parse(tok["max_num_waggons_base"].ToString());
            refresh(tok);
        }

        [Browsable(false)]
        public string Id
        {
            get
            {
                return id_;
            }
        }

        public EngineType Type
        {
            get
            {
                return type_;
            }
        }

        public int Era
        {
            get
            {
                return type_.era();
            }
        }

        public RouteLoad Load
        {
            get
            {
                return load_;
            }
        }

        public float Reliability
        {
            get
            {
                return reliability_;
            }
            private set
            {
                if (reliability_ != value)
                {
                    reliability_ = value;
                    OnPropertyChanged("Reliability");
                }
            }
        }

        public int ProfitLastHour
        {
            get
            {
                return profitLastHour_;
            }
            private set
            {
                if (profitLastHour_ != value)
                {
                    profitLastHour_ = value;
                    OnPropertyChanged("ProfitLastHour");
                }
            }
        }

        public int ProfitToday
        {
            get
            {
                return profitToday_;
            }
            private set
            {
                if (profitToday_ != value)
                {
                    profitToday_ = value;
                    OnPropertyChanged("ProfitToday");
                }
            }
        }

        public int MaxWaggons
        {
            get
            {
                TrainStats stats = Stats;
                if (stats != null)
                    return stats.MaxWaggons;
                return maxWaggons_;
            }
        }

        public TrainStats Stats
        {
            get
            {
                if (stats_ == null)
                {
                    stats_ = TrainStats.getStats(type_, appliedUpgrades_);
                }
                return stats_;
            }
        }

        public bool HasMechanic
        {
            get
            {
                return mechanicEnd_.CompareTo(DateTime.Now) > 0;
            }
        }

        [Browsable(false)]
        public DateTime MechanicEnd
        {
            get
            {
                return mechanicEnd_;
            }
            private set
            {
                if (mechanicEnd_.CompareTo(value) != 0)
                {
                    mechanicEnd_ = value;
                    OnPropertyChanged("MechanicEnd");
                    OnPropertyChanged("MechanicTime");
                    OnPropertyChanged("HasMechanic");
                }
            }
        }

        public TimeSpan MechanicTime
        {
            get
            {
                return mechanicEnd_.Subtract(DateTime.Now);
            }
        }

        [Browsable(false)]
        public DateTime BoostEnd
        {
            get
            {
                return boostEnd_;
            }
            private set
            {
                if (boostEnd_.CompareTo(value) != 0)
                {
                    boostEnd_ = value;
                    OnPropertyChanged("BoostEnd");
                    OnPropertyChanged("BoostTime");
                    OnPropertyChanged("HasBoost");
                }
            }
        }

        public bool HasBoost
        {
            get
            {
                return boostEnd_.CompareTo(DateTime.Now) > 0;
            }
        }

        public TimeSpan BoostTime
        {
            get
            {
                return boostEnd_.Subtract(DateTime.Now);
            }
        }

        [Browsable(false)]
        public string CurrentLocationId
        {
            get
            {
                return currentLocation_;
            }
            private set
            {
                if (currentLocation_ != value)
                {
                    currentLocation_ = value;
                    OnPropertyChanged("CurrentLocationId", "CurrentLocation");
                }
            }
        }

        public LocationInfo CurrentLocation
        {
            get
            {
                return World.getLocation(currentLocation_);
            }
        }

        [Browsable(false)]
        public string NextLocationId
        {
            get
            {
                return nextLocation_;
            }
            private set
            {
                if (nextLocation_ != value)
                {
                    nextLocation_ = value;
                    OnPropertyChanged("NextLocationId", "NextLocation");
                }
            }
        }

        public LocationInfo NextLocation
        {
            get
            {
                return World.getLocation(nextLocation_);
            }
        }

        [Browsable(false)]
        public string NextVisibleLocationId
        {
            get
            {
                return nextVisibleLocation_;
            }
            private set
            {
                if (nextVisibleLocation_ != value)
                {
                    nextVisibleLocation_ = value;
                    OnPropertyChanged("NextVisibleLocationId", "NextVisibleLocation");
                }
            }
        }

        public LocationInfo NextVisibleLocation
        {
            get
            {
                return World.getLocation(nextVisibleLocation_);
            }
        }

        public void refresh()
        {
        }

        public void refresh(JToken tok)
        {
            if (tok["reliability"] != null)
                Reliability = float.Parse(tok["reliability"].ToString());

            if (tok["profit_last_hour"] != null)
                ProfitLastHour = int.Parse(tok["profit_last_hour"].ToString());

            if (tok["profit_today"] != null)
                ProfitToday = int.Parse(tok["profit_today"].ToString());

            if (tok["mechanic_end"] != null)
            {
                double end = double.Parse(tok["mechanic_end"].ToString());
                MechanicEnd = DateTime.Now.AddSeconds(end);
            }
            if (tok["boost_end"] != null)
            {
                double boostEnd = double.Parse(tok["boost_end"].ToString());
                BoostEnd = DateTime.Now.AddSeconds(boostEnd);
            }

            if (tok["waggons"] != null)
            {
                load_.refresh(tok["waggons"]);
                if (baseNumWaggons_ > maxWaggons_)
                {
                    maxWaggons_ = baseNumWaggons_;
                }
                if (load_.MaxLoad > maxWaggons_)
                {
                    maxWaggons_ = load_.MaxLoad;
                }
                OnPropertyChanged("Load");
            }

            if (tok["upgrades"] != null && tok["upgrades"] is JArray)
            {
                JArray upgrades = tok["upgrades"] as JArray;
                if (upgrades.Count > 0)
                {
                    int new_ups = 0;
                    foreach (JToken u in upgrades)
                    {
                        int uid = u["id"].ToObject<int>();
                        if (!appliedUpgrades_.Contains(uid))
                        {
                            appliedUpgrades_.Add(uid);
                            ++new_ups;
                        }
                    }
                    if (new_ups > 0)
                    {
                        stats_ = null;
                        OnPropertyChanged("Stats");
                    }
                }
            }

            if (tok["navigation"] != null)
            {
                CurrentLocationId = tok["navigation"]["current_location_id"].ToString();
                NextLocationId = tok["navigation"]["next_location_id"].ToString();
                NextVisibleLocationId = tok["navigation"]["next_visible_location_id"].ToString();
            }
        }

        private T post<T>(string command, params object[] data) where T : JToken
        {
            object[] args = new object[data.Length + 1];
            args[0] = id_;
            data.CopyTo(args, 1);
            return commander_.exec<T>("TrainInterface", command, args);
        }

        public void repair()
        {
            Trace.TraceInformation("{0} Repair train {1} ({2})", commander_.Owner.UserName, type_, id_);
            post<JToken>("doMaintenance");
        }
        public void boost(int hours = 1)
        {
            Trace.TraceInformation("{0} Boost train {1} ({2}) for {3} hours", commander_.Owner.UserName, type_, id_, hours);
            for (int i = 0; i < hours; ++i)
            {
                post<JToken>("buyBoost");
            }
        }
        public void mechanic()
        {
            Trace.TraceInformation("{0} Buy mechanic for train {1} ({2})", commander_.Owner.UserName, type_, id_);
            post<JToken>("buyMechanic");
        }

        public string routeStart()
        {
            JValue val = post<JValue>("getTrainLocation");
            if (val != null)
                return val.ToString();
            return CurrentLocationId;
        }

        public void setRoute(List<RouteStop> route)
        {
            Trace.TraceInformation("{0} Set route for train {1} ({2}) = {3}", commander_.Owner.UserName, type_, id_, route.ToJson());
            JValue val = post<JValue>("setRoadMap", route);
            if (val != null)
            {
                try
                {
                    bool res = Boolean.Parse(val.ToString());
                    if (!res)
                    {
                        Trace.TraceWarning("{0} Retry set route for train {1} ({2})", commander_.Owner.UserName, type_, id_);
                        val = post<JValue>("setRoadMap", route);
                        res = Boolean.Parse(val.ToString());
                    }
                    Trace.TraceInformation("{0} Set route for train {1} ({2}): {3}", commander_.Owner.UserName, type_, id_, res ? "SUCCESS" : "FAIL");
                }
                catch (Exception e)
                {
                    Trace.TraceError("{0} Unexpected setRoadMap reply {1}", commander_.Owner.UserName, val.ToString());
                }
            }
        }

        public override string ToString()
        {
            return String.Format( "{0}({1}) {2}", type_, Era, load_) ;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                if (Thread.CurrentThread.IsBackground)
                {
                    foreach(string propertyName in propertyNames)
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    foreach (string propertyName in propertyNames)
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
