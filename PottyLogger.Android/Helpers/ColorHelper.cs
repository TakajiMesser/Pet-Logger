using Android.App;
using Android.Graphics;
using Android.Support.V4.Content;

namespace PottyLogger.Droid.Helpers
{
    public static class ColorHelper
    {
        public static Color GetColor(int resourceID) => new Color(ContextCompat.GetColor(Application.Context, resourceID));

        public static Color Primary => GetColor(Resource.Color.colorPrimary);
    }
}