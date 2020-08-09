using Android.Content;
using Android.Runtime;
using Android.Support.V7.Preferences;
using Android.Util;
using Android.Widget;
using PetLogger.Droid.Helpers;
using System;

namespace PetLogger.Droid.Components.Preferences
{
    [Register("com.petlogger.android.components.preferences.CustomPreference")]
    public class CustomPreference : Preference
    {
        private int _titleIconResourceID;

        public CustomPreference(Context context) : base(context) { }
        public CustomPreference(Context context, IAttributeSet attrs) : base(context, attrs) => InitializeFromAttributes(context, attrs);
        public CustomPreference(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) => InitializeFromAttributes(context, attrs);

        public CustomPreference(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        private void InitializeFromAttributes(Context context, IAttributeSet attrs)
        {
            var typedArray = context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomPreference, 0, 0);

            _titleIconResourceID = typedArray.GetResourceId(Resource.Styleable.CustomPreference_titleIcon, -1);
            IconSpaceReserved = false;
        }

        public override void OnBindViewHolder(PreferenceViewHolder holder)
        {
            base.OnBindViewHolder(holder);

            var titleText = holder.ItemView.FindViewById<TextView>(Android.Resource.Id.Title);
            //titleText.SetPaddingRelative(100, 0, 30, 0);
            titleText.SetPadding(50, 0, 50, 0);
            titleText.TextSize = 18;
            titleText.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoLight);

            if (_titleIconResourceID >= 0)
            {
                titleText.SetCompoundDrawablesRelativeWithIntrinsicBounds(_titleIconResourceID, 0, 0, 0);
                titleText.CompoundDrawablePadding = 30;
            }

            var summaryText = holder.ItemView.FindViewById<TextView>(Android.Resource.Id.Summary);
            summaryText.SetPaddingRelative(30, 0, 30, 0);
            summaryText.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoLight);
        }
    }
}
