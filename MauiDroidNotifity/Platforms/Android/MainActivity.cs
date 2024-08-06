using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using MauiDroidNotifity.Platforms.Android;

namespace MauiDroidNotifity;
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnDestroy()
	{
		// cancel the notification, probably the best is to cancel by ID
		var notificationManager = NotificationManagerCompat.From(this);
		notificationManager.CancelAll();
		base.OnDestroy();
	}
}
