using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using PetLogger.Droid.Fragments;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Models.Logging;
using PetLogger.Shared.DataAccessLayer;
using System;
using System.Globalization;
using System.Threading.Tasks;
using static Android.Support.V4.App.ActivityCompat;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnRequestPermissionsResultCallback
    {
        private bool _isInSettings;

        private int _currentTabID;
        private int _backStackSnapshotCount;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetUpUnhandledExceptionHandlers();

            PermissionHelper.RequestPermissions(this, OnPermissionsGranted);

            if (PreferenceHelper.StayActive)
            {
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (!PermissionHelper.HandlePermissionsResult(this, requestCode, permissions, grantResults, OnPermissionsGranted))
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        private void OnPermissionsGranted()
        {
            DebugLog.LazyWrite(this, "All permissions granted");
            InitializeDatabase();

            SetContentView(Resource.Layout.activity_main);
            InitializeUI();
        }

        private void SetUpUnhandledExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Log.Debug("Unhandled Exception", e.ExceptionObject.ToString());
                DebugLog.LazyWrite(this, "UnhandledException: " + e.ExceptionObject);
            };

            AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                Log.Debug("Unhandled Exception", e.Exception.ToString());
                DebugLog.LazyWrite(this, "UnhandledExceptionRaiser: " + e.Exception);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log.Debug("Unhandled Exception", e.Exception.ToString());
                DebugLog.LazyWrite(this, "UnobservedTaskException: " + e.Exception);
            };
        }

        private void InitializeDatabase()
        {
            DBAccess.InitializeTables();

            // TODO - Set up database values for testing purposes
            /*DBAccess.ResetTables();
            BuiltIn.InitializeBuiltInTypes();

            var dog = DBTable.Get<PetType>(p => p.Name == "Dog");
            var cat = DBTable.Get<PetType>(p => p.Name == "Cat");

            DBTable.Insert(new Pet()
            {
                Name = "Cooper",
                PetTypeID = dog.ID
            });

            DBTable.Insert(new Pet()
            {
                Name = "Kora",
                PetTypeID = cat.ID
            });

            var cooper = DBTable.Get<Pet>(p => p.Name == "Cooper");
            var pee = DBTable.Get<IncidentType>(i => i.Name == "Pee");
            var poo = DBTable.Get<IncidentType>(i => i.Name == "Poo");

            DBTable.Insert(new Incident()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                Time = DateTime.Now - TimeSpan.FromHours(27),
            });

            DBTable.Insert(new Incident()
            {
                PetID = cooper.ID,
                IncidentTypeID = poo.ID,
                Time = DateTime.Now - TimeSpan.FromHours(10),
            });

            DBTable.Insert(new LoggerDefinition()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                IncludeDays = true,
                IncludeHours = true,
                IncludeMinutes = true,
                IncludeSeconds = true,
            });

            DBTable.Insert(new LoggerDefinition()
            {
                PetID = cooper.ID,
                IncidentTypeID = poo.ID,
                IncludeDays = true,
                IncludeHours = true,
                IncludeMinutes = true,
                IncludeSeconds = true,
            });*/
        }

        private void InitializeUI()
        {
            //OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_out_top);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var tabView = FindViewById<BottomNavigationView>(Resource.Id.tab_view);
            tabView.NavigationItemSelected += (s, args) => SwitchToTabFragment(args.Item.ItemId);

            SwitchToTabFragment(Resource.Id.tab_home);
        }

        private void SwitchToTabFragment(int resourceID)
        {
            _isInSettings = false;

            if (_currentTabID != resourceID)
            {
                _currentTabID = resourceID;
                FragmentHelper.PopAll(this);
                FragmentHelper.Push(this, GetTabFragment(_currentTabID));
            }
            else
            {
                FragmentHelper.PopAllButOne(this);
            }
        }

        private Fragment GetTabFragment(int resourceID)
        {
            switch (resourceID)
            {
                case Resource.Id.tab_home:
                    return HomeFragment.Instantiate();
                case Resource.Id.tab_pets:
                    return PetsFragment.Instantiate();
                case Resource.Id.tab_reminders:
                    return RemindersFragment.Instantiate();
                case Resource.Id.tab_history:
                    return HistoryFragment.Instantiate();
            }

            throw new ArgumentOutOfRangeException("No corresponding tab fragment found for resource ID " + resourceID);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main_actions, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                case Resource.Id.action_settings:
                    if (!_isInSettings)
                    {
                        // We need to store the current back stack so we know when we fully back out of settings again
                        _isInSettings = true;
                        _backStackSnapshotCount = SupportFragmentManager.BackStackEntryCount;

                        FragmentHelper.Push(this, SettingsFragment.Instantiate());
                    }
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 1)
            {
                if (_isInSettings)
                {
                    // Check if we've actually exit the settings fragments and returned to the main tabs or not
                    if (SupportFragmentManager.BackStackEntryCount <= _backStackSnapshotCount + 1)
                    {
                        _isInSettings = false;
                    }
                }

                SupportFragmentManager.PopBackStack();
            }
            else
            {
                Finish();
            }
        }
    }
}
