using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using ForegroundServiceDemo.Droid.Services;
using ForegroundServiceDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ForegroundService))]
namespace ForegroundServiceDemo.Droid.Services
{
    [Service]
    public class ForegroundService : Service, IForegroundService
    {
        public static bool IsForegroundServiceRunning;
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(() =>
            {
                while (IsForegroundServiceRunning)
                {
                    System.Diagnostics.Debug.WriteLine("foreground Service is Running");
                    Thread.Sleep(2000);
                }
            });

            string channelID = "ForeGroundServiceChannel";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            if(Android.OS.Build.VERSION.SdkInt>= Android.OS.BuildVersionCodes.O)
            {
                var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(notfificationChannel);
            }

            var notificationBuilder = new NotificationCompat.Builder(this, channelID)
                                         .SetContentTitle("ForeGroundServiceStarted")
                                         .SetSmallIcon(Resource.Mipmap.icon)
                                         .SetContentText("Service Running in Foreground")
                                         .SetPriority(1)
                                         .SetOngoing(true)
                                         .SetChannelId(channelID)
                                         .SetAutoCancel(true);


            StartForeground(1001,notificationBuilder.Build());
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            IsForegroundServiceRunning = true;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            IsForegroundServiceRunning = false;
        }

        public void StartMyForegroundService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            Android.App.Application.Context.StartForegroundService(intent);
        }

        public void StopMyForegroundService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            Android.App.Application.Context.StopService(intent);
        }

        public bool IsForeGroundServiceRunning()
        {
            return IsForegroundServiceRunning;
        }
    }
}