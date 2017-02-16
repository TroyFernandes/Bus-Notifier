using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Net;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace Bus_Notifier
{
    [Activity(Label = "Bus_Notifier", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

           // SendNotification();
            TextView tv1 = FindViewById<TextView>(Resource.Id.textView1);
            TextView tv2 = FindViewById<TextView>(Resource.Id.textView2);
            TextView tv3 = FindViewById<TextView>(Resource.Id.textView3);

            string[] line_time = getJsonData("https://myttc.ca/genthorn_and_kipling.json").Split(';');
            //2,1,3
            tv1.Text = line_time[0];//line name
            tv2.Text = line_time[2];//direction
            tv3.Text = line_time[1];//time

            SendNotification(line_time[2], line_time[1]);


        }

        public void SendNotification(String line, String text)
        {
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle(line)
                .SetContentText(text)
                .SetDefaults(NotificationDefaults.Vibrate)
                .SetSmallIcon(Resource.Drawable.unnamed);

            Notification notification = builder.Build();

            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);

        }

        public string getJsonData(String url)
        {
            WebClient n = new WebClient();
            var busTimes = n.DownloadString(url);
            var busData = Convert.ToString(busTimes);
            dynamic data = JObject.Parse(busData);
            
            return data.stops[0].name.ToString() + ";" + 
                data.stops[0].routes[0].stop_times[0].departure_time.ToString()+";"+ 
                data.stops[0].routes[0].stop_times[0].shape.ToString();
        }


    }
}

