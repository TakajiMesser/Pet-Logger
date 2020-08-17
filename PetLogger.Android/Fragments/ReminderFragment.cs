using Android.Media;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;
using System.Timers;
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
        private TextView _soundLabel;
        private Spinner _soundSpinner;
        private EditDurationView _timePicker;
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

            _soundSpinner = view.FindViewById<Spinner>(Resource.Id.sound_spinner);
            _soundSpinner.Adapter = new RingtoneAdapter(Context, Android.Media.RingtoneType.Notification);
            _soundSpinner.ItemSelected += SoundSpinner_ItemSelected;

            _soundLabel = view.FindViewById<TextView>(Resource.Id.sound_label);

            _timePicker = view.FindViewById<EditDurationView>(Resource.Id.time_picker);
            _vibrateToggle = view.FindViewById<ToggleButton>(Resource.Id.vibrate_toggle);
            
            _snoozePicker = view.FindViewById<NumberPicker>(Resource.Id.snooze_picker);
            _snoozePicker.MinValue = 0;
            _snoozePicker.MaxValue = 60;

            _petSpinner = view.FindViewById<Spinner>(Resource.Id.pet_spinner);
            _petSpinner.Adapter = new ForeignEntityAdapter(Context, PetLogger.Shared.Helpers.EntityHelper.GetIdentifierProperty(typeof(Pet)), DBTable.GetAll<Pet>());

            _incidentTypeSpinner = view.FindViewById<Spinner>(Resource.Id.incident_type_spinner);
            _incidentTypeSpinner.Adapter = new ForeignEntityAdapter(Context, PetLogger.Shared.Helpers.EntityHelper.GetIdentifierProperty(typeof(IncidentType)), DBTable.GetAll<IncidentType>());

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

        private void SoundSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var ringtoneAdapter = _soundSpinner.Adapter as RingtoneAdapter;
            var soundPath = ringtoneAdapter.GetPath(e.Position);

            if (!string.IsNullOrEmpty(soundPath))
            {
                var ringtone = RingtoneManager.GetRingtone(Context, Android.Net.Uri.Parse(soundPath));

                if (ringtone != null)
                {
                    var timer = new Timer(3000)
                    {
                        AutoReset = false
                    };

                    timer.Elapsed += (s, args) =>
                    {
                        ringtone.Stop();
                        timer.Stop();
                    };

                    ringtone.Play();
                    timer.Start();
                }
            }
        }

        private void ReminderTypeSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var reminderTypeAdapter = _reminderTypeSpinner.Adapter as EnumAdapter<ReminderTypes>;
            var reminderType = reminderTypeAdapter.GetValue(e.Position);

            switch (reminderType)
            {
                case ReminderTypes.Alarm:
                    _soundSpinner.Adapter = new RingtoneAdapter(Context, Android.Media.RingtoneType.Alarm);
                    _soundLabel.Visibility = ViewStates.Visible;
                    _soundSpinner.Visibility = ViewStates.Visible;
                    break;
                case ReminderTypes.Notification:
                    _soundSpinner.Adapter = new RingtoneAdapter(Context, Android.Media.RingtoneType.Notification);
                    _soundLabel.Visibility = ViewStates.Visible;
                    _soundSpinner.Visibility = ViewStates.Visible;
                    break;
                default:
                    _soundLabel.Visibility = ViewStates.Gone;
                    _soundSpinner.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void LoadDefaultValues()
        {
            if (_reminderTypeSpinner.Adapter is EnumAdapter<ReminderTypes> enumAdapter)
            {
                //var position = enumAdapter.GetPosition(_reminder.ReminderType);
                //_reminderTypeSpinner.SetSelection(position);
            }

            if (_soundSpinner.Adapter is RingtoneAdapter ringtoneAdapter)
            {
                //var position = ringtoneAdapter.GetPosition(_reminder.SoundPath);
                //_soundSpinner.SetSelection(position);
            }

            _timePicker.Duration = TimeSpan.FromHours(3);
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
            if (_reminderTypeSpinner.Adapter is EnumAdapter<ReminderTypes> reminderTypeAdapter)
            {
                var position = reminderTypeAdapter.GetValuePosition(_reminder.ReminderType);
                _reminderTypeSpinner.SetSelection(position);
            }

            if (_soundSpinner.Adapter is RingtoneAdapter ringtoneAdapter)
            {
                var position = ringtoneAdapter.GetPathPosition(_reminder.SoundPath);
                _soundSpinner.SetSelection(position, false);
            }

            _timePicker.Duration = _reminder.TimeBetween;
            _vibrateToggle.Checked = _reminder.Vibrate;
            _snoozePicker.Value = _reminder.SnoozeMinutes;

            if (_petSpinner.Adapter is ForeignEntityAdapter petAdapter)
            {
                var position = petAdapter.GetIDPosition(_reminder.PetID);
                _petSpinner.SetSelection(position);
            }

            if (_incidentTypeSpinner.Adapter is ForeignEntityAdapter incidentTypeAdapter)
            {
                var position = incidentTypeAdapter.GetIDPosition(_reminder.IncidentTypeID);
                _incidentTypeSpinner.SetSelection(position);
            }
        }

        private void UpdateValues()
        {
            if (_reminderTypeSpinner.Adapter is EnumAdapter<ReminderTypes> enumAdapter)
            {
                _reminder.ReminderType = enumAdapter.GetValue(_reminderTypeSpinner.SelectedItemPosition);
            }

            if (_soundSpinner.Adapter is RingtoneAdapter ringtoneAdapter)
            {
                _reminder.SoundPath = ringtoneAdapter.GetPath(_soundSpinner.SelectedItemPosition);
            }

            _reminder.TimeBetween = _timePicker.Duration;
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

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ProgressDialogHelper.RunTask(Activity, "Submitting...", () =>
            {
                var isUpdate = false;

                if (_reminder != null)
                {
                    isUpdate = true;
                    UpdateValues();
                    _reminder.Update();
                }
                else
                {
                    _reminder = new Reminder();

                    UpdateValues();
                    _reminder.Insert();
                }

                var latestIncident = DBTable.GetAll<Incident>(i => i.PetID == _reminder.PetID && i.IncidentTypeID == _reminder.IncidentTypeID)
                    .OrderByDescending(i => i.Time)
                    .FirstOrDefault();

                // If this results in a reminder that would be scheduled in the past, simply ignore/cancel it
                if (latestIncident != null && latestIncident.Time + _reminder.TimeBetween > DateTime.Now)
                {
                    ReminderHelper.ScheduleReminder(Context, _reminder, latestIncident.Time);
                }
                else
                {
                    ReminderHelper.CancelReminder(Context, _reminder);
                }

                Activity.RunOnUiThread(() =>
                {
                    Toast.MakeText(Context, "Successfully " + (isUpdate ? "updated existing" : "added new") + " reminder", ToastLength.Long).Show();
                    FragmentHelper.PopOne(Activity);
                });
            });
        }
    }
}
