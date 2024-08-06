using MauiDroidNotifity.Platforms.Android;

namespace MauiDroidNotifity;

public partial class MainPage : ContentPage
{
	int count = 0;
	MusicService musicService = new();
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		musicService.Init();
	}
}

