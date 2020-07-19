using SQLite;

namespace PetLogger.Shared.DataAccessLayer
{
    public interface IEntity
    {
        [PrimaryKey, AutoIncrement]
        int ID { get; set; }
    }
}