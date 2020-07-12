using System.Collections.Generic;

namespace PottyLogger.Droid.Models
{
    public class TableCard
    {
        public string Name { get; set; }
        public int RowCount { get; set; }
        public List<string> Columns { get; set; }
    }
}
