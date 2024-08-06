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
		mediaSession = new MediaSessionCompat(ctx, "notification");
		mediaSession.SetCallback(new MediaCallback(this));

		Update(true);
	}

	static void ConfigurePlaybackState(bool isPlaying = true)
	{
		var state = isPlaying ? PlaybackStateCompat.StatePlaying : PlaybackStateCompat.StatePaused;
		var playbackState = new PlaybackStateCompat.Builder()
			.SetState(state, PlaybackStateCompat.PlaybackPositionUnknown, 1.0f)
			.SetActions(PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionPause | PlaybackStateCompat.ActionSkipToNext | PlaybackStateCompat.ActionSkipToPrevious)
			.Build();

		mediaSession.SetPlaybackState(playbackState);
	}

	public void Update(bool isPlaying)
	{
		var ctx = Platform.AppContext;
		var pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
		? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
		: PendingIntentFlags.UpdateCurrent;

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
			.SetOnlyAlertOnce(true)
			.SetPriority(NotificationCompat.PriorityHigh)
			.SetStyle(style)
			.SetContentIntent(PendingIntent.GetActivity(Platform.CurrentActivity, 24, intent, pendingIntentFlags))
			.SetVisibility(NotificationCompat.VisibilityPublic)
			.SetSound(null);

		mediaSession.Active = true;

		var notification = notificationBuilder.Build();

		var notificationManager = NotificationManagerCompat.From(ctx);
		var metaData = new MediaMetadataCompat.Builder();
		metaData.PutString(MediaMetadataCompat.MetadataKeyTitle, "Caxangá")
			.PutString(MediaMetadataCompat.MetadataKeyArtist, "Milton Nascimento")
			.PutLong(MediaMetadataCompat.MetadataKeyDuration, 60 * 5);

		// If you don't call this method, the notification will be handled by the broadcast service instead of 
		// the MusicSession.Callback
		ConfigurePlaybackState(isPlaying);

		mediaSession.SetMetadata(metaData.Build());
		notificationManager.Notify(321, notification);
	}
}


sealed class MediaCallback : MediaSessionCompat.Callback
{
	private readonly MusicService music;

	public MediaCallback(MusicService music)
	{
		this.music = music;
	}
	public override void OnPlay()
	{
		base.OnPlay();
		music.Update(true);
	}

	public override void OnPause()
	{
		base.OnPause();
		music.Update(false);
	}
	public override void OnSkipToNext()
	{
		base.OnSkipToNext();
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

