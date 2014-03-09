using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        private static Regex TRAIN_ERA_RE = new Regex("[1,3](\\d)\\d{4,4}");

        public static int era(this EngineType eng)
        {
            int num = (int)eng;
            String engine = num.ToString();
            Match m = TRAIN_ERA_RE.Match(engine);
            if (m.Success)
            {
                return int.Parse(m.Groups[1].Value);
            }
            return 0;
        }

        // TODO Check hooks 
        public static bool canHaul(this EngineType e, ProductType p)
        {
            return e.era() >= p.era();
        }
    }
}
