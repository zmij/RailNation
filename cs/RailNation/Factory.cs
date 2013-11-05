using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RailNation
{
    public class Factory : LocationInfo
    {
        private string cityId_;
        private CityDirection cityDirection_;
        private ProductType productType_;

        public Factory(JToken tok)
            : base(tok)
        {
            cityId_ = tok["city_location_id"].ToString();
            cityDirection_ = (RailNation.CityDirection)int.Parse(tok["city_direction"].ToString());
            productType_ = (RailNation.ProductType)int.Parse(tok["factoryType"].ToString());
        }

        public string CityId
        {
            get
            {
                return cityId_;
            }
        }
        public CityDirection CityDirection
        {
            get
            {
                return cityDirection_;
            }
        }
        public ProductType ProductType
        {
            get
            {
                return productType_;
            }
        }

        public override string Name
        {
            get
            {
                LocationInfo city = World.getLocation(cityId_);
                return String.Format("{0} {1} - {2}",
                    city != null ? city.Name : cityId_, cityDirection_, productType_);
            }
        }

        public string ShortName
        {
            get
            {
                LocationInfo city = World.getLocation(cityId_);
                return String.Format("{0} {1}",
                    city != null ? city.Name : cityId_, cityDirection_);
            }
        }

        public override void updateDetails(GameCommander commander)
        {
        }
    }
}
