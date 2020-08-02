using Android.Content;
using Android.Runtime;
using Android.Util;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.CountDownView")]
    public class CountDownView : TimeSpanView
    {
        private enum UpdateFrequencyModes
        {
            Never,
            NextMinute,
            NextSecond
        }

        private DateTime _dateTime;
        private Timer _updateTimer = new Timer();

        private UpdateFrequencyModes _updateFrequencyMode;

        public CountDownView(Context context) : base(context) { }
        public CountDownView(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public CountDownView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        public CountDownView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public DateTime InitialTime
        {
            get => _dateTime;
            set
            {
                _updateTimer.Stop();
                _dateTime = value;
                _updateTimer.Interval = 1000;
            }
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            _updateTimer.Elapsed += UpdateTimer_Elapsed;
        }

        protected override void OnDetachedFromWindow()
        {
            _updateTimer.Stop();
            base.OnDetachedFromWindow();
        }

        public void Start()
        {
            _updateTimer.Start();
            Task.Run(() => UpdateTimer_Elapsed(this, null));
        }

        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var updatedTime = DateTime.Now - _dateTime;
            Post(() => TimeSpan = updatedTime);

            /*var nDays = (int)updatedTime.TotalDays;
            var nHours = updatedTime.Hours;
            var nMinutes = updatedTime.Minutes;
            var nSeconds = updatedTime.Seconds;

            if (nDays >= 1)
            {
                // [Day, Hour] - Don't bother with updates
                _updateFrequencyMode = UpdateFrequencyModes.Never;
                _updateTimer.Stop();
            }
            else if (nHours >= 1)
            {
                // [Hour, Minute] - Update in however many seconds until we hit the next minute mark
                var interval = 60 - nSeconds;

                // Let's say we're 43 seconds in -> update in approx. 17 seconds
                // 
                _updateTimer.

                if (_updateTimer.Interval != 1000)
                {
                    _updateTimer.Stop();
                    _updateTimer.Interval = 1000;
                    _updateTimer.Start();
                }
            }
            else
            {
                // [Minute, Second] - Update every second
                if (_updateFrequencyMode != UpdateFrequencyModes.NextSecond)
                {
                    _updateTimer.Interval = 1000;
                }
                if (_updateTimer.Interval != 1000)
                {
                    _updateTimer.Stop();
                    _updateTimer.Interval = 1000;
                    _updateTimer.Start();
                }
            }*/
        }
    }
}
