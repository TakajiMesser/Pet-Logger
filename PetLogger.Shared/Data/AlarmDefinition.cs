using PetLogger.Shared.DataAccessLayer;
using SQLite;
using System;

namespace PetLogger.Shared.Data
{
    public class AlarmDefinition : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public TimeSpan TimeBetween { get; set; }

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
