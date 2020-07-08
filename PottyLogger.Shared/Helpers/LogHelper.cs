using System;
using System.Collections.Generic;

namespace PottyLogger.Shared.Helpers
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

        public static string GetLogPath(PottyTypes pottyType) => Path.Combine(LogDirectory, PottyHelper.ToPottyTypeString(pottyType));

        public static void GetPeeLog()
        {
            var filePath = GetLogPath(PottyTypes.Pee);
        }

        public static void GetPooLog()
        {

        }
    }
}