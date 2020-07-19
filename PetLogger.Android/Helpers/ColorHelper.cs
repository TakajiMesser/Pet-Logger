using Android.App;
using Android.Graphics;
using Android.Support.V4.Content;

namespace PetLogger.Droid.Helpers
{
    public static class ColorHelper
    {
        public static Color GetColor(int resourceID) => new Color(ContextCompat.GetColor(Application.Context, resourceID));

        public static Color Primary => GetColor(Resource.Color.colorPrimary);
        public static Color TextGray => GetColor(Resource.Color.textGray);
    }
}