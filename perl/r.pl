#!/usr/bin/perl

use strict;
use utf8;
use JSON;
use Digest::MD5 qw(md5_hex);
use URI::Escape;
use AnyEvent::HTTP;
use AnyEvent::ReadLine::Gnu;
use Term::ANSIColor;

use Data::Dumper;

use RN;

my $login_data = do '../../login.pld';
my $rl;

my $rn = new RN %$login_data, logcb => sub { rlout(@_) };
$rl = new AnyEvent::ReadLine::Gnu prompt => "> ", on_line => sub {
    if ($_[0] =~/^rn\s+(.+)$/i) {
        rlsay('RN: ', $1);
    } elsif ($_[0] =~/^eval\s+(.+)$/i) {
        rlsay('EVAL: ', $1);
        rlout(Dumper eval $1);
    } elsif ($_[0] =~/^req\s+(\w+)\s+(\w+)\s+(\[.+\])\s*$/i) {
        my $data = eval $3;
        return rlsay(ERROR => $@) if $@;
        $rn->req($1, $2, $data, sub {rlsay(got=>Dumper @_)});
        rlsay(requested =>"$1, $2, $3");
    } elsif ( $_[0] =~ /^get_clan_station/i) {
        $rn->get_clan_station();
    } elsif ( $_[0] =~ /^start_collectables/i) {
        $rn->start_collectables();
    } else {
        rlout("NN: $_[0]");
    }
};

my $cv = AE::cv;

# TODO: тут сделать опциональный $rn->load, который выставит куку и загрузит {w} из json файла
# TODO: при выходе по команде save - делаем $rn->save
$rn->login;

#rlout(Dumper $rn);
#$rn->log(Dumper $rn->req(Train => getMyTrains => [])->recv);

rlsay("done");
$cv->recv;

sub rlsay {
    my ($p, @rest)=@_;
    $rl->print($p ? colored($p, 'green') :(), @rest ? colored(join('', @rest),'white'):() , "\n");
}

sub rlout {
    $rl->print(colored(join('', @_), 'bright_black')."\n");
}
