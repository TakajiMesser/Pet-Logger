using PetLogger.Shared.DataAccessLayer;
using SQLite;

namespace PetLogger.Shared.Data
{
    public class Modifier : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Name { get; set; }

        [ForeignKey(typeof(Incident))]
        public int IncidentID { get; set; }

        [Ignore]
        public Incident Incident => DBTable.Get<Incident>(IncidentID);
    }
}
