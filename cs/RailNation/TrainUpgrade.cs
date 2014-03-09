using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RailNation
{
    public class TrainUpgrade
    {
        private int id_;
        private UpgradeType type_;
        private int effect_;
        private int cost_;

        private TrainUpgrade(int id, JObject tok)
        {
            id_ = id;
            type_ = (UpgradeType)tok["type"].ToObject<int>();
            effect_ = tok["effect"].ToObject<int>();
            cost_ = tok["cost"].ToObject<int>();
        }

        public int Id
        {
            get
            {
                return id_;
            }
        }

        public UpgradeType Type
        {
            get
            {
                return type_;
            }
        }

        public int Effect
        {
            get
            {
                return effect_;
            }
        }

        public int Cost
        {
            get
            {
                return cost_;
            }
        }

        public int Era
        {
            get
            {
                return (id_ / 10000) % 10;
            }
        }

        public override string ToString()
        {
            return String.Format("{1} +{2}", type_, effect_);
        }

        private static Dictionary<int, TrainUpgrade> upgrades_ = new Dictionary<int,TrainUpgrade>();
        private static Dictionary<int, TrainUpgrade> couplingUpgrades_ = new Dictionary<int, TrainUpgrade>();

        internal static void addUpgrade(JProperty prop)
        {
            int id = int.Parse(prop.Name);
            JObject tok = prop.Value as JObject;
            if (tok != null)
            {
                TrainUpgrade tu = new TrainUpgrade(id, tok);
                upgrades_.Add(id, tu);
                if (tu.Type == UpgradeType.Hitch)
                {
                    couplingUpgrades_.Add(tu.Era, tu);
                }
            }
        }

        public static TrainUpgrade getUpgrade(int id)
        {
            if (upgrades_.ContainsKey(id))
            {
                return upgrades_[id];
            }
            return null;
        }

        public static TrainUpgrade getCouplingTech(int era)
        {
            if (couplingUpgrades_.ContainsKey(era))
            {
                return couplingUpgrades_[era];
            }
            return null;
        }
    }
}
