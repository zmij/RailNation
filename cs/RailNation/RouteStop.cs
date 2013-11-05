using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RailNation
{
    public class RouteStop
    {
        public List<ScheduleLoad> loading {
            get
            {
                List<ScheduleLoad> val = new List<ScheduleLoad>();
                if (unloadType != ProductType.UNDEFINED && waggonCount > 0)
                {
                    val.Add(
                        new ScheduleLoad
                        {
                            load = 0,
                            type = (int)unloadType,
                            unload = waggonCount
                        });
                }
                if (loadType != ProductType.UNDEFINED && waggonCount > 0)
                {
                    val.Add(
                        new ScheduleLoad
                        {
                            load = waggonCount,
                            type = (int)loadType,
                            unload = 0
                        });
                }
                return val;
            }
        }
        public int scheduleType { get; set; }
        public string dest_id { get; set; }
        public int wait { get; set; }

        [JsonIgnore]
        public ProductType loadType { get; set; }
        [JsonIgnore]
        public ProductType unloadType { get; set; }
        [JsonIgnore]
        public int waggonCount { get; set; }

        public RouteStop()
        {
            wait = -1;
            scheduleType = 2;
            loadType = ProductType.UNDEFINED;
            unloadType = ProductType.UNDEFINED;
            waggonCount = 0;
        }

        public RouteStop copy( int waggonCnt )
        {
            return new RouteStop { 
                dest_id = dest_id,
                wait = wait,
                scheduleType = scheduleType,
                loadType = loadType,
                unloadType = unloadType,
                waggonCount = waggonCnt
            };
        }
    }
}
