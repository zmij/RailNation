using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailNation
{
    public enum EngineType
    {
        UNDEFINED   = 0,
        #region Era 1
	    Swallow         = 110100,
	    Raven           = 110200,
	    Rhinoceros      = 110300,
        Donkey          = 110400,
        Falcon          = 110500,
        Mole            = 110600,
        #endregion
        #region Era 2
	    Bat             = 120100,
        Panther         = 120200,
        BlackBear       = 120300,
        Lynx            = 120400,
        Boar            = 120500,
        Elephant        = 120600,
        #endregion
        #region Era 3
	    Odin            = 130100,
	    Dionysos        = 130200,
	    Herakles        = 130300,
	    Prometheus      = 130400,
	    Osiris          = 130500,
        Moprpheus       = 130600,
        #endregion
        #region Era 4
        Apollo          = 140100,
        Aries           = 140200,
        Neptune         = 140300,
        Horus           = 140400,
        Thor            = 140500,
        Poseidon        = 140600,
        #endregion
        #region Era 5
        Unicorn         = 150100,
        Medusa          = 150200,
        Basilisk        = 150300,
        Satyr           = 150400,
        Leviathan       = 150500,
        Centaur         = 150600,
        #endregion
        #region Era 6
	    Ogre            = 160100,
	    Phoenix         = 160200,
	    Pegasus         = 160300,
	    Lindworm        = 160400,
	    LernaeanHydra   = 160500,
	    Olymp           = 160600,
        #endregion
        #region Bonus engines
	    RedKite         = 310100,
	    Bull            = 320100,
	    Isis            = 330100,
	    Zeus            = 340100,
	    Sphinx          = 350100,
	    Valkyrie        = 360100,
	    Titan           = 360200,
        #endregion
    }
}
