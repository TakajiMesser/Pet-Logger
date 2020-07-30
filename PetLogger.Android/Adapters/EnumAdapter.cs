using Android.Content;
using Android.Widget;
using System;

namespace PetLogger.Droid.Adapters
{
    public class EnumAdapter : ArrayAdapter<string>
    {
        private Type _enumType;

        public EnumAdapter(Context context, Type enumType) : base(context, Android.Resource.Layout.SimpleSpinnerDropDownItem, Enum.GetNames(enumType))
        {
            if (!enumType.IsEnum) throw new ArgumentException(nameof(enumType) + " must be Enum");

            _enumType = enumType;
        }

        public object GetValue(int position) => Enum.GetValues(_enumType).GetValue(position);

        public object GetValue(string name) => Enum.Parse(_enumType, name);
    }
}