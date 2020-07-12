using System;
using System.Globalization;

namespace PottyLogger.Shared.Models
{
    public class PottyLogEntry
    {
        public const string HEADERS = "Type,Date,Time";

        private const string DATE_FORMAT = "MM-dd-yyyy";
        private const string TIME_FORMAT = "HH:mm";

        /*public PottyTypes PottyType { get; set; }
        public DateTime Time { get; set; }

        public string ToCSVRow() => ToPottyTypeString(PottyType)
            + "," + Time.ToString(DATE_FORMAT)
            + "," + Time.ToString(TIME_FORMAT);

        public static PottyLogEntry ParseCSVRow(string csvRow)
        {
            var values = csvRow.Split(",");

            var pottyType = ParsePottyType(values[0]);

            if (DateTime.TryParseExact(values[1], DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date)) { }
            if (DateTime.TryParseExact(values[2], DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time)) { }

            return new PottyLogEntry
            {
                PottyType = pottyType,
                Time = date + time
            };
        }*/
    }
}