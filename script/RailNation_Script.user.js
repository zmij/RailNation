// ==UserScript==
// @name        RailNation Script
// @namespace   RN
// @description Bulk train commands, numbers watching. WARNING!!! Consumes a lot of gold!
// @include     http://*.railnation.ru/web/?key=*
// @version     1.4.0.1
// @downloadURL	https://github.com/Vany/RailNation/raw/master/script/RailNation_Script.user.js
// @updateURL	https://github.com/Vany/RailNation/raw/master/script/RailNation_Script.user.js
// @author		sergei.a.fedorov at gmail dot com
// @require     http://crypto-js.googlecode.com/svn/tags/3.1.2/build/rollups/md5.js
// @grant       GM_getValue
// @grant       GM_setValue
// @grant       GM_registerMenuCommand
// @grant       GM_xmlhttpRequest
// ==/UserScript==

var RES_IDS = {
    '1'   : 'Coal',
    '2'   : 'Grain',
    '3'   : 'Iron',
    '4'   : 'Wood',
    '5'   : 'Boards',
    '6'   : 'Iron Ore',
    '7'   : 'Cotton',
    '8'   : 'Textiles',
    '9'   : 'Cattle',
    '10'  : 'Meat',
    '11'  : 'Thread',
    '12'  : 'Hardware',
    '13'  : 'Paper',
    '14'  : 'Leather',
    '15'  : 'Pastries',
    '16'  : 'Flour',
    '17'  : 'Copper Ore',
    '18'  : 'Quartz',
    '19'  : 'Copper',
    '20'  : 'Steel',
    '21'  : 'Shoes',
    '22'  : 'Glassware',
    '23'  : 'Wires',
    '24'  : 'Pipes',
    '25'  : 'Packaging',
    '26'  : 'Windows',
	'27'  : 'Steel plates',
	'28'  : 'Silicone',
	'29'  : 'Food',
	'30'  : 'Oil',
	'31'  : 'Lamps',
	'32'  : 'Chemicals',
	'33'  : 'Clothes',
	'34'  : 'Stainless steel',
	'35'  : 'Boxites',
	'36'  : 'Engines',
	'37'  : 'Plastic',
	'38'  : 'Aluminium',
	'39'  : 'Ceramics',
	'40'  : 'Steel beams',
	'41'  : 'Gasoline',
	'42'  : 'Cars',
	'43'  : 'Kitchenware',
	'44'  : 'Electronics',
	'45'  : 'Toys',
	'46'  : 'Sport goods',
	'47'  : 'Assware',
	'48'  : 'Drugs'
};

var REQ_QTYS = {
	"1" : 2344,
	"4" : 3550,
	"5" : 4086,
	"6" : 4689,
	"7" : 5359,
	"8" : 6029,
	"9" : 6699,
	"10" : 7369,
	"11" : 8039,
	"12" : 8709,
	"13" : 9379,
	"14" : 10049,
	"15" : 10719,
	"16" : 11389,
	"17" : 12059,
	"18" : 12729,
	"19" : 13399,
	"20" : 14069,
	"21" : 14739,
	"22" : 15409,
	"23" : 16079,
	"24" : 16749,
	"25" : 17319,
	"26" : 18089,
	"27" : 18759,
	"28" : 19429,
	"29" : 20099,
	"30" : 20769,
	"31" : 21439,
	"32" : 22109,
	"33" : 22779,
	"34" : 23449,
	"35" : 24119,
	"36" : 24789,
	"37" : 25459,
	"38" : 26129,
	"39" : 26799,
	"40" : 27469,
	"41" : 28139,
	"42" : 28809,
	"43" : 29479,
	"44" : 30149,
	"45" : 33500,
	"46" : 36849,
	"47" : 40199,
	"48" : 43549,
	"49" : 46899,
	"50" : 50249,
	"51" : 53599,
	"52" : 56949,
	// 53 - ?
	"54" : 63649,
	"55" : 67000
};

var BUILDING_IDS = {
    "1"     : 'Construction yard',
    "2"     : 'Track production',
    "3"     : 'Engine house',
    "4"     : 'Bank',
    "5"     : 'Laboratory',
    "6"     : 'License trade',
    "8"     : 'Station concourse',
    "9"     : 'Shopping centre',
    "10"    : 'Hotel',
    "11"    : 'Restaurant',
    "12"    : 'Lottery'
};

var COLLECTIBLE_BUILDINGS = [ 9, 10, 11 ];

var ENGINE_IDS = {
	// Era 1
	'1'	: 'Swallow',
	'2'	: 'Raven',
	'3'	: 'Rhinoceros',
	'4'	: 'Mole',
	'5'	: 'Falcon',
	'31'	: 'Red kite',
	// Era 2
	'6'	: 'Bat',
	'7'	: 'Lynx',
	'8'	: 'Black bear',
	'9'	: 'Panther',
	'10'	: 'Elephant',
	'32'	: 'Bull',
	// Era 3
	'11'	: 'Odin',
	'12'	: 'Dionysos',
	'13'	: 'Herakles',
	'14'	: 'Prometheus',
	'15'	: 'Osiris',
	'33'	: 'Isis',
	// Era 4
	'16'	: 'Apollo',
	'17'	: 'Aries',
	'18'	: 'Neptune',
	'19'	: 'Horus',
	'20'	: 'Thor',
	'34'	: 'Zeus',
	// Era 5
	'21'	: 'Unicorn',
	'22'	: 'Medusa',
	'23'	: 'Basilisk',
	'24'	: 'Satyr',
	'25'	: 'Leviathan',
	'35'	: 'Sphinx',
	// Era 6
	'26'	: 'Ogre',
	'27'	: 'Phoenix',
	'28'	: 'Pegasus',
	'29'	: 'Lindworm',
	'30'	: 'Lernaean Hydra',
	'38'	: 'Wotan',
	'36'	: 'Valkyrie',
	// Final
	'37'	: 'Titan'
};

var ENGINE_BY_ERAS = {
	'1'	: [  1,  2,  3,  4,  5, 31 ],
	'2'	: [  6,  7,  8,  9, 10, 32],
	'3'	: [ 11, 12, 13, 14, 15, 33 ],
	'4'	: [ 16, 17, 18, 19, 20, 34],
	'5'	: [ 21, 22, 23, 24, 25, 35 ],
	'6'	: [ 26, 27, 28, 29, 30, 38, 36 ],
	'7'	: [ 37 ]
};

var BONUS_TRAINS = [ 31, 32, 33, 34, 35, 36, 37 ];

function resource_name (res_type) {
    var res_name = RES_IDS[res_type];
	if (!res_name) {
		return "Res id " + res_type;
	}
	return res_name;
}

function engine_name (engine_type) {
	var engine_name = ENGINE_IDS[engine_type];
	if (!engine_name) {
		return "Train type " + engine_type;
	}
	return engine_name + ' (' + engine_type + ')';
}

function engine_era(engine_type) {
	if (engine_type <= 0) {
		return 0;
	} else if (engine_type <= 30) {
		return Math.floor( (engine_type - 1) / 5 ) + 1;
	} else if (engine_type <= 37 ) {
		return engine_type - 30;
	} else if (engine_type == 38) { // Wotan
		return 6;
	}
}

function consume_amount(level) {
	return REQ_QTYS[level];
}

// Requests
function xpath(query, object) {
    if(!object) var object = document;
    return document.evaluate(query, object, null, XPathResult.ANY_TYPE, null);
}


var checksum    = null;
var key         = null;
var api         = null;
var referer     = null;
var user_id     = null;

var collect_bonuses = GM_getValue( 'CollectBonuses', 1 );

function postCommand(iface, method, params, resp_func) {
    try {
        var data = {
            checksum    : checksum,
            client      : 1,
            parameters  : params,
            hash        : "" + CryptoJS.MD5(JSON.stringify(params))
        };
        GM_xmlhttpRequest({
            method:         "POST",
            url:            api + "?interface=" + iface + "&method=" + method,
            headers:        {
                "Referer"       : referer,
                "Content-Type"  : "application/json",
                "Accept"        : "application/json"
            },
            data            : JSON.stringify(data),
            onload          : function(response) {
                if (resp_func != null) {
                    resp_func(response.responseText);
                }
            }
        });
    } catch (e) {
        console.error(e);
    }
}

function logRespJson(respText) {
    console.log( JSON.parse(respText) );
}
// Данные по городу
/*
POST http://s4.railnation.ru/web/rpc/flash.php?interface=TownInterface&method=getDetails
{"checksum":"{checksum}","client":1,"parameters":["{town id}"]}
*/
function requestTownDetails(townId) {
    console.log( "Request details for town %s", townId );
    postCommand( 
        "TownInterface", 
        "getDetails", 
        [ townId ],
        function( resp_text ) {
            var obj = JSON.parse(resp_text);
            console.log(obj);
            // Выведем в алерт потребление приоритетных ресурсов
            var msg = "Resource consumtion:";
			var consumption = consume_amount(obj.Body.town.level);
            for (var i = 0; i < obj.Body.resources.length; i++) {
                var o = obj.Body.resources[i];
                if (o.priority == 1) {
                    console.log("Resourse %s is 1st priority\n%s", 
                        resource_name(o.resource_type), JSON.stringify(o));
                    var percent = o.amount != 0 ? (o.consume_amount / o.amount) : 0;
                    console.log("Consume to amount %.2f, consume to capacity %.2f", percent * 100, (o.consume_amount / o.capacity * 100));
					var required = (consumption / (1 - percent)).toFixed();
                    msg += "\n" + resource_name(o.resource_type) + " consumption " + o.consume_amount + " (" + (percent * 100).toFixed(2) 
						+ "%) required amount " + required + " difference " + (o.amount - required);
                }
            }
            alert(msg);
        }
    );
}

function trainAction ( method, trainId )
{
    console.log( "Call train method %s for train %s", method, trainId );
    postCommand(
        'TrainInterface',
        method,
        [ trainId ],
        logRespJson // TODO replace with null
    );
}

function allTrainsAction( trainList, method, condition ) {
    for (var i = 0; i < trainList.length; i++) {
        var train = trainList[i];
        if (condition == null || condition(train)) {
            trainAction( method, train.ID );
        }
    }
}

// Список поездов
/*
POST http://s4.railnation.ru/web/rpc/flash.php?interface=TrainInterface&method=getTrains
{"checksum":"{checksum}","client":1,"parameters":[true,"User id"]}
*/
function processTrains(method, condition, uid) {
    console.log( "Process trains list" );
	if (uid == null) {
		uid = user_id;
	}
	
    if (uid != null) {
        postCommand(
            'TrainInterface',
            'getTrains',
            [ true, uid ],
            function( resp_text ) {
                var obj = JSON.parse(resp_text);
                var trainList = obj.Body;
                console.log(trainList);
                allTrainsAction(trainList, method, condition);
            }
        );
    } else {
        alert("No user id, try to reload page");
    }
}

function needsMaintenance( train ) {
    return train.maintenance_costs > 0;
}

function needsMechanic( train ) {
    return train.mechanic_end < 0;
}

function canCancelWait( train ) {
	return train.navigation.location_wait_time > 1;
}
/**
 * Messaging
 */
function sendMessage( rcpts, text ) {
    postCommand(
        'MessageInterface',
        'send',
        ["", text, rcpts, "00000000-0000-0000-0000-000000000000"],
        null
    );
}

/**
 * Station funcs
 */
function processStation( member, stationHandler ) {
    var uid;
    
    if (member != null) {
        uid = member.user_id;
    } else {
        uid = user_id;
    }
    
    if (uid != null) {
        postCommand(
            'BuildingsInterface',
            'getAll',
            [uid],
            function(resp_text) {
                var obj = JSON.parse(resp_text);
                if (stationHandler != null) {
                    stationHandler(member, obj.Body);
                } else {
                    console.log(obj.Body);
                }
            }
        );
    }
}

function logBuildingLevels ( member, station ) {
    if (member != null) {
        console.log(member.name);
    }
    console.log(station);
    for (var id in station) {
        var name = BUILDING_IDS[id];
        var bld = station[id];
        
        console.log( name + ' level ' + bld.level + ' of ' + bld.maxLevel +
            (bld.finished != 0 ? ' under construction' : ''));
    }
}

function collectBuildingLevels ( member, station, agg ) {
    agg.table += "<tr>"
    if (member != null) {
        //agg.str += "<b>" + member.name + "</b><br/>\n";
        agg.table += "<th>" + member.name + "</th>";
        console.log(member.name);
    } else {
        agg.table += "<th></th>";
    }

    station['user_name'] = member.name;
    console.log(station);
    for (var id in BUILDING_IDS) {
        var name = BUILDING_IDS[id];
        var bld = station[id];

        //console.log( name + ' level ' + bld.level + ' of ' + bld.maxLevel +
        //    (bld.finished != 0 ? ' under construction' : '') + (bld.productionTime == 0 ? ' has bonus' : ''));
        agg.table += "<td>" + bld.level + '/' + bld.maxLevel +
            (bld.finished != 0 ? ' +' : '') + "</td>\n";
    }
    agg.table += "</tr>"
    --agg.cnt;
    console.log( 'Left to process ' + agg.cnt );
    if (agg.cnt == 0) {
        var popup = window.open('', 'Buildings', 
            'width=1000,height=800,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=0,top=0');
        if (popup != null) {
            agg.table += "</table>";
            var impl = document.implementation;
            popup.document = impl.createHTMLDocument('Buildings');
            popup.document.body.isContentEditable = true;
            popup.document.body.innerHTML = agg.table;
            popup.focus();
        } else {
            alert('Please allow popups for this page');
            //alert(agg.str);
        }
    } else {
        agg.str += "\n\n";
    }
}

/**
 * Collect bonus  	http://s4.railnation.ru/web/rpc/flash.php?interface=BuildingsInterface&method=collect 
 */
function collectBonuses ( member, station ) {
    var name = '';
    if (member != null) {
        name = member.name;
    }
    //console.log(station);
    var next = 0;
    for (var i = 0; i < COLLECTIBLE_BUILDINGS.length; i++) {
        var id = COLLECTIBLE_BUILDINGS[i];
        var bld = station[id];
        if (bld.productionTime == 0) {
            console.log(name + ' ' + BUILDING_IDS[id] + ' has collected bonus');
            var params = [id];
            if (member != null && member.user_id != user_id) {
                params.push(member.user_id);
            }
            postCommand(
                'BuildingsInterface',
                'collect',
                params,
                null
            );
        } else if (bld.productionTime > 0 && (next == 0 || bld.productionTime < next)) {
            next = bld.productionTime;
        }
    }
    if (next <= 0) {
        next = 600; // just in case
    }
    if (next > 0 && collect_bonuses) {
        console.log( "Schedule next bonus collection for " + member.name + " in " + next + " seconds" );
        setTimeout(
            function () {
                processStation( member,
                    function( member, station ) {
                        collectBonuses(member, station);
                    }
                );
            },
            (next * 1000 + 200)
        );
    }
}

/**
 * Corporation funcs
 */
function processCorporation( uid, corpHandler ) {
    if (uid == null) {
        uid = user_id;
    }
        
    if (uid != null) {
        postCommand(
            'ProfileInterface',
            'getVCard',
            [[ uid ]],
            function( resp_text ) {
                var obj = JSON.parse(resp_text);
                var corp = obj.Body[uid].corporation;
                // Obtain corporation data
                postCommand(
                    'CorporationInterface',
                    'get',
                    [corp.corporation_id],
                    function(resp_text) {
                        try {
                            var obj = JSON.parse(resp_text);
                            if (corpHandler != null) {
                                console.log('Call corporation handler');
                                corpHandler( obj.Body );
                            } else {
                                console.log(obj.Body);
                            }
                        } catch (e) {
                            console.error(e);
                        }
                    }
                );
            }
        );
    } else {
        alert("No user id, try to reload page");
    }
}

function corporationBuildingLevels( corporation ) {
    var agg = {
        str : "",
        table: "<table width='100%'><tr><th>User</th>",
        cnt : 0
    };
    for (var id in BUILDING_IDS) {
        agg.table += "<th>" + BUILDING_IDS[id] + "</th>";
    }
    agg.table += "</tr>\n";
    for (var i = 0; i < corporation.members.length; ++i) {
        var member = corporation.members[i];
        console.log('check buildings for ' + member.name);
        ++agg.cnt;
        processStation( member,
            function( member, station ) {
                collectBuildingLevels(member, station, agg);
            }
        );
    }
}

function collectTrainLoads( member, trainList, agg ) {
	agg.str += "<strong>" + member.name + "</strong> " + trainList.length + "  trains<br/>\n";
	console.log("Train list for " + member.name + " " + trainList.length + "  trains");
	try {
		agg.str += "<table width='100%'>";
		for (var i = 0; i < trainList.length; ++i) {
			var train = trainList[i];
			agg.str += "<tr><td>" + engine_name(train.type) + "</td><td>";
			console.log( "Train type " + engine_name(train.type));
			if (train.waggons.length > 0) {
				for (var j = 0; j < train.waggons.length; ++j) {
					var load = train.waggons[j];
					agg.str += load.amount + " x " + resource_name( load.type );
					console.log( load.amount + " x " + resource_name( load.type ) );
				}
			} else {
				console.log("No waggons");
			}
			agg.str += "</td></tr>\n";
		}
		agg.str += "</table>\n";
	} catch (e) {
		console.error(e);
	}
	--agg.cnt;
	if (agg.cnt == 0) {
		// Output data
        var popup = window.open('', 'Trains load', 
            'width=1000,height=800,toolbar=0,menubar=0,location=0,status=1,scrollbars=1,resizable=1,left=0,top=0');
        if (popup != null) {
            var impl = document.implementation;
            popup.document = impl.createHTMLDocument('Trains load');
            popup.document.body.isContentEditable = true;
            popup.document.body.innerHTML = agg.str;
            popup.focus();
        } else {
            alert('Please allow popups for this page');
            //alert(agg.str);
        }
	} else {
		agg.str += "<br/>\n\n";
	}
}

function corporationProcessTrains( member, cb ) {
        postCommand(
            'TrainInterface',
            'getTrains',
            [ true, member.user_id ],
            function( resp_text ) {
                var obj = JSON.parse(resp_text);
                var trainList = obj.Body;
				if (cb != null) {
					cb(member, trainList);
				}
            }
        );
}

function corporationTraindLoad( corporation ) {
	var agg = {
		str	: "",
		cnt : 0
	};
	
	for (var i = 0; i < corporation.members.length; ++i) {
        var member = corporation.members[i];
		++agg.cnt;
		corporationProcessTrains( member,
			function (member, trainList) {
				collectTrainLoads(member, trainList, agg);
			}
		);
	}
}

function corporationCollectBonuses( corporation ) {
    for (var i = 0; i < corporation.members.length; ++i) {
        var member = corporation.members[i];
        processStation( member,
            function( member, station ) {
                collectBonuses(member, station);
            }
        );
    }
}

function sendMessageToCorporation( corporation, text ) {
    for (var i = 0; i < corporation.members.length; ++i) {
        var member = corporation.members[i];
        if (member.user_id != user_id) {
            sendMessage( [member.user_id], text );
        }
    }
}

function addCorporationMenus( corporation ) {
    console.log( 'Add menus for ' + corporation.name );
    try {
        GM_registerMenuCommand(
            corporation.name + ' to console',
            function(e) {
                console.log(corporation);
            }
        );

        GM_registerMenuCommand(
            corporation.name + ' buildings',
            function(e) {
                corporationBuildingLevels(corporation);
            }
        );
        var cmd = GM_registerMenuCommand(
            corporation.name + ' trains load',
            function(e) {
                corporationTraindLoad(corporation);
            }
        );
        GM_registerMenuCommand(
            corporation.name + ' message',
            function(e) {
                // acquire text
                var msg = prompt('Enter message');
                if (msg != null) {
                    // send message
                    sendMessageToCorporation(corporation, msg);
                }
            }
        );
        GM_registerMenuCommand(
            corporation.name + ' toggle bonuses collection',
            function(e) {
				collect_bonuses = !collect_bonuses;
				GM_setValue( 'CollectBonuses', collect_bonuses );
				var msg = 'Now ' + ( collect_bonuses ? '' : ' not ') + 'collecting bonuses';
				alert(msg);
				
				if (collect_bonuses) {
					corporationCollectBonuses(corporation);
				}
            }
        );
		if (collect_bonuses) {
			console.log( 'Launch bonus collection' );
			corporationCollectBonuses(corporation);
		}
    } catch (e) {
        console.error(e);
    }
}

/**
 * Initialization
 */
if (document.body) {
    var result = xpath("//script", document.head);
    var p = result.iterateNext();
    var re = /load_swf\([^)]*"assets\/([^"]+)",\s+"[^"]+",\s+"([^"]+)",\s+"([^"]+)"/gi;
    while (p) {
        var m = re.exec(p.text);
        if (m != null) {
            checksum = m[1];
            key = m[2];
            api = m[3];
        }
        p = result.iterateNext();
    }
    if (api != null) {
        referer = api.replace("rpc", "assets");
        referer += checksum + "/Railnation.swf";
        api += "flash.php";
    }
}

postCommand(
    'AccountInterface', 
    'is_logged_in',
    [ key ],
    function(resp_text) {
        user_id = JSON.parse(resp_text).Body;
        console.log( user_id );
        // TODO Detect user_id is object and mark hash is needed
        // Obtain corporation data
        processCorporation(user_id, addCorporationMenus);
    	// Repair trains upon entrance
    	processTrains( 
    		'doMaintenance', 
    		function( train ) {
    			return train.reliability < GM_getValue( 'LastReliability', 90 ) && needsMaintenance(train);
    		}, 
    		null 
    	);

    }
);

try {
	
	GM_registerMenuCommand(
		'Mechanic for all trains',
		function(e) {
		    processTrains( 'buyMechanic', needsMechanic, null );
		}
	);
	GM_registerMenuCommand(
		'Mechanic for all trains (era)',
		function(e) {
			var lastEra = GM_getValue( 'LastBoostEra', 6 );
			var era = parseInt( prompt( 'Select era', lastEra ) );
			if (!isNaN(era)) {
				processTrains( 
					'buyMechanic', 
					function(train) {
						return (engine_era(train.type) >= era) && needsMechanic(train);
					}, 
					null 
				);
				GM_setValue( 'LastBoostEra', era );
			}
		}
	);
	GM_registerMenuCommand(
		'Boost all trains for an hour',
		function(e) {
			processTrains( 'buyBoost', null, null );
		}
	);
	GM_registerMenuCommand(
		'Boost all trains for an hour (era)',
		function(e) {
			var lastEra = GM_getValue( 'LastBoostEra', 6 );
			var era = parseInt( prompt( 'Select era', lastEra ) );
			if (!isNaN(era)) {
				processTrains( 
					'buyBoost', 
					function(train) {
						return engine_era(train.type) >= era;
					}, 
					null 
				);
				GM_setValue( 'LastBoostEra', era );
			}
		}
	);
	GM_registerMenuCommand(
		'Boost all trains for N hours',
		function(e) {
		    var lastCnt = GM_getValue( 'LastBoostHours', 5 );
		    var cnt = parseInt(prompt("How many hours?", lastCnt));
		    if (!isNaN(cnt)) {
			for (var i = 0; i < cnt; i++) {
			    processTrains( 'buyBoost', null, null );
			}
			GM_setValue( 'LastBoostHours', cnt );
		    }
		}
	);
	GM_registerMenuCommand(
		'Boost all trains for N hours (era)',
		function(e) {
		    var lastCnt = GM_getValue( 'LastBoostHours', 5 );
		    var cnt = parseInt(prompt("How many hours?", lastCnt));
		    if (!isNaN(cnt)) {
			var lastEra = GM_getValue( 'LastBoostEra', 6 );
			var era = parseInt( prompt( 'Select era', lastEra ) );
			if (!isNaN(era)) {
				for (var i = 0; i < cnt; i++) {
					processTrains( 
						'buyBoost', 
						function(train) {
							return engine_era(train.type) >= era;
						}, 
						null 
					);
				}
				GM_setValue( 'LastBoostHours', cnt );
			}
		    }
		}
	);

	GM_registerMenuCommand(
		'Repair all trains',
		function(e) {
		    processTrains( 'doMaintenance', needsMaintenance, null );
		}
	);
	GM_registerMenuCommand(
		'Repair all trains (reliability threshold)',
		function(e) {
		    	var lastRel = GM_getValue( 'LastReliability', 90 );
			var rel = parseInt(prompt( 'Max reliability, %?', lastRel ));
			if (!isNaN(rel)) {
				processTrains( 
					'doMaintenance', 
					function( train ) {
						return train.reliability < rel + 1 && needsMaintenance(train);
					}, 
					null 
				);
				GM_setValue( 'LastReliability', rel );
			}
		}
	);
	GM_registerMenuCommand(
		'Cancel all wait',
		function(e) {
			processTrains( 'cancelWait', canCancelWait, null );
		}
	);
	GM_registerMenuCommand(
		'Sell trains by type',
		function(e) {
	    var lastType = GM_getValue( 'LastSellTrain', 0 );
	    var type = parseInt(prompt("Input train type?", lastType));
	    if (!isNaN(type)) {
				processTrains( 
					'sellTrain', 
					function (train) {
						return train.type == type;
					},
					null 
				);
		GM_setValue( 'LastSellTrain', type );
	    }
		}
	);
	GM_registerMenuCommand( 
	'Oakhill debug details',
	function(e) {
	    requestTownDetails('644f0d32-7040-4a8e-bc76-a46ddd5a5ef1');
	}
	);
	GM_registerMenuCommand( 
	'Vasyuki debug details',
	function(e) {
	    requestTownDetails('2b19ba34-da2b-41d3-9a43-98e26268292f');
	}
	);
	GM_registerMenuCommand( 
	'St.Nicklas debug details',
	function(e) {
	    requestTownDetails('e80ab342-35e5-4f57-a491-4a0b65f91c59');
	}
	);
	GM_registerMenuCommand( 
	'Station to console',
	function(e) {
	    processStation(null, logBuildingLevels);
	}
	);    
} catch (e) {
    console.error(e);
}


