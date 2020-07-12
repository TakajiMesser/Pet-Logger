using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using PottyLogger.Droid.Helpers;
using System;

namespace PottyLogger.Droid.Components
{
    [Register("com.pottylogger.android.components.TimeSpanView")]
    public class TimeSpanView : ViewGroup
    {
        private LinearLayout _times;
        private LinearLayout _labels;

        private TextView _daysText;
        private TextView _hoursText;
        private TextView _minutesText;
        private TextView _secondsText;

        private TextView _daysLabel;
        private TextView _hoursLabel;
        private TextView _minutesLabel;
        private TextView _secondsLabel;

        private TimeSpan _timeSpan;

        public TimeSpan TimeSpan
        {
            get => _timeSpan;
            set
            {
                _timeSpan = value;
                UpdateTimes();
            }
        }

        public bool IncludeDays { get; set; }
        public bool IncludeHours { get; set; }
        public bool IncludeMinutes { get; set; }
        public bool IncludeSeconds { get; set; }

        public float TimeTextSize { get; private set; }
        public Color TimeTextColor { get; private set; }
        public Typeface TimeTypeface { get; private set; }
        public float LabelTextSize { get; private set; }
        public Color LabelTextColor { get; private set; }
        public Typeface LabelTypeface { get; private set; }

        public TimeSpanView(Context context) : base(context) { }
        public TimeSpanView(Context context, IAttributeSet attrs) : base(context, attrs) { InitializeFromAttributes(context, attrs); }
        public TimeSpanView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitializeFromAttributes(context, attrs); }

        public TimeSpanView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var attr = context.ObtainStyledAttributes(attrs, Resource.Styleable.TimeSinceView, 0, 0);

            TimeTextSize = attr.GetFloat(Resource.Styleable.TimeSinceView_timeTextSize, 20.0f);
            TimeTextColor = attr.GetColor(Resource.Styleable.TimeSinceView_timeTextColor, unchecked((int)0xFF0000FF));

            var timeFontID = attr.GetResourceId(Resource.Styleable.TimeSinceView_timeFontFamily, -1);
            /*if (headerFontID != -1)
            {
                HeaderTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            LabelTextSize = attr.GetFloat(Resource.Styleable.TimeSinceView_labelTextSize, 20.0f);
            LabelTextColor = attr.GetColor(Resource.Styleable.TimeSinceView_labelTextColor, unchecked((int)0xFF0000FF));

            var labelFontID = attr.GetResourceId(Resource.Styleable.TimeSinceView_labelFontFamily, -1);
            /*if (headerFontID != -1)
            {
                HeaderTypeface = (true) ? ResourcesCompat.Get(Context, headerFontID) : Context.Resources.GetFont(headerFontID);
            }*/

            CreateTimeViews();
            CreateLabelViews();
        }

        private void CreateTimeViews()
        {
            _times = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal
            };

            _daysText = CreateTimeView();
            _hoursText = CreateTimeView();
            _minutesText = CreateTimeView();
            _secondsText = CreateTimeView();

            _times.AddView(_daysText);
            _times.AddView(_hoursText);
            _times.AddView(_minutesText);
            _times.AddView(_secondsText);
        }

        private void CreateLabelViews()
        {
            _labels = new LinearLayout(Context)
            {
                Orientation = Orientation.Horizontal
            };

            _daysLabel = CreateLabelView("Days");
            _hoursLabel = CreateLabelView("Hours");
            _minutesLabel = CreateLabelView("Minutes");
            _secondsLabel = CreateLabelView("Seconds");

            _labels.AddView(_daysLabel);
            _labels.AddView(_hoursLabel);
            _labels.AddView(_minutesLabel);
            _labels.AddView(_secondsLabel);
        }

        private TextView CreateLabelView(string text)
        {
            var labelView = new TextView(Context)
            {
                Text = text,
                Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
            };
            labelView.SetTextColor(LabelTextColor);
            labelView.SetPadding(20, 10, 10, 10);
            labelView.SetTextSize(ComplexUnitType.Dip, LabelTextSize);

            //var ems = _adapter.GetMaxCharacters(i);
            //labelView.SetMinEms(ems);

            return labelView;
        }

        private TextView CreateTimeView()
        {
            var timeView = new TextView(Context)
            {
                Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular)
            };
            timeView.SetTextColor(TimeTextColor);
            timeView.SetPadding(20, 10, 10, 10);
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

            if (!IncludeDays)
            {
                nHours += nDays * 24;
                _daysText.Visibility = ViewStates.Gone;
                _daysLabel.Visibility = ViewStates.Gone;
            }

            if (!IncludeHours)
            {
                nMinutes += nHours * 60;
                _hoursText.Visibility = ViewStates.Gone;
                _hoursLabel.Visibility = ViewStates.Gone;
            }

            if (!IncludeMinutes)
            {
                nSeconds += nMinutes * 60;
                _minutesText.Visibility = ViewStates.Gone;
                _minutesLabel.Visibility = ViewStates.Gone;
            }

            if (!IncludeSeconds)
            {
                _secondsText.Visibility = ViewStates.Gone;
                _secondsLabel.Visibility = ViewStates.Gone;
            }

            _daysText.Text = nDays.ToString();
            _hoursText.Text = nHours.ToString();
            _minutesText.Text = nMinutes.ToString();
            _secondsText.Text = nSeconds.ToString();
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            FocusableInTouchMode = true;

            AddView(_times, GenerateDefaultLayoutParams());
            BringChildToFront(_times);

            AddView(_labels, GenerateDefaultLayoutParams());
            BringChildToFront(_labels);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            /*for (var i = 0; i < ChildCount; i++)
            {
                var child = GetChildAt(i);
                if (child.Visibility != ViewStates.Gone)
                {
                    MeasureChildWithMargins(child, widthMeasureSpec, 0, heightMeasureSpec, 0);
                }
            }

            var marginLayoutParameters = (MarginLayoutParams)LayoutParameters;*/

            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            /*int width = (IsOpened || LayoutParameters.Width == ViewGroup.LayoutParams.MatchParent)
                ? GetDefaultSize(SuggestedMinimumWidth, widthMeasureSpec) + marginLayoutParameters.LeftMargin + marginLayoutParameters.RightMargin
                : _menuButton.MeasuredWidth + PaddingLeft + PaddingRight;

            int height = (IsOpened || LayoutParameters.Height == ViewGroup.LayoutParams.MatchParent)
                ? GetDefaultSize(SuggestedMinimumHeight, heightMeasureSpec) + marginLayoutParameters.TopMargin + marginLayoutParameters.BottomMargin
                : _menuButton.MeasuredHeight + PaddingTop + PaddingBottom;*/

            var width = _times.MeasuredWidth > _labels.MeasuredWidth
                ? _times.MeasuredWidth
                : _labels.MeasuredWidth;

            var height = _times.MeasuredHeight > _labels.MeasuredHeight
                ? _times.MeasuredHeight
                : _labels.MeasuredHeight;

            SetMeasuredDimension(width + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var timesLeft = r - l - _times.MeasuredWidth - PaddingRight;
            var timesTop = t;//b - t;// - _times.MeasuredHeight - PaddingBottom;
            var timesRight = timesLeft + _times.MeasuredWidth;
            var timesBottom = timesTop + _times.MeasuredHeight;

            _times.Layout(0, 0, timesRight, timesBottom);

            var labelsLeft = timesLeft;
            var labelsTop = timesBottom;
            var labelsRight = r;// rowLeft + _rows.MeasuredWidth;
            var labelsBottom = b;// rowTop + _rows.MeasuredHeight;

            _labels.Layout(0, labelsTop, labelsRight, labelsBottom);
        }
    }
}
