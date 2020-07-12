using SQLite;

namespace PottyLogger.Shared.DataAccessLayer
{
    public class Entity : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}