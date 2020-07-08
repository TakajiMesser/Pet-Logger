using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using PottyLogger.Droid.Components;
using Fragment = Android.Support.V4.App.Fragment;

namespace PottyLogger.Droid.Fragments
{
    public class HistoryFragment : Fragment, AbsListView.IMultiChoiceModeListener
    {
        private CSVTableView _tableView;

        public static HistoryFragment Instantiate() => new HistoryFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_history, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var activity = Activity as AppCompatActivity;
            activity.SupportActionBar.Title = "History";

            _tableView = view.FindViewById<CSVTableView>(Resource.Id.table);
            _tableView.SetMultiChoiceModeListener(this);
            _tableView.ItemClick += (s, e) =>
            {
                /*Activity.FragmentManager.BeginTransaction()
                    .AddToBackStack(null)
                    .Replace(Resource.Id.frame_layout, AddEditRowFragment.Instantiate(_tableName, e.Row.ID))
                    .Commit();*/
            };
        }

        private void DeleteSelectedItems(ActionMode mode)
        {
            /*AlertDialogHelper.DisplayDialog(Activity, "Delete selected rows", "Are you sure?", () =>
            {
                ProgressDialogHelper.RunTask(Activity, "Deleting rows...", () =>
                {
                    DBTable.DeleteAll(_dataTable.SelectedIDs, DBAccess.ParseTableName(_tableName));
                    _dataTable.DeleteSelectedItems();
                });

                Activity.RunOnUiThread(() => mode.Finish());
            });*/
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            //mode.MenuInflater.Inflate(Resource.Menu.list_menu, menu);
            //return true;
            return false;
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            switch (item.ItemId)
            {
                /*case Resource.Id.action_delete:
                    DeleteSelectedItems(mode);
                    return true;
                case Resource.Id.action_select_all:
                    _dataTable.SelectAllItems();
                    return true;*/
                default:
                    return false;
            }
        }

        public void OnItemCheckedStateChanged(ActionMode mode, int position, long id, bool @checked) { }

        public void OnDestroyActionMode(ActionMode mode) { }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => false;
    }
}
