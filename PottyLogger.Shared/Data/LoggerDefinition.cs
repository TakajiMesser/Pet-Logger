using PottyLogger.Shared.DataAccessLayer;
using SQLite;

namespace PottyLogger.Shared.Data
{
    public class LoggerDefinition : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Name { get; set; }

        public string ImageName { get; set; }

        public int Order { get; set; }

        [ForeignKey(typeof(Pet))]
        public int PetID { get; set; }

        [ForeignKey(typeof(IncidentType))]
        public int IncidentTypeID { get; set; }

        [Ignore]
        public Pet Pet => DBTable.Get<Pet>(PetID);

        [Ignore]
        public IncidentType IncidentType => DBTable.Get<IncidentType>(IncidentTypeID);
    }
}
