using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Bus_Notifier
{
    class TTCJson
    {

        public class Rootobject
        {
            public int time { get; set; }
            public Stop[] stops { get; set; }
            public string uri { get; set; }
            public string name { get; set; }
        }

        public class Stop
        {
            public string name { get; set; }
            public string uri { get; set; }
            public Route[] routes { get; set; }
            public string agency { get; set; }
        }

        public class Route
        {
            public string name { get; set; }
            public string uri { get; set; }
            public string route_group_id { get; set; }
            public Stop_Times[] stop_times { get; set; }
        }

        public class Stop_Times
        {
            public int service_id { get; set; }
            public int departure_timestamp { get; set; }
            public string departure_time { get; set; }
            public string shape { get; set; }
        }
    }

    

}