using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailNation
{
    using Load = List<TrainLoad>;
    using Newtonsoft.Json.Linq;
    public class RouteLoad
    {
        private Load load_ = new Load();
        private int maxLoad_ = 0;

        public RouteLoad()
        {
        }
        public RouteLoad(JToken tok)
        {
            refresh(tok);
        }

        public int MaxLoad
        {
            get
            {
                return maxLoad_;
            }
        }

        public int StopsCount
        {
            get
            {
                return load_.Count;
            }
        }

        public void refresh(JToken tok)
        {
            load_.Clear();
            maxLoad_ = 0;
            JEnumerable<JToken> children = tok.Children();
            foreach (JToken child in children)
            {
                TrainLoad l = new TrainLoad(child);
                load_.Add(l);
                if (l.Amount > maxLoad_)
                    maxLoad_ = l.Amount;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (TrainLoad l in load_)
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.Append(l);
            }
            if (sb.Length == 0)
                return "IDLE";
            return sb.ToString();
        }
    }
}
