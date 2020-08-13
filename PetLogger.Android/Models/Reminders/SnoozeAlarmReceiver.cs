using Android.Content;
using Android.Widget;
using PetLogger.Droid.Helpers;

namespace PetLogger.Droid.Models.Reminders
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class SnoozeAlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Alarm snoozed", ToastLength.Long).Show();

            // Stop ringtone service
            var ringtoneIntent = intent.GetParcelableExtra(Intent.ExtraIntent) as Intent;
            context.StopService(ringtoneIntent);

            // Parse reminder and create new snoozed alarm
            var reminder = ReminderHelper.ParseReminderFromURI(intent.Data);

            // TODO - Report or log error for malformed data URI
            if (reminder != null)
            {
                ReminderHelper.SnoozeReminder(context, reminder);
            }
        }
    }
}
