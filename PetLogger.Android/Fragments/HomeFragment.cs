﻿using Android.OS;
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
    public class HomeFragment : Fragment, AbsListView.IMultiChoiceModeListener
    {
        public static HomeFragment Instantiate() => new HomeFragment();

        private IncidentLoggerAdapter _loggerAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_home, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Pet Logger");
            ToolbarHelper.HideToolbarBackButton(Activity);

            var loggerRecycler = View.FindViewById<RecyclerView>(Resource.Id.incident_logger_list);
            SetUpLoggerRecycler(loggerRecycler, view);

            SetUpFloatingActionMenu(view);
        }

        private void SetUpLoggerRecycler(RecyclerView recyclerView, View view)
        {
            ((SimpleItemAnimator)recyclerView.GetItemAnimator()).SupportsChangeAnimations = false;
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            recyclerView.AddItemDecoration(new VerticalSpaceItemDecoration(40));

            _loggerAdapter = new IncidentLoggerAdapter(Activity, GetIncidentLoggers().ToList());
            _loggerAdapter.SetMultiChoiceModeListener(this);
            _loggerAdapter.ItemClick += (s, args) => FragmentHelper.Add(Activity, IncidentDetailFragment.Instantiate(args.Item.PetID, args.Item.IncidentTypeID));
            _loggerAdapter.IncidentLogged += (s, args) => Snackbar.Make(view, args.Logger.Title + " logged", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null)
                .Show();

            recyclerView.SetAdapter(_loggerAdapter);
        }

        private void SetUpFloatingActionMenu(View view)
        {
            var fam = view.FindViewById<FloatingActionMenu>(Resource.Id.fam_add);
            //fam.Visibility = ViewStates.Visible;

            var fabAddPet = view.FindViewById<FloatingActionButton>(Resource.Id.fam_add_pet);
            fabAddPet.Click += (s, args) => FragmentHelper.Add(Activity, AddEntityFragment<Pet>.Instantiate("Pet"));

            var fabAddIncidentType = view.FindViewById<FloatingActionButton>(Resource.Id.fam_add_incident_type);
            fabAddIncidentType.Click += (s, args) => FragmentHelper.Add(Activity, AddEntityFragment<IncidentType>.Instantiate("Incident Type"));

            var fabAddLogger = view.FindViewById<FloatingActionButton>(Resource.Id.fam_add_logger);
            fabAddLogger.Click += (s, args) => FragmentHelper.Add(Activity, AddEntityFragment<LoggerDefinition>.Instantiate("Incident Logger"));
        }

        private IEnumerable<IncidentLogger> GetIncidentLoggers() => DBTable.GetAll<LoggerDefinition>()
            .OrderBy(d => d.Order)
            .Select(d => new IncidentLogger(d));

        private void DeleteSelectedItems(ActionMode mode)
        {
            AlertDialogHelper.DisplayDialog(Activity, "Remove selected loggers", "Are you sure? This will remove ALL selected incident loggers!", () =>
            {
                var dialog = ProgressDialogHelper.Display(Activity, "Removing selected loggers...");

                ProgressDialogHelper.RunTask(Activity, dialog, () =>
                {
                    _loggerAdapter.RemoveSelectedLoggers();
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
                    _loggerAdapter.SetAllItemsChecked(true);
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