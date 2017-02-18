using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Net;
using System;
using Newtonsoft.Json.Linq;
using Android.Views;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Bus_Notifier
{
    [Activity(Label = "Bus_Notifier", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView tv1;
        TextView tv2;
        TextView tv3;
        string[] line_time;
        string url = "https://myttc.ca/kipling_station.json";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            //create toolbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Bus Notifier";

            tv1 = FindViewById<TextView>(Resource.Id.textView1);
            tv2 = FindViewById<TextView>(Resource.Id.textView2);
            tv3 = FindViewById<TextView>(Resource.Id.textView3);

            updateFields();
            SendNotification(line_time[2], line_time[1]);

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            updateFields();
            SendNotification(line_time[2], line_time[1]);
            return base.OnOptionsItemSelected(item);
        }



        public void SendNotification(String line, String text)
        {

            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle(line)
                .SetContentText(text + "m | The next bus will leave in " + subAndConvert().Minute + "minutes")
                //.SetDefaults(NotificationDefaults.Vibrate)
                .SetSmallIcon(Resource.Drawable.notificationIcon);
                

            Notification notification = builder.Build();

            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);




        }

        public string getJsonData(String url, int routesIndex, int stopTimesIndex, bool timeStamp)
        {

            WebClient n = new WebClient();
            var busTimes = n.DownloadString(url);
            var busData = Convert.ToString(busTimes);
            dynamic data = JObject.Parse(busData);
            //dont change first index "data.stops[0]
            //routes can change max of 2 [0-1]
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
            line_time = getJsonData(url, 2,0, false).Split(';');
            //2,1,3
            tv1.Text = line_time[0];//line name
            tv2.Text = line_time[2];//direction
            tv3.Text = line_time[1];//time
        }

        public string getArrivalTime(string url)
        {
            string[] departure_time = getJsonData(url, 2, 0, false).Split(';');
            //departure_time[1] = Regex.Replace(departure_time[1],"[a||p]" ,"");
            return departure_time[1];


        }
        public string getDepartureTime(string url)
        {
            string[] departure_time = getJsonData(url,2,1, false).Split(';') ;
            //departure_time[1] = Regex.Replace(departure_time[1], "[a||p]", "");
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
            return Convert.ToDouble(departure_time[1]);//departure_time[1];
        }
        public DateTime subAndConvert()
        {
            double remainingTime = (getDepartureTimeStamp(url) - getArrivalTimeStamp(url));
            return FromUnixTime(Convert.ToInt64(remainingTime));
        }

        //public string inStation()
        //{
        //   
        //}

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }


    }
}

