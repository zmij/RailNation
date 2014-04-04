// vim:ts=4:sw=4:expandtab:
function trains_init() {
    $('#submit').bind('click', function(event) { event.preventDefault(); trains_show(); });
    trains_show();
}

function create_input(name, idx, value) {
    return '<input type="text" id="' + name + '_' + idx + '" value="' + value + '"/>' + "\n";
}

function train_form_line(id, train) {
    var line =  create_input('txt', id, train.name)
             +  create_input('spd', id, train.max_speed)
             +  create_input('acc', id, train.acceleration)
             +  create_input('vol', id, train.max_load)
             +  create_input('res', id, 0)
             +  create_input('r70', id, 0)
             ;
    if (train.slots > 1) {
        line += create_input('div', id, train.slots);
    }
    line += '<br/>';
    return line;
}

function trains_load() {
    var data = train_data();
    console.log(data);
    var items = [];
    var idx = 0;
    for (var i = 0; i < data.trains.length; i++) {
        var train = data.trains[i];
        if (train.name == 'hr') {
            items.push("<hr/>\n");
        } else {
            console.log(train.name + ' ' + train.slots);
            items.push( train_form_line(idx, train) );
            idx++;
        }
    }
    $('#trains').append(items.join(''));
    trains_init();
}

function trains_show() {
    var n = 0;
    var w = 0+$('#wtime').val();

    // Старый расчет дистанции
    // var ftime = 0+$('#ftime').val();
    // var mtime = 0+(ftime-w)/2;
    // var d = Math.round(dist_from_time( mtime, $('#spd_0').val(), $('#acc_0').val() ) / 125) * 125;
    // $('#dist').val( d );

    var d = 0+$('#dist').val();

    var out = [];
    while($('#acc_' + n).val()) {
        var txt = $('#txt_' + n).val();
        var acc = $('#acc_' + n).val();
        var spd = $('#spd_' + n).val();
        var s70 = 0.7 * $('#spd_' + n).val();
        var vol = $('#vol_' + n).val();
        var div = $('#div_' + n).val() || 1;
        var circle_time = 2*time_to_dist(d, spd, acc) + 1*w;
        var c70         = 2*time_to_dist(d, s70, acc) + 1*w;
        var volume      = Math.floor(10*vol*3600/circle_time/div)/10;
        var volume70    = Math.floor(10*vol*3600/c70        /div)/10;
        console.log(txt + ' ' + circle_time);
        $('#res_' + n).val( volume   );
        $('#r70_' + n).val( volume70 );
        out[n] = [ txt, spd, acc, volume ];
        n++;
    }
    console.log(out);
}

function sec_to_max( max_speed, acc) {
    return max_speed/acc;
}

function dist_to_max( max_speed, acc ) {
    var t = sec_to_max( max_speed, acc );
    console.log( acc*t*t*1000/3600/2 );
    return acc*t*t*1000/3600/2
}

function time_to_dist( d, max_speed, acc ) {
    var dmax = dist_to_max( max_speed, acc );
    var t = 0;
    if( dmax <= d ) {
        t = (d-dmax) / (max_speed*1000/3600);
        t += sec_to_max( max_speed, acc );
    }
    else {
        t = Math.sqrt(d*2*3600/1000/acc);
    }
    return t;
}

function dist_from_time( t, max_speed, acc ) {
    var tmax = t-sec_to_max( max_speed, acc );
    var dist = dist_to_max( max_speed, acc );
    if( tmax > 0 ) dist += tmax * max_speed * 1000 / 3600;
    return dist;
}
