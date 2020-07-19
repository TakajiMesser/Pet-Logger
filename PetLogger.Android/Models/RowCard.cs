using System.Collections.Generic;
using System.Text;

namespace PetLogger.Droid.Models
{
    public class RowCard
    {
        public int ID { get; set; }
        public List<string> Cells { get; } = new List<string>();

        public override string ToString()
        {
            var builder = new StringBuilder(ID.ToString());

            foreach (var cell in Cells)
            {
                builder.Append(" | " + cell);
            }

            return builder.ToString();
        }
    }
}
