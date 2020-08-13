using Android.Content;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Shared.DataAccessLayer;
using System;
using System.Linq;
using System.Reflection;

namespace PetLogger.Droid.Helpers
{
    public static class EntityHelper
    {
        public static object GetPropertyValue(View view, Type propertyType)
        {
            if (view is EditText valueText)
            {
                if (propertyType == typeof(int))
                {
                    return int.Parse(valueText.Text);
                }
                else if (propertyType == typeof(double))
                {
                    return double.Parse(valueText.Text);
                }
                else if (propertyType == typeof(string))
                {
                    return valueText.Text;
                }
                else if (propertyType == typeof(DateTime))
                {
                    return DateTime.Parse(valueText.Text);
                }
            }
            else if (view is ToggleButton valueToggle)
            {
                return valueToggle.Checked;
            }
            else if (view is Spinner valueSpinner)
            {
                if (valueSpinner.Adapter is ForeignEntityAdapter entityAdapter)
                {
                    return entityAdapter.GetID(valueSpinner.SelectedItemPosition);
                }
                else if (valueSpinner.Adapter is EnumAdapter enumAdapter)
                {
                    return enumAdapter.GetValue(valueSpinner.SelectedItemPosition);
                }
            }
            else if (view is EditTimeView valueTime)
            {
                return valueTime.Time;
            }

            return null;
        }

        public static View CreateEntityPropertyView(Context context, PropertyInfo property, object currentValue = null)
        {
            if (Attribute.GetCustomAttribute(property, typeof(ForeignKeyAttribute)) is ForeignKeyAttribute foreignKeyAttribute)
            {
                var foreigners = DBTable.GetAll(foreignKeyAttribute.Type).OfType<IEntity>();
                var identifier = PetLogger.Shared.Helpers.EntityHelper.GetIdentifierProperty(foreignKeyAttribute.Type);

                if (identifier != null)
                {
                    var spinner = new Spinner(context)
                    {
                        Adapter = new ForeignEntityAdapter(context, identifier, foreigners)
                    };

                    return spinner;
                }
            }
            else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
            {
                var editText = new EditText(context)
                {
                    Text = currentValue != null ? currentValue.ToString() : "",
                    InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberVariationNormal,
                };
                editText.SetTextColor(ColorHelper.TextGray);
                return editText;
            }
            else if (property.PropertyType == typeof(string))
            {
                var editText = new EditText(context)
                {
                    Text = currentValue != null ? (string)currentValue : "",
                    InputType = Android.Text.InputTypes.TextVariationEmailAddress,
                };
                editText.SetTextColor(ColorHelper.TextGray);
                return editText;
            }
            else if (property.PropertyType == typeof(bool))
            {
                return new ToggleButton(context)
                {
                    Checked = currentValue != null && (bool)currentValue
                };
            }
            else if (property.PropertyType.IsEnum)
            {
                var spinner = new Spinner(context)
                {
                    Adapter = new EnumAdapter(context, property.PropertyType)
                };

                return spinner;
            }
            else if (property.PropertyType == typeof(TimeSpan))
            {
                var editTimeView = new EditTimeView(context);

                /*if (currentValue != null)
                {
                    editTimeView.Time = (TimeSpan)currentValue;
                }*/

                return editTimeView;
            }

            return null;
        }
    }
}