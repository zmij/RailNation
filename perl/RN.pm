package RN;

use strict;
use utf8;
use JSON;
use Digest::MD5 qw(md5_hex);
use URI::Escape;
use AnyEvent::HTTP;
use Data::Dumper;

my $json = new JSON;


sub new {
    my ($class, %args) = @_;
    die "Need numeric world unless" unless $args{world}=~/^\d+$/;
    die "need email and pass" unless $args{email} and $args{pass};
    $args{url} ||= 'http://www.railnation.ru/#login';
    return bless {%args, cookie=>{}, lurl=>$args{url} }, $class;
}


sub log {
    my $self = shift;
    if ($self->{logfile}) {
        open F, '>>', $self->{logfile};
        print(F @_, "\n");
        close F;
    }
    print(@_, "\n");
}


sub login {
    my ($self) = @_;
    my $cv = AE::cv;
    $self->rail_http(POST=>'https://railnation-sam.traviangames.com//iframe/login/consumer/railnation-ru-meta/applicationLanguage/ru-RU',
                     headers => {'Content-Type' => 'application/x-www-form-urlencoded' },
                     body => 'className=login+&email='.uri_escape_utf8($self->{email}).
                         '&password='.uri_escape_utf8($self->{pass}).
                             '&remember_me=1&submit=%D0%92%D1%85%D0%BE%D0%B4',
                     sub {
                         $self->log("password sent");
                         $cv->send;
                     });
    $cv->recv; $cv = AE::cv;

    $self->rail_http(POST=>'https://railnation-sam.traviangames.com//iframe/log-into/consumer/railnation-ru-meta/applicationLanguage/ru-RU',
                     headers => {'Content-Type' => 'application/x-www-form-urlencoded' },
                     body => 'world='.$self->{world},
                     sub {
                         ($self->{burl}, $self->{key}) = ($_[0] =~m|document.location.href="(http://[^\?]+)\?key=([^"]+)";|);
                         die "Something wrong with parsing" unless $self->{burl} and $self->{key};
                         $self->log("World Choosed ($self->{burl}, $self->{key}) ");
                         $cv->send;
                     });
    $cv->recv; $cv = AE::cv;

    $self->rail_http(GET=>$self->{burl}.'?key='.$self->{key},
                     sub {
                         $self->log("GET SMTH");
                         $cv->send;
                     });
    $cv->recv; $cv = AE::cv;

    $self->{w}->{properties} = $self->req(Properties => getData => [])->recv()->{properties};
    my $me = $self->{me} = $self->req(Account => is_logged_in => [$self->{key}])->recv();
    $cv->begin; $self->req(GUI => get_initial_gui =>[])->cb(sub{ $self->{w}->{GUI} = $_[0]->recv; $cv->end; });
    $cv->begin; $self->req(Location => get =>[])->cb(sub{ $self->{w}->{Location} = $_[0]->recv; $cv->end; });
    $cv->begin; $self->req(Rail => get => [$me])->cb(sub{ $self->{w}->{Rail} = $_[0]->recv; $cv->end; });
    $cv->begin; $self->req(Train => getTrains => [JSON::true, $me])->cb(sub{ $self->{w}->{Train} = $_[0]->recv; $cv->end; });
    $cv->recv;
}


sub rail_http {
    my $self = shift;
    my $sub = pop;
    my ($m, $u, %arg) = @_;
    $arg{headers}->{'User-Agent'} = 'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:25.0) Gecko/20100101 Firefox/25.0';
    $arg{headers}->{Accept} = 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8';
    $arg{headers}->{Referer} = $self->{lurl};
    http_request($m, $u,
                 cookie_jar => $self->{cookie},
                 session    => 'paravoziki'.$self->{email},
                 persistent => 1,
                 %arg, sub {$sub->(@_);});
    $self->{lurl} = $u;
}


sub req {
    my ($self, $iface, $method, $arg ) =@_;
    my $ret = AE::cv;
    http_request(POST => $self->{burl}.'rpc/flash.php'.'?interface='.$iface.'Interface&method='.$method,
                 headers => {
                     Referer        => $self->{burl},
                     'User-Agent'   => 'Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:25.0) Gecko/20100101 Firefox/25.0',
                     'Content-Type' => 'application/json',
                     Accept         => 'application/json',
                 },
                 body       => $json->encode({
                     checksum   => 1,
                     client     => 1,
                     parameters => $arg,
                     hash       => md5_hex($json->encode($arg)),
                 }),
                 cookie_jar => $self->{cookie},
                 session    => 'paravoziki'.$self->{email},
                 persistent => 1,
                 sub {
                     # XXX error handling here
                     my ($body, $header) =@_;
                     return $ret->croak($body, $header) if !$body or $body =~/^{"number":/;
                     $body = $json->decode($body);
                     $ret->send($body->{Body});
                 });
    $ret;
}







1;
