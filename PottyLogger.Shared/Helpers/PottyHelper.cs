using System;

namespace PottyLogger.Shared.Helpers
{
    public enum PottyTypes
    {
        Pee,
        Poo
    }

    public static class PottyHelper
    {
        public static PottyTypes ParsePottyType(string value)
        {
            switch (value)
            {
                case "Pee":
                    return PottyTypes.Pee;
                case "Poo":
                    return PottyTypes.Poo;
            }

            throw new ArgumentOutOfRangeException("Could not parse potty type with value " + value);
        }

        public static string ToPottyTypeString(PottyTypes value)
        {
            switch (value)
            {
                case PottyTypes.Pee:
                    return "Pee";
                case PottyTypes.Poo:
                    return "Poo";
            }

            throw new NotImplementedException("Could not find string for potty type " + value);
        }
    }
}