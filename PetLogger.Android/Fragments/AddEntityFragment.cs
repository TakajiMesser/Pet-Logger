using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using SQLite;
using System;
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

            var entity = Activator.CreateInstance<TEntity>();

            foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && !Attribute.IsDefined(property, typeof(AutoIncrementAttribute)))
                {
                    AddPropertyViews(property, inputsLayout);
                }
            }

            submitButton.Click += (s, e) => ProgressDialogHelper.RunTask(Activity, "Submitting...", () =>
            {
                Submit(view, entity);

                // TODO - Come up with a smarter way of doing this...
                if (entity is Reminder reminder)
                {
                    ReminderHelper.ScheduleReminder(Context, reminder);
                }

                Activity.RunOnUiThread(() =>
                {
                    Toast.MakeText(Context, "Successfully added new entity", ToastLength.Long).Show();
                    FragmentHelper.PopOne(Activity);
                });
            });
        }

        private void AddPropertyViews(PropertyInfo property, LinearLayout layout)
        {
            var inputView = EntityHelper.CreateEntityPropertyView(Context, property);

            if (inputView != null)
            {
                var textView = new TextView(Activity)
                {
                    Text = property.Name,
                    TextSize = 12.0f
                };
                textView.SetTextColor(ColorHelper.TextGray);

                layout.AddView(textView);
                layout.AddView(inputView);
            }
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

                    if (property != null)
                    {
                        var value = EntityHelper.GetPropertyValue(valueView, property.PropertyType);

                        if (value != null)
                        {
                            property.SetValue(entity, value);
                        }
                    }
                }
            }

            DBTable.Insert(entity);
        }
    }
}
