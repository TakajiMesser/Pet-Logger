using SQLite;

namespace PottyLogger.Shared.DataAccessLayer
{
    public interface IEntity
    {
        [PrimaryKey, AutoIncrement]
        int ID { get; set; }
    }
}