using Android.App;
using Android.Support.V7.App;

namespace PetLogger.Droid.Helpers
{
    public static class ToolbarHelper
    {
        public static void ShowToolbar(Activity activity, string title)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                compatActivity.SupportActionBar.Title = title;
                //compatActivity.SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_back);
                //compatActivity.SupportActionBar.SetDisplayShowHomeEnabled(true);
                //compatActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                //compatActivity.SupportActionBar.SetHomeButtonEnabled(true);
                compatActivity.SupportActionBar.Show();
            }
        }

        public static void HideToolbar(Activity activity)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                compatActivity.SupportActionBar.Hide();
            }
        }

        public static void ShowToolbarBackButton(Activity activity)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                //compatActivity.SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_back);
                //compatActivity.SupportActionBar.SetDisplayShowHomeEnabled(true);
                compatActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                //compatActivity.SupportActionBar.SetHomeButtonEnabled(true);
            }
        }

        public static void HideToolbarBackButton(Activity activity)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                //compatActivity.SupportActionBar.SetDisplayShowHomeEnabled(false);
                compatActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                //compatActivity.SupportActionBar.SetHomeButtonEnabled(false);
            }
        }
    }
}