using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PottyLogger.Droid.Helpers;
using PottyLogger.Droid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PottyLogger.Droid.Components
{
    public class TextRowAdapter : RecyclerView.Adapter, AbsListView.IMultiChoiceModeListener, IFilterable
    {
        public const float DEFAULT_TEXT_SIZE = 18.0f;
        public const int DEFAULT_TEXT_COLOR = unchecked((int)0xFF0000FF);
        public const float DEFAULT_NUMBER_SIZE = 16.0f;
        public const int DEFAULT_NUMBER_COLOR = unchecked((int)0xFFBBBBBB);

        private List<string> _rows = new List<string>();
        private List<int> _selectedPositions = new List<int>();
        private ActionMode _actionMode;
        private AbsListView.IMultiChoiceModeListener _listener;
        private TextRowFilter _filter;

        public Context Context { get; private set; }
        public float RowTextSize { get; set; } = DEFAULT_TEXT_SIZE;
        public Color RowTextColor { get; set; } = Color.ParseColor("#" + Integer.ToHexString(DEFAULT_TEXT_COLOR));
        public Typeface RowTypeface { get; set; }
        public Drawable RowBackground { get; set; }
        public float LineNumberTextSize { get; set; } = DEFAULT_TEXT_SIZE;
        public Color LineNumberColor { get; set; } = Color.ParseColor("#" + Integer.ToHexString(DEFAULT_TEXT_COLOR));
        public Typeface LineNumberTypeface { get; set; }
        public Drawable LineNumberBackground { get; set; }
        public Drawable VerticalDivider { get; set; }
        public Filter Filter => _filter;
        public override int ItemCount => _rows.Count;
        public int MaxRowLength { get; private set; }
        public int MaxNumberLength { get; private set; }

        /*public IEnumerable<int> SelectedIndices
        {
            get
            {
                foreach (var position in _selectedPositions.OrderBy(p => p))
                {
                    yield return _rows[position].ID;
                }
            }
        }*/

        public EventHandler<ItemClickEventArgs> ItemClick;
        public EventHandler<ItemLongClickEventArgs> ItemLongClick;

        public TextRowAdapter(Context context)
        {
            Context = context;
            _filter = new TextRowFilter(this);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var row = _rows[position];

            viewHolder.Cells.Tag = position;

            var lineNumberText = viewHolder.Cells.GetChildAt(0) as TextView;
            lineNumberText.Text = position.ToString();

            var lineText = viewHolder.Cells.GetChildAt(1) as TextView;
            lineText.Text = row;

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

            var lineNumberText = new TextView(parent.Context)
            {
                Typeface = FontHelper.GetTypeface(parent.Context, CustomFonts.RobotoCondensedRegular)
            };
            lineNumberText.SetTextSize(ComplexUnitType.Dip, LineNumberTextSize);
            lineNumberText.SetTextColor(LineNumberColor);
            lineNumberText.SetMaxLines(1);
            lineNumberText.SetPadding(20, 10, 10, 10);
            lineNumberText.SetMinEms(MaxNumberLength);

            var layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            row.AddView(lineNumberText, layoutParams);

            var lineText = new TextView(parent.Context)
            {
                Typeface = FontHelper.GetTypeface(parent.Context, CustomFonts.RobotoCondensedRegular)
            };
            lineText.SetTextSize(ComplexUnitType.Dip, RowTextSize);
            lineText.SetTextColor(RowTextColor);
            lineText.SetMaxLines(1);
            lineText.SetPadding(10, 10, 20, 10);
            lineText.SetMinEms(MaxRowLength);

            row.AddView(lineText, layoutParams);

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

        public void AddRow(string row)
        {
            if (row.Length > MaxRowLength)
            {
                MaxRowLength = row.Length;
            }

            _rows.Add(row);
            MaxNumberLength = _rows.Count.ToString().Length;

            NotifyItemInserted(_rows.Count - 1);
        }

        public void AddRows(IEnumerable<string> rows)
        {
            foreach (var row in rows)
            {
                if (row.Length > MaxRowLength)
                {
                    MaxRowLength = row.Length;
                }

                _rows.Add(row);
            }
            MaxNumberLength = _rows.Count.ToString().Length;

            NotifyDataSetChanged();
        }

        private class TextRowFilter : Filter
        {
            private readonly TextRowAdapter _adapter;
            private List<string> _originalRows;

            public TextRowFilter(TextRowAdapter adapter)
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

                var results = new List<string>();

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
                    _adapter._rows = values.ToArray<Java.Lang.Object>().Select(a => a.ToNetObject<string>()).ToList();
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
            public string RowText { get; set; }

            public ItemClickEventArgs(int position, string rowText)
            {
                Position = position;
                RowText = rowText;
            }
        }

        public class ItemLongClickEventArgs : EventArgs
        {
            public int Position { get; set; }
            public string RowText { get; set; }

            public ItemLongClickEventArgs(int position, string rowText)
            {
                Position = position;
                RowText = rowText;
            }
        }
    }
}
