using Android.Content;
using Android.Content.Res;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PetLogger.Droid.Components.Preferences
{
    [Register("com.petlogger.android.components.preferences.EditNumberPreference")]
    public class EditNumberPreference : EditTextPreference
    {
        private int _initialValue;

        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public EditNumberPreference(Context context) : base(context) { }
        public EditNumberPreference(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public EditNumberPreference(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public EditNumberPreference(System.IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.NumberPickerPreference, 0, 0);

            MinValue = typedArray.GetInteger(Resource.Styleable.EditNumberPreference_minValue, 0);
            MaxValue = typedArray.GetInteger(Resource.Styleable.EditNumberPreference_maxValue, int.MaxValue);
        }

        protected override void OnAddEditTextToDialogView(View dialogView, EditText editText)
        {
            editText.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberVariationNormal;
            editText.Text = _initialValue.ToString();

            base.OnAddEditTextToDialogView(dialogView, editText);
        }

        public override void OnClick(IDialogInterface dialog, int which)
        {
            switch ((DialogButtonType)which)
            {
                case DialogButtonType.Positive:
                    // Validate data
                    if (!int.TryParse(EditText.Text, out int result))
                    {
                        DisplayError("Please enter a valid number");
                    }
                    else if (result < MinValue)
                    {
                        DisplayError("Value must be greater than " + MinValue);
                    }
                    else if (result > MaxValue)
                    {
                        DisplayError("Value must be less than " + MaxValue);
                    }
                    else
                    {
                        base.OnClick(dialog, which);
                    }
                    break;
            }
        }

        private void DisplayError(string message)
        {
            var alert = new AlertDialog.Builder(Context, Resource.Style.AlertsDialogTheme)
                .SetTitle("Invalid value")
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton("OK", (s, args) => { })
                .Create();

            alert.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(Context, Resource.Drawable.rounded_border_dark));
            alert.Show();

            EditText.ClearFocus();
        }

        protected override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                EditText.ClearFocus();
                int value = int.Parse(EditText.Text);
                SetValue(value);
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

        protected override Java.Lang.Object OnGetDefaultValue(TypedArray a, int index)
        {
            _initialValue = a.GetInt(index, 1);
            return _initialValue;
        }
    }
}
