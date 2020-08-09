using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;

namespace PetLogger.Droid.Helpers
{
    public static class NotificationHelper
    {
        public const int NOTIFICATION_ID = 1000;
        public const string CHANNEL_ID = "channel id";
        public const string CHANNEL_NAME = "channel name";
        public const string CHANNEL_DESCRIPTION = "channel description";

        public static void CreateNotification(Context context, PendingIntent contentIntent)
        {
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

            // Build the notification:
            CreateNotificationChannel(context);

            var builder = new NotificationCompat.Builder(context, CHANNEL_ID)
                .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                .SetContentTitle("Button Clicked") // Set the title
                .SetNumber(5) // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.ic_action_attach) // This is the icon to display
                .SetContentText("The button has been clicked 5 times."); // the message to display.

            if (contentIntent != null)
            {
                builder = builder.SetContentIntent(contentIntent); // Start up this activity when the user clicks the intent.
            }

            // Finally, publish the notification:
            var notificationManager = NotificationManagerCompat.From(context); 
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());
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
    }
}