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
    public class TableAdapter : MultiSelectListAdapter<TableCard>, IFilterable
    {
        private SearchFilter<TableCard> _filter;

        public TableAdapter(Context context, IList<TableCard> tables) : base(context, tables) =>
            _filter = CreateSearchFilter(l => l.Name);

        public Filter Filter => _filter;

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
            var tableCard = GetItemAt(position);

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
    }
}
