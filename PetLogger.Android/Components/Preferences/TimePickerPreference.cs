using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Util;
using System;
using DialogPreference = Android.Support.V7.Preferences.DialogPreference;

namespace PetLogger.Droid.Components.Preferences
{
    [Register("com.petlogger.android.components.preferences.TimePickerPreference")]
    public class TimePickerPreference : DialogPreference
    {
        private int _nMinutes;

        public TimeSpan Time
        {
            get => TimeSpan.FromMinutes(_nMinutes);
            set => SetValue((int)value.TotalMinutes);
        }

        public TimePickerPreference(Context context) : base(context) { }
        public TimePickerPreference(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public TimePickerPreference(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public TimePickerPreference(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            //var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.NumberPickerPreference, 0, 0);

            //MinValue = typedArray.GetInteger(Resource.Styleable.EditNumberPreference_minValue, 0);
            //MaxValue = typedArray.GetInteger(Resource.Styleable.EditNumberPreference_maxValue, int.MaxValue);
        }

        protected override void OnSetInitialValue(Java.Lang.Object defaultValue)
        {
            int valueInt = (int)defaultValue;
            SetValue(GetPersistedInt(valueInt));
        }

        private void SetValue(int value)
        {
            if (ShouldPersist())
            {
                PersistInt(value);
            }

            if (_nMinutes != value)
            {
                _nMinutes = value;
                NotifyChanged();
            }
        }

        protected override Java.Lang.Object OnGetDefaultValue(TypedArray a, int index)
        {
            _nMinutes = a.GetInt(index, 1);
            return _nMinutes;
        }
    }
}
