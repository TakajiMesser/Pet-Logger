namespace PottyLogger.Droid.Helpers
{
    public static class ImageHelper
    {
        public static string[] ImageResourceIDs = new []
        {
            "alarm"
        };

        public static int GetImageResourceID(string name)
        {
            switch (name)
            {
                case "alarm":
                    return Resource.Drawable.perm_group_device_alarms;
            }

            return -1;
        }
    }
}