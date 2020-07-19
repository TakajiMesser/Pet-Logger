using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetLogger.Droid.Components
{
    public class CSVTableAdapter : RecyclerView.Adapter, AbsListView.IMultiChoiceModeListener, IFilterable
    {
        public const float DEFAULT_TEXT_SIZE = 20.0f;
        public const int DEFAULT_TEXT_COLOR = unchecked((int)0xFFFFFFFF);

        private List<CSVTableRow> _rows = new List<CSVTableRow>();
        private List<int> _selectedPositions = new List<int>();
        private List<int> _maxColumnLengths = new List<int>();
        private ActionMode _actionMode;
        private AbsListView.IMultiChoiceModeListener _listener;
        private CSVTableFilter _filter;

        public Context Context { get; private set; }
        public float TextSize { get; set; } = DEFAULT_TEXT_SIZE;
        public Color TextColor { get; set; } = Color.ParseColor("#" + Integer.ToHexString(DEFAULT_TEXT_COLOR));
        public Typeface Typeface { get; set; }
        public Drawable RowBackground { get; set; }
        public Drawable VerticalDivider { get; set; }
        public Filter Filter => _filter;
        public override int ItemCount => _rows.Count;

        public IEnumerable<int> SelectedPositions => _selectedPositions.OrderBy(p => p);

        public EventHandler<ItemClickEventArgs> ItemClick;
        public EventHandler<ItemClickEventArgs> ItemLongClick;

        public CSVTableAdapter(Context context)
        {
            Context = context;
            _filter = new CSVTableFilter(this);
        }

        public void SetColumnLength(int cellIndex, int length) => _maxColumnLengths[cellIndex] = length;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var row = _rows[position];

            viewHolder.Cells.Tag = position;

            for (var i = 0; i < viewHolder.Cells.ChildCount; i++)
            {
                var cell = viewHolder.Cells.GetChildAt(i) as TextView;
                cell.Text = row.Cells[i];
            }

            viewHolder.Cells.Selected = _selectedPositions.Contains(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var row = new LinearLayout(parent.Context)
            {
                Orientation = Orientation.Horizontal,
                ShowDividers = ShowDividers.Middle
            };

            if (RowBackground != null)
            {
                row.Background = RowBackground.GetConstantState().NewDrawable().Mutate();
            }

            if (VerticalDivider != null)
            {
                row.SetDividerDrawable(VerticalDivider);
            }

            for (var i = 0; i < _maxColumnLengths.Count; i++)
            {
                var textView = new TextView(parent.Context)
                {
                    Typeface = FontHelper.GetTypeface(parent.Context, CustomFonts.RobotoCondensedRegular)
                };
                textView.SetTextSize(ComplexUnitType.Dip, TextSize);
                textView.SetTextColor(TextColor);
                textView.SetMaxLines(1);
                textView.SetPadding(20, 20, 10, 40);
                textView.SetMinEms(_maxColumnLengths[i]);

                var layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                row.AddView(textView, layoutParams);
            }

            var viewHolder = new ViewHolder(row);

            viewHolder.Cells.Click += (s, e) =>
            {
                int position = (int)((LinearLayout)s).Tag;

                if (_selectedPositions.Any())
                {
                    OnItemCheckedStateChanged(_actionMode, position, ((LinearLayout)s).Id, !_selectedPositions.Contains(position));
                }
                else
                {
                    ItemClick?.Invoke(s, new ItemClickEventArgs(position, _rows[position]));
                }
            };

            viewHolder.Cells.LongClick += (s, e) =>
            {
                if (!_selectedPositions.Any())
                {
                    int position = (int)((LinearLayout)s).Tag;

                    if (Context is Activity activity && _listener != null)
                    {
                        _actionMode = activity.StartActionMode(this);
                        OnItemCheckedStateChanged(_actionMode, position, ((LinearLayout)s).Id, true);
                    }
                }
            };

            return viewHolder;
        }

        public void AddRow(CSVTableRow row)
        {
            for (var i = 0; i < row.Cells.Count; i++)
            {
                if (row.Cells[i].Length > _maxColumnLengths[i])
                {
                    _maxColumnLengths[i] = row.Cells[i].Length;
                }
            }

            _rows.Add(row);
            NotifyItemInserted(_rows.Count - 1);
        }

        public void AddRows(IEnumerable<CSVTableRow> rows)
        {
            foreach (var row in rows)
            {
                for (var i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].Length > _maxColumnLengths[i])
                    {
                        _maxColumnLengths[i] = row.Cells[i].Length;
                    }
                }

                _rows.Add(row);
            }

            NotifyDataSetChanged();
        }

        private class CSVTableFilter : Filter
        {
            private readonly CSVTableAdapter _adapter;
            private List<CSVTableRow> _originalRows;

            public CSVTableFilter(CSVTableAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                if (_originalRows == null)
                {
                    _originalRows = _adapter._rows;
                }

                var filterResults = new FilterResults();

                if (constraint == null)
                {
                    return filterResults;
                }

                var results = new List<CSVTableRow>();

                if (_originalRows != null && _originalRows.Any())
                {
                    var constraintString = constraint.ToString();
                    if (constraintString != null)
                    {
                        results.AddRange(_originalRows.Where(r => r.ToString().ToLower().Contains(constraintString.ToLower())));
                    }
                }

                filterResults.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                filterResults.Count = results.Count;

                constraint.Dispose();

                return filterResults;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                {
                    _adapter._rows = values.ToArray<Java.Lang.Object>().Select(a => a.ToNetObject<CSVTableRow>()).ToList();
                }

                _adapter.NotifyDataSetChanged();

                constraint.Dispose();
                results.Dispose();
            }
        }

        public void SetMultiChoiceModeListener(AbsListView.IMultiChoiceModeListener listener) => _listener = listener;

        public void SetAllItemsChecked(bool @checked)
        {
            if (@checked)
            {
                for (var i = 0; i < _rows.Count; i++)
                {
                    if (!_selectedPositions.Contains(i))
                    {
                        _selectedPositions.Add(i);
                    }
                }
            }
            else
            {
                _selectedPositions.Clear();
            }

            NotifyDataSetChanged();
        }

        public void DeleteSelectedItems()
        {
            foreach (var position in _selectedPositions.OrderByDescending(p => p))
            {
                _rows.RemoveAt(position);
            }

            NotifyDataSetChanged();
        }

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

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public LinearLayout Cells { get; set; }
            public StateListDrawable Background { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Cells = (LinearLayout)itemView;
                Cells.Clickable = true;
                Cells.LongClickable = true;

                Background = (StateListDrawable)itemView.Background;
            }
        }

        public class ItemClickEventArgs : EventArgs
        {
            public int Position { get; set; }
            public CSVTableRow Row { get; set; }

            public ItemClickEventArgs(int position, CSVTableRow row)
            {
                Position = position;
                Row = row;
            }
        }
    }
}
