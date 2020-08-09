using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using PetLogger.Droid.Models.Reminders;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;

namespace PetLogger.Droid.Helpers
{
    public static class ReminderHelper
    {
        public static void ReplaceReminder(Context context, int petID, int incidentTypeID)
        {
            // TODO - Handle case where there is more than one reminder for a given pet incident
            var reminders = DBTable.GetAll<Reminder>()
                .Where(r => r.PetID == petID && r.IncidentTypeID == incidentTypeID);

            foreach (var reminder in reminders)
            {
                switch (reminder.ReminderType)
                {
                    case ReminderTypes.Alarm:
                        ReplaceSystemAlarm(context, reminder.Title, DateTime.Now + reminder.TimeBetween);
                        break;
                    case ReminderTypes.Timer:
                        ReplaceSystemTimer(context, reminder.Title, reminder.TimeBetween);
                        break;
                    case ReminderTypes.Notification:
                        //ReminderHelper.SetUpNotification(Context, reminder.Title, DateTime.Now + reminder.TimeBetween);
                        break;
                }
            }
        }

        public static void DeleteReminder(Context context, int petID, int incidentTypeID)
        {
            // TODO - Handle case where there is more than one reminder for a given pet incident
            var reminders = DBTable.GetAll<Reminder>()
                .Where(r => r.PetID == petID && r.IncidentTypeID == incidentTypeID);

            foreach (var reminder in reminders)
            {
                switch (reminder.ReminderType)
                {
                    case ReminderTypes.Alarm:
                        DeleteSystemAlarm(context, reminder.Title);
                        break;
                    case ReminderTypes.Timer:
                        DeleteSystemTimer(context, reminder.Title);
                        break;
                    case ReminderTypes.Notification:
                        //ReminderHelper.SetUpNotification(Context, reminder.Title, DateTime.Now + reminder.TimeBetween);
                        break;
                }
            }
        }

        public static void ReplaceSystemAlarm(Context context, string label, DateTime time, string ringtone = "content://settings/system/alarm_alert")
        {
            DeleteSystemAlarm(context, label);
            CreateSystemAlarm(context, label, time, ringtone);
        }

        public static void CreateSystemAlarm(Context context, string label, DateTime time, string ringtone = "content://settings/system/alarm_alert")
        {
            context.StartActivity(new Intent(AlarmClock.ActionSetAlarm)
            //.PutExtra(AlarmClock.ExtraDays, Calendar.Sunday); // Day-of-week for repeating alarm
                .PutExtra(AlarmClock.ExtraHour, time.Hour) // Integer 0-23
                .PutExtra(AlarmClock.ExtraMinutes, time.Minute) // Integer 0-59
                .PutExtra(AlarmClock.ExtraMessage, label)
                .PutExtra(AlarmClock.ExtraRingtone, ringtone)
                .PutExtra(AlarmClock.ExtraSkipUi, true)
                .PutExtra(AlarmClock.ExtraVibrate, true));
        }

        public static void DeleteSystemAlarm(Context context, string label)
        {
            context.StartActivity(new Intent(AlarmClock.ActionDismissAlarm)
                .PutExtra(AlarmClock.ExtraAlarmSearchMode, AlarmClock.AlarmSearchModeLabel)
                .PutExtra(AlarmClock.ExtraMessage, label)
                .PutExtra(AlarmClock.ExtraSkipUi, true));
        }

        public static void ReplaceSystemTimer(Context context, string label, TimeSpan time, string ringtone = "content://settings/system/alarm_alert")
        {
            DeleteSystemTimer(context, label);
            CreateSystemTimer(context, label, time, ringtone);
        }

        public static void CreateSystemTimer(Context context, string label, TimeSpan time, string ringtone = "content://settings/system/alarm_alert")
        {
            context.StartActivity(new Intent(AlarmClock.ActionSetTimer)
                .PutExtra(AlarmClock.ExtraLength, (int)time.TotalSeconds) // Integer 1-86400 (24 hours)
                .PutExtra(AlarmClock.ExtraMessage, label)
                .PutExtra(AlarmClock.ExtraRingtone, ringtone)
                .PutExtra(AlarmClock.ExtraSkipUi, true)
                .PutExtra(AlarmClock.ExtraVibrate, true));
        }

        public static void DeleteSystemTimer(Context context, string label)
        {
            context.StartActivity(new Intent(AlarmClock.ActionDismissTimer)
                .PutExtra(AlarmClock.ExtraAlarmSearchMode, AlarmClock.AlarmSearchModeLabel)
                .PutExtra(AlarmClock.ExtraMessage, label)
                .PutExtra(AlarmClock.ExtraSkipUi, true));
        }

        public static void SetUpNotification(Context context, string label, TimeSpan time, string ringtone = "content://settings/system/alarm_alert")
        {
            
        }

        public static void SetUpAlarm(Context context)
        {
            var bundle = new Bundle();
            //bundle.PutString("path", RingtoneManager.GetActualDefaultRingtoneUri(context, RingtoneType.Alarm).Path);
            bundle.PutString("path", Settings.System.DefaultAlarmAlertUri.ToString());

            var alarmIntent = new Intent(context, typeof(AlarmReceiver)).PutExtras(bundle);
            var broadcastIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.OneShot);

            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Set(AlarmType.RtcWakeup, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5000, broadcastIntent);
        }

        public static void CancelAlarm(Context context)
        {
            var alarmIntent = new Intent(context, typeof(AlarmReceiver));
            var broadcastIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.OneShot);

            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Cancel(broadcastIntent);
        }
    }
}