using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetLogger.Droid.Adapters
{
    public abstract class MultiSelectAdapter<T> : RecyclerView.Adapter, AbsListView.IMultiChoiceModeListener
    {
        private List<int> _selectedPositions = new List<int>();
        private ActionMode _actionMode;
        private AbsListView.IMultiChoiceModeListener _listener;

        public EventHandler<ItemClickEventArgs> ItemClick;
        public EventHandler<ItemLongClickEventArgs> ItemLongClick;

        public MultiSelectAdapter(Context context) => Context = context;

        public Context Context { get; }

        public int SelectionCount => _selectedPositions.Count;

        public IEnumerable<int> SelectedPositions
        {
            get
            {
                foreach (var position in _selectedPositions.OrderBy(p => p))
                {
                    yield return position;
                }
            }
        }

        protected abstract T GetItemAt(int position);

        public bool IsSelected(int position) => _selectedPositions.Contains(position);

        public void SetAllItemsChecked(bool @checked)
        {
            if (@checked)
            {
                for (var i = 0; i < ItemCount; i++)
                {
                    if (!_selectedPositions.Contains(i))
                    {
                        OnItemCheckedStateChanged(_actionMode, i, i + 1, true);
                    }
                }
            }
            else
            {
                foreach (var position in _selectedPositions)
                {
                    OnItemCheckedStateChanged(_actionMode, position, position + 1, false);
                }
            }

            NotifyDataSetChanged();
        }

        public void SetUpItemViewClickEvents(View itemView)
        {
            itemView.Click += (s, e) =>
            {
                int position = (int)((View)s).Tag;

                if (_selectedPositions.Any())
                {
                    OnItemCheckedStateChanged(_actionMode, position, ((View)s).Id, !_selectedPositions.Contains(position));
                }
                else
                {
                    ItemClick?.Invoke(s, new ItemClickEventArgs(position, GetItemAt(position)));
                }
            };

            itemView.LongClick += (s, e) =>
            {
                if (!SelectedPositions.Any())
                {
                    int position = (int)((View)s).Tag;

                    if (Context is Activity activity && _listener != null)
                    {
                        _actionMode = activity.StartActionMode(this);
                        OnItemCheckedStateChanged(_actionMode, position, ((View)s).Id, true);
                    }
                }
            };
        }

        public void SetMultiChoiceModeListener(AbsListView.IMultiChoiceModeListener listener) => _listener = listener;

        public void OnItemCheckedStateChanged(ActionMode mode, int position, long id, bool @checked)
        {
            if (@checked)
            {
                _selectedPositions.Add(position);
            }
            else
            {
                _selectedPositions.Remove(position);
                if (!_selectedPositions.Any())
                {
                    mode.Finish();
                }
            }

            mode.Title = _selectedPositions.Count + " Selected";

            NotifyItemChanged(position);
            _listener.OnItemCheckedStateChanged(mode, position, id, @checked);
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item) => _listener.OnActionItemClicked(mode, item);

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            NotifyDataSetChanged();
            return _listener.OnCreateActionMode(mode, menu);
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            _selectedPositions.Clear();
            NotifyDataSetChanged();
            _listener.OnDestroyActionMode(mode);
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => _listener.OnPrepareActionMode(mode, menu);

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

        public class ItemLongClickEventArgs : EventArgs
        {
            public int Position { get; set; }
            public T Item { get; set; }

            public ItemLongClickEventArgs(int position, T item)
            {
                Position = position;
                Item = item;
            }
        }
    }
}