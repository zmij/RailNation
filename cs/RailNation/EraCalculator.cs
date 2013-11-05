using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailNation
{
    public static class EraCalculator
    {
        public static int era(this ProductType pt)
        {
            if (pt >= ProductType.Electronics)
                return 6;
            if (pt >= ProductType.Bauxite)
                return 5;
            if (pt >= ProductType.SheetMetal)
                return 4;
            if (pt >= ProductType.CopperOre)
                return 3;
            if (pt == ProductType.Textiles ||
                pt == ProductType.Meat ||
                pt == ProductType.Hardware ||
                pt == ProductType.Paper ||
                pt >= ProductType.Pastries)
                return 2;
            if (pt > ProductType.UNDEFINED)
                return 1;
            return 0;
        }

        public static int era(this EngineType eng)
        {
            int num = (int)eng;
            if (num <= 0)
                return 0;
            if (num <= 30)
                return (int)Math.Floor( (double)((num - 1) / 5) ) + 1;
            if (num <= 37)
                return num - 30;
            if (num == 38)
                return 6;
            return 0;
        }

        public static bool canHaul(this EngineType e, ProductType p)
        {
            return e.era() >= p.era();
        }
    }
}
