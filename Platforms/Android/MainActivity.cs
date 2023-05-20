using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Extensions.Logging;
using ReproductionProjectBug_ServiceCrash.Platforms.Android.Services;

namespace ReproductionProjectBug_ServiceCrash;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

      
       
        StartServices();
    }

    void StartServices()
    {


        try
        {
            // Check if the background service is already running
            if (!IsServiceRunning(typeof(BackgroundService)))
            {
                // Start the background service
                StartService(new Intent(this, typeof(BackgroundService)));
            }
        }
        catch (Exception ex)
        {
          
        }

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            try
            {
                // Check if the foreground service is already running
                if (!IsServiceRunning(typeof(Platforms.Android.Services.ForegroundService)))
                {
                    // Start the foreground service
                    StartForegroundService(new Intent(this, typeof(Platforms.Android.Services.ForegroundService)));
                }
            }
            catch (Exception ex)
            {
             
            }
        }


    }


    public bool IsServiceRunning(System.Type cls)
    {
        ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);

        IList<ActivityManager.RunningServiceInfo> runningServices = manager.GetRunningServices(int.MaxValue);

        for (int i = 0; i < runningServices.Count; i++)
        {
            ActivityManager.RunningServiceInfo service = runningServices[i];

            if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
            {
                return true;
            }

        }

        return false;
    }

}
