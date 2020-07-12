using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using System;

namespace PottyLogger.Droid.Helpers
{
    public static class AlertDialogHelper
    {
        /// <summary>
        /// Displays an alert
        /// </summary>
        /// <param name="activity">The Activity to display the alert in</param>
        /// <param name="message">The message to display within the alert dialog</param>
        public static void DisplayAlert(Context context, string title, string message)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 24
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 5);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.indicator_input_error, 0, 0, 0);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton("OK", (s, args) => { })
                .SetCustomTitle(titleView)
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayAlert(Context context, int message, Action action)
        {
            var messageView = new TextView(context)
            {
                TextSize = 16
            };
            messageView.SetText(message);
            messageView.SetTextColor(Color.White);
            messageView.SetPadding(5, 5, 5, 5);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCancelable(false)
                .SetPositiveButton("OK", (s, args) =>
                {
                    action();
                    ((Dialog)s).Dismiss();
                })
                .SetCustomTitle(messageView)
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayDialog(Context context, string title, string message, Action positiveAction)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 2);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_notifications, 0, 0, 0);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(titleView)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton("Cancel", (s, args) => { })
                .SetPositiveButton("OK", (s, args) => positiveAction())
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayResults(Context context, string title, string message)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 2);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_add, 0, 0, 0);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(titleView)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton("OK", (s, args) => { })
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static LinearLayout GetLegendRow(Context context, string key, string description)
        {
            var layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                DividerPadding = 10
            };
            layout.SetClipToPadding(true);
            layout.SetGravity(GravityFlags.CenterVertical);

            var keyText = new TextView(context)
            {
                Text = key,
                TextSize = 20.0f
            };
            keyText.SetMinimumWidth(80);
            keyText.SetTextColor(Color.White);
            keyText.SetPadding(10, 10, 10, 10);

            var descriptionText = new TextView(context)
            {
                Text = description,
                TextSize = 20.0f
            };
            descriptionText.SetTextColor(Color.White);
            descriptionText.SetPadding(10, 10, 10, 10);

            layout.AddView(keyText);
            layout.AddView(descriptionText);

            return layout;
        }

        public static LinearLayout GetLegendRow(Context context, int iconID, string description)
        {
            var layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                DividerPadding = 10
            };
            layout.SetClipToPadding(true);
            layout.SetGravity(GravityFlags.CenterVertical);

            var icon = new ImageView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(80, 80)
            };
            icon.SetMinimumWidth(80);
            icon.SetImageResource(iconID);
            icon.SetPadding(10, 10, 10, 10);

            var descriptionText = new TextView(context)
            {
                Text = description,
                TextSize = 20.0f
            };
            descriptionText.SetTextColor(Color.White);
            descriptionText.SetPadding(10, 10, 10, 10);

            layout.AddView(icon);
            layout.AddView(descriptionText);

            return layout;
        }

        public static LinearLayout GetLegendRow(Context context, Color color, string description)
        {
            var layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                DividerPadding = 10
            };
            layout.SetClipToPadding(true);
            layout.SetGravity(GravityFlags.CenterVertical);

            var key = new View(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(80, 80),
                BackgroundTintList = ColorStateList.ValueOf(color)
            };
            key.SetBackgroundResource(Resource.Drawable.box);
            key.SetPadding(20, 10, 20, 20);

            var descriptionText = new TextView(context)
            {
                Text = description,
                TextSize = 20.0f
            };
            descriptionText.SetTextColor(Color.White);
            descriptionText.SetPadding(20, 10, 20, 20);

            layout.AddView(key);
            layout.AddView(descriptionText);

            return layout;
        }

        public static void DisplayLegend(Context context, string title, params LinearLayout[] rows)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 2);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_add, 0, 0, 0);

            var layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Vertical
            };
            layout.SetPadding(100, 10, 100, 10);
            layout.SetClipToPadding(true);

            foreach (var row in rows)
            {
                layout.AddView(row);
            }

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(titleView)
                .SetView(layout)
                .SetCancelable(true)
                .SetNegativeButton("OK", (s, args) => { })
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayMarker(Context context, string title, Action action)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 2);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_myplaces, 0, 0, 0);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(titleView)
                .SetCancelable(true)
                .SetNegativeButton("Cancel", (s, args) => { })
                .SetPositiveButton("Select", (s, args) => action())
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static void DisplayMarker(Context context, string title, string message)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(5, 5, 5, 2);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_myplaces, 0, 0, 0);

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(titleView)
                .SetMessage(message)
                .SetCancelable(true)
                .SetNegativeButton("OK", (s, args) => { })
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            dialog.Show();
        }

        public static AlertDialog BuildForm(Context context, string title, Action action, params View[] formContents)
        {
            var titleView = new TextView(context)
            {
                Text = title,
                TextSize = 20,
                Gravity = GravityFlags.Center,
            };
            titleView.SetTextColor(Color.White);
            titleView.SetPadding(10, 16, 6, 6);
            titleView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_menu_myplaces, 0, 0, 0);

            var layout = new LinearLayout(context)
            {
                Orientation = Android.Widget.Orientation.Vertical
            };
            layout.SetPadding(10, 16, 6, 6);

            foreach (var content in formContents)
            {
                layout.AddView(content);
            }

            var dialog = new AlertDialog.Builder(context, Resource.Style.AlertsDialogTheme)
                .SetCustomTitle(titleView)
                .SetView(layout)
                .SetCancelable(true)
                .SetNegativeButton("Cancel", (s, args) => { })
                .SetPositiveButton("OK", (s, args) => action())
                .Create();

            dialog.Window.SetBackgroundDrawable(ContextCompat.GetDrawable(context, Resource.Drawable.rounded_border_dark));
            return dialog;
        }
    }
}