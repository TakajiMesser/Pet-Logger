using Android.App;
using Android.Support.V7.App;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Helpers
{
    public static class FragmentHelper
    {
        public static void Push(Activity activity, Fragment fragment)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                compatActivity.SupportFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    //.SetCustomAnimations(Resource.Animation.fragment_slide_in_from_bottom, Resource.Animation.fragment_fade_out, Resource.Animation.fragment_fade_in, Resource.Animation.fragment_slide_out_to_bottom)
                    .SetCustomAnimations(Resource.Animation.fragment_fade_in, Resource.Animation.fragment_fade_out, Resource.Animation.fragment_fade_in, Resource.Animation.fragment_fade_out)
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

        public static void PopAllButOne(Activity activity)
        {
            if (activity is AppCompatActivity compatActivity)
            {
                var stackCount = compatActivity.SupportFragmentManager.BackStackEntryCount;

                for (var i = 0; i < stackCount - 1; i++)
                {
                    compatActivity.SupportFragmentManager.PopBackStack();
                }
            }
        }
    }
}