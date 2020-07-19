using Android.App;
using Android.Support.V7.App;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Helpers
{
    public static class FragmentHelper
    {
        public static void Add(Activity activity, Fragment fragment)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                compatActivity.SupportFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();
            }
        }

        public static void Replace(Activity activity, Fragment fragment)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                compatActivity.SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();
            }
        }

        public static void PopOne(Activity activity)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                if (compatActivity.SupportFragmentManager.BackStackEntryCount > 0)
                {
                    compatActivity.SupportFragmentManager.PopBackStack();
                }
            }
        }

        public static void PopAll(Activity activity)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                var stackCount = compatActivity.SupportFragmentManager.BackStackEntryCount;

                for (var i = 0; i < stackCount; i++)
                {
                    compatActivity.SupportFragmentManager.PopBackStack();
                }
            }
        }
    }
}