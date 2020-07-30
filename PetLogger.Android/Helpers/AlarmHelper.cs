using Android.Content;
using Android.Icu.Util;
using Android.Provider;
using PetLogger.Shared.Data;

namespace PetLogger.Droid.Helpers
{
    public static class AlarmHelper
    {
        public static void SetSystemAlarm(Context context, Reminder reminder)
        {
            var alarmIntent = new Intent(AlarmClock.ActionSetAlarm);
            alarmIntent.PutExtra(AlarmClock.ExtraDays, Calendar.Sunday); // Day-of-week for repeating alarm
            alarmIntent.PutExtra(AlarmClock.ExtraHour, 0); // Integer 0-23
            alarmIntent.PutExtra(AlarmClock.ExtraMinutes, 0); // Integer 0-59
            alarmIntent.PutExtra(AlarmClock.ExtraMessage, "");
            alarmIntent.PutExtra(AlarmClock.ExtraRingtone, "content://settings/system/alarm_alert");
            alarmIntent.PutExtra(AlarmClock.ExtraSkipUi, true);
            alarmIntent.PutExtra(AlarmClock.ExtraVibrate, true);

            context.StartActivity(alarmIntent);
        }

        public static void SetSystemTimer(Context context, Reminder reminder)
        {
            var timerIntent = new Intent(AlarmClock.ActionSetTimer);
            timerIntent.PutExtra(AlarmClock.ExtraLength, 1); // Integer 1-86400 (24 hours)
            timerIntent.PutExtra(AlarmClock.ExtraMessage, "");
            timerIntent.PutExtra(AlarmClock.ExtraRingtone, "content://settings/system/alarm_alert");
            timerIntent.PutExtra(AlarmClock.ExtraSkipUi, true);
            timerIntent.PutExtra(AlarmClock.ExtraVibrate, true);

            context.StartActivity(timerIntent);
        }
    }
}