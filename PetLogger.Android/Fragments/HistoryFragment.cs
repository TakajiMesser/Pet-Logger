using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System.Collections.Generic;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Fragments
{
    public class HistoryFragment : Fragment
    {
        public const int LAZY_LOAD_LIMIT = 20;

        public static HistoryFragment Instantiate() => new HistoryFragment();

        private HistoryAdapter _historyAdapter;
        private bool _isFetchingRows = false;
        private int _rowOffset;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_history, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Incident History");
            ToolbarHelper.HideToolbarBackButton(Activity);

            var historyRecycler = view.FindViewById<RecyclerView>(Resource.Id.history_recycler);
            SetUpHistoryAdapter(historyRecycler, view);
        }

        private void SetUpHistoryAdapter(RecyclerView recyclerView, View view)
        {
            var layoutManager = new LinearLayoutManager(Context);

            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new HorizontalDividerItemDecoration(Context, layoutManager.Orientation, Resource.Drawable.horizontal_divider_semi));

            _historyAdapter = new HistoryAdapter(Activity);
            recyclerView.SetAdapter(_historyAdapter);

            recyclerView.ScrollChange += (s, args) =>
            {
                //if (args.ScrollY > args.OldScrollY)
                {
                    var firstVisibleItemPosition = layoutManager.FindFirstVisibleItemPosition();

                    if (!_isFetchingRows && firstVisibleItemPosition > 0 && _historyAdapter.ItemCount % LAZY_LOAD_LIMIT == 0 && firstVisibleItemPosition + recyclerView.ChildCount >= _historyAdapter.ItemCount)
                    {
                        _isFetchingRows = true;

                        /*var dialog = new ProgressDialog(Context, Resource.Style.ProgressDialogTheme)
                        {
                            Indeterminate = true
                        };
                        dialog.SetCancelable(false);
                        dialog.Show();*/

                        _historyAdapter.AddItems(GetNextIncidentSet(_rowOffset));
                        _rowOffset += LAZY_LOAD_LIMIT;

                        //dialog.Dismiss();
                        _isFetchingRows = false;
                    }
                }
            };

            // Get initial set of rows
            _isFetchingRows = true;

            _rowOffset = 0;
            _historyAdapter.AddItems(GetNextIncidentSet(_rowOffset));
            _rowOffset += LAZY_LOAD_LIMIT;

            _isFetchingRows = false;
        }

        public IEnumerable<Incident> GetNextIncidentSet(int offset) => DBAccess.Query<Incident>("SELECT * FROM " + DBAccess.GetMapping<Incident>().TableName
            + " ORDER BY `Time` DESC"
            + " LIMIT " + LAZY_LOAD_LIMIT
            + " OFFSET " + offset);

        // TODO - HistoryAdapter should lazy load incident history in sets
        private IEnumerable<Incident> GetIncidentHistory() => DBTable.GetAll<Incident>()
            .OrderByDescending(i => i.Time);
    }
}
