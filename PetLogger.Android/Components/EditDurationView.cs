using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.EditDurationView")]
    public class EditDurationView : DurationView
    {
        // TODO - Change these to adjustable attributes
        public const int PICKER_SPEED = 50;
        public new const int HORIZONTAL_SPACING = 20;
        public new const int VERTICAL_SPACING = 10;

        public EditDurationView(Context context) : base(context) { }
        public EditDurationView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public EditDurationView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public EditDurationView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var attr = context.ObtainStyledAttributes(attrs, Resource.Styleable.EditDurationView, 0, 0);
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            FocusableInTouchMode = false;

            Click += (s, args) => ShowDialog();
        }

        private void ShowDialog()
        {
            var builder = new AlertDialog.Builder(Context, Resource.Style.AlertsDialogTheme);

            var titleView = new TextView(Context)
            {
                Text = "Edit Duration",
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 2);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_notifications, 0, 0, 0);

            var layout = new LinearLayout(Context)
            {
                Orientation = Android.Widget.Orientation.Horizontal
            };
            layout.SetPadding(100, 10, 100, 10);
            layout.SetClipToPadding(true);

            var unitPickers = CreateUnitPickers();
            _unitPickers = unitPickers.ToList();

            foreach (var unitPicker in _unitPickers)
            {
                layout.AddView(unitPicker);
            }

            var dialog = builder.SetCustomTitle(titleView)
                .SetView(layout)
                .SetCancelable(true)
                .SetNegativeButton("Cancel", (s, args) => { })
                .SetPositiveButton("OK", (s, args) => Duration = ParseDuration(_unitPickers))
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(Context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        private List<TimeUnitPicker> _unitPickers;

        private IEnumerable<TimeUnitPicker> CreateUnitPickers()
        {
            var nDays = (int)Duration.TotalDays;
            var nHours = Duration.Hours;
            var nMinutes = Duration.Minutes;
            var nSeconds = Duration.Seconds;

            if (DisplayDays)
            {
                yield return new TimeUnitPicker(Context, this, TimeUnits.Days, nDays);
            }
            else
            {
                nHours += nDays * 24;
            }

            if (DisplayHours)
            {
                yield return new TimeUnitPicker(Context, this, TimeUnits.Hours, nHours);
            }
            else
            {
                nMinutes += nHours * 60;
            }

            if (DisplayMinutes)
            {
                yield return new TimeUnitPicker(Context, this, TimeUnits.Minutes, nMinutes);
            }
            else
            {
                nSeconds += nMinutes * 60;
            }

            if (DisplaySeconds)
            {
                yield return new TimeUnitPicker(Context, this, TimeUnits.Seconds, nSeconds);
            }
        }

        private TimeSpan ParseDuration(IEnumerable<TimeUnitPicker> unitPickers)
        {
            var duration = TimeSpan.Zero;

            foreach (var unitPicker in unitPickers)
            {
                if (unitPicker.Unit == TimeUnits.Days)
                {
                    duration += TimeSpan.FromDays(unitPicker.Value);
                }
                else if (unitPicker.Unit == TimeUnits.Hours)
                {
                    duration += TimeSpan.FromHours(unitPicker.Value);
                }
                else if (unitPicker.Unit == TimeUnits.Minutes)
                {
                    duration += TimeSpan.FromMinutes(unitPicker.Value);
                }
                else if (unitPicker.Unit == TimeUnits.Seconds)
                {
                    duration += TimeSpan.FromSeconds(unitPicker.Value);
                }
            }

            return duration;
        }

        private class TimeUnitPicker : ViewGroup
        {
            private NumberPicker _valuePicker;
            private TextView _unitLabel;

            private int _combinedWidth;

            public TimeUnitPicker(Context context) : base(context) { }
            public TimeUnitPicker(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
            public TimeUnitPicker(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

            public TimeUnitPicker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

            public TimeUnitPicker(Context context, EditDurationView parentView, TimeUnits unit, int value) : base(context)
            {
                Unit = unit;
                Value = value;

                _valuePicker = new NumberPicker(Context)
                {
                    WrapSelectorWheel = true,
                    MinValue = 0,
                    MaxValue = GetMaxValue(unit),
                    Value = value
                };
                _valuePicker.SetBackgroundColor(parentView.ValueTextColor);
                _valuePicker.SetOnLongPressUpdateInterval(PICKER_SPEED);
                _valuePicker.ValueChanged += (s, args) => Value = args.NewVal;

                _unitLabel = new TextView(Context)
                {
                    Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular),
                    Text = GetUnitText(unit)
                };
                _unitLabel.SetTextColor(parentView.UnitTextColor);
                _unitLabel.SetTextSize(ComplexUnitType.Dip, parentView.UnitTextSize);

                AddView(_valuePicker, GenerateDefaultLayoutParams());
                AddView(_unitLabel, GenerateDefaultLayoutParams());
            }

            public TimeUnits Unit { get; }
            public int Value { get; private set; }

            private void InitializeFromAttributes(Context context, IAttributeSet attrs) { }

            private int GetMaxValue(TimeUnits unit)
            {
                switch (unit)
                {
                    case TimeUnits.Days:
                        return 100;
                    case TimeUnits.Hours:
                        return 23;
                    case TimeUnits.Minutes:
                        return 59;
                    case TimeUnits.Seconds:
                        return 59;
                }

                throw new ArgumentOutOfRangeException("Could not handle time unit type " + unit);
            }

            protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
            {
                _valuePicker.Measure(widthMeasureSpec, heightMeasureSpec);
                _unitLabel.Measure(widthMeasureSpec, heightMeasureSpec);

                _combinedWidth = _valuePicker.MeasuredWidth > _unitLabel.MeasuredWidth
                    ? _valuePicker.MeasuredWidth
                    : _unitLabel.MeasuredWidth;

                var height = _valuePicker.MeasuredHeight + _unitLabel.MeasuredHeight + VERTICAL_SPACING;

                SetMeasuredDimension(_combinedWidth + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
            }

            protected override void OnLayout(bool changed, int l, int t, int r, int b)
            {
                var valuePickerLeft = PaddingLeft + (_combinedWidth - _valuePicker.MeasuredWidth) / 2;
                var valuePickerTop = PaddingTop;
                var valuePickerRight = valuePickerLeft + _valuePicker.MeasuredWidth;
                var valuePickerBottom = valuePickerTop + _valuePicker.MeasuredHeight;

                _valuePicker.Layout(valuePickerLeft, valuePickerTop, valuePickerRight, valuePickerBottom);

                var unitLabelLeft = PaddingLeft + (_combinedWidth - _unitLabel.MeasuredWidth) / 2;
                var unitLabelBottom = MeasuredHeight - PaddingBottom;
                var unitLabelTop = unitLabelBottom - _unitLabel.MeasuredHeight;
                var unitLabelRight = unitLabelLeft + _unitLabel.MeasuredWidth;

                _unitLabel.Layout(unitLabelLeft, unitLabelTop, unitLabelRight, unitLabelBottom);
            }
        }
    }
}
