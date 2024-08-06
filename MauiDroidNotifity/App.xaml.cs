using MauiDroidNotifity.Platforms.Android;

namespace MauiDroidNotifity;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}


	protected override async void OnStart()
	{
		await MusicService.CheckAndRequestForegroundPermission();
	}
}
