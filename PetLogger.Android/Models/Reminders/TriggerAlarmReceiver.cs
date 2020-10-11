using Android.Content;
using Android.Widget;
using PetLogger.Droid.Helpers;
using System;

namespace PetLogger.Droid.Models.Reminders
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class TriggerAlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Alarm triggered", ToastLength.Long).Show();

            // Parse reminder, start ringtone, and show corresponding notification
            var reminder = ReminderHelper.ParseReminderFromURI(intent.Data);

            // TODO - Report or log error for malformed data URI
            if (reminder != null)
            {
                // Only trigger the alarm if we are currently in the active hour span
                if (IsTimeBetween(DateTime.Now.TimeOfDay, PreferenceHelper.WakeTime, PreferenceHelper.SleepTime))
                {
                    var ringtoneService = new Intent(context, typeof(RingtoneService))
                        .SetData(intent.Data);
                        context.StartService(ringtoneService);

                    NotificationHelper.CreateReminderNotification(context, reminder, ringtoneService);
                }
            }
            else
            {
                Toast.MakeText(context, "Trigger Reminder was null", ToastLength.Short).Show();
            }
        }

        private static bool IsTimeBetween(TimeSpan time, TimeSpan startTime, TimeSpan endTime)
        {
            // TODO - Need to verify the validity of this timespan check
            // Wake = 11 PM   (23:00)
            // Sleep = 3 AM   (03:00)
            // Current = 2 AM (02:00) <- This is fine, because it is Current < Wake AND Current < Sleep AND Sleep < Wake
            if (time > startTime && time < endTime) return true;
            if (time < endTime && endTime < startTime) return true;
            if (time > startTime && startTime > endTime) return true;

            return false;
        }
    }
}
