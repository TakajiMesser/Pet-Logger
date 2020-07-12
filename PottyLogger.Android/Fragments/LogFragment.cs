using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using PottyLogger.Droid.Components;
using PottyLogger.Droid.Models.Logging;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Fragments
{
    public class DebugLogFragment : Fragment
    {
        public static DebugLogFragment Instantiate() => new DebugLogFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_log, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var activity = Activity as AppCompatActivity;
            activity.SupportActionBar.Title = "Debug Log";

            var fileTextView = View.FindViewById<FileTextView>(Resource.Id.fileText);
            fileTextView.SetFile(DebugLog.BaseLog.FilePath);
        }
    }
}
