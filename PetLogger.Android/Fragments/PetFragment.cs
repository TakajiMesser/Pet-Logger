using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using PetLogger.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Fragments
{
    public class PetFragment : Fragment
    {
        public static PetFragment Instantiate(Pet pet)
        {
            var fragment = new PetFragment()
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutInt("petID", pet.ID);

            return fragment;
        }

        private Pet _pet;
        private PetIncidentAdapter _petIncidentAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _pet = DBTable.Get<Pet>(Arguments.GetInt("petID"));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_pet, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, _pet.Name);
            ToolbarHelper.HideToolbarBackButton(Activity);

            var petIncidentRecycler = view.FindViewById<RecyclerView>(Resource.Id.pet_incident_recycler);
            SetUpPetIncidentAdapter(petIncidentRecycler, view);
        }

        private void SetUpPetIncidentAdapter(RecyclerView recyclerView, View view)
        {
            var layoutManager = new LinearLayoutManager(Context);

            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new HorizontalDividerItemDecoration(Context, layoutManager.Orientation, Resource.Drawable.horizontal_divider_semi)
            {
                ShouldShowAfterLast = true
            });

            _petIncidentAdapter = new PetIncidentAdapter(Activity, GetPetIncidents().ToList());
            _petIncidentAdapter.ItemClick += (s, args) => FragmentHelper.Push(Activity, IncidentDetailsFragment.Instantiate(args.Item.PetID, args.Item.IncidentTypeID));

            recyclerView.SetAdapter(_petIncidentAdapter);
        }

        private IEnumerable<PetIncident> GetPetIncidents()
        {
            // TODO - We need to create a PetIncident for ALL available incident types
            // However, we then need to sort them by most-recent logged incident
            var latestPetIncidents = new List<Tuple<DateTime, PetIncident>>();

            foreach (var incidentType in DBTable.GetAll<IncidentType>())
            {
                var latestIncidentTime = DBTable.GetAll<Incident>(i => i.PetID == _pet.ID && i.IncidentTypeID == incidentType.ID)
                    .OrderByDescending(i => i.Time)
                    .Select(i => i.Time)
                    .FirstOrDefault();

                latestPetIncidents.Add(Tuple.Create(latestIncidentTime, new PetIncident(_pet, incidentType)));
            }

            foreach (var latestPetIncident in latestPetIncidents.OrderByDescending(t => t.Item1))
            {
                yield return latestPetIncident.Item2;
            }

            /*return DBTable.GetAll<IncidentType>()
                .OrderByDescending(t => DBTable.GetAll<Incident>(i => i.PetID == _pet.ID && i.IncidentTypeID == t.ID)
                    .OrderByDescending(i => i.Time)
                    .Select(i => i.Time)
                    .First())
                .Select(t => new PetIncident(_pet, t));*/
        }
    }
}
