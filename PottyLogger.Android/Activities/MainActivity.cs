using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using PottyLogger.Droid.Fragments;
using System;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private int _currentTabID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            InitializeUI();
        }

        private void InitializeUI()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
