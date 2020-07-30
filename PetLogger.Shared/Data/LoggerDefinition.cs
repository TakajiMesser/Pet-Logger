using PetLogger.Shared.DataAccessLayer;
using SQLite;

namespace PetLogger.Shared.Data
{
    public class LoggerDefinition : Entity
    {
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
        [Identifier]
        public string Title => Pet.Name + " " + IncidentType.Name;

        [Ignore]
        public Pet Pet => DBTable.Get<Pet>(PetID);

        [Ignore]
        public IncidentType IncidentType => DBTable.Get<IncidentType>(IncidentTypeID);
    }
}
