using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public abstract class ListAdapter<T> : RecyclerView.Adapter
    {
        private IList<T> _items;

        public ListAdapter(Context context) : this(context, new List<T>()) { }
        public ListAdapter(Context context, IList<T> items)
        {
            Context = context;
            _items = items;
        }

        public Context Context { get; }

        public override int ItemCount => _items.Count;

        public EventHandler<ItemClickEventArgs> ItemClick;

        protected T GetItemAt(int position) => _items[position];

        public void AddItem(T item)
        {
            _items.Add(item);
            NotifyItemInserted(_items.Count - 1);
        }

        public void AddItems(IEnumerable<T> items)
        {
            var startPosition = _items.Count;
            foreach (var item in items)
            {
                _items.Add(item);
            }

            NotifyItemRangeInserted(startPosition, _items.Count - startPosition);
        }

        public void RemoveItem(T item)
        {
            var index = _items.IndexOf(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                NotifyItemRemoved(index);
            }
        }

        public void SetUpItemViewClickEvents(View itemView)
        {
            itemView.Click += (s, e) =>
            {
                var position = (int)((View)s).Tag;
                ItemClick?.Invoke(s, new ItemClickEventArgs(position, GetItemAt(position)));
            };
        }

        protected SearchFilter<T> CreateSearchFilter(Func<T, string> func) => new SearchFilter(this, func);

        public class ItemClickEventArgs : EventArgs
        {
            public int Position { get; set; }
            public T Item { get; set; }

            public ItemClickEventArgs(int position, T item)
            {
                Position = position;
                Item = item;
            }
        }

        private class SearchFilter : SearchFilter<T>
        {
            private ListAdapter<T> _adapter;
            private Func<T, string> _func;

            public SearchFilter(ListAdapter<T> adapter, Func<T, string> func)
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