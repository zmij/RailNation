using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailNation
{
    public class ScheduleLoad
    {
        public int load { get; set; }
        public int type { get; set; }
        public int unload { get; set; }

        public ScheduleLoad copy(int waggonCount)
        {
            return new ScheduleLoad
            {
                load = load == 0 ? 0 : waggonCount,
                type = type,
                unload = unload == 0 ? 0 : waggonCount
            };
        }
    }
}
