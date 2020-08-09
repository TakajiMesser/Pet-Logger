using Android.Content;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using System;

namespace PetLogger.Droid.Components.Preferences
{
    public class TimePickerPreferenceDialogFragment : PreferenceDialogFragmentCompat
    {
        private TimePicker _picker;

        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public static TimePickerPreferenceDialogFragment Instance(Preference preference)
        {
            var fragment = new TimePickerPreferenceDialogFragment()
            {
                Arguments = new Android.OS.Bundle()
            };
            fragment.Arguments.PutString("key", preference.Key);

            return fragment;
        }

        public TimePickerPreferenceDialogFragment() { }
        public TimePickerPreferenceDialogFragment(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        protected override View OnCreateDialogView(Context context)
        {
            _picker = new TimePicker(context);
            _picker.SetIs24HourView(new Java.Lang.Boolean(false));

            return _picker;
        }

        protected override void OnBindDialogView(View view)
        {
            base.OnBindDialogView(view);

            if (Preference is TimePickerPreference timePreference)
            {
                var time = timePreference.Time;

                _picker.Hour = time.Hours;
                _picker.Minute = time.Minutes;
            }
        }

        public override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                _picker.ClearFocus();

                if (Preference is TimePickerPreference timePreference)
                {
                    timePreference.Time = TimeSpan.FromHours(_picker.Hour) + TimeSpan.FromMinutes(_picker.Minute);
                }
            }
        }
    }
}
