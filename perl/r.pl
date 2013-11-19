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

my $rn = new RN %$login_data;

my $cv = AE::cv;

$rn->login;
print Dumper $rn;

print Dumper $rn->req(TrainInterface => getMyTrains => [])->recv;


$cv->recv;



