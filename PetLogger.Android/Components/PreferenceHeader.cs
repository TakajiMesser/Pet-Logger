using Android.Content;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Util;
using Android.Widget;
using PetLogger.Droid.Helpers;
using System;

namespace PetLogger.Droid.Components
{
    [Register("com.petlogger.android.components.PreferenceHeader")]
    public class PreferenceHeader : PreferenceCategory
    {
        public PreferenceHeader(Context context) : base(context) { }
        public PreferenceHeader(Context context, IAttributeSet attrs) : base(context, attrs) => InitializeFromAttributes(context, attrs);
        public PreferenceHeader(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) => InitializeFromAttributes(context, attrs);

        public PreferenceHeader(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.PreferenceHeader, 0, 0);

            //MinValue = typedArray.GetInteger(Resource.Styleable.NumberPickerPreference_minValue, 0);

            LayoutResource = Resource.Layout.preference_category;
            IconSpaceReserved = false;
        }

        public override void OnBindViewHolder(PreferenceViewHolder holder)
        {
            base.OnBindViewHolder(holder);

            var titleText = holder.ItemView.FindViewById<TextView>(Resource.Id.title);
            titleText.Text = Title;
            //titleText.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedBold);
            //titleText.SetTypeface();
        }
    }
}
