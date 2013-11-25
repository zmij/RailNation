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
    } else {
        rlout($_[0]);
    }
};


my $cv = AE::cv;

$rn->login;
rlout(Dumper $rn);
#$rn->log(Dumper $rn->req(Train => getMyTrains => [])->recv);

print "done\n";
$cv->recv;



sub rlsay {
    my ($p, @rest)=@_;
    $rl->print($p ? colored($p, 'green') :(), @rest ? colored(join('', @rest),'white'):() , "\n");
}

sub rlout {
    $rl->print(colored(join('', @_), 'bright_black')."\n");
}
