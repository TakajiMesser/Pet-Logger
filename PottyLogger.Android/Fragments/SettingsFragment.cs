using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Views;
using PottyLogger.Droid.Helpers;
using PottyLogger.Shared.DataAccessLayer;

namespace PottyLogger.Droid.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public static SettingsFragment Instantiate() => new SettingsFragment();

        public static SettingsFragment Instantiate(string screenKey)
        {
            var fragment = new SettingsFragment();

            var bundle = new Bundle();
            bundle.PutString(ArgPreferenceRoot, screenKey);
            fragment.Arguments = bundle;

            return fragment;
        }

        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_settings, container, false);

        private string _rootKey;

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            _rootKey = rootKey;
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var activity = Activity as AppCompatActivity;
            activity.SupportActionBar.Title = "Settings";

            var viewDebugLog = PreferenceManager.FindPreference("view_debug_log");
            if (viewDebugLog != null)
            {
                viewDebugLog.PreferenceClick += ViewDebugLog_PreferenceClick;
            }

            var viewDatabase = PreferenceManager.FindPreference("view_database");
            if (viewDatabase != null)
            {
                viewDatabase.PreferenceClick += ViewDatabase_PreferenceClick;
            }

            var resetDatabase = PreferenceManager.FindPreference("reset_database");
            if (resetDatabase != null)
            {
                resetDatabase.PreferenceClick += ResetDatabase_PreferenceClick;
            }

            var resetDefaults = PreferenceManager.FindPreference("reset_defaults");
            if (resetDefaults != null)
            {
                resetDefaults.PreferenceClick += ResetDefaults_PreferenceClick;
            }

            var version = PreferenceManager.FindPreference("version");
            if (version != null)
            {
                version.Summary = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0).VersionName;
            }
        }

        public override void OnNavigateToScreen(PreferenceScreen preferenceScreen)
        {
            base.OnNavigateToScreen(preferenceScreen);

            Activity.SupportFragmentManager.BeginTransaction()
                .AddToBackStack(null)
                .Replace(Resource.Id.content_frame, Instantiate(preferenceScreen.Key))
                .Commit();
        }

        private void ViewDebugLog_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            Activity.SupportFragmentManager.BeginTransaction()
                .AddToBackStack(null)
                .Replace(Resource.Id.content_frame, DebugLogFragment.Instantiate())
                .Commit();
        }

        private void ViewDatabase_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            Activity.SupportFragmentManager.BeginTransaction()
                .AddToBackStack(null)
                .Replace(Resource.Id.content_frame, TableListFragment.Instantiate())
                .Commit();
        }

        private void ResetDatabase_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Reset database", "This will reset ALL local data! Are you sure?", () =>
            {
                ProgressDialogHelper.RunTask(Activity, "Resetting local database...", () =>
                {
                    DBAccess.ResetTables();

                    /*StartActivity(new Intent(Activity, typeof(LoginActivity))
                        .SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask)
                        .PutExtra("logout", true));
                    Activity.Finish();*/
                });
            });
        }

        private void ResetDefaults_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Reset settings to default", "This will reset ALL settings to default values! Are you sure?", () =>
            {
                ProgressDialogHelper.RunTask(Activity, "Resetting settings...", () =>
                {
                    PreferenceHelper.ResetToDefaults();

                    PreferenceScreen = null;
                    SetPreferencesFromResource(Resource.Xml.preferences, _rootKey);
                    //InitializePreferences();
                });
            });
        }
    }
}
