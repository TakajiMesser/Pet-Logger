using Android.App;
using Android.Content;
using Android.Support.Annotation;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using System;

namespace PetLogger.Droid.Helpers
{
    public static class DialogHelper
    {
        public static void DisplayAlert(Context context, [StringRes] int message)
        {
            var alertText = CreateAlertTextView(context, message);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCancelable(true)
                .SetCustomTitle(alertText)
                .SetNegativeButton("OK", (s, args) => { })
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayPrompt(Context context, [StringRes] int message, Action action)
        {
            var alertText = CreateAlertTextView(context, message);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCancelable(false)
                .SetCustomTitle(alertText)
                .SetPositiveButton("OK", (s, args) =>
                {
                    action();
                    ((Dialog)s).Dismiss();
                })
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayDialog(Context context, [StringRes] int message, Action positiveAction)
        {
            var messageText = CreateMessageTextView(context, message);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCancelable(true)
                .SetCustomTitle(messageText)
                .SetNegativeButton("Cancel", (s, args) => { })
                .SetPositiveButton("OK", (s, args) => positiveAction())
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayForm(Context context, [StringRes] int message, Action submitAction, Action dismissAction, params View[] formContents)
        {
            var layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Vertical
            };
            layout.SetPadding(10, 16, 6, 6);

            foreach (var content in formContents)
            {
                layout.AddView(content);
            }

            var formText = CreateFormTextView(context, message);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(formText)
                .SetView(layout)
                .SetCancelable(true)
                .SetNegativeButton("Cancel", (s, args) => { })
                .SetPositiveButton("OK", (s, args) => submitAction())
                .Create();

            dialog.DismissEvent += (s, args) => dismissAction();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        private static TextView CreateAlertTextView(Context context, [StringRes] int message)
        {
            var alertText = new TextView(context)
            {
                TextSize = 20.0f,
                CompoundDrawablePadding = 6,
                Gravity = GravityFlags.CenterVertical
            };
            alertText.SetText(message);
            alertText.SetTextColor(ColorHelper.TextGray);
            alertText.SetPadding(12, 12, 12, 12);
            alertText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.indicator_input_error, 0, 0, 0);

            return alertText;
        }

        private static TextView CreateMessageTextView(Context context, [StringRes] int message)
        {
            var messageText = new TextView(context)
            {
                TextSize = 18.0f,
                CompoundDrawablePadding = 6,
                Gravity = GravityFlags.CenterVertical
            };
            messageText.SetText(message);
            messageText.SetTextColor(ColorHelper.TextGray);
            messageText.SetPadding(12, 12, 12, 12);
            messageText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_notifications, 0, 0, 0);

            return messageText;
        }

        private static TextView CreateFormTextView(Context context, [StringRes] int message)
        {
            var formText = new TextView(context)
            {
                TextSize = 18.0f,
                CompoundDrawablePadding = 6,
                Gravity = GravityFlags.CenterVertical
            };
            formText.SetText(message);
            formText.SetTextColor(ColorHelper.TextGray);
            formText.SetPadding(12, 12, 12, 12);
            formText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_myplaces, 0, 0, 0);

            return formText;
        }
    }
}