using Android.Content;
using Android.Media;
using Android.Widget;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class RingtoneAdapter : ArrayAdapter<string>
    {
        private List<string> _paths = new List<string>();

        public RingtoneAdapter(Context context, RingtoneType ringtoneType) : base(context, Android.Resource.Layout.SimpleSpinnerDropDownItem)
        {
            AddItem("None", "");

            var ringtoneManager= new RingtoneManager(context);
            ringtoneManager.SetType(ringtoneType);

            var cursor = ringtoneManager.Cursor;
            while (cursor.MoveToNext())
            {
                var title = cursor.GetString((int)RingtoneColumnIndex.Title);
                var path = cursor.GetString((int)RingtoneColumnIndex.Uri) + "/" + cursor.GetString((int)RingtoneColumnIndex.Id);

                AddItem(title, path);
            }
        }

        public void AddItem(string name, string path)
        {
            Add(name);
            _paths.Add(path);
        }

        public string GetPath(int position) => _paths[position];

        public int GetPathPosition(string path) => _paths.IndexOf(path);
    }
}