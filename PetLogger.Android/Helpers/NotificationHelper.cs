using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using PetLogger.Droid.Models.Reminders;
using PetLogger.Shared.Data;

namespace PetLogger.Droid.Helpers
{
    public static class NotificationHelper
    {
        public const int NOTIFICATION_ID = 1000;
        public const string CHANNEL_ID = "channel id";
        public const string CHANNEL_NAME = "channel name";
        public const string CHANNEL_DESCRIPTION = "channel description";

        public static void CreateReminderNotification(Context context, Reminder reminder, Intent ringtoneService)
        {
            CreateNotificationChannel(context);

            // Create snooze intent to cancel the current ringtone AND set up a new one
            var snoozeIntent = new Intent(context, typeof(SnoozeAlarmReceiver))
               .SetData(Android.Net.Uri.Parse("content://reminder/" + reminder.ID))
               .PutExtra(Intent.ExtraIntent, ringtoneService);

            var snoozeBroadcast = PendingIntent.GetBroadcast(context, 0, snoozeIntent, PendingIntentFlags.OneShot);

            var dismissIntent = new Intent(context, typeof(DismissAlarmReceiver))
               .SetData(Android.Net.Uri.Parse("content://reminder/" + reminder.ID))
               .PutExtra(Intent.ExtraIntent, ringtoneService);

            var dismissBroadcast = PendingIntent.GetBroadcast(context, 0, dismissIntent, PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(context, CHANNEL_ID)
                .SetAutoCancel(false) // Dismiss the notification from the notification area when the user clicks on it
                .SetContentTitle(reminder.Title + " reminder") // Set the title
                //.SetNumber(5) // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.ic_action_attach) // This is the icon to display
                .SetContentText("Snoozed") // the message to display
                .AddAction(Resource.Drawable.baseline_snooze_black_36dp, "Snooze", snoozeBroadcast)
                .AddAction(Resource.Drawable.baseline_snooze_black_36dp, "Dismiss", dismissBroadcast)
                .SetContentIntent(PendingIntent.GetActivity(context, 0, new Intent(context, typeof(Activities.MainActivity)), PendingIntentFlags.OneShot)); // Start up this activity when the user clicks the intent.

            var notification = builder.Build();
            notification.Flags = NotificationFlags.NoClear;

            // Finally, publish the notification
            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(NOTIFICATION_ID, notification);
        }

        public static void DismissReminderNotification(Context context, Reminder reminder)
        {
            var notificationManager = NotificationManagerCompat.From(context);

            // TODO - Use more intelligent notification IDs so we can dismiss this particular notification (based on corresponding reminder)
            notificationManager.Cancel(NOTIFICATION_ID);
        }

        public static void CreateNotificationChannel(Context context)
        {
            // Notification channels are unnecessary below API level 26
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.High)
                {
                    Description = CHANNEL_DESCRIPTION,
                    LockscreenVisibility = NotificationVisibility.Public
                };
                channel.EnableVibration(true);

                var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                notificationManager.CreateNotificationChannel(channel);
            }   
        }

        /*public class NotificationPublisher : BroadcastReceiver
        {

        }*/

        // Pass the current button press count value to the next activity:
        //var valuesForActivity = new Bundle();
        //valuesForActivity.PutInt(COUNT_KEY, count);

        // When the user clicks the notification, SecondActivity will start up.
        //var resultIntent = new Intent(context, typeof(SecondActivity));

        // Pass some values to SecondActivity:
        //resultIntent.PutExtras(valuesForActivity);

        // Construct a back stack for cross-task navigation:
        /*var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);
        stackBuilder.AddParentStack(Class.FromType(typeof(SecondActivity)));
        stackBuilder.AddNextIntent(resultIntent);*/

        // Create the PendingIntent with the back stack:
        //var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
    }
}