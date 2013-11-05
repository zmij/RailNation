using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RailNation
{
    public class TrainLoad
    {
        private ProductType type_;
        private int amount_;

        public TrainLoad(JToken tok)
        {
            type_ = (ProductType)int.Parse(tok["type"].ToString());
            amount_ = int.Parse(tok["amount"].ToString());
        }

        public ProductType Type
        {
            get
            {
                return type_;
            }
        }

        public int Amount
        {
            get
            {
                return amount_;
            }
        }

        public override string ToString()
        {
            return type_.ToString() + " x " + amount_.ToString();
        }
    }
}
