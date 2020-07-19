using Android;
using Android.App;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using PetLogger.Droid.Models.Logging;
using System;
using System.Linq;

namespace PetLogger.Droid.Helpers
{
    public static class PermissionHelper
    {
        private const int PERMISSIONS_REQUEST_CODE = 2;
        private static string[] PERMISSIONS = new string[] { Manifest.Permission.WriteExternalStorage };

        public static void RequestPermissions(Activity activity, Action onGranted)
        {
            var neededPermissions = PERMISSIONS.Where(p => ContextCompat.CheckSelfPermission(activity, p) != Permission.Granted);

            if (neededPermissions.Any())
            {
                ActivityCompat.RequestPermissions(activity, neededPermissions.ToArray(), PERMISSIONS_REQUEST_CODE);
            }
            else
            {
                activity.RunOnUiThread(() => onGranted());
            }
        }

        public static bool HandlePermissionsResult(Activity activity, int requestCode, string[] permissions, Permission[] grantResults, Action onGranted)
        {
            if (requestCode == PERMISSIONS_REQUEST_CODE)
            {
                for (var i = 0; i < permissions.Length; i++)
                {
                    if (i >= grantResults.Length)
                    {
                        break;
                    }
                    else
                    {
                        var permission = permissions[i];
                        var grantResult = grantResults[i];

                        if (grantResult != Permission.Granted)
                        {
                            DebugLog.LazyWrite(activity, "Permission not granted: results len = " + grantResults.Length + " Result code = " + (grantResults.Length > 0 ? grantResults[0].ToString() : "(empty"));
                            HandlePermissionDenial(activity, permission, onGranted);
                            return true;
                        }
                        else
                        {
                            DebugLog.LazyWrite(activity, permission + " permission granted");
                        }
                    }
                }

                DebugLog.LazyWrite(activity, "All permissions granted");

                activity.RunOnUiThread(() => onGranted());
                return true;
            }
            else
            {
                DebugLog.LazyWrite(activity, "Got unexpected permission result: " + requestCode);
                return false;
            }
        }

        private static void HandlePermissionDenial(Activity activity, string permission, Action onGranted)
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, permission))
            {
                AlertDialogHelper.DisplayAlert(activity, GetRationalMessageID(permission), () => RequestPermissions(activity, onGranted));
            }
            else
            {
                AlertDialogHelper.DisplayAlert(activity, GetFailureMessageID(permission), () => activity.Finish());
            }
        }

        private static int GetRationalMessageID(string permission)
        {
            switch (permission)
            {
                case Manifest.Permission.WriteExternalStorage:
                    return Resource.String.permission_write_external_storage_rationale;
            }

            return -1;
        }

        private static int GetFailureMessageID(string permission)
        {
            switch (permission)
            {
                case Manifest.Permission.WriteExternalStorage:
                    return Resource.String.no_write_external_storage_permission;
            }

            return -1;
        }
    }
}