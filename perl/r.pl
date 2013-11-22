#!/usr/bin/perl

use strict;
use utf8;
use JSON;
use Digest::MD5 qw(md5_hex);
use URI::Escape;
use AnyEvent::HTTP;
use Data::Dumper;

use RN;

my $login_data = do '../../login.pld';

my $rn = new RN %$login_data, logfile=>'t.log';

my $cv = AE::cv;

$rn->login;
$rn->log(Dumper $rn);
#$rn->log(Dumper $rn->req(Train => getMyTrains => [])->recv);


$cv->recv;



