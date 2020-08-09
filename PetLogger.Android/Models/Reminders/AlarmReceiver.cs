using Android.App;
using Android.Content;
using Android.Widget;
using PetLogger.Droid.Helpers;

namespace PetLogger.Droid.Models.Reminders
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "This is a broadcast alarm!", ToastLength.Long).Show();

            var path = intent.Extras.GetString("path");
            var ringtoneIntent = AudioHelper.GetRingtoneService(context, path);

            context.StartService(ringtoneIntent);

            ringtoneIntent.SetAction(RingtoneService.STOP_ACTION);
            var cancelRingtoneIntent = PendingIntent.GetService(context, 0, ringtoneIntent, PendingIntentFlags.OneShot);

            NotificationHelper.CreateNotification(context, cancelRingtoneIntent);
        }
    }
}
