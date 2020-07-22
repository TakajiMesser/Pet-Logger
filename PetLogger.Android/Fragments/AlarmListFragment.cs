using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Models;
using PetLogger.Shared.DataAccessLayer;
using PetLogger.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;
using PetLogger.Shared.Data;

namespace PetLogger.Droid.Fragments
{
    public class AlarmListFragment : Fragment, /*ISearchFragment, */AbsListView.IMultiChoiceModeListener
    {
        private AlarmAdapter _alarmAdapter;

        public static AlarmListFragment Instantiate() => new AlarmListFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_alarm_list, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Alarms");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var alarmRecycler = View.FindViewById<RecyclerView>(Resource.Id.alarm_list);
            SetUpAlarmRecycler(alarmRecycler, view);

            var fabAddAlarm = view.FindViewById<FloatingActionButton>(Resource.Id.fab_add_alarm);
            fabAddAlarm.Click += (s, args) => FragmentHelper.Add(Activity, AddEntityFragment<AlarmDefinition>.Instantiate("Alarm"));
        }

        private void SetUpAlarmRecycler(RecyclerView recyclerView, View view)
        {
            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            recyclerView.AddItemDecoration(new VerticalSpaceItemDecoration(40));

            _alarmAdapter = new AlarmAdapter(Activity, GetAlarms().ToList());
            _alarmAdapter.SetMultiChoiceModeListener(this);
            //_alarmAdapter.ItemClick += (s, args) => FragmentHelper.Add(Activity, IncidentDetailFragment.Instantiate(args.Item.PetID, args.Item.IncidentTypeID));

            recyclerView.SetAdapter(_alarmAdapter);
        }

        private IEnumerable<Alarm> GetAlarms() => DBTable.GetAll<AlarmDefinition>()
            .Select(d => new Alarm(d));

        private void DeleteSelectedItems(ActionMode mode)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Remove selected alarms", "Are you sure? This will remove ALL selected alarms!", () =>
            {
                var dialog = ProgressDialogHelper.Display(Activity, "Removing selected alarms...");

                ProgressDialogHelper.RunTask(Activity, dialog, () =>
                {
                    _alarmAdapter.RemoveSelectedAlarms();
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
                    _alarmAdapter.SetAllItemsChecked(true);
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
