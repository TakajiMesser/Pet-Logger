using Android.Content;
using Android.OS;
using Android.Support.V7.Preferences;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Activities;
using PetLogger.Droid.Components;
using PetLogger.Droid.Components.Preferences;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Utilities;
using PetLogger.Shared.DataAccessLayer;

namespace PetLogger.Droid.Fragments
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

        public override void OnDisplayPreferenceDialog(Preference preference)
        {
            if (preference is TimePickerPreference)
            {
                var fragment = TimePickerPreferenceDialogFragment.Instance(preference);
                fragment.SetTargetFragment(this, 0);
                fragment.Show(FragmentManager, "android.support.v7.preference.PreferenceFragment.DIALOG");
            }
            else
            {
                base.OnDisplayPreferenceDialog(preference);
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            _rootKey = rootKey;
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
        }

        private void SetUpRecyclerView()
        {
            if (ListView != null && Context != null)
            {
                var layoutManager = new LinearLayoutManager(Context);
                ListView.SetLayoutManager(layoutManager);

                ListView.AddItemDecoration(new HorizontalDividerItemDecoration(Context, layoutManager.Orientation, Resource.Drawable.horizontal_divider_light)
                {
                    ShouldShowBeforeFirst = false,
                    ShouldShowAfterLast = true,
                    PaddingLeft = 12,
                    PaddingRight = 12
                });
            }

            ListView.SetBackgroundColor(Android.Graphics.Color.White);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Settings");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            SetUpRecyclerView();

            var displayTheme = PreferenceManager.FindPreference("display_theme");
            if (displayTheme != null)
            {
                displayTheme.PreferenceChange += DisplayTheme_PreferenceChange;
            }

            var stayActive = PreferenceManager.FindPreference("stay_active");
            if (stayActive != null)
            {
                stayActive.PreferenceChange += StayActive_PreferenceChange;
            }

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

            var backUpDatabase = PreferenceManager.FindPreference("back_up_database");
            if (backUpDatabase != null)
            {
                backUpDatabase.PreferenceClick += BackUpDatabase_PreferenceClick;
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

        private void DisplayTheme_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            var value = ((Java.Lang.String)e.NewValue).ToNetString();
            var theme = ThemeHelper.ParseTheme(value);

            if (theme == Themes.Light)
            {
                StartActivity(new Intent(Context, typeof(LightActivity))
                    .SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask));
                    //.PutExtra("logout", true));
                Activity.Finish();
            }
            else if (theme == Themes.Dark)
            {
                StartActivity(new Intent(Context, typeof(DarkActivity))
                    .SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask));
                //.PutExtra("logout", true));
                Activity.Finish();
            }
        }

        //public static Themes DisplayTheme => ThemeHelper.ParseTheme(Preferences.GetString("display_theme", "light"));

        private void StayActive_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            var value = (bool)e.NewValue;

            if (value)
            {
                Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                Activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
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
                .Replace(Resource.Id.content_frame, TablesFragment.Instantiate())
                .Commit();
        }

        private void BackUpDatabase_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            DBAccess.BackUpDatabase();
            Toast.MakeText(Context, "Successfully backed up database", ToastLength.Short).Show();
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
