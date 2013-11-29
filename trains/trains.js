// vim:ts=4:sw=4:expandtab:
function trains_init() {
    $('#submit').bind('click', function(event) { event.preventDefault(); trains_show(); });
    trains_show();
}

function trains_show() {
    var n = 0;
    var ftime = 0+$('#ftime').val();
    var w = 0+$('#wtime').val();
    var mtime = 0+(ftime-w)/2;
    var d = 0+dist_from_time( mtime, $('#spd_0').val(), $('#acc_0').val() );
    console.log(d);

    var out = [];
    while($('#acc_' + n).val()) {
        var txt = $('#txt_' + n).val();
        var acc = $('#acc_' + n).val();
        var spd = $('#spd_' + n).val();
        var vol = $('#vol_' + n).val();
        var div = $('#div_' + n).val() || 1;
        var circle_time = 2*time_to_dist(d, spd, acc) + 1*w;
        var volume      = Math.floor(10*vol*3600/circle_time/div)/10;
        console.log(txt + ' ' + circle_time);
        $('#res_' + n).val( volume );
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
    if( dmax < d ) t = (d-dmax) / (max_speed*1000/3600);
    t += sec_to_max( max_speed, acc );
    return t;
}

function dist_from_time( t, max_speed, acc ) {
    var tmax = t-sec_to_max( max_speed, acc );
    var dist = dist_to_max( max_speed, acc );
    if( tmax > 0 ) dist += tmax * max_speed * 1000 / 3600;
    return dist;
}
