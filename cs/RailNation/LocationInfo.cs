using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace RailNation
{
    public abstract class LocationInfo
    {
        string id_;
        int level_;

        int x_ = 0;
        int y_ = 0;

        public LocationInfo(JToken tok)
        {
            id_ = tok["ID"].ToString();
            level_ = int.Parse(tok["level"].ToString());
            x_ = int.Parse(tok["x"].ToString());
            y_ = int.Parse(tok["y"].ToString());
        }

        [Browsable(false)]
        public string Id
        {
            get
            {
                return id_;
            }
        }

        public abstract string Name
        {
            get;
        }

        public int Level
        {
            get
            {
                return level_;
            }
        }

        [Browsable(false)]
        public int X
        {
            get
            {
                return x_;
            }
        }

        [Browsable(false)]
        public int Y
        {
            get
            {
                return y_;
            }
        }

        public int manhattanDistance(LocationInfo loc)
        {
            return Math.Abs(x_ - loc.x_) + Math.Abs(y_ - loc.y_);
        }

        public abstract void updateDetails(GameCommander commander);

        public override string ToString()
        {
            return Name;
        }
    }
}
