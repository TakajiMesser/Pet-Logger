using Android.App;
using Android.Content;
using Android.OS;
using PetLogger.Droid.Helpers;

namespace PetLogger.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/LightTheme", MainLauncher = true)]
    public class LightActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (PreferenceHelper.DisplayTheme == Themes.Dark)
            {
                StartActivity(new Intent(this, typeof(DarkActivity))
                    .SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask));
                //.PutExtra("logout", true));
                Finish();
            }
        }
    }
}
