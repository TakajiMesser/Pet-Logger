using Android.App;
using Android.Content;
using Android.Support.V7.Preferences;

namespace PetLogger.Droid.Helpers
{
    public static class PreferenceHelper
    {
        // Android
        public static ISharedPreferences Preferences => PreferenceManager.GetDefaultSharedPreferences(Application.Context);

        /*// iOS
        public static NSUserDefaults IOSPreferences => null;

        // Windows Phone
        public static IsolatedStorageSettings WindowsPreferences => null;*/

        public static void ResetToDefaults()
        {
            Preferences.Edit()
                .Clear()
                .Commit();
        }

        public static int PredictionsThreshold => int.Parse(Preferences.GetString("predictions_timestamp_threshold", "5"));

        public static bool StayActive => Preferences.GetBoolean("stay_active", false);
    }
}