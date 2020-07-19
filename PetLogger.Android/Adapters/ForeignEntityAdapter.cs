using Android.Content;
using Android.Widget;
using PetLogger.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PetLogger.Droid.Adapters
{
    public class ForeignEntityAdapter : ArrayAdapter<string>
    {
        private List<int> _ids = new List<int>();

        public ForeignEntityAdapter(Context context, PropertyInfo identifier, IEnumerable<IEntity> foreigners) : base(context, Android.Resource.Layout.SimpleSpinnerItem, foreigners.Select(f => identifier.GetValue(f).ToString()).ToList())
        {
            _ids.AddRange(foreigners.Select(f => f.ID));
        }

        public void AddItem(string name, int id)
        {
            Add(name);
            _ids.Add(id);
        }

        public int GetID(int position) => _ids[position];
    }
}