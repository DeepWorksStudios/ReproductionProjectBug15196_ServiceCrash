using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

using Microsoft.Extensions.Logging;

namespace ReproductionProjectBug_ServiceCrash.Platforms.Android.Services;

[Service(Exported = true, Enabled = true)]
public class ForegroundService : Service
{
    
    private Handler _handler;
    private Action _action;
    private const int INTERVAL = 1000;
    private const int SERVICE_NOTIFICATION_ID = 1;
    private const string CHANNEL_ID = "my_channel_id";
    private const string CHANNEL_NAME = "My Channel";
    private const string CHANNEL_DESCRIPTION = "My Channel Description";
    private ILogger<ForegroundService> _logger;
    private static bool _isRunning;

    public static bool IsRunning
    {
        get { return _isRunning; }
        set { _isRunning = value; }
    }
    public override void OnCreate()
    {
        base.OnCreate();
       
        _handler = new Handler();
        _action = new Action(async () =>
        {
            if (_isRunning)
            {
                // Do your background work here
                try
                {
                    await RunBackgroundTask(180); // replace 10 with the number of seconds you want the task to run
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running foreground task.");
                    await AppShell.Current.DisplayAlert($"Foreground Task Exception!", ex.Message, "OK");
                }
            }
            _handler.PostDelayed(_action, INTERVAL);
        });

        CreateNotificationChannel();
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        _isRunning = true;
        _handler.PostDelayed(_action, INTERVAL);

        StartForeground(SERVICE_NOTIFICATION_ID, CreateNotification());
        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _isRunning = false;
    }

    public override IBinder OnBind(Intent intent)
    {
        return null;
    }


    public async Task RunBackgroundTask(int seconds)
    {
        int count = 0;
        while (count < seconds)
        {
            Console.WriteLine("Foreground task is running...");
            await Task.Delay(1000);
            count++;
        }
        Console.WriteLine($"Foreground task completed after {seconds} seconds.");

        //    await AppShell.Current.DisplaySnackbar($"Foreground task completed after {seconds} seconds.", null, "OK", TimeSpan.FromSeconds(10));

    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.High)
            {
                Description = CHANNEL_DESCRIPTION
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }

    private Notification CreateNotification()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return null;

        var notificationBuilder = new Notification.Builder(this, CHANNEL_ID)
        .SetSmallIcon(Microsoft.Maui.Resource.Drawable.notification_icon_background)
        .SetContentTitle("Foreground Service Title")
        .SetContentText("Foreground Service Description")
        .SetOngoing(true)
        .SetAutoCancel(true);

        var notification = notificationBuilder.Build();
        return notification;
    }
}