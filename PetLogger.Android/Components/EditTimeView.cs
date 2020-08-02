using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using System;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.EditTimeView")]
    public class EditTimeView : ViewGroup
    {
        // TODO - Change these to adjustable attributes
        public const int PICKER_SPEED = 50;
        public const int HORIZONTAL_SPACING = 20;
        public const int VERTICAL_SPACING = 10;

        private NumberPicker _hourPicker;
        private NumberPicker _minutePicker;

        private TextView _hourLabel;
        private TextView _minuteLabel;

        public float TimeTextSize { get; private set; }
        public Color TimeTextColor { get; private set; }
        public Typeface TimeTypeface { get; private set; }
        public float LabelTextSize { get; private set; }
        public Color LabelTextColor { get; private set; }
        public Typeface LabelTypeface { get; private set; }

        public TimeSpan Time
        {
            get => TimeSpan.FromHours(_hourPicker.Value) + TimeSpan.FromMinutes(_minutePicker.Value);
            set
            {
                _hourPicker.Value = (int)value.TotalHours;
                _minutePicker.Value = value.Minutes;
                Invalidate();
            }
        }

        public EditTimeView(Context context) : base(context) { }
        public EditTimeView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public EditTimeView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public EditTimeView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var attr = context.ObtainStyledAttributes(attrs, Resource.Styleable.EditTimeView, 0, 0);

            TimeTextSize = attr.GetFloat(Resource.Styleable.EditTimeView_timeTextSize, 20.0f);
            TimeTextColor = attr.GetColor(Resource.Styleable.EditTimeView_timeTextColor, unchecked((int)0xFF0000FF));

            var timeFontID = attr.GetResourceId(Resource.Styleable.EditTimeView_timeFontFamily, -1);
            /*if (headerFontID != -1)
            {
                HeaderTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            LabelTextSize = attr.GetFloat(Resource.Styleable.EditTimeView_labelTextSize, 20.0f);
            LabelTextColor = attr.GetColor(Resource.Styleable.EditTimeView_labelTextColor, unchecked((int)0xFF0000FF));

            var labelFontID = attr.GetResourceId(Resource.Styleable.EditTimeView_labelFontFamily, -1);
            /*if (headerFontID != -1)
            {
                HeaderTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/
        }

        private TextView CreateLabelView(string text)
        {
            var labelView = new TextView(Context)
            {
                Text = text,
                Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
            };
            labelView.SetTextColor(LabelTextColor);
            //labelView.SetPadding(20, 10, 10, 10);
            labelView.SetTextSize(ComplexUnitType.Dip, LabelTextSize);

            //var ems = _adapter.GetMaxCharacters(i);
            //labelView.SetMinEms(ems);

            return labelView;
        }

        private NumberPicker CreatePickerView(int minValue, int maxValue)
        {
            var picker = new NumberPicker(Context)
            {
                WrapSelectorWheel = true,
                MinValue = minValue,
                MaxValue = maxValue
            };

            picker.SetOnLongPressUpdateInterval(PICKER_SPEED);

            return picker;
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            FocusableInTouchMode = true;

            /*_hourPicker = CreatePickerView(1, 24);
            _minutePicker = CreatePickerView(1, 59);
            _hourLabel = CreateLabelView("Hours");
            _minuteLabel = CreateLabelView("Minutes");

            AddView(_hourPicker, GenerateDefaultLayoutParams());
            AddView(_minutePicker, GenerateDefaultLayoutParams());
            AddView(_hourLabel, GenerateDefaultLayoutParams());
            AddView(_minuteLabel, GenerateDefaultLayoutParams());*/
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            _hourPicker = CreatePickerView(1, 24);
            _minutePicker = CreatePickerView(1, 59);
            _hourLabel = CreateLabelView("Hours");
            _minuteLabel = CreateLabelView("Minutes");

            AddView(_hourPicker, GenerateDefaultLayoutParams());
            AddView(_minutePicker, GenerateDefaultLayoutParams());
            AddView(_hourLabel, GenerateDefaultLayoutParams());
            AddView(_minuteLabel, GenerateDefaultLayoutParams());
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            var pickersWidth = _hourPicker.MeasuredWidth + HORIZONTAL_SPACING + _minutePicker.MeasuredWidth;
            var pickersHeight = _hourPicker.MeasuredHeight > _minutePicker.MeasuredHeight
                ? _hourPicker.MeasuredHeight
                : _minutePicker.MeasuredHeight;

            var labelsWidth = _hourLabel.MeasuredWidth + HORIZONTAL_SPACING + _minuteLabel.MeasuredWidth;
            var labelsHeight = _hourLabel.MeasuredHeight > _minuteLabel.MeasuredHeight
                ? _hourLabel.MeasuredHeight
                : _minuteLabel.MeasuredHeight;

            var width = pickersWidth > labelsWidth
                ? pickersWidth
                : labelsWidth;

            var height = pickersHeight + VERTICAL_SPACING + labelsHeight;

            SetMeasuredDimension(width + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var x = PaddingLeft;
            var y = PaddingTop;

            var hourPickerLeft = x;
            var hourPickerRight = hourPickerLeft + _hourPicker.MeasuredWidth;

            _hourPicker.Layout(hourPickerLeft, y, hourPickerRight, y + _hourPicker.MeasuredHeight);

            var hourLabelRight = hourPickerLeft + _hourLabel.MeasuredWidth;
            var hourLabelBottom = MeasuredHeight - PaddingBottom;
            _hourLabel.Layout(hourPickerLeft, hourLabelBottom - _hourLabel.MeasuredHeight, hourLabelRight, hourLabelBottom);

            x = hourPickerRight > hourLabelRight
                ? hourPickerRight
                : hourLabelRight;

            x += HORIZONTAL_SPACING;

            var minutePickerLeft = x;
            var minutePickerRight = minutePickerLeft + _minutePicker.MeasuredWidth;

            _minutePicker.Layout(minutePickerLeft, y, minutePickerRight, y + _minutePicker.MeasuredHeight);

            var minuteLabelRight = minutePickerLeft + _minuteLabel.MeasuredWidth;
            var minuteLabelBottom = MeasuredHeight - PaddingBottom;
            _minuteLabel.Layout(minutePickerLeft, minuteLabelBottom - _minuteLabel.MeasuredHeight, minuteLabelRight, minuteLabelBottom);

            x = minutePickerRight > minuteLabelRight
                ? minutePickerRight
                : minuteLabelRight;
        }
    }
}
