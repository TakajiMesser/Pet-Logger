using Android.OS;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using Fragment = Android.Support.V4.App.Fragment;
using View = Android.Views.View;

namespace PetLogger.Droid.Fragments
{
    public class IncidentDetailsFragment : Fragment
    {
        public static IncidentDetailsFragment Instantiate(int petID, int incidentTypeID)
        {
            var fragment = new IncidentDetailsFragment
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutInt("petID", petID);
            fragment.Arguments.PutInt("incidentTypeID", incidentTypeID);
            return fragment;
        }

        private Pet _pet;
        private IncidentType _incidentType;

        private LinearLayout _detailList;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _pet = DBTable.Get<Pet>(Arguments.GetInt("petID"));
            _incidentType = DBTable.Get<IncidentType>(Arguments.GetInt("incidentTypeID"));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_incident_details, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Indicent Details");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = _pet.Name + " " + _incidentType.Name + " Details";

            _detailList = view.FindViewById<LinearLayout>(Resource.Id.detail_list);

            AddDetailView(Resource.Drawable.baseline_addchart_black_36dp, "", "");

            // Bar graph
            // Show incident count over last X days
            // Running average?
            // Lifetime count?
        }

        private void AddDetailView(int iconResourceID, string title, string value)
        {
            var view = LayoutInflater.From(Context).Inflate(Resource.Layout.item_incident_detail, _detailList, false);

            var detailIcon = view.FindViewById<ImageView>(Resource.Id.detail_icon);
            detailIcon.SetImageResource(iconResourceID);
            detailIcon.SetScaleType(ImageView.ScaleType.FitCenter);

            view.FindViewById<TextView>(Resource.Id.detail_value).Text = value;
            view.FindViewById<TextView>(Resource.Id.detail_label).Text = title;

            _detailList.AddView(view);
        }
    }
}
