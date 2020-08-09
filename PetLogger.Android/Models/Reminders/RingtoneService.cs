using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;

namespace PetLogger.Droid.Models.Reminders
{
    [Service]
    public class RingtoneService : Service
    {
        public const string STOP_ACTION = "com.takamesser.petlogger.ACTION_STOP";

        private Ringtone _ringtone;

        // Return null because this is a pure started service
        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (intent.Action == STOP_ACTION)
            {
                StopSelf();
            }
            else
            {
                var path = intent.Extras.GetString("path");

                _ringtone = RingtoneManager.GetRingtone(this, Android.Net.Uri.Parse(path));
                _ringtone?.Play();
            }

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            _ringtone?.Stop();
            base.OnDestroy();
        }
    }
}
