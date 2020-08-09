using System;

namespace PetLogger.Droid.Helpers
{
    public enum Themes
    {
        Light,
        Dark
    }

    public static class ThemeHelper
    {
        public static string[] ImageKeys = new []
        {
            "add-alarm",
            "umbrella",
            "wc",
            "bath"
        };

        public static Themes ParseTheme(string value)
        {
            switch (value)
            {
                case "light":
                    return Themes.Light;
                case "dark":
                    return Themes.Dark;
            }

            throw new ArgumentOutOfRangeException("Could not find parse theme from " + value);
        }

        public static int GetImageResourceID(string imageKey, Themes theme)
        {
            switch (imageKey)
            {
                case "add-alarm":
                    return GetAddAlarmResourceID(theme);
                case "umbrella":
                    return GetUmbrellaResourceID(theme);
                case "wc":
                    return GetWCResourceID(theme);
                case "bath":
                    return GetBathResourceID(theme);
            }

            return -1;
        }

        private static int GetAddAlarmResourceID(Themes theme)
        {
            switch (theme)
            {
                case Themes.Light:
                    return Resource.Drawable.baseline_alarm_add_black_36dp;
                case Themes.Dark:
                    return Resource.Drawable.baseline_alarm_add_white_36dp;
            }

            return -1;
        }

        private static int GetUmbrellaResourceID(Themes theme)
        {
            switch (theme)
            {
                case Themes.Light:
                    return Resource.Drawable.baseline_umbrella_black_36dp;
                case Themes.Dark:
                    return Resource.Drawable.baseline_umbrella_white_36dp;
            }

            return -1;
        }

        private static int GetWCResourceID(Themes theme)
        {
            switch (theme)
            {
                case Themes.Light:
                    return Resource.Drawable.baseline_wc_black_36dp;
                case Themes.Dark:
                    return Resource.Drawable.baseline_wc_white_36dp;
            }

            return -1;
        }

        private static int GetBathResourceID(Themes theme)
        {
            switch (theme)
            {
                case Themes.Light:
                    return Resource.Drawable.baseline_bathtub_black_36dp;
                case Themes.Dark:
                    return Resource.Drawable.baseline_bathtub_white_36dp;
            }

            return -1;
        }
    }
}