using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using PottyLogger.Droid.Adapters;
using PottyLogger.Droid.Components;
using PottyLogger.Shared.Data;
using PottyLogger.Shared.DataAccessLayer;
using PottyLogger.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Fragments
{
    public class HomeFragment : Fragment//, AbsListView.IMultiChoiceModeListener
    {
        public static HomeFragment Instantiate() => new HomeFragment();

        private InfoCardAdapter _infoAdapter;
        private LoggerAdapter _loggerAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_home, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var activity = Activity as AppCompatActivity;
            activity.SupportActionBar.Title = "Pet Logger";

            var infoRecycler = View.FindViewById<RecyclerView>(Resource.Id.info_list);
            SetUpInfoRecycler(infoRecycler);

            var loggerRecycler = View.FindViewById<RecyclerView>(Resource.Id.logger_list);
            SetUpLoggerRecycler(loggerRecycler, view);
        }

        private void SetUpInfoRecycler(RecyclerView recyclerView)
        {
            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            recyclerView.AddItemDecoration(new VerticalSpaceItemDecoration(40));

            _infoAdapter = new InfoCardAdapter(Activity, GetInfoCards().ToList());
            /*_infoAdapter.ItemClick += (s, e) =>
            {
                Activity.SupportFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.content_frame, TableFragment.Instantiate(e.Item.Name))
                    .Commit();
            };*/
            //_infoAdapter.SetMultiChoiceModeListener(this);
            recyclerView.SetAdapter(_infoAdapter);
        }

        private void SetUpLoggerRecycler(RecyclerView recyclerView, View view)
        {
            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(new GridLayoutManager(Context, 4));
            recyclerView.AddItemDecoration(new VerticalSpaceItemDecoration(40));

            _loggerAdapter = new LoggerAdapter(Activity, GetIncidentLoggers().ToList());
            _loggerAdapter.ItemClick += (s, args) =>
            {
                args.Item.LogIncident();
                Snackbar.Make(view, "Log pee", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null)
                    .Show();
            };
            //_infoAdapter.SetMultiChoiceModeListener(this);
            recyclerView.SetAdapter(_loggerAdapter);
        }

        private IEnumerable<InfoCard> GetInfoCards() => DBTable.GetAll<InfoDefinition>()
            .OrderBy(d => d.Order)
            .Select(d => new InfoCard(d));

        private IEnumerable<IncidentLogger> GetIncidentLoggers() => DBTable.GetAll<LoggerDefinition>()
            .OrderBy(d => d.Order)
            .Select(d => new IncidentLogger(d));
    }
}
