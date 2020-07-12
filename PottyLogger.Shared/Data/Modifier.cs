using PottyLogger.Shared.DataAccessLayer;
using SQLite;

namespace PottyLogger.Shared.Data
{
    public class Modifier : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Name { get; set; }

        public string Value { get; set; }

        [ForeignKey(typeof(Incident))]
        public int IncidentID { get; set; }

        [Ignore]
        public Incident Incident => DBTable.Get<Incident>(IncidentID);
    }
}
