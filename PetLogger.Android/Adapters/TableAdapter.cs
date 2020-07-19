using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Models;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class TableAdapter : MultiSelectAdapter<TableCard>, IFilterable
    {
        public Filter Filter => _filter;
        public override int ItemCount => _tables.Count;
        public IEnumerable<TableCard> SelectedItems
        {
            get
            {
                foreach (var position in SelectedPositions)
                {
                    yield return _tables[position];
                }
            }
        }

        private List<TableCard> _tables;
        private SearchFilter _filter;

        public TableAdapter(Context context, List<TableCard> tables) : base(context)
        {
            _tables = tables;
            _filter = new SearchFilter(this);
        }

        protected override TableCard GetItemAt(int position) => _tables[position];

        public void ClearSelectedTables()
        {
            foreach (var tableCard in SelectedItems)
            {
                DBTable.Create(DBAccess.ParseTableName(tableCard.Name), true);
                tableCard.RowCount = 0;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_table, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var tableCard = _tables[position];

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);

            viewHolder.TableName.Text = tableCard.Name;
            viewHolder.TableName.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            int nRows = tableCard.RowCount;
            viewHolder.RowCount.Text = nRows + " row";
            if (nRows != 1)
            {
                viewHolder.RowCount.Text += "s";
            }
            viewHolder.RowCount.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            viewHolder.Columns.Text = "Columns: " + string.Join(" | ", tableCard.Columns);
            viewHolder.Columns.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView TableName { get; set; }
            public TextView RowCount { get; set; }
            public TextView Columns { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                TableName = itemView.FindViewById<TextView>(Resource.Id.table_name);
                RowCount = itemView.FindViewById<TextView>(Resource.Id.row_count);
                Columns = itemView.FindViewById<TextView>(Resource.Id.columns);
            }
        }

        private class SearchFilter : SearchFilter<TableCard>
        {
            private TableAdapter _adapter;

            public SearchFilter(TableAdapter adapter) => _adapter = adapter;

            protected override List<TableCard> GetItemsToFilter() => _adapter._tables;
            protected override string GetSearchStringFromItem(TableCard item) => item.Name;

            protected override void SetItemsFromResults(List<TableCard> items)
            {
                _adapter._tables = items;
                _adapter.NotifyDataSetChanged();
            }
        }
    }
}