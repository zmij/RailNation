using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace RailNation
{
    public class City : LocationInfo
    {
        private string name_;
        public City(JToken tok)
            : base(tok)
        {
            int nameIdx = int.Parse(tok["name"].ToString());
            name_ = World.getCityName(nameIdx);
        }

        public override string Name
        {
            get
            {
                return name_;
            }
        }
        public override void updateDetails(GameCommander commander)
        {
        }
    }
}
