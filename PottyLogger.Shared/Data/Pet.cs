using PottyLogger.Shared.DataAccessLayer;
using SQLite;

namespace PottyLogger.Shared.Data
{
    public class Pet : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Name { get; set; }

        [Ignore]
        public TableQuery<Incident> Incidents => DBTable.GetAll<Incident>(i => i.PetID == ID);
    }
}
