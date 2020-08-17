using Android.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using ActionMode = Android.Support.V7.View.ActionMode;

namespace PetLogger.Droid.Adapters
{
    public abstract class MultiSelectAdapter<T> : RecyclerView.Adapter, ActionMode.ICallback
    {
        private List<int> _selectedPositions = new List<int>();
        private ActionMode _actionMode;
        private ActionMode.ICallback _actionModeCallback;

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

        public EventHandler<ItemClickEventArgs> ItemClick;
        public EventHandler<ItemLongClickEventArgs> ItemLongClick;

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
                var position = (int)((View)s).Tag;

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
                    var position = (int)((View)s).Tag;

                    if (Context is AppCompatActivity activity && _actionModeCallback != null)
                    {
                        _actionMode = activity.StartSupportActionMode(this);
                        OnItemCheckedStateChanged(_actionMode, position, ((View)s).Id, true);
                    }
                }
            };
        }

        public void SetActionModeCallback(ActionMode.ICallback callback) => _actionModeCallback = callback;

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
            //_actionModeCallback.OnItemCheckedStateChanged(mode, position, id, @checked);
        }

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item) => _actionModeCallback.OnActionItemClicked(mode, item);

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            NotifyDataSetChanged();
            return _actionModeCallback.OnCreateActionMode(mode, menu);
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            _selectedPositions.Clear();
            NotifyDataSetChanged();
            _actionModeCallback.OnDestroyActionMode(mode);
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => _actionModeCallback.OnPrepareActionMode(mode, menu);

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