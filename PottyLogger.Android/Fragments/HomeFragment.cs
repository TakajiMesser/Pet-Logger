using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Fragments
{
    public class HomeFragment : Fragment
    {
        public static HomeFragment Instantiate() => new HomeFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_home, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var peeButton = view.FindViewById<TextView>(Resource.Id.pee_button);
            peeButton.Click += (s, args) =>
            {
                Snackbar.Make(view, "Log pee", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null)
                    .Show();
            };

            var pooButton = view.FindViewById<TextView>(Resource.Id.poo_button);
            pooButton.Click += (s, args) =>
            {
                Snackbar.Make(view, "Log poo", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null)
                    .Show();
            };
        }
    }
}
