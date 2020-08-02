using SQLite;

namespace PetLogger.Shared.DataAccessLayer
{
    public class Entity : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public void Insert() => DBTable.Insert(this);

        public void Update() => DBTable.Update(this);

        public void Delete()
        {
            if (ID >= 0)
            {
                DBTable.Delete(this);
            }
        }
    }
}