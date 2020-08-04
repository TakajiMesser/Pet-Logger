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
    public class UpdateEntityFragment<TEntity> : Fragment where TEntity : class, IEntity, new()
    {
        public static UpdateEntityFragment<TEntity> Instantiate(string entityName, int entityID)
        {
            var fragment = new UpdateEntityFragment<TEntity>
            {
                Arguments = new Bundle()
            };
            fragment.Arguments.PutString("entityName", entityName);
            fragment.Arguments.PutInt("entityID", entityID);
            return fragment;
        }

        private string _entityName;
        private int _entityID;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _entityName = Arguments.GetString("entityName");
            _entityID = Arguments.GetInt("entityID");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => inflater.Inflate(Resource.Layout.fragment_add_entity, container, false);

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            ToolbarHelper.ShowToolbar(Activity, _entityName);
            ToolbarHelper.ShowToolbarBackButton(Activity);

            var title = view.FindViewById<TextView>(Resource.Id.title);
            title.Text = "Update " + _entityName;

            var inputsLayout = view.FindViewById<LinearLayout>(Resource.Id.inputs);
            var submitButton = view.FindViewById<AppCompatButton>(Resource.Id.btn_submit);

            var entity = DBTable.Get<TEntity>(_entityID);

            foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && !Attribute.IsDefined(property, typeof(AutoIncrementAttribute)))
                {
                    AddPropertyViews(entity, property, inputsLayout);
                }
            }

            submitButton.Click += (s, e) =>
            {
                ProgressDialogHelper.RunTask(Activity, "Submitting...", () =>
                {
                    Submit(view, entity);

                    // TODO - Come up with a smarter way of doing this...
                    if (entity is Reminder reminder)
                    {
                        ReminderHelper.ReplaceReminder(Context, reminder.PetID, reminder.IncidentTypeID);
                    }

                    Activity.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Activity, "Successfully updated entity", ToastLength.Long).Show();
                        FragmentHelper.PopOne(Activity);
                    });
                });
            };
        }

        private void AddPropertyViews(TEntity entity, PropertyInfo property, LinearLayout layout)
        {
            var value = property.GetValue(entity);
            var inputView = EntityHelper.CreateEntityPropertyView(Context, property, value);

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
                    var value = EntityHelper.GetPropertyValue(valueView, property.PropertyType);

                    if (value != null)
                    {
                        property.SetValue(entity, value);
                    }
                }
            }

            DBTable.Update(entity);
        }
    }
}
