using PetLogger.Shared.DataAccessLayer;
using SQLite;

namespace PetLogger.Shared.Data
{
    public class PetType : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Identifier]
        public string Name { get; set; }

        [Ignore]
        public TableQuery<Pet> Pets => DBTable.GetAll<Pet>(p => p.PetTypeID == ID);
    }
}
