using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class EnumAdapter<T> : ArrayAdapter<string> where T : Enum
    {
        private List<T> _enumValues = new List<T>();

        public EnumAdapter(Context context) : base(context, Android.Resource.Layout.SimpleSpinnerDropDownItem)
        {
            var values = Enum.GetValues(typeof(T));

            for (var i = 0; i < values.Length; i++)
            {
                var value = (T)values.GetValue(i);

                _enumValues.Add(value);
                Add(Enum.GetName(typeof(T), value));
            }
        }

        public T GetValue(int position) => _enumValues[position];

        public int GetPosition(T value) => _enumValues.IndexOf(value);
    }

    public class EnumAdapter : ArrayAdapter<string>
    {
        private List<object> _enumValues = new List<object>();

        public EnumAdapter(Context context, Type enumType) : base(context, Android.Resource.Layout.SimpleSpinnerDropDownItem)
        {
            if (!enumType.IsEnum) throw new ArgumentException(nameof(enumType) + " must be Enum");

            var values = Enum.GetValues(enumType);

            for (var i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i);

                _enumValues.Add(value);
                Add(Enum.GetName(enumType, value));
            }
        }

        public object GetValue(int position) => _enumValues[position];

        public int GetPosition(object value) => _enumValues.IndexOf(value);
    }
}