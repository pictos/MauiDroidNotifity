using Android.Media.Session;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using static Android.Resource.Drawable;
using Android.App;
using Android.Content;
using static Microsoft.Maui.ApplicationModel.Permissions;
using AndroidX.Core.Content;
using AndroidX.Media.Session;
using Google.Android.Material.Color.Utilities;

namespace MauiDroidNotifity.Platforms.Android;
public sealed class MusicService
{
	MyReceiver receiver = new();
	public static MediaSessionCompat mediaSession = default!;
	const string channelId = "music123";
	public const string ActionPlay = "ACTION_PLAY";
	public const string ActionPause = "ACTION_PAUSE";
	public const string ActionNext = "ACTION_NEXT";

	public static async Task CheckAndRequestForegroundPermission(CancellationToken cancellationToken = default)
	{
		var status = await Permissions.CheckStatusAsync<MediaPermissions>().WaitAsync(cancellationToken);
		if (status is PermissionStatus.Granted)
		{
			return;
		}

		await Permissions.RequestAsync<MediaPermissions>().WaitAsync(cancellationToken).ConfigureAwait(false);
	}

	void CreateNotificationChannel()
	{
		var channel = new NotificationChannel(channelId, "Media notification", NotificationImportance.High);
		channel.Description = "Media playback controls";

		var notificationManager = (NotificationManager)Platform.CurrentActivity!.GetSystemService(Context.NotificationService)!;
		notificationManager.CreateNotificationChannel(channel);
	}

	public void Init()
	{
		CreateNotificationChannel();
		var ctx = Platform.AppContext;


		var pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
			? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
			: PendingIntentFlags.UpdateCurrent;


		mediaSession = new MediaSessionCompat(ctx, "notification");

		mediaSession.SetCallback(new MediaCallback());



		var style =
				new AndroidX.Media.App.NotificationCompat.MediaStyle()
				.SetMediaSession(mediaSession.SessionToken)
				//.SetShowActionsInCompactView(0, 1, 2)
				;

		var intent = new Intent(Platform.CurrentActivity, typeof(MainActivity)).SetAction("OpenPlayer");

		var notificationBuilder = new NotificationCompat.Builder(ctx, channelId)
			.SetSmallIcon(IcMenuDay)
			.SetContentTitle("Caxangá")
			.SetContentText("Milton Nascimento")
			//.SetOngoing(true)
			.SetOnlyAlertOnce(true)
			.SetPriority(NotificationCompat.PriorityHigh)
			.SetStyle(style)
			.SetContentIntent(PendingIntent.GetActivity(Platform.CurrentActivity, 24, intent, pendingIntentFlags))
			.SetVisibility(NotificationCompat.VisibilityPublic)
			.SetSound(null);


		var playIntent = new Intent(ctx, typeof(MyReceiver)).SetAction(ActionPlay);
		var pauseIntent = new Intent(ctx, typeof(MyReceiver)).SetAction(ActionPlay);
		var nextIntent = new Intent(ctx, typeof(MyReceiver)).SetAction(ActionNext);

		var playPendingIntent = MediaButtonReceiver.BuildMediaButtonPendingIntent(ctx, PlaybackStateCompat.ActionPlay);   //PendingIntent.GetBroadcast(ctx, 0, playIntent, pendingIntentFlags);
		var pausePendingIntent = PendingIntent.GetBroadcast(ctx, 0, pauseIntent, pendingIntentFlags);
		var nextPendingIntent = PendingIntent.GetBroadcast(ctx, 0, nextIntent, pendingIntentFlags);
		mediaSession.SetMediaButtonReceiver(playPendingIntent);
		mediaSession.Active = true;
		notificationBuilder.AddAction(IcMediaPrevious, "previous", playPendingIntent)
			.AddAction(IcMediaPause, "pause", pausePendingIntent)
			.AddAction(IcMediaNext, "next", nextPendingIntent);

		var notification = notificationBuilder.Build();

		var notificationManager = NotificationManagerCompat.From(ctx);
		var metaData = new MediaMetadataCompat.Builder();
		metaData.PutString(MediaMetadataCompat.MetadataKeyTitle, "Caxangá")
			.PutString(MediaMetadataCompat.MetadataKeyArtist, "Milton Nascimento")
			.PutLong(MediaMetadataCompat.MetadataKeyDuration, 60 * 5);

		// If you don't call this method, the notification will be handled by the broadcast service instead of 
		// the MusicSession.Callback
		ConfigurePlaybackState();

		mediaSession.SetMetadata(metaData.Build());
		notificationManager.Notify(321, notification);
	}

	private static void ConfigurePlaybackState()
	{
		var playback = new PlaybackStateCompat.Builder()
					.SetState(PlaybackStateCompat.StatePlaying, 60, 1.0f)
					.SetActions(PlaybackState.ActionPlay | PlaybackState.ActionPause | PlaybackState.ActionSkipToNext | PlaybackState.ActionSkipToPrevious | PlaybackStateCompat.ActionSeekTo)
					//.SetActions(PlaybackStateCompat.ActionSeekTo)
					.Build();

		mediaSession.SetPlaybackState(playback);
	}
}

[BroadcastReceiver(Exported = true)]
[IntentFilter([MusicService.ActionNext, MusicService.ActionPause, MusicService.ActionPlay])]

sealed class MyReceiver : BroadcastReceiver
{
	public override void OnReceive(Context? context, Intent? intent)
	{
		if (intent is null)
			return;



		var action = intent.Action;

	}
}


sealed class MediaCallback : MediaSessionCompat.Callback
{

	public override void OnPlay()
	{
		base.OnPlay();
	}

	public override void OnPause()
	{
		base.OnPause();
	}
}


sealed class MediaPermissions : BasePlatformPermission
{
	static readonly Lazy<(string androidPermission, bool isRuntime)[]> permissionsHolder = new(CreatePermissions);

	public override (string androidPermission, bool isRuntime)[] RequiredPermissions => permissionsHolder.Value;

	static (string androidPermission, bool isRuntime)[] CreatePermissions()
	{
		var requiredPermissionsList = new (string androidPermission, bool isRuntime)[2];

		requiredPermissionsList[0] = (global::Android.Manifest.Permission.ForegroundService, true);
		requiredPermissionsList[1] = (global::Android.Manifest.Permission.PostNotifications, true);

		return requiredPermissionsList;
	}
}

