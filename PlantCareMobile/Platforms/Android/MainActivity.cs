using Android.App;
using Android.Content.PM;
using Android.OS;

namespace PlantCareMobile;

[Activity(Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    ConfigurationChanges = ConfigChanges.ScreenSize | 
                          ConfigChanges.Orientation | 
                          ConfigChanges.UiMode | 
                          ConfigChanges.ScreenLayout | 
                          ConfigChanges.SmallestScreenSize | 
                          ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Solicitar permisos en tiempo de ejecución para Android 13+
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            RequestPermissions(new[]
            {
                Android.Manifest.Permission.ReadMediaImages,
                Android.Manifest.Permission.Camera
            }, 0);
        }
        else
        {
            RequestPermissions(new[]
            {
                Android.Manifest.Permission.ReadExternalStorage,
                Android.Manifest.Permission.WriteExternalStorage,
                Android.Manifest.Permission.Camera
            }, 0);
        }
    }
}
