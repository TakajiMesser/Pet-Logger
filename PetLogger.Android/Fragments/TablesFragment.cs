using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Models;
using PetLogger.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Fragments
{
    public class TablesFragment : Fragment, /*ISearchFragment, */AbsListView.IMultiChoiceModeListener
    {
        public static TablesFragment Instantiate() => new TablesFragment();

        private TableAdapter _tableAdapter;
        private RecyclerView _recyclerView;

        public void Filter(string text) => _tableAdapter.Filter.InvokeFilter(text);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_tables, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Database Tables");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.table_recycler);
            ((SimpleItemAnimator)_recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));

            var spacingDecoration = new VerticalSpaceItemDecoration(40);
            _recyclerView.AddItemDecoration(spacingDecoration);

            _tableAdapter = new TableAdapter(Activity, GetTableCards().ToList());
            _tableAdapter.ItemClick += (s, e) =>
            {
                Activity.SupportFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.content_frame, TableFragment.Instantiate(e.Item.Name))
                    .Commit();
            };
            //_tableAdapter.SetMultiChoiceModeListener(this);
            _recyclerView.SetAdapter(_tableAdapter);
        }

        private IEnumerable<TableCard> GetTableCards()
        {
            foreach (var tableType in DBAccess.EntityTypes)
            {
                yield return new TableCard()
                {
                    Name = tableType.Name,
                    RowCount = DBTable.GetCount(tableType),
                    Columns = DBTable.GetColumnNames(tableType).ToList()
                };
            }
        }

        private void DeleteSelectedItems(ActionMode mode)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Clear selected tables", "Are you sure? This will drop ALL data in the selected tables!", () =>
            {
                var dialog = ProgressDialogHelper.Display(Activity, "Clearing selected tables...");

                ProgressDialogHelper.RunTask(Activity, dialog, () =>
                {
                    _tableAdapter.ClearSelectedTables();
                    Activity.RunOnUiThread(() => mode.Finish());
                });
            });
        }

        public void OnItemCheckedStateChanged(ActionMode mode, int position, long id, bool @checked) { }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete:
                    DeleteSelectedItems(mode);
                    return true;
                case Resource.Id.action_select_all:
                    _tableAdapter.SetAllItemsChecked(true);
                    return true;
                default:
                    return false;
            }
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            mode.MenuInflater.Inflate(Resource.Menu.menu_list, menu);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode) { }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => false;
    }
}
