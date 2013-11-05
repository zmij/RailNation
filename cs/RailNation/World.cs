using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml;

namespace RailNation
{
    using LocationById = Dictionary<string, LocationInfo>;
    using FactoryConatiner = LinkedList<Factory>;
    using FactoryByProductType = Dictionary<ProductType, LinkedList<Factory>>;
    using CityList = BindingSource<City>;

    public static class World
    {
        private static LocationById locationsById_ = new LocationById();
        private static CityList cityList_ = new CityList();
        private static FactoryByProductType factoriesByType_ = new FactoryByProductType();

        private static XmlDocument cityNames_;

        private enum State
        {
            NotInitialized,
            Initializing,
            Initialized
        }

        private static State state_ = State.NotInitialized;

        private enum LangIds
        {
            de  = 0,
            en,
            fr,
            sv,
            no,
            nl,
            da,
            ru,
            pt,
            tr,
            el,
            daDK,
            it,
            itIt,
            es,
            esES,
        }

        static World()
        {
            cityNames_ = new XmlDocument();
            cityNames_.LoadXml( RailNation.Properties.Resources.cityNames );
        }

        public static bool Initialized
        {
            get
            {
                return state_ == State.Initialized;
            }
        }

        public static CityList Cities
        {
            get
            {
                return cityList_;
            }
        }

        public static void init(GameCommander commander)
        {
            lock (locationsById_)
            {
                if (state_ != State.NotInitialized)
                    return;
                state_ = State.Initializing;
            }
            JArray tok = commander.exec<JArray>("LocationInterface", "get");
            if (tok != null)
            {
                JEnumerable<JToken> children = tok.Children();
                int cityCount = 0;
                int factoryCount = 0;
                Regex nameRe = new Regex("^Factory_.*");
                foreach (JToken locTok in children)
                {
                    JToken nameTok = locTok["name"];
                    if (nameTok != null)
                    {
                        LocationInfo loc = null;
                        if (nameRe.IsMatch(nameTok.ToString()))
                        {
                            ++factoryCount;
                            Factory fac = new Factory(locTok);
                            addFactory(fac);
                            loc = fac;
                        }
                        else
                        {
                            ++cityCount;
                            City city = new City(locTok);
                            cityList_.Add(city);
                            loc = city;
                        }
                        locationsById_.Add(loc.Id, loc);
                    }
                }
                state_ = State.Initialized;
                Trace.TraceInformation("{0} cities {1} factories", cityCount, factoryCount);
            }
        }

        private static void addFactory(Factory f)
        {
            if (!factoriesByType_.ContainsKey(f.ProductType))
            {
                factoriesByType_.Add(f.ProductType, new FactoryConatiner());
            }
            factoriesByType_[f.ProductType].AddLast(f);
        }

        public static string getCityName(int id)
        {
            LangIds li = (LangIds)Enum.Parse(typeof(LangIds), Config.COUNTRY_ID);

            string path = String.Format("//tu[@tuid='IDS_CITY_NAME_{0:000}_{1}']/tuv/seg", id, (int)li);

            XmlNode node = cityNames_.SelectSingleNode(path);

            return node != null ? node.InnerText : String.Format("City {0}", id);
        }

        public static LocationInfo getLocation(string id)
        {
            if (!String.IsNullOrEmpty(id) && locationsById_.ContainsKey(id))
                return locationsById_[id];
            return null;
        }

        public static FactoryConatiner getFactories(ProductType pType)
        {
            if (factoriesByType_.ContainsKey(pType))
                return factoriesByType_[pType];
            return null;
        }
    }
}
