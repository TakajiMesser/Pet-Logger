using Android.Content;
using Android.Media;
using Android.OS;
using PetLogger.Droid.Models.Reminders;

namespace PetLogger.Droid.Helpers
{
    public static class AudioHelper
    {
        public static void PlaySound(Context context, string path) => MediaPlayer.Create(context, Android.Net.Uri.Parse(path)).Start();

        public static Intent GetRingtoneService(Context context, string path)
        {
            var bundle = new Bundle();
            bundle.PutString("path", path);

            var intent = new Intent(context, typeof(RingtoneService));
            intent.PutExtras(bundle);

            return intent;
        }
    }
}