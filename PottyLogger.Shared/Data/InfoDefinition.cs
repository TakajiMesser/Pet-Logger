using PottyLogger.Shared.DataAccessLayer;
using SQLite;

namespace PottyLogger.Shared.Data
{
    public class InfoDefinition : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IncludeDays { get; set; }
        public bool IncludeHours { get; set; }
        public bool IncludeMinutes { get; set; }
        public bool IncludeSeconds { get; set; }

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
