using Android.Content;
using Android.Widget;
using PottyLogger.Shared.Models.Logging;
using System;
using System.IO;

namespace PottyLogger.Droid.Models.Logging
{
    public class DebugLog : Log
    {
        public const string DIRECTORY_NAME = "PottyLogger Debug Logs";
        public const string DEFAULT_LOG_NAME = "Base Debug Log";

        public static DebugLog BaseLog => new DebugLog(DEFAULT_LOG_NAME);

        private static readonly object _lock = new object();

        public DebugLog(string fileName) : base(fileName) { }

        public override string LogDirectory => Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, DIRECTORY_NAME);

        /*public static IEnumerable<DebugLog> GetDebugLogs()
        {
            if (Directory.Exists(LogDirectory))
            {
                foreach (var file in Directory.GetFiles(LogDirectory, "*" + FILE_EXTENSION).OrderByDescending(f => f))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    yield return new DebugLog(fileName);
                }
            }
        }*/

        public static void LazyWrite(Context context, string message)
        {
            try
            {
                BaseLog.Append(message);
            }
            catch (FileNotFoundException ex)
            {
                Toast.MakeText(context, "Unable to find debug log file path (" + ex.Message + ")", ToastLength.Long).Show();
            }
            catch (DirectoryNotFoundException ex)
            {
                Toast.MakeText(context, "Unable to find debug log file directory (" + ex.Message + ")", ToastLength.Long).Show();
            }
            catch (UnauthorizedAccessException ex)
            {
                Toast.MakeText(context, "Unable to access debug log (" + ex.Message + ")", ToastLength.Long).Show();
            }
        }

        public static void LazyWrite(Context context, Exception ex)
        {
            try
            {
                BaseLog.Append(ex);
            }
            catch (FileNotFoundException e)
            {
                Toast.MakeText(context, "Unable to find debug log file path (" + e.Message + ")", ToastLength.Long).Show();
            }
            catch (DirectoryNotFoundException e)
            {
                Toast.MakeText(context, "Unable to find debug log file directory (" + e.Message + ")", ToastLength.Long).Show();
            }
            catch (UnauthorizedAccessException e)
            {
                Toast.MakeText(context, "Unable to access debug log (" + e.Message + ")", ToastLength.Long).Show();
            }
        }
    }
}