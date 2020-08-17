using Android.Content;
using Android.Runtime;
using Android.Util;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.LiveDurationView")]
    public class LiveDurationView : DurationView
    {
        public enum CountDirections
        {
            Up,
            Down
        }

        private enum UpdateFrequencyModes
        {
            Never,
            NextMinute,
            NextSecond
        }

        private DateTime _dateTime;
        private Timer _updateTimer = new Timer();

        private UpdateFrequencyModes _updateFrequencyMode;

        public LiveDurationView(Context context) : base(context) { }
        public LiveDurationView(Context context, IAttributeSet attrs) : base(context, attrs) { }
        public LiveDurationView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        public LiveDurationView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public CountDirections CountDirection { get; set; }

        public DateTime Time
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
            if (CountDirection == CountDirections.Up)
            {
                var updatedTime = DateTime.Now - _dateTime;
                if (updatedTime < TimeSpan.Zero)
                {
                    updatedTime = TimeSpan.Zero;
                }

                Post(() => Duration = updatedTime);
            }
            else if (CountDirection == CountDirections.Down)
            {
                var updatedTime = _dateTime - DateTime.Now;
                if (updatedTime < TimeSpan.Zero)
                {
                    updatedTime = TimeSpan.Zero;
                }

                Post(() => Duration = updatedTime);
            }

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
