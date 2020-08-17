using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.DurationView")]
    public class DurationView : ViewGroup
    {
        protected enum TimeUnits
        {
            Days,
            Hours,
            Minutes,
            Seconds
        }

        // TODO - Change these to adjustable attributes
        public const int HORIZONTAL_SPACING = 20;
        public const int VERTICAL_SPACING = 10;

        private const int DISPLAY_ALL = 0;
        private const int DISPLAY_INVISIBLE = 1;
        private const int DISPLAY_GONE = 2;
        private const int DISPLAY_DYNAMIC = 3;

        private TimeSpan _duration;
        private TextView _separator;

        public DurationView(Context context) : base(context) { }
        public DurationView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public DurationView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public DurationView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public float ValueTextSize { get; private set; }
        public Color ValueTextColor { get; private set; }
        public Typeface ValueTypeface { get; private set; }

        public float UnitTextSize { get; private set; }
        public Color UnitTextColor { get; private set; }
        public Typeface UnitTypeface { get; private set; }

        public bool DisplayDays { get; set; }
        public bool DisplayHours { get; set; }
        public bool DisplayMinutes { get; set; }
        public bool DisplaySeconds { get; set; }

        public int DisplayType { get; private set; }
        public int DynamicDisplayCount { get; private set; }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    UpdateTimes();
                    Invalidate();
                }
            }
        }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var attr = context.ObtainStyledAttributes(attrs, Resource.Styleable.DurationView, 0, 0);

            ValueTextSize = attr.GetFloat(Resource.Styleable.DurationView_valueTextSize, 28.0f);
            ValueTextColor = attr.GetColor(Resource.Styleable.DurationView_valueTextColor, unchecked((int)0xFFFFFFFF));

            var timeFontID = attr.GetResourceId(Resource.Styleable.DurationView_valueFontFamily, -1);
            /*if (headerFontID != -1)
            {
                ValueTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            UnitTextSize = attr.GetFloat(Resource.Styleable.DurationView_unitTextSize, 12.0f);
            UnitTextColor = attr.GetColor(Resource.Styleable.DurationView_unitTextColor, unchecked((int)0xFFFFFFFF));

            var labelFontID = attr.GetResourceId(Resource.Styleable.DurationView_unitFontFamily, -1);
            /*if (headerFontID != -1)
            {
                UnitTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            DisplayDays = attr.GetBoolean(Resource.Styleable.DurationView_displayDays, true);
            DisplayHours = attr.GetBoolean(Resource.Styleable.DurationView_displayHours, true);
            DisplayMinutes = attr.GetBoolean(Resource.Styleable.DurationView_displayMinutes, true);
            DisplaySeconds = attr.GetBoolean(Resource.Styleable.DurationView_displaySeconds, true);

            DisplayType = attr.GetInteger(Resource.Styleable.DurationView_displayType, 0);
            DynamicDisplayCount = attr.GetInteger(Resource.Styleable.DurationView_dynamicDisplayCount, 0);
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            FocusableInTouchMode = false;

            if (DisplayDays)
            {
                AddView(new TimeUnitView(Context, this, TimeUnits.Days), GenerateDefaultLayoutParams());
            }

            if (DisplayHours)
            {
                AddView(new TimeUnitView(Context, this, TimeUnits.Hours), GenerateDefaultLayoutParams());
            }

            if (DisplayMinutes)
            {
                AddView(new TimeUnitView(Context, this, TimeUnits.Minutes), GenerateDefaultLayoutParams());
            }

            if (DisplaySeconds)
            {
                AddView(new TimeUnitView(Context, this, TimeUnits.Seconds), GenerateDefaultLayoutParams());
            }

            _separator = new TextView(Context)
            {
                Text = ":",
                Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
            };
            _separator.SetTextColor(ValueTextColor);
            _separator.SetTextSize(ComplexUnitType.Dip, ValueTextSize);

            AddView(_separator, GenerateDefaultLayoutParams());

            UpdateTimes();
            Invalidate();
        }

        private IEnumerable<TimeUnitView> GetTimeUnitViews()
        {
            for (var i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i) is TimeUnitView timeUnitView)
                {
                    yield return timeUnitView;
                }
            }
        }

        private TimeUnitView GetTimeUnitView(TimeUnits unit)
        {
            for (var i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i) is TimeUnitView timeUnitView && timeUnitView.Unit == unit)
                {
                    return timeUnitView;
                }
            }

            return null;
        }

        private void UpdateTimes()
        {
            var nDays = (int)_duration.TotalDays;
            var nHours = _duration.Hours;
            var nMinutes = _duration.Minutes;
            var nSeconds = _duration.Seconds;

            var nUsedViews = 0;

            if (UpdateTimeUnit(TimeUnits.Days, nDays, nUsedViews))
            {
                nUsedViews++;
            }
            else
            {
                nHours += nDays * 24;
            }

            if (UpdateTimeUnit(TimeUnits.Hours, nHours, nUsedViews))
            {
                nUsedViews++;
            }
            else
            {
                nMinutes += nHours * 60;
            }

            if (UpdateTimeUnit(TimeUnits.Minutes, nMinutes, nUsedViews))
            {
                nUsedViews++;
            }
            else
            {
                nSeconds += nMinutes * 60;
            }

            UpdateTimeUnit(TimeUnits.Seconds, nSeconds, nUsedViews);
        }

        private bool UpdateTimeUnit(TimeUnits unit, int value, int nUsed)
        {
            var timeUnitView = GetTimeUnitView(unit);

            if (timeUnitView != null)
            {
                // TODO - Currently hardcoding dynamic display check
                if (DisplayType == DISPLAY_ALL
                    || DisplayType == DISPLAY_DYNAMIC && nUsed < DynamicDisplayCount && (nUsed > 0 || value > 0 || (DynamicDisplayCount == 2 && unit == TimeUnits.Minutes))
                    || (DisplayType == DISPLAY_INVISIBLE || DisplayType == DISPLAY_GONE) && value > 0)
                {
                    timeUnitView.Visibility = ViewStates.Visible;
                    timeUnitView.Value = value;
                    return true;
                }
                else
                {
                    timeUnitView.Visibility = DisplayType == DISPLAY_INVISIBLE
                        ? ViewStates.Invisible
                        : ViewStates.Gone;
                }
            }

            return false;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            var width = 0;
            var height = 0;

            var nUnitViews = 0;

            for (var i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i) is TimeUnitView timeUnitView && timeUnitView.Visibility != ViewStates.Gone)
                {
                    width += timeUnitView.MeasuredWidth;

                    if (nUnitViews > 0)
                    {
                        width += HORIZONTAL_SPACING;
                    }

                    if (timeUnitView.MeasuredHeight > height)
                    {
                        height = timeUnitView.MeasuredHeight;
                    }

                    nUnitViews++;
                }
            }

            SetMeasuredDimension(width + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var x = PaddingLeft;

            for (var i = 0; i < ChildCount; i++)
            {
                // TODO - Determine where to lay out separator
                if (GetChildAt(i) is TimeUnitView timeUnitView && timeUnitView.Visibility != ViewStates.Gone)
                {
                    var timeUnitLeft = x;
                    var timeUnitTop = PaddingTop;
                    var timeUnitRight = timeUnitLeft + timeUnitView.MeasuredWidth;
                    var timeUnitBottom = timeUnitTop + timeUnitView.MeasuredHeight;

                    timeUnitView.Layout(timeUnitLeft, timeUnitTop, timeUnitRight, timeUnitBottom);
                    x = timeUnitRight + HORIZONTAL_SPACING;
                }
            }
        }

        protected static string GetUnitText(TimeUnits unit)
        {
            switch (unit)
            {
                case TimeUnits.Days:
                    return "Day";
                case TimeUnits.Hours:
                    return "Hour";
                case TimeUnits.Minutes:
                    return "Minute";
                case TimeUnits.Seconds:
                    return "Second";
            }

            throw new ArgumentOutOfRangeException("Could not handle time unit type " + unit);
        }

        private class TimeUnitView : ViewGroup
        {
            private TextView _valueLabel;
            private TextView _unitLabel;

            private int _value;
            private int _labelWidth;

            public TimeUnitView(Context context) : base(context) { }
            public TimeUnitView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
            public TimeUnitView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

            public TimeUnitView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

            public TimeUnitView(Context context, DurationView parentView, TimeUnits unit) : base(context)
            {
                Unit = unit;

                _valueLabel = new TextView(Context)
                {
                    Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
                };
                _valueLabel.SetTextColor(parentView.ValueTextColor);
                _valueLabel.SetTextSize(ComplexUnitType.Dip, parentView.ValueTextSize);

                _unitLabel = new TextView(Context)
                {
                    Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular),
                    Text = GetUnitText(unit)
                };
                _unitLabel.SetTextColor(parentView.UnitTextColor);
                _unitLabel.SetTextSize(ComplexUnitType.Dip, parentView.UnitTextSize);

                AddView(_valueLabel, GenerateDefaultLayoutParams());
                AddView(_unitLabel, GenerateDefaultLayoutParams());
            }

            public int Value
            {
                get => _value;
                set
                {
                    _value = value;
                    _valueLabel.Text = _value.ToString();
                    _unitLabel.Text = GetUnitText(Unit) + (_value == 1 ? "" : "s");
                }
            }

            public TimeUnits Unit { get; }

            private void InitializeFromAttributes(Context context, IAttributeSet attrs) { }

            protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
            {
                _valueLabel.Measure(widthMeasureSpec, heightMeasureSpec);
                _unitLabel.Measure(widthMeasureSpec, heightMeasureSpec);

                _labelWidth = _valueLabel.MeasuredWidth > _unitLabel.MeasuredWidth
                    ? _valueLabel.MeasuredWidth
                    : _unitLabel.MeasuredWidth;

                var height = _valueLabel.MeasuredHeight + _unitLabel.MeasuredHeight + VERTICAL_SPACING;

                SetMeasuredDimension(_labelWidth + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
            }

            protected override void OnLayout(bool changed, int l, int t, int r, int b)
            {
                var valueLabelLeft = PaddingLeft + (_labelWidth - _valueLabel.MeasuredWidth) / 2;
                var valueLabelTop = PaddingTop;
                var valueLabelRight = valueLabelLeft + _valueLabel.MeasuredWidth;
                var valueLabelBottom = valueLabelTop + _valueLabel.MeasuredHeight;

                _valueLabel.Layout(valueLabelLeft, valueLabelTop, valueLabelRight, valueLabelBottom);

                var unitLabelLeft = PaddingLeft + (_labelWidth - _unitLabel.MeasuredWidth) / 2;
                var unitLabelBottom = MeasuredHeight - PaddingBottom;
                var unitLabelTop = unitLabelBottom - _unitLabel.MeasuredHeight;
                var unitLabelRight = unitLabelLeft + _unitLabel.MeasuredWidth;

                _unitLabel.Layout(unitLabelLeft, unitLabelTop, unitLabelRight, unitLabelBottom);
            }
        }
    }
}
