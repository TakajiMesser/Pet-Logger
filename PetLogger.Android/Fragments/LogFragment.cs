using Android.OS;
using Android.Views;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Models.Logging;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Fragments
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

            ToolbarHelper.ShowToolbar(Activity, "Debug Log");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var fileTextView = View.FindViewById<FileTextView>(Resource.Id.fileText);
            fileTextView.SetFile(DebugLog.BaseLog.FilePath);
        }
    }
}
