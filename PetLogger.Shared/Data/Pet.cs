using PetLogger.Shared.DataAccessLayer;
using SQLite;

namespace PetLogger.Shared.Data
{
    public class Pet : Entity
    {
        [Identifier]
        public string Name { get; set; }

        [ForeignKey(typeof(PetType))]
        public int PetTypeID { get; set; }

        [Ignore]
        public PetType PetType => DBTable.Get<PetType>(PetTypeID);

        [Ignore]
        public TableQuery<Incident> Incidents => DBTable.GetAll<Incident>(i => i.PetID == ID);
    }
}
