using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.DataAccessLayer;
using Fragment = Android.Support.V4.App.Fragment;
using View = Android.Views.View;

namespace PetLogger.Droid.Fragments
{
    public class TableFragment : Fragment, /*ISearchFragment, */AbsListView.IMultiChoiceModeListener
    {
        private DataTableView _dataTable;
        private string _tableName;

        public static TableFragment Instantiate(string tableName)
        {
            var fragment = new TableFragment()
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutString("tableName", tableName);

            return fragment;
        }

        public void Filter(string text) => _dataTable.Filter(text);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _tableName = Arguments.GetString("tableName");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_table, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, _tableName);
            ToolbarHelper.ShowToolbarBackButton(Activity);

            _dataTable = view.FindViewById<DataTableView>(Resource.Id.table);
            _dataTable.Connection = DBAccess.Connection;
            _dataTable.SetTableMapping(DBAccess.GetMapping(DBAccess.ParseTableName(_tableName)));
            _dataTable.SetMultiChoiceModeListener(this);
            _dataTable.ItemClick += (s, e) =>
            {
                Activity.SupportFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.content_frame, TableRowFragment.Instantiate(_tableName, e.Row.ID))
                    .Commit();
            };

            var addButton = view.FindViewById<FloatingActionButton>(Resource.Id.fab_add);
            addButton.Click += (s, e) =>
            {
                Activity.SupportFragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.content_frame, TableRowFragment.Instantiate(_tableName))
                    .Commit();
            };
        }

        private void DeleteSelectedItems(ActionMode mode)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Delete selected rows", "Are you sure?", () =>
            {
                ProgressDialogHelper.RunTask(Activity, "Deleting rows...", () =>
                {
                    DBTable.DeleteAll(_dataTable.SelectedIDs, DBAccess.ParseTableName(_tableName));
                    _dataTable.DeleteSelectedItems();
                });

                Activity.RunOnUiThread(() => mode.Finish());
            });
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            mode.MenuInflater.Inflate(Resource.Menu.menu_list, menu);
            return true;
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete:
                    DeleteSelectedItems(mode);
                    return true;
                case Resource.Id.action_select_all:
                    _dataTable.SelectAllItems();
                    return true;
                default:
                    return false;
            }
        }

        public void OnItemCheckedStateChanged(ActionMode mode, int position, long id, bool @checked) { }

        public void OnDestroyActionMode(ActionMode mode) { }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => false;
    }
}
