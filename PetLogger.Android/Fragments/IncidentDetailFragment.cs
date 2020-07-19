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
    public class IncidentDetailFragment : Fragment
    {
        public static IncidentDetailFragment Instantiate(int petID, int incidentTypeID)
        {
            var fragment = new IncidentDetailFragment
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutInt("petID", petID);
            fragment.Arguments.PutInt("incidentTypeID", incidentTypeID);
            return fragment;
        }

        private Pet _pet;
        private IncidentType _incidentType;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _pet = DBTable.Get<Pet>(Arguments.GetInt("petID"));
            _incidentType = DBTable.Get<IncidentType>(Arguments.GetInt("incidentTypeID"));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_incident_detail, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Indicent Details");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = _pet.Name + " " + _incidentType.Name + " Details";
        }
    }
}
