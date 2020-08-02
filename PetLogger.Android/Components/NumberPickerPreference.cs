using Android.Content;
using Android.Content.Res;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.NumberPickerPreference")]
    public class NumberPickerPreference : DialogPreference
    {
        private const long PICKER_SPEED = 50;

        private NumberPicker _picker;
        private int _initialValue;

        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public NumberPickerPreference(Context context) : base(context) { }
        public NumberPickerPreference(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public NumberPickerPreference(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public NumberPickerPreference(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.NumberPickerPreference, 0, 0);

            MinValue = typedArray.GetInteger(Resource.Styleable.NumberPickerPreference_minValue, 0);
            MaxValue = typedArray.GetInteger(Resource.Styleable.NumberPickerPreference_maxValue, int.MaxValue);
        }

        protected override View OnCreateDialogView()
        {
            _picker = new NumberPicker(Context)
            {
                WrapSelectorWheel = true,
                MinValue = MinValue,
                MaxValue = MaxValue
            };

            _picker.Value = _initialValue;
            _picker.SetOnLongPressUpdateInterval(PICKER_SPEED);

            return _picker;
        }

        protected override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                _picker.ClearFocus();
                SetValue(_picker.Value);
            }
        }

        protected override void OnSetInitialValue(bool restorePersistedValue, Java.Lang.Object defaultValue)
        {
            int valueInt = (int)defaultValue;
            SetValue(restorePersistedValue ? GetPersistedInt(valueInt) : valueInt);
        }

        private void SetValue(int value)
        {
            if (ShouldPersist())
            {
                PersistInt(value);
            }

            if (_initialValue != value)
            {
                _initialValue = value;
                NotifyChanged();
            }
        }

        protected override Java.Lang.Object OnGetDefaultValue(TypedArray a, int index) => a.GetInt(index, 1);
    }
}
