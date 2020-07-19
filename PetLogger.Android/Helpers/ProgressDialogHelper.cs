using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using System.Threading.Tasks;

namespace PetLogger.Droid.Helpers
{
    public static class ProgressDialogHelper
    {
        public static ProgressDialog Display(Activity activity, string message)
        {
            var dialog = new ProgressDialog(activity, Resource.Style.ProgressDialogTheme)
            {
                Indeterminate = true
            };
            dialog.SetMessage(message);
            dialog.SetCancelable(false);
            dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

            activity.RunOnUiThread(() => dialog.Show());

            return dialog;
        }

        public static void RunTask(Activity activity, string message, Action action)
        {
            var dialog = Display(activity, message);
            RunTask(activity, dialog, action);
        }

        public static void RunTask(Activity activity, ProgressDialog dialog, Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    activity.RunOnUiThread(() => AlertDialogHelper.DisplayAlert(activity, "ERROR", ex.Message));
                }
                finally
                {
                    Dismiss(activity, dialog);
                }
            });
        }

        public static void RunTask(Activity activity, string message, Func<Task> func)
        {
            var dialog = Display(activity, message);
            RunTask(activity, dialog, func);
        }

        public static void RunTask(Activity activity, ProgressDialog dialog, Func<Task> func)
        {
            Task.Run(async () =>
            {
                try
                {
                    await func();
                }
                catch (Exception ex)
                {
                    activity.RunOnUiThread(() => AlertDialogHelper.DisplayAlert(activity, "ERROR", ex.Message));
                }
                finally
                {
                    Dismiss(activity, dialog);
                }
            });
        }

        public static void UpdateMessage(Activity activity, ProgressDialog dialog, string message)
        {
            if (IsActivityAlive(activity) && dialog != null && dialog.IsShowing)
            {
                activity.RunOnUiThread(() => dialog.SetMessage(message));
            }
        }

        public static void Dismiss(Activity activity, ProgressDialog dialog)
        {
            if (IsActivityAlive(activity) && dialog != null && dialog.IsShowing)
            {
                activity.RunOnUiThread(() => dialog.Dismiss());
            }
        }

        private static bool IsActivityAlive(Activity activity)
        {
            return Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1
                ? !activity.IsDestroyed
                : !activity.IsFinishing;
        }
    }
}