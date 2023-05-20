using Android.App;
using Android.Content;
using Android.OS;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReproductionProjectBug_ServiceCrash.Platforms.Android.Services;

[Service(Exported = true, Enabled = true)]
public class BackgroundService : Service
{
  
    private Handler _handler;
    private Action _action;
    private const int INTERVAL = 10000;
    private ILogger<BackgroundService> _logger;
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
                    _logger.LogError(ex, "Error running background task.");
                    await AppShell.Current.DisplayAlert($"Background Task Exception!", ex.Message, "OK");
                }
            }
            _handler.PostDelayed(_action, INTERVAL);
        });
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        _isRunning = true;
        _handler.PostDelayed(_action, INTERVAL);
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
            Console.WriteLine("Background task is running...");
            await Task.Delay(1000);
            count++;
        }
        Console.WriteLine($"Background task completed after {seconds} seconds.");
        // await AppShell.Current.DisplaySnackbar($"Background task completed after {seconds} seconds.", null, "OK", TimeSpan.FromSeconds(10));
    }
}