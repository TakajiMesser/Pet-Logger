using Android.Content;
using Android.Widget;
using PetLogger.Droid.Helpers;

namespace PetLogger.Droid.Models.Reminders
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class DismissAlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Alarm dismissed", ToastLength.Long).Show();

            var ringtoneIntent = intent.GetParcelableExtra(Intent.ExtraIntent) as Intent;
            context.StopService(ringtoneIntent);

            // Parse reminder and dismiss corresponding notification
            var reminder = ReminderHelper.ParseReminderFromURI(intent.Data);

            // TODO - Report or log error for malformed data URI
            if (reminder != null)
            {
                NotificationHelper.DismissReminderNotification(context, reminder);
            }
        }
    }
}
