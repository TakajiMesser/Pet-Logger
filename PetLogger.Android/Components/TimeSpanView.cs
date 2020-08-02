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
    [Register("com.petlogger.android.components.TimeSpanView")]
    public class TimeSpanView : ViewGroup
    {
        // TODO - Change these to adjustable attributes
        public const int HORIZONTAL_SPACING = 20;
        public const int VERTICAL_SPACING = 10;

        private TextView _largeUnitLabel;
        private TextView _largeUnitValue;

        private TextView _smallUnitLabel;
        private TextView _smallUnitValue;

        private TextView _separator;

        private TimeSpan _timeSpan;

        private int _largeUnitWidth;
        private int _smallUnitWidth;

        public TimeSpanView(Context context) : base(context) { }
        public TimeSpanView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public TimeSpanView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public TimeSpanView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public float TimeTextSize { get; private set; }
        public Color TimeTextColor { get; private set; }
        public Typeface TimeTypeface { get; private set; }
        public float LabelTextSize { get; private set; }
        public Color LabelTextColor { get; private set; }
        public Typeface LabelTypeface { get; private set; }

        public TimeSpan TimeSpan
        {
            get => _timeSpan;
            set
            {
                if (_timeSpan != value)
                {
                    _timeSpan = value;
                    UpdateTimes();
                    Invalidate();
                }
            }
        }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var attr = context.ObtainStyledAttributes(attrs, Resource.Styleable.TimeSpanView, 0, 0);

            TimeTextSize = attr.GetFloat(Resource.Styleable.TimeSpanView_timeTextSize, 20.0f);
            TimeTextColor = attr.GetColor(Resource.Styleable.TimeSpanView_timeTextColor, unchecked((int)0xFF0000FF));

            var timeFontID = attr.GetResourceId(Resource.Styleable.TimeSpanView_timeFontFamily, -1);
            /*if (headerFontID != -1)
            {
                HeaderTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            LabelTextSize = attr.GetFloat(Resource.Styleable.TimeSpanView_labelTextSize, 20.0f);
            LabelTextColor = attr.GetColor(Resource.Styleable.TimeSpanView_labelTextColor, unchecked((int)0xFF0000FF));

            var labelFontID = attr.GetResourceId(Resource.Styleable.TimeSpanView_labelFontFamily, -1);
            /*if (headerFontID != -1)
            {
                HeaderTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            BuildViews();
        }

        private void BuildViews()
        {
            _largeUnitLabel = CreateLabelView();
            _largeUnitValue = CreateValueView();

            _smallUnitLabel = CreateLabelView();
            _smallUnitValue = CreateValueView();

            _separator = CreateValueView();
            _separator.Text = ":";
        }

        private TextView CreateLabelView()
        {
            var labelView = new TextView(Context)
            {
                Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
            };
            labelView.SetTextColor(LabelTextColor);
            //labelView.SetPadding(20, 10, 10, 10);
            labelView.SetTextSize(ComplexUnitType.Dip, LabelTextSize);

            //var ems = _adapter.GetMaxCharacters(i);
            //labelView.SetMinEms(ems);

            return labelView;
        }

        private TextView CreateValueView()
        {
            var timeView = new TextView(Context)
            {
                Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
            };
            timeView.SetTextColor(TimeTextColor);
            //timeView.SetPadding(20, 10, 10, 10);
            timeView.SetTextSize(ComplexUnitType.Dip, TimeTextSize);

            //var ems = _adapter.GetMaxCharacters(i);
            //timeView.SetMinEms(ems);

            return timeView;
        }

        private void UpdateTimes()
        {
            var nDays = (int)_timeSpan.TotalDays;
            var nHours = _timeSpan.Hours;
            var nMinutes = _timeSpan.Minutes;
            var nSeconds = _timeSpan.Seconds;

            if (nDays >= 1)
            {
                _largeUnitLabel.Text = "Day" + (nDays == 1 ? "" : "s");
                _largeUnitValue.Text = nDays.ToString();

                _smallUnitLabel.Text = "Hour" + (nHours == 1 ? "" : "s");
                _smallUnitValue.Text = nHours.ToString();
            }
            else if (nHours >= 1)
            {
                _largeUnitLabel.Text = "Hour" + (nHours == 1 ? "" : "s");
                _largeUnitValue.Text = nHours.ToString();

                _smallUnitLabel.Text = "Minute" + (nMinutes == 1 ? "" : "s");
                _smallUnitValue.Text = nMinutes.ToString();
            }
            else
            {
                _largeUnitLabel.Text = "Minute" + (nMinutes == 1 ? "" : "s");
                _largeUnitValue.Text = nMinutes.ToString();

                _smallUnitLabel.Text = "Second" + (nSeconds == 1 ? "" : "s");
                _smallUnitValue.Text = nSeconds.ToString();
            }
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            FocusableInTouchMode = true;

            AddView(_largeUnitLabel, GenerateDefaultLayoutParams());
            AddView(_largeUnitValue, GenerateDefaultLayoutParams());
            AddView(_smallUnitLabel, GenerateDefaultLayoutParams());
            AddView(_smallUnitValue, GenerateDefaultLayoutParams());
            AddView(_separator, GenerateDefaultLayoutParams());
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            var labelsWidth = _largeUnitLabel.MeasuredWidth + HORIZONTAL_SPACING + _smallUnitLabel.MeasuredWidth;
            var labelsHeight = _largeUnitLabel.MeasuredHeight > _smallUnitLabel.MeasuredHeight
                ? _largeUnitLabel.MeasuredHeight
                : _smallUnitLabel.MeasuredHeight;

            var valuesWidth = _largeUnitValue.MeasuredWidth + HORIZONTAL_SPACING + _separator.MeasuredWidth + HORIZONTAL_SPACING + _smallUnitValue.MeasuredWidth;
            var valuesHeight = _separator.MeasuredHeight;

            if (_largeUnitValue.MeasuredHeight > valuesHeight)
            {
                valuesHeight = _largeUnitValue.MeasuredHeight;
            }

            if (_smallUnitValue.MeasuredHeight > valuesHeight)
            {
                valuesHeight = _smallUnitValue.MeasuredHeight;
            }

            _largeUnitWidth = _largeUnitValue.MeasuredWidth > _largeUnitLabel.MeasuredWidth
                ? _largeUnitValue.MeasuredWidth
                : _largeUnitLabel.MeasuredWidth;

            _smallUnitWidth = _smallUnitValue.MeasuredWidth > _smallUnitLabel.MeasuredWidth
                ? _smallUnitValue.MeasuredWidth
                : _smallUnitLabel.MeasuredWidth;

            var width = valuesWidth > labelsWidth
                ? valuesWidth
                : labelsWidth;

            var height = valuesHeight + labelsHeight + VERTICAL_SPACING;

            SetMeasuredDimension(width + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var largeUnitValueLeft = PaddingLeft + (_largeUnitWidth - _largeUnitValue.MeasuredWidth) / 2;
            var largeUnitValueTop = PaddingTop;
            var largeUnitValueRight = largeUnitValueLeft + _largeUnitValue.MeasuredWidth;
            var largeUnitValueBottom = largeUnitValueTop + _largeUnitValue.MeasuredHeight;

            _largeUnitValue.Layout(largeUnitValueLeft, largeUnitValueTop, largeUnitValueRight, largeUnitValueBottom);

            var largeUnitLabelLeft = PaddingLeft + (_largeUnitWidth - _largeUnitLabel.MeasuredWidth) / 2;
            var largeUnitLabelBottom = MeasuredHeight - PaddingBottom;
            var largeUnitLabelTop = largeUnitLabelBottom - _largeUnitLabel.MeasuredHeight;
            var largeUnitLabelRight = largeUnitLabelLeft + _largeUnitLabel.MeasuredWidth;

            _largeUnitLabel.Layout(largeUnitLabelLeft, largeUnitLabelTop, largeUnitLabelRight, largeUnitLabelBottom);

            var smallUnitValueRight = MeasuredWidth - PaddingRight - (_smallUnitWidth - _smallUnitValue.MeasuredWidth) / 2;
            var smallUnitValueLeft = smallUnitValueRight - _smallUnitValue.MeasuredWidth;
            var smallUnitValueTop = PaddingTop;
            var smallUnitValueBottom = smallUnitValueTop + _smallUnitValue.MeasuredHeight;

            _smallUnitValue.Layout(smallUnitValueLeft, smallUnitValueTop, smallUnitValueRight, smallUnitValueBottom);

            var smallUnitLabelRight = MeasuredWidth - PaddingRight - (_smallUnitWidth - _smallUnitLabel.MeasuredWidth) / 2;
            var smallUnitLabelLeft = smallUnitLabelRight - _smallUnitLabel.MeasuredWidth;
            var smallUnitLabelBottom = MeasuredHeight - PaddingBottom;
            var smallUnitLabelTop = smallUnitLabelBottom - _smallUnitLabel.MeasuredHeight;

            _smallUnitLabel.Layout(smallUnitLabelLeft, smallUnitLabelTop, smallUnitLabelRight, smallUnitLabelBottom);

            // TODO - Determine where to lay out separator
        }
    }
}
