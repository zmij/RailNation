using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace RailNation
{
    using Rails = Dictionary<string, HashSet<string>>;
    using Path = List<string>;
using System.Diagnostics;


    public class RailGraph
    {
        private Rails rails_ = new Rails();

        public bool Empty
        {
            get
            {
                return rails_.Count == 0;
            }
        }
        public Rails.KeyCollection ConnectedLocations
        {
            get
            {
                return rails_.Keys;
            }
        }

        public void Clear()
        {
            rails_.Clear();
        }

        public void refresh(JArray source)
        {
            JEnumerable<JToken> children = source.Children();
            foreach (JToken railTok in children)
            {
                addBiDirectional(railTok["location_id1"].ToString(), railTok["location_id2"].ToString());
            }
        }
        private void addBiDirectional(string from, string to)
        {
            addRail(from, to);
            addRail(to, from);
        }

        private void addRail(string from, string to)
        {
            if (!rails_.ContainsKey(from))
            {
                rails_.Add(from, new HashSet<string>());
            }
            if (!rails_[from].Contains(to))
                rails_[from].Add(to);
        }

        public HashSet<string> connectionsFrom(string from)
        {
            if (rails_.ContainsKey(from))
                return rails_[from];
            return null;
        }

        public bool isLocationConnected(string locId)
        {
            return rails_.ContainsKey(locId);
        }

        [DebuggerDisplay("{F} = {G}+{H} ({Location.Name})")]
        private class SearchState
        {
            string id_;
            int g_;
            int h_;

            SearchState prev_;
            LocationInfo loc_;

            public SearchState(string id, int g, int h, LocationInfo loc, 
                SearchState prev = null)
            {
                id_ = id;
                g_ = g;
                h_ = h;
                prev_ = prev;
                loc_ = loc;
            }

            public string Id
            {
                get
                {
                    return id_;
                }
            }

            public int G
            {
                get
                {
                    return g_;
                }
                set
                {
                    g_ = value;
                }
            }

            public int H
            {
                get
                {
                    return h_;
                }
            }

            public int F
            {
                get
                {
                    return g_ + h_;
                }
            }

            public SearchState Prev
            {
                get
                {
                    return prev_;
                }
                set
                {
                    prev_ = value;
                }
            }

            public LocationInfo Location
            {
                get
                {
                    return loc_;
                }
            }

            public override string ToString()
            {
                return String.Format("{0}: G={1}, H={2} ({3} {4})", F, G, H, 
                    prev_ == null ? "Start" : (prev_.loc_.Name + " ->"),
                    loc_.Name);
            }
        }

        public Path findPath(string from, string to)
        {
            Path path = new Path();

            if (isLocationConnected(from) && isLocationConnected(to))
            {
                LocationInfo loc = World.getLocation(from);
                LocationInfo goal = World.getLocation(to);
                if (loc != null && goal != null)
                {
                    HashSet<string> closed = new HashSet<string>();
                    List<SearchState> open = new List<SearchState>();

                    SearchState s = new SearchState(from, 0, 
                        goal.manhattanDistance(loc), loc);
                    open.Add(s);

                    while (open.Count > 0)
                    {
                        open.Sort((lhs, rhs)=>( lhs.F.CompareTo(rhs.F) ));
                        s = open[0];
                        open.RemoveAt(0);
                        closed.Add(s.Id);

                        if (s.Id == to)
                            break;

                        HashSet<string> neighbours = connectionsFrom(s.Id);
                        foreach (string n in neighbours)
                        {
                            if (closed.Contains(n))
                                continue;

                            SearchState e = open.Find(ss => ss.Id == n);
                            if (e == null)
                            {
                                loc = World.getLocation(n);
                                e = new SearchState(n, s.G + 1, 
                                    goal.manhattanDistance(loc), loc, s);
                                open.Add(e);
                            }
                            else
                            {
                                if (e.G > s.G + 1)
                                {
                                    // Better path
                                    e.G = s.G + 1;
                                    e.Prev = s;
                                }
                            }
                        }
                    }

                    if (s.Id == to)
                    {
                        while (s != null)
                        {
                            path.Add(s.Id);
                            s = s.Prev;
                        }
                        path.Reverse();
                    }
                }
            }

            return path;
        }
    }
}
