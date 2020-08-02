using Android.Animation;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.ProgressSpinner")]
    public class ProgressSpinner : ProgressBar
    {
        public const int ANIMATION_DURATION = 250;

        public ProgressSpinner(Context context) : base(context) { }
        public ProgressSpinner(Context context, IAttributeSet attrs) : base(context, attrs) => InitializeFromAttributes(context, attrs);
        public ProgressSpinner(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) => InitializeFromAttributes(context, attrs);

        public ProgressSpinner(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public new bool IsShown { get; private set; }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            //var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomPreference, 0, 0);
            Visibility = Android.Views.ViewStates.Invisible;
        }

        public void Show()
        {
            if (!IsShown)
            {
                IsShown = true;
                Visibility = ViewStates.Visible;

                var backgroundAnimation = ObjectAnimator.OfInt(this, "backgroundColor", unchecked((int)0x00000000), unchecked((int)0xA0000000));
                backgroundAnimation.SetEvaluator(new ArgbEvaluator());
                backgroundAnimation.SetDuration(ANIMATION_DURATION)
                    .Start();
            }
        }

        public void Dismiss()
        {
            if (IsShown)
            {
                IsShown = false;
                Visibility = ViewStates.Invisible;

                var backgroundAnimation = ObjectAnimator.OfInt(this, "backgroundColor", unchecked((int)0xA0000000), unchecked((int)0x00000000));
                backgroundAnimation.SetEvaluator(new ArgbEvaluator());
                backgroundAnimation.SetDuration(ANIMATION_DURATION)
                    .Start();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var minWidth = SuggestedMinimumWidth;
            var minHeight = SuggestedMinimumHeight;

            var width = GetDefaultSize(minWidth, widthMeasureSpec);
            var height = GetDefaultSize(minHeight, heightMeasureSpec);

            var paddingLeft = (width - minWidth) / 2;
            var paddingTop = (height - minHeight) / 2;

            SetPadding(paddingLeft, paddingTop, paddingLeft, paddingTop);
            SetMeasuredDimension(width, height);
        }

        public override bool OnTouchEvent(MotionEvent e) => IsShown;
    }
}
