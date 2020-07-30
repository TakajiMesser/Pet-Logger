using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Adapters;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.DataAccessLayer;
using PetLogger.Shared.Helpers;
using SQLite;
using System;
using System.Linq;
using System.Reflection;
using Fragment = Android.Support.V4.App.Fragment;
using View = Android.Views.View;

namespace PetLogger.Droid.Fragments
{
    public class AddEntityFragment<TEntity> : Fragment where TEntity : IEntity
    {
        public static AddEntityFragment<TEntity> Instantiate(string entityName)
        {
            var fragment = new AddEntityFragment<TEntity>
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutString("entityName", entityName);
            return fragment;
        }

        private string _entityName;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _entityName = Arguments.GetString("entityName");
            //HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_add_entity, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, _entityName);
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = "Add " + _entityName;

            var inputsLayout = view.FindViewById<LinearLayout>(Resource.Id.inputs);
            var submitButton = view.FindViewById<AppCompatButton>(Resource.Id.btn_submit);

            var entity = Activator.CreateInstance(typeof(TEntity));

            foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && !Attribute.IsDefined(property, typeof(AutoIncrementAttribute)))
                {
                    AddInput(property, inputsLayout);
                }
            }

            submitButton.Click += (s, e) =>
            {
                ProgressDialogHelper.RunTask(Activity, "Submitting...", () =>
                {
                    Submit(view, entity);

                    Activity.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Activity, "Successfully added new entity", ToastLength.Long).Show();

                        if (Activity is AppCompatActivity compatActivity)
                        {
                            compatActivity.SupportFragmentManager.PopBackStack();
                        }
                    });
                });
            };
        }

        private void Submit(View view, object entity)
        {
            var inputs = view.FindViewById<LinearLayout>(Resource.Id.inputs);
            var type = entity.GetType();

            for (var i = 0; i < inputs.ChildCount; i++)
            {
                if (inputs.GetChildAt(i) is TextView labelText)
                {
                    var property = type.GetProperty(labelText.Text);
                    var valueView = inputs.GetChildAt(i + 1);

                    if (valueView is EditText valueText)
                    {
                        SetPropertyValue(property, entity, valueText.Text);
                    }
                    else if (valueView is ToggleButton valueToggle)
                    {
                        property.SetValue(entity, valueToggle.Checked);
                    }
                    else if (valueView is Spinner valueSpinner)
                    {
                        if (valueSpinner.Adapter is ForeignEntityAdapter entityAdapter)
                        {
                            property.SetValue(entity, entityAdapter.GetID(valueSpinner.SelectedItemPosition));
                        }
                        else if (valueSpinner.Adapter is EnumAdapter enumAdapter)
                        {
                            property.SetValue(entity, enumAdapter.GetValue(valueSpinner.SelectedItemPosition));
                        }
                    }
                    else if (valueView is EditTimeView valueTime)
                    {
                        property.SetValue(entity, valueTime.Time);
                    }
                }
            }

            DBTable.Insert(entity);
        }

        private void SetPropertyValue(PropertyInfo property, object entity, string valueText)
        {
            if (property.PropertyType == typeof(int))
            {
                int value = int.Parse(valueText);
                property.SetValue(entity, value);
            }
            else if (property.PropertyType == typeof(double))
            {
                double value = double.Parse(valueText);
                property.SetValue(entity, value);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(entity, valueText);
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                DateTime value = DateTime.Parse(valueText);
                property.SetValue(entity, value);
            }
        }

        private void AddInput(PropertyInfo property, LinearLayout layout)
        {
            var textView = new TextView(Activity)
            {
                Text = property.Name,
                TextSize = 12.0f
            };
            textView.SetTextColor(ColorHelper.TextGray);
            layout.AddView(textView);

            if (Attribute.GetCustomAttribute(property, typeof(ForeignKeyAttribute)) is ForeignKeyAttribute foreignKeyAttribute)
            {
                var foreigners = DBTable.GetAll(foreignKeyAttribute.Type).OfType<IEntity>();
                var identifier = EntityHelper.GetIdentifierProperty(foreignKeyAttribute.Type);

                if (identifier != null)
                {
                    var spinner = new Spinner(Activity)
                    {
                        Adapter = new ForeignEntityAdapter(Activity, identifier, foreigners)
                    };

                    layout.AddView(spinner);
                }
            }
            else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
            {
                var editText = new EditText(Activity)
                {
                    InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberVariationNormal,
                };
                editText.SetTextColor(ColorHelper.TextGray);
                layout.AddView(editText);
            }
            else if (property.PropertyType == typeof(string))
            {
                var editText = new EditText(Activity)
                {
                    InputType = Android.Text.InputTypes.TextVariationEmailAddress,
                };
                editText.SetTextColor(ColorHelper.TextGray);
                layout.AddView(editText);
            }
            else if (property.PropertyType == typeof(bool))
            {
                layout.AddView(new ToggleButton(Activity));
            }
            else if (property.PropertyType.IsEnum)
            {
                var spinner = new Spinner(Activity)
                {
                    Adapter = new EnumAdapter(Activity, property.PropertyType)
                };

                layout.AddView(spinner);
            }
            else if (property.PropertyType == typeof(TimeSpan))
            {
                layout.AddView(new EditTimeView(Activity));
            }
        }
    }
}
