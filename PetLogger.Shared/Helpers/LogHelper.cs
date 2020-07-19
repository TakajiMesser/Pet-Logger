using System.IO;

namespace PetLogger.Shared.Helpers
{
    public static class LogHelper
    {
        public const string DIRECTORY_NAME = "Potty Logs";

        public static string LogDirectory =>
            #if __ANDROID__
                Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, DIRECTORY_NAME);
            #elif __IOS__
                throw new NotImplementedException();
            #else
                throw new NotImplementedException();
            #endif

        public static string GetLogPath(PottyTypes pottyType) => Path.Combine(LogDirectory, PottyHelper.ToPottyTypeString(pottyType) + "Log.csv");

        /*public static PottyLog GetPeeLog()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            var filePath = GetLogPath(PottyTypes.Pee);

            if (File.Exists(filePath))
            {

            }
            else
            {

            }
        }

        public static PottyLog GetPooLog()
        {

        }*/
    }
}