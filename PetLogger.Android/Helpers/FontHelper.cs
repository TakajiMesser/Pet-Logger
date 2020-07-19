using Android.Content;
using Android.Graphics;

namespace PetLogger.Droid.Helpers
{
    public enum CustomFonts
    {
        RobotoRegular,
        RobotoBlack,
        RobotoBlackItalic,
        RobotoBold,
        RobotoBoldItalic,
        RobotoLight,
        RobotoLightItalic,
        RobotoMedium,
        RobotoMediumItalic,
        RobotoThin,
        RobotoThinItalic,
        RobotoCondensedRegular,
        RobotoCondensedBold,
        RobotoCondensedBoldItalic,
        RobotoCondensedItalic,
        RobotoCondensedLight,
        RobotoCondensedLightItalic
    }

    public static class FontHelper
    {
        private static string[] _paths = new string[]
        {
            "Fonts/Roboto-Regular.ttf",
            "Fonts/Roboto-Black.ttf",
            "Fonts/Roboto-BlackItalic.ttf",
            "Fonts/Roboto-Bold.ttf",
            "Fonts/Roboto-BoldItalic.ttf",
            "Fonts/Roboto-Light.ttf",
            "Fonts/Roboto-LightItallic.ttf",
            "Fonts/Roboto-Medium.ttf",
            "Fonts/Roboto-MediummItalic.ttf",
            "Fonts/Roboto-Thin.ttf",
            "Fonts/Roboto-ThinItalic.ttf",
            "Fonts/RobotoCondensed-Regular.ttf",
            "Fonts/RobotoCondensed-Bold.ttf",
            "Fonts/RobotoCondensed-BoldItalic.ttf",
            "Fonts/RobotoCondensed-Italic.ttf",
            "Fonts/RobotoCondensed-Light.ttf",
            "Fonts/RobotoCondensed-LightItalic.ttf"
        };

        private static Typeface[] _loadedTypefaces = new Typeface[_paths.Length];

        public static Typeface GetTypeface(Context context, CustomFonts font)
        {
            if ((int)font > _loadedTypefaces.Length)
            {
                LoadTypefaces(context);
            }

            return _loadedTypefaces[(int)font];
        }

        private static void LoadTypefaces(Context context)
        {
            for (var i = 0; i < _paths.Length; i++)
            {
                _loadedTypefaces[i] = Typeface.CreateFromAsset(context.Assets, _paths[i]);
            }
        }
    }
}