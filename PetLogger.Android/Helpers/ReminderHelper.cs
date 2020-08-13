using Android.App;
using Android.Content;
using PetLogger.Droid.Models.Reminders;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;

namespace PetLogger.Droid.Helpers
{
    public static class ReminderHelper
    {
        // "content://settings/system/alarm_alert"
        public static void ScheduleReminder(Context context, Reminder reminder)
        {
            var alarmIntent = new Intent(context, typeof(TriggerAlarmReceiver))
               .SetData(Android.Net.Uri.Parse("content://reminder/" + reminder.ID));

            var broadcastIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.OneShot);

            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Set(AlarmType.RtcWakeup, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5000, broadcastIntent);
        }

        public static void SnoozeReminder(Context context, Reminder reminder)
        {
            var alarmIntent = new Intent(context, typeof(TriggerAlarmReceiver))
               .SetData(Android.Net.Uri.Parse("content://reminder/" + reminder.ID));

            var broadcastIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.OneShot);

            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Set(AlarmType.RtcWakeup, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5000, broadcastIntent);
        }

        public static void CancelReminder(Context context, Reminder reminder)
        {
            var alarmIntent = new Intent(context, typeof(TriggerAlarmReceiver))
                .SetData(Android.Net.Uri.Parse("content://reminder/" + reminder.ID));

            var broadcastIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.OneShot);

            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Cancel(broadcastIntent);
        }

        public static Reminder ParseReminderFromURI(Android.Net.Uri data)
        {
            if (data.Scheme == "content" && data.Host == "reminder")
            {
                var pathSegments = data.PathSegments;

                if (pathSegments.Count == 1)
                {
                    if (int.TryParse(pathSegments[0], out int reminderID))
                    {
                        return DBTable.Get<Reminder>(reminderID);
                    }
                }
            }

            return null;
        }
    }
}