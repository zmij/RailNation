using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RailNation
{
    public class TrainStats
    {
        private static Dictionary<EngineType, TrainStats> stats_ = new Dictionary<EngineType,TrainStats>();

        private EngineType type_;
        private int speed_;
        private int acceleration_;
        private int endurance_;
        private int maxWaggons_;
        private int slotCount_;
        private int price_;

        private int art_;

        public static TrainStats getStats(EngineType type)
        {
            if (stats_.ContainsKey(type))
            {
                return stats_[type];
            }
            return null;
        }

        public static TrainStats getStats(EngineType type, HashSet<int> upgrades)
        {
            TrainStats stats = getStats(type);
            if (stats != null)
                return getStats(type).ApplyUpgrades(upgrades);
            return null;
        }

        internal static void addStats(JProperty prop)
        {
            EngineType et = (EngineType)int.Parse(prop.Name);
            JObject tok = prop.Value as JObject;
            if (tok != null)
            {
                TrainStats stats = new TrainStats(et, tok);
                stats_.Add(et, stats);
            }
        }

        private TrainStats(EngineType type, JObject tok)
        {
            type_ = type;
            speed_ = tok["speed"].ToObject<int>();
            acceleration_ = tok["acc"].ToObject<int>();
            endurance_ = tok["endurance"].ToObject<int>();
            maxWaggons_ = tok["max_num_waggons"].ToObject<int>();
            slotCount_ = tok["train_slots"].ToObject<int>();
            price_ = tok["price"].ToObject<int>();
            art_ = tok["art"].ToObject<int>();
        }

        private TrainStats(TrainStats rhs)
        {
            type_ = rhs.type_;
            speed_ = rhs.speed_;
            acceleration_ = rhs.acceleration_;
            endurance_ = rhs.endurance_;
            maxWaggons_ = rhs.maxWaggons_;
            slotCount_ = rhs.slotCount_;
            price_ = rhs.price_;
            art_ = rhs.art_;
        }

        public TrainStats ApplyUpgrades(HashSet<int> upgrades)
        {
            TrainStats stats = new TrainStats(this);
            foreach (int uid in upgrades)
            {
                TrainUpgrade up = TrainUpgrade.getUpgrade(uid);
                if (up != null)
                {
                    switch (up.Type)
                    {
                        case UpgradeType.Acceleration:
                            {
                                stats.acceleration_ += up.Effect;
                                break;
                            }
                        case UpgradeType.Power:
                            {
                                stats.maxWaggons_ += up.Effect;
                                break;
                            }
                        case UpgradeType.Speed:
                            {
                                stats.speed_ += up.Effect;
                                break;
                            }
                        case UpgradeType.Reliability:
                            {
                                stats.endurance_ += up.Effect;
                                break;
                            }
                    }
                }
            }
            return stats;
        }

        public EngineType EngineType
        {
            get
            {
                return type_;
            }
        }

        public int Speed
        {
            get
            {
                return speed_;
            }
        }
        public int Acceleration
        {
            get
            {
                return acceleration_;
            }
        }
        public int Endurance
        {
            get
            {
                return Endurance;
            }
        }
        public int MaxWaggons
        {
            get
            {
                return maxWaggons_;
            }
        }
        public int SlotCount
        {
            get
            {
                return slotCount_;
            }
        }
        public int Price
        {
            get
            {
                return price_;
            }
        }
        public int Art
        {
            get
            {
                return art_;
            }
        }

        public override string ToString()
        {
            String s = String.Format("{0}(s{1} a{2} w{3} e{4})", type_, speed_, acceleration_, maxWaggons_, endurance_);
            return s;
        }
    }
}
