using PetLogger.Shared.DataAccessLayer;
using SQLite;

namespace PetLogger.Shared.Data
{
    public class IncidentType : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Name { get; set; }

        public string ImageName { get; set; }

        [Ignore]
        public TableQuery<Incident> Incidents => DBTable.GetAll<Incident>(i => i.IncidentTypeID == ID);
    }
}
