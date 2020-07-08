using Android.OS;
using Android.Views;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Fragments
{
    public class SettingsFragment : Fragment
    {
        public static SettingsFragment Instantiate() => new SettingsFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_settings, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState) => base.OnViewCreated(view, savedInstanceState);
    }
}
