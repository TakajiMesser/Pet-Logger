using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;
using Fragment = Android.Support.V4.App.Fragment;
using View = Android.Views.View;

namespace PetLogger.Droid.Fragments
{
    public class ReminderFragment : Fragment
    {
        public static ReminderFragment Instantiate() => new ReminderFragment();

        public static ReminderFragment Instantiate(int reminderID)
        {
            var bundle = new Bundle();
            bundle.PutInt("reminderID", reminderID);

            var fragment = new ReminderFragment()
            {
                Arguments = bundle
            };

            return fragment;
        }

        private Reminder _reminder;

        private Spinner _reminderTypeSpinner;
        private TimePicker _timePicker;
        private Spinner _soundSpinner;
        private ToggleButton _vibrateToggle;
        private NumberPicker _snoozePicker;
        private Spinner _petSpinner;
        private Spinner _incidentTypeSpinner;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            if (Arguments != null)
            {
                var reminderID = Arguments.GetInt("reminderID");
                if (reminderID > 0)
                {
                    _reminder = DBTable.Get<Reminder>(reminderID);
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_reminder, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, "Reminder");
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = (_reminder != null ? "Add" : "Update") + " Reminder";

            _reminderTypeSpinner = view.FindViewById<Spinner>(Resource.Id.reminder_type_spinner);
            _reminderTypeSpinner.Adapter = new EnumAdapter<ReminderTypes>(Context);
            _reminderTypeSpinner.ItemSelected += ReminderTypeSpinner_ItemSelected;

            _timePicker = view.FindViewById<TimePicker>(Resource.Id.time_picker);

            _soundSpinner = view.FindViewById<Spinner>(Resource.Id.sound_spinner);
            _soundSpinner.Adapter = new RingtoneAdapter(Context, Android.Media.RingtoneType.Alarm);

            _vibrateToggle = view.FindViewById<ToggleButton>(Resource.Id.vibrate_toggle);

            _snoozePicker = view.FindViewById<NumberPicker>(Resource.Id.snooze_picker);

            _petSpinner = view.FindViewById<Spinner>(Resource.Id.pet_spinner);
            _petSpinner.Adapter = new ForeignEntityAdapter(Context, typeof(Reminder).GetProperty(nameof(Reminder.PetID)), DBTable.GetAll<Pet>());

            _incidentTypeSpinner = view.FindViewById<Spinner>(Resource.Id.incident_type_spinner);
            _incidentTypeSpinner.Adapter = new ForeignEntityAdapter(Context, typeof(Reminder).GetProperty(nameof(Reminder.IncidentTypeID)), DBTable.GetAll<IncidentType>());

            if (_reminder != null)
            {
                LoadValues();
            }
            else
            {
                LoadDefaultValues();
            }

            var submitButton = view.FindViewById<AppCompatButton>(Resource.Id.btn_submit);
            submitButton.Click += SubmitButton_Click;
        }

        private void ReminderTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var reminderTypeAdapter = _reminderTypeSpinner.Adapter as EnumAdapter<ReminderTypes>;
            var reminderType = reminderTypeAdapter.GetValue(e.Position);

            switch (reminderType)
            {
                case ReminderTypes.Alarm:
                    _soundSpinner.Adapter = new RingtoneAdapter(Context, Android.Media.RingtoneType.Alarm);
                    _soundSpinner.Visibility = ViewStates.Visible;
                    break;
                case ReminderTypes.Notification:
                    _soundSpinner.Adapter = new RingtoneAdapter(Context, Android.Media.RingtoneType.Notification);
                    _soundSpinner.Visibility = ViewStates.Visible;
                    break;
                default:
                    _soundSpinner.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void SubmitButton_Click(object sender, System.EventArgs e)
        {
            ProgressDialogHelper.RunTask(Activity, "Submitting...", () =>
            {
                if (_reminder != null)
                {
                    UpdateValues();
                    _reminder.Update();
                }
                else
                {
                    _reminder = new Reminder();

                    UpdateValues();
                    _reminder.Insert();
                }

                ReminderHelper.ScheduleReminder(Context, _reminder);

                Activity.RunOnUiThread(() =>
                {
                    Toast.MakeText(Context, "Successfully added new entity", ToastLength.Long).Show();
                    FragmentHelper.PopOne(Activity);
                });
            });
        }

        private void LoadDefaultValues()
        {
            if (_reminderTypeSpinner.Adapter is EnumAdapter<ReminderTypes> enumAdapter)
            {
                //var position = enumAdapter.GetPosition(_reminder.ReminderType);
                //_reminderTypeSpinner.SetSelection(position);
            }

            _timePicker.Hour = 3;
            _timePicker.Minute = 0;

            if (_soundSpinner.Adapter is RingtoneAdapter ringtoneAdapter)
            {
                //var position = ringtoneAdapter.GetPosition(_reminder.SoundPath);
                //_soundSpinner.SetSelection(position);
            }

            _vibrateToggle.Checked = true;
            _snoozePicker.Value = 10;

            if (_petSpinner.Adapter is ForeignEntityAdapter petAdapter)
            {
                //var position = petAdapter.GetPosition(_reminder.PetID);
                //_petSpinner.SetSelection(position);
            }

            if (_incidentTypeSpinner.Adapter is ForeignEntityAdapter incidentTypeAdapter)
            {
                //var position = incidentTypeAdapter.GetPosition(_reminder.IncidentTypeID);
                //_incidentTypeSpinner.SetSelection(position);
            }
        }

        private void LoadValues()
        {
            if (_reminderTypeSpinner.Adapter is EnumAdapter<ReminderTypes> enumAdapter)
            {
                var position = enumAdapter.GetPosition(_reminder.ReminderType);
                _reminderTypeSpinner.SetSelection(position);
            }

            _timePicker.Hour = _reminder.TimeBetween.Hours;
            _timePicker.Minute = _reminder.TimeBetween.Minutes;

            if (_soundSpinner.Adapter is RingtoneAdapter ringtoneAdapter)
            {
                var position = ringtoneAdapter.GetPosition(_reminder.SoundPath);
                _soundSpinner.SetSelection(position);
            }

            _vibrateToggle.Checked = _reminder.Vibrate;
            _snoozePicker.Value = _reminder.SnoozeMinutes;

            if (_petSpinner.Adapter is ForeignEntityAdapter petAdapter)
            {
                var position = petAdapter.GetPosition(_reminder.PetID);
                _petSpinner.SetSelection(position);
            }

            if (_incidentTypeSpinner.Adapter is ForeignEntityAdapter incidentTypeAdapter)
            {
                var position = incidentTypeAdapter.GetPosition(_reminder.IncidentTypeID);
                _incidentTypeSpinner.SetSelection(position);
            }
        }

        private void UpdateValues()
        {
            if (_reminderTypeSpinner.Adapter is EnumAdapter<ReminderTypes> enumAdapter)
            {
                _reminder.ReminderType = (ReminderTypes)enumAdapter.GetValue(_reminderTypeSpinner.SelectedItemPosition);
            }

            _reminder.TimeBetween = TimeSpan.FromHours(_timePicker.Hour) + TimeSpan.FromMinutes(_timePicker.Minute);

            if (_soundSpinner.Adapter is RingtoneAdapter ringtoneAdapter)
            {
                _reminder.SoundPath = ringtoneAdapter.GetPath(_soundSpinner.SelectedItemPosition);
            }

            _reminder.Vibrate = _vibrateToggle.Checked;
            _reminder.SnoozeMinutes = _snoozePicker.Value;

            if (_petSpinner.Adapter is ForeignEntityAdapter petAdapter)
            {
                _reminder.PetID = petAdapter.GetID(_petSpinner.SelectedItemPosition);
            }

            if (_incidentTypeSpinner.Adapter is ForeignEntityAdapter incidentTypeAdapter)
            {
                _reminder.IncidentTypeID = incidentTypeAdapter.GetID(_incidentTypeSpinner.SelectedItemPosition);
            }
        }
    }
}
