using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using PottyLogger.Droid.Fragments;
using PottyLogger.Droid.Helpers;
using PottyLogger.Droid.Models.Logging;
using PottyLogger.Shared.Data;
using PottyLogger.Shared.DataAccessLayer;
using System;
using System.Globalization;
using System.Threading.Tasks;
using static Android.Support.V4.App.ActivityCompat;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnRequestPermissionsResultCallback
    {
        private int _currentTabID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetUpUnhandledExceptionHandlers();

            PermissionHelper.RequestPermissions(this, OnPermissionsGranted);
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
            DBAccess.ResetTables();

            DBTable.Insert(new Pet()
            {
                Name = "Cooper"
            });

            DBTable.Insert(new Pet()
            {
                Name = "Kora"
            });

            DBTable.Insert(new IncidentType()
            {
                Name = "Pee"
            });

            DBTable.Insert(new IncidentType()
            {
                Name = "Poo"
            });

            var cooper = DBTable.Get<Pet>(p => p.Name == "Cooper");
            var pee = DBTable.Get<IncidentType>(i => i.Name == "Pee");

            DBTable.Insert(new Incident()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                Time = DateTime.Now - TimeSpan.FromHours(27),
            });

            DBTable.Insert(new InfoDefinition()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                Title = "Last Cooper Pee",
                Description = "I am an amazing description",
                IncludeDays = true,
                IncludeHours = true,
                IncludeMinutes = true,
                IncludeSeconds = false,
                Order = 1
            });

            DBTable.Insert(new LoggerDefinition()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                Name = "Cooper Pee",
                Order = 1,
                ImageName = "alarm"
            });
        }

        private void InitializeUI()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            /*var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;*/

            var tabView = FindViewById<BottomNavigationView>(Resource.Id.tab_view);
            tabView.NavigationItemSelected += (s, args) => SwitchToFragment(args.Item.ItemId);

            SwitchToFragment(Resource.Id.tab_home);
        }

        private void SwitchToFragment(int resourceID)
        {
            if (_currentTabID != resourceID)
            {
                _currentTabID = resourceID;

                var fragment = GetFragment(resourceID);
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();
            }
        }

        private Fragment GetFragment(int resourceID)
        {
            switch (resourceID)
            {
                case Resource.Id.tab_home:
                    return HomeFragment.Instantiate();
                case Resource.Id.tab_history:
                    return HistoryFragment.Instantiate();
                case Resource.Id.tab_settings:
                    return SettingsFragment.Instantiate();
            }

            throw new ArgumentOutOfRangeException("No corresponding tab fragment found for resource ID " + resourceID);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            var view = (View)sender;

            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null)
                .Show();
        }
	}
}
