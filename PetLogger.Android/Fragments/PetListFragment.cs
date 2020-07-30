using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using PetLogger.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace PetLogger.Droid.Fragments
{
    public class PetListFragment : Fragment, /*ISearchFragment, */AbsListView.IMultiChoiceModeListener
    {
        private ReminderAdapter _reminderAdapter;

        public static ReminderListFragment Instantiate() => new ReminderListFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_reminder_list, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Reminders");
            ToolbarHelper.HideToolbarBackButton(Activity);

            var reminderRecycler = View.FindViewById<RecyclerView>(Resource.Id.reminder_list);
            SetUpReminderAdapter(reminderRecycler, view);

            var fabAddReminder = view.FindViewById<FloatingActionButton>(Resource.Id.fab_add_reminder);
            fabAddReminder.Click += (s, args) => FragmentHelper.Add(Activity, AddEntityFragment<Reminder>.Instantiate("Reminder"));
        }

        private void SetUpReminderAdapter(RecyclerView recyclerView, View view)
        {
            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            recyclerView.AddItemDecoration(new VerticalSpaceItemDecoration(40));

            _reminderAdapter = new ReminderAdapter(Activity, DBTable.GetAll<Reminder>().ToList());
            _reminderAdapter.SetMultiChoiceModeListener(this);
            //_reminderAdapter.ItemClick += (s, args) => FragmentHelper.Add(Activity, IncidentDetailFragment.Instantiate(args.Item.PetID, args.Item.IncidentTypeID));

            recyclerView.SetAdapter(_reminderAdapter);
        }

        private void CreateReminder(Reminder reminder)
        {

        }

        private void DeleteSelectedItems(ActionMode mode)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Remove selected reminders", "Are you sure? This will remove ALL selected reminders!", () =>
            {
                var dialog = ProgressDialogHelper.Display(Activity, "Removing selected reminders...");

                ProgressDialogHelper.RunTask(Activity, dialog, () =>
                {
                    _reminderAdapter.RemoveSelectedAlarms();
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
                    _reminderAdapter.SetAllItemsChecked(true);
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
