using Android.Content;
using Android.Runtime;
using Android.Util;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace PetLogger.Droid.Components
{
    [Register("com.pottylogger.android.components.CountDownView")]
    public class CountDownView : TimeSpanView
    {
        private DateTime _dateTime;
        private Timer _updateTimer = new Timer();

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
        }
    }
}
