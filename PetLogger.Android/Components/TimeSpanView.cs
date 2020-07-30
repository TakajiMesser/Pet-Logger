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
    [Register("com.pottylogger.android.components.TimeSpanView")]
    public class TimeSpanView : ViewGroup
    {
        // TODO - Change these to adjustable attributes
        public const int HORIZONTAL_SPACING = 20;
        public const int VERTICAL_SPACING = 10;

        private TextView _daysText;
        private TextView _hoursText;
        private TextView _minutesText;
        private TextView _secondsText;

        private TextView _daysLabel;
        private TextView _hoursLabel;
        private TextView _minutesLabel;
        private TextView _secondsLabel;

        private TimeSpan _timeSpan;

        private bool _includeDays;
        private bool _includeHours;
        private bool _includeMinutes;
        private bool _includeSeconds;

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

        public bool IncludeDays
        {
            get => _includeDays;
            set
            {
                if (_includeDays != value)
                {
                    _includeDays = value;

                    if (_includeDays)
                    {
                        _daysText.Visibility = ViewStates.Visible;
                        _daysLabel.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        _daysText.Visibility = ViewStates.Gone;
                        _daysLabel.Visibility = ViewStates.Gone;
                    }

                    Invalidate();
                }
            }
        }

        public bool IncludeHours
        {
            get => _includeHours;
            set
            {
                if (_includeHours != value)
                {
                    _includeHours = value;

                    if (_includeHours)
                    {
                        _hoursText.Visibility = ViewStates.Visible;
                        _hoursLabel.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        _hoursText.Visibility = ViewStates.Gone;
                        _hoursLabel.Visibility = ViewStates.Gone;
                    }

                    Invalidate();
                }
            }
        }

        public bool IncludeMinutes
        {
            get => _includeMinutes;
            set
            {
                if (_includeMinutes != value)
                {
                    _includeMinutes = value;

                    if (_includeMinutes)
                    {
                        _minutesText.Visibility = ViewStates.Visible;
                        _minutesLabel.Visibility = ViewStates.Visible;
                        // Potentially need to change timer clock
                    }
                    else
                    {
                        _minutesText.Visibility = ViewStates.Gone;
                        _minutesLabel.Visibility = ViewStates.Gone;
                        // Potentially need to change timer clock
                    }

                    Invalidate();
                }
            }
        }

        public bool IncludeSeconds
        {
            get => _includeSeconds;
            set
            {
                if (_includeSeconds != value)
                {
                    _includeSeconds = value;

                    if (_includeSeconds)
                    {
                        _secondsText.Visibility = ViewStates.Visible;
                        _secondsLabel.Visibility = ViewStates.Visible;
                        // Potentially need to change timer clock
                    }
                    else
                    {
                        _secondsText.Visibility = ViewStates.Gone;
                        _secondsLabel.Visibility = ViewStates.Gone;
                        // Potentially need to change timer clock
                    }

                    Invalidate();
                }
            }
        }

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

            CreateTimeViews();
            CreateLabelViews();
        }

        private void CreateTimeViews()
        {
            _daysText = CreateTimeView();
            _hoursText = CreateTimeView();
            _minutesText = CreateTimeView();
            _secondsText = CreateTimeView();
        }

        private void CreateLabelViews()
        {
            _daysLabel = CreateLabelView("Days");
            _hoursLabel = CreateLabelView("Hours");
            _minutesLabel = CreateLabelView("Minutes");
            _secondsLabel = CreateLabelView("Seconds");
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

        private TextView CreateTimeView()
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

            if (!IncludeDays)
            {
                nHours += nDays * 24;
            }

            if (!IncludeHours)
            {
                nMinutes += nHours * 60;
            }

            if (!IncludeMinutes)
            {
                nSeconds += nMinutes * 60;
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

            AddView(_daysText, GenerateDefaultLayoutParams());
            AddView(_hoursText, GenerateDefaultLayoutParams());
            AddView(_minutesText, GenerateDefaultLayoutParams());
            AddView(_secondsText, GenerateDefaultLayoutParams());

            AddView(_daysLabel, GenerateDefaultLayoutParams());
            AddView(_hoursLabel, GenerateDefaultLayoutParams());
            AddView(_minutesLabel, GenerateDefaultLayoutParams());
            AddView(_secondsLabel, GenerateDefaultLayoutParams());
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureChildren(widthMeasureSpec, heightMeasureSpec);

            /*var marginLayoutParameters = (MarginLayoutParams)LayoutParameters;
             
            int width = (IsOpened || LayoutParameters.Width == ViewGroup.LayoutParams.MatchParent)
                ? GetDefaultSize(SuggestedMinimumWidth, widthMeasureSpec) + marginLayoutParameters.LeftMargin + marginLayoutParameters.RightMargin
                : _menuButton.MeasuredWidth + PaddingLeft + PaddingRight;

            int height = (IsOpened || LayoutParameters.Height == ViewGroup.LayoutParams.MatchParent)
                ? GetDefaultSize(SuggestedMinimumHeight, heightMeasureSpec) + marginLayoutParameters.TopMargin + marginLayoutParameters.BottomMargin
                : _menuButton.MeasuredHeight + PaddingTop + PaddingBottom;*/

            //var timesWidth = _daysText.Width + _hoursText.Width + _minutesText.Width + _secondsText.Text;

            var timesWidth = 0;
            var timesHeight = 0;
            var labelsWidth = 0;
            var labelsHeight = 0;

            var isFirst = true;

            if (IncludeDays)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    timesWidth += HORIZONTAL_SPACING;
                    labelsWidth += HORIZONTAL_SPACING;
                }

                timesWidth += _daysText.MeasuredWidth;
                labelsWidth += _daysLabel.MeasuredWidth;

                if (_daysText.MeasuredHeight > timesHeight)
                {
                    timesHeight = _daysText.MeasuredHeight;
                }

                if (_daysLabel.MeasuredHeight > labelsHeight)
                {
                    labelsHeight = _daysLabel.MeasuredHeight;
                }
            }

            if (IncludeHours)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    timesWidth += HORIZONTAL_SPACING;
                    labelsWidth += HORIZONTAL_SPACING;
                }

                timesWidth += _hoursText.MeasuredWidth;
                labelsWidth += _hoursLabel.MeasuredWidth;

                if (_hoursText.MeasuredHeight > timesHeight)
                {
                    timesHeight = _hoursText.MeasuredHeight;
                }

                if (_hoursLabel.MeasuredHeight > labelsHeight)
                {
                    labelsHeight = _hoursLabel.MeasuredHeight;
                }
            }

            if (IncludeMinutes)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    timesWidth += HORIZONTAL_SPACING;
                    labelsWidth += HORIZONTAL_SPACING;
                }

                timesWidth += _minutesText.MeasuredWidth;
                labelsWidth += _minutesLabel.MeasuredWidth;

                if (_minutesText.MeasuredHeight > timesHeight)
                {
                    timesHeight = _minutesText.MeasuredHeight;
                }

                if (_minutesLabel.MeasuredHeight > labelsHeight)
                {
                    labelsHeight = _minutesLabel.MeasuredHeight;
                }
            }

            if (IncludeSeconds)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    timesWidth += HORIZONTAL_SPACING;
                    labelsWidth += HORIZONTAL_SPACING;
                }

                timesWidth += _secondsText.MeasuredWidth;
                labelsWidth += _secondsLabel.MeasuredWidth;

                if (_secondsText.MeasuredHeight > timesHeight)
                {
                    timesHeight = _secondsText.MeasuredHeight;
                }

                if (_secondsLabel.MeasuredHeight > labelsHeight)
                {
                    labelsHeight = _secondsLabel.MeasuredHeight;
                }
            }

            var width = timesWidth > labelsWidth
                ? timesWidth
                : labelsWidth;

            var height = timesHeight + labelsHeight + VERTICAL_SPACING;

            SetMeasuredDimension(width + PaddingStart + PaddingEnd, height + PaddingTop + PaddingBottom);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var x = PaddingLeft;
            var y = PaddingTop;

            var isFirst = true;

            if (IncludeDays)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    x += HORIZONTAL_SPACING;
                }

                var left = x;
                var textRight = left + _daysText.MeasuredWidth;

                _daysText.Layout(left, y, textRight, y + _daysText.MeasuredHeight);

                var labelRight = left + _daysLabel.MeasuredWidth;
                var labelBottom = MeasuredHeight - PaddingBottom;
                _daysLabel.Layout(left, labelBottom - _daysLabel.MeasuredHeight, labelRight, labelBottom);

                x = textRight > labelRight
                    ? textRight
                    : labelRight;
            }

            if (IncludeHours)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    x += HORIZONTAL_SPACING;
                }

                var left = x;
                var textRight = left + _hoursText.MeasuredWidth;

                _hoursText.Layout(left, y, textRight, y + _hoursText.MeasuredHeight);

                var labelRight = left + _hoursLabel.MeasuredWidth;
                var labelBottom = MeasuredHeight - PaddingBottom;
                _hoursLabel.Layout(left, labelBottom - _hoursLabel.MeasuredHeight, labelRight, labelBottom);

                x = textRight > labelRight
                    ? textRight
                    : labelRight;
            }

            if (IncludeMinutes)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    x += HORIZONTAL_SPACING;
                }

                var left = x;
                var textRight = left + _minutesText.MeasuredWidth;

                _minutesText.Layout(left, y, textRight, y + _minutesText.MeasuredHeight);

                var labelRight = left + _minutesLabel.MeasuredWidth;
                var labelBottom = MeasuredHeight - PaddingBottom;
                _minutesLabel.Layout(left, labelBottom - _minutesLabel.MeasuredHeight, labelRight, labelBottom);

                x = textRight > labelRight
                    ? textRight
                    : labelRight;
            }

            if (IncludeSeconds)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    x += HORIZONTAL_SPACING;
                }

                var left = x;
                var textRight = left + _secondsText.MeasuredWidth;

                _secondsText.Layout(left, y, textRight, y + _secondsText.MeasuredHeight);

                var labelRight = left + _secondsLabel.MeasuredWidth;
                var labelBottom = MeasuredHeight - PaddingBottom;
                _secondsLabel.Layout(left, labelBottom - _secondsLabel.MeasuredHeight, labelRight, labelBottom);

                x = textRight > labelRight
                    ? textRight
                    : labelRight;
            }
        }
    }
}
