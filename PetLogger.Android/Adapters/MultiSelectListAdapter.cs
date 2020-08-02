using Android.Content;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public abstract class MultiSelectListAdapter<T> : MultiSelectAdapter<T>
    {
        private IList<T> _items;

        public MultiSelectListAdapter(Context context, IList<T> items) : base(context) => _items = items;

        public override int ItemCount => _items.Count;

        public IEnumerable<T> SelectedItems
        {
            get
            {
                foreach (var position in SelectedPositions)
                {
                    yield return _items[position];
                }
            }
        }

        protected override T GetItemAt(int position) => _items[position];

        public void AddItem(T item)
        {
            _items.Add(item);
            NotifyItemInserted(_items.Count - 1);
        }

        protected SearchFilter<T> CreateSearchFilter(Func<T, string> func) => new SearchFilter(this, func);

        private class SearchFilter : SearchFilter<T>
        {
            private MultiSelectListAdapter<T> _adapter;
            private Func<T, string> _func;

            public SearchFilter(MultiSelectListAdapter<T> adapter, Func<T, string> func)
            {
                _adapter = adapter;
                _func = func;
            }

            protected override IList<T> GetItemsToFilter() => _adapter._items;
            protected override string GetSearchStringFromItem(T item) => _func(item);

            protected override void SetItemsFromResults(List<T> items)
            {
                _adapter._items = items;
                _adapter.NotifyDataSetChanged();
            }
        }
    }
}