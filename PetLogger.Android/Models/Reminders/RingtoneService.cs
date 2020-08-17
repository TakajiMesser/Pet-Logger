using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using PetLogger.Droid.Helpers;

namespace PetLogger.Droid.Models.Reminders
{
    [Service]
    public class RingtoneService : Service
    {
        public static long[] VIBRATION_PATTERN = new[] { 0L, 250L, 250L, 250L };

        private Ringtone _ringtone;
        private Vibrator _vibrator;

        // Return null because this is a pure started service
        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            var reminder = ReminderHelper.ParseReminderFromURI(intent.Data);

            // TODO - Report or log error for malformed data URI
            if (reminder != null)
            {
                if (!string.IsNullOrEmpty(reminder.SoundPath))
                {
                    //_ringtone = RingtoneManager.GetRingtone(this, Android.Provider.Settings.System.DefaultAlarmAlertUri);
                    _ringtone = RingtoneManager.GetRingtone(this, Android.Net.Uri.Parse(reminder.SoundPath));
                    _ringtone?.Play();
                }

                if (reminder.Vibrate)
                {
                    _vibrator = GetSystemService(VibratorService) as Vibrator;
                    _vibrator?.Vibrate(VibrationEffect.CreateWaveform(VIBRATION_PATTERN, 0));
                }
            }

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            _ringtone?.Stop();
            _vibrator?.Cancel();

            base.OnDestroy();
        }
    }
}
