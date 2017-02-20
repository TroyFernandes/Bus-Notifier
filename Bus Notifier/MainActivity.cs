using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Net;
using System;
using Newtonsoft.Json.Linq;
using Android.Views;
using Android.Support.V4.App;

namespace Bus_Notifier
{
    [Activity(Label = "Bus Notifier", MainLauncher = true, Icon = "@drawable/appIcon")]
    public class MainActivity : Activity
    {

        string[] line_time45,line_time45E, line_time45A;
        
        string url = "https://myttc.ca/kipling_station.json";

        private Handler mHandler = new Handler();
        //private long mStartRX = 0;
        //private long mStartTX = 0;
    


    protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
                        int uid = ApplicationInfo.Uid;

            //Set custom toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Bus Notifier";
            //update the fields and send a notification
            updateFields();
            SendNotification(line_time45[2], line_time45[1]);

            /* 
            Data usage numbers

            mStartRX = TrafficStats.GetUidRxBytes(uid);
            mStartTX = TrafficStats.GetUidTxBytes(uid);
            System.Diagnostics.Debug.WriteLine("mstartRX: " + Convert.ToDouble(mStartRX / 1024 / 1024));
            System.Diagnostics.Debug.WriteLine("mstartTX: " + Convert.ToDouble(mStartTX / 1024 / 1024));
            */

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            updateFields();
            SendNotification(line_time45[2], line_time45[1]);
            return base.OnOptionsItemSelected(item);
        }

        //Send notification to the user
        //Uses API 24+ to send bundled notifications
        public void SendNotification(String line, String text)
        {

            //Main notification
            NotificationCompat.Builder builderSummary =
                new NotificationCompat.Builder(this)
                .SetContentTitle("Bus Arrival Times")
                .SetGroup("group_key_notify")
                .SetGroupSummary(true)
                .SetSmallIcon(Resource.Drawable.notificationIcon);
            //45 Kipling 
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
                .SetContentTitle("45 ")
                .SetContentText("Next bus at " + text + "m")
                .SetGroup("group_key_notify")
                .SetSmallIcon(Resource.Drawable.notificationIcon);
            //45E Kipling 
            NotificationCompat.Builder builder2 = new NotificationCompat.Builder(this)
               .SetContentTitle("45E")
               .SetContentText("Next bus at " + line_time45E[1] + "m")
               .SetGroup("group_key_notify")
               .SetSmallIcon(Resource.Drawable.notificationIcon);
            //45A Kipling 
            NotificationCompat.Builder builder3 = new NotificationCompat.Builder(this)
               .SetContentTitle("45A")
               .SetContentText("Next bus at " + line_time45A[1] + "m")
               .SetGroup("group_key_notify")
               .SetSmallIcon(Resource.Drawable.notificationIcon);


            Notification notificationSummary = builderSummary.Build();
            Notification notification = builder.Build();
            Notification notification2 = builder2.Build();
            Notification notification3 = builder3.Build();

            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            const int notificationId0 = 0;
            const int notificationID1 = 1;
            const int notificationID2 = 2;
            const int notificationID3 = 3;
            notificationManager.Notify(notificationID1, builder.Build());
            notificationManager.Notify(notificationID2, builder2.Build());
            notificationManager.Notify(notificationID3, builder3.Build());
            notificationManager.Notify(notificationId0, builderSummary.Build());


        }

        //Gets data from the JSON url
        public string getJsonData(String url, int routesIndex, int stopTimesIndex, bool timeStamp)
        {

            WebClient n = new WebClient();
            var busTimes = n.DownloadString(url);
            var busData = Convert.ToString(busTimes);
            dynamic data = JObject.Parse(busData);
            // dont change first index "data.stops[0]"
            // routes can change max of 2 [0-1]
            // stops can change max of 3 [0-2]

            if (timeStamp)
            {
                return data.stops[0].name.ToString() + ";" +
                data.stops[1].routes[routesIndex].stop_times[stopTimesIndex].departure_timestamp.ToString() + ";" +
                data.stops[1].routes[routesIndex].stop_times[stopTimesIndex].shape.ToString();
            }
            else
            {
                return data.stops[0].name.ToString() + ";" +
                data.stops[1].routes[routesIndex].stop_times[stopTimesIndex].departure_time.ToString() + ";" +
                data.stops[1].routes[routesIndex].stop_times[stopTimesIndex].shape.ToString();
            }
            
        }

        public void updateFields()
        {
            line_time45 = getJsonData(url, 2, 0, false).Split(';');
            line_time45E = getJsonData(url, 2, 6, false).Split(';');
            line_time45A = getJsonData(url, 2, 3, false).Split(';');
            
        }

        public string getArrivalTime(string url)
        {
            string[] departure_time = getJsonData(url, 2, 0, false).Split(';');
            return departure_time[1];

        }
        public string getDepartureTime(string url)
        {
            string[] departure_time = getJsonData(url,2,1, false).Split(';') ;
            return departure_time[1];
        }

        public double getArrivalTimeStamp(string url)
        {
            string[] departure_time = getJsonData(url, 2, 0,true).Split(';');
            return Convert.ToDouble(departure_time[1]);
        }
        public double getDepartureTimeStamp(string url)
        {
            string[] departure_time = getJsonData(url, 2, 1, true).Split(';');
            return Convert.ToDouble(departure_time[1]);
        }
        public DateTime subAndConvert()
        {
            double remainingTime = (getDepartureTimeStamp(url) - getArrivalTimeStamp(url));
            return FromUnixTime(Convert.ToInt64(remainingTime));
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }



    }
}

