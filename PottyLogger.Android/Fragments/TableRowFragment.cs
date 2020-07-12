using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PottyLogger.Droid.Helpers;
using PottyLogger.Shared.DataAccessLayer;
using SQLite;
using System;
using System.Reflection;
using Fragment = Android.Support.V4.App.Fragment;
using View = Android.Views.View;

namespace PottyLogger.Droid.Fragments
{
    public class TableRowFragment : Fragment
    {
        private enum SubmitModes
        {
            Add,
            Edit
        }

        private SubmitModes _submitMode;
        private int _id;
        private string _tableName;

        public static TableRowFragment Instantiate(string tableName)
        {
            var fragment = new TableRowFragment()
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutString("tableName", tableName);

            return fragment;
        }

        public static TableRowFragment Instantiate(string tableName, int id)
        {
            var fragment = new TableRowFragment()
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutString("tableName", tableName);
            fragment.Arguments.PutInt("id", id);

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
            _tableName = Arguments.GetString("tableName");

            // ID will be returned as zero if no mapping was found (i.e. we are adding, rather than editing)
            _id = Arguments.GetInt("id");
            _submitMode = (_id == 0) ? SubmitModes.Add : SubmitModes.Edit;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_row_edit, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = _submitMode.ToString() + " " + _tableName;

            var readFields = view.FindViewById<LinearLayout>(Resource.Id.read_fields);
            var writeFields = view.FindViewById<LinearLayout>(Resource.Id.write_fields);
            var submitButton = view.FindViewById<AppCompatButton>(Resource.Id.btn_submit);

            Type type = DBAccess.ParseTableName(_tableName);
            var entity = _submitMode == SubmitModes.Add ? Activator.CreateInstance(type) : DBTable.Get(type, _id);

            foreach (var property in type.GetProperties())
            {
                if (Attribute.IsDefined(property, typeof(PrimaryKeyAttribute)))
                {
                    if (_submitMode == SubmitModes.Edit)
                    {
                        AddReadField(property.Name, property.GetValue(entity).ToString(), readFields);
                    }
                }
                /*else if (Attribute.IsDefined(property, typeof(ForeignKeyAttribute)) && _submitMode == SubmitModes.Edit)
                {
                    AddReadField(property.Name, property.GetValue(entity).ToString(), readFields);
                }*/
                else if (!Attribute.IsDefined(property, typeof(IgnoreAttribute)))
                {
                    var value = property.GetValue(entity);
                    string currentValue = (value != null) ? value.ToString() : "";

                    AddWriteField(property.Name, currentValue, writeFields);
                }
            }

            submitButton.Click += (s, e) =>
            {
                ProgressDialogHelper.RunTask(Activity, "Submitting...", () =>
                {
                    Submit(view, entity);
                    //Activity.OnBackPressed();
                });
            };
        }

        private void Submit(View view, object entity)
        {
            var readFields = view.FindViewById<LinearLayout>(Resource.Id.read_fields);
            var writeFields = view.FindViewById<LinearLayout>(Resource.Id.write_fields);

            var type = entity.GetType();

            for (var i = 0; i < readFields.ChildCount; i += 2)
            {
                var textField = readFields.GetChildAt(i) as TextView;
                var valueField = readFields.GetChildAt(i + 1) as EditText;

                var property = type.GetProperty(textField.Text);
                SetPropertyValue(property, entity, valueField.Text);
            }

            for (var i = 0; i < writeFields.ChildCount; i += 2)
            {
                var textField = writeFields.GetChildAt(i) as TextView;
                var valueField = writeFields.GetChildAt(i + 1) as EditText;

                var property = type.GetProperty(textField.Text);
                SetPropertyValue(property, entity, valueField.Text);
            }

            switch (_submitMode)
            {
                case SubmitModes.Add:
                    DBTable.Insert(entity);
                    break;
                case SubmitModes.Edit:
                    DBTable.Update(entity);
                    break;
            }
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

        private void AddReadField(string fieldName, string currentValue, LinearLayout layout)
        {
            var textView = new TextView(Activity)
            {
                Text = fieldName,
                TextSize = 12.0f
            };
            textView.SetTextColor(ColorHelper.Primary);

            var editText = new EditText(Activity)
            {
                Text = currentValue,
                InputType = Android.Text.InputTypes.Null,
                Enabled = false
            };
            editText.SetTextColor(ColorHelper.Primary);

            layout.AddView(textView);
            layout.AddView(editText);
        }

        private void AddWriteField(string fieldName, string currentValue, LinearLayout layout)
        {
            var textView = new TextView(Activity)
            {
                Text = fieldName,
                TextSize = 12.0f
            };
            textView.SetTextColor(ColorHelper.Primary);

            var editText = new EditText(Activity)
            {
                Text = (_submitMode == SubmitModes.Edit) ? currentValue : "",
                InputType = Android.Text.InputTypes.TextVariationEmailAddress,
            };
            editText.SetTextColor(ColorHelper.Primary);

            layout.AddView(textView);
            layout.AddView(editText);
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            /*var item = menu.FindItem(Resource.Id.action_search);
            item.SetEnabled(false);
            item.SetVisible(false);*/
        }
    }
}
