using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PottyLogger.Droid.Helpers;
using PottyLogger.Shared.Models;
using System.Collections.Generic;

namespace PottyLogger.Droid.Adapters
{
    public class LoggerAdapter : MultiSelectAdapter<IncidentLogger>, IFilterable
    {
        public Filter Filter => _filter;
        public override int ItemCount => _loggers.Count;
        public IEnumerable<IncidentLogger> SelectedItems
        {
            get
            {
                foreach (var position in SelectedPositions)
                {
                    yield return _loggers[position];
                }
            }
        }

        private List<IncidentLogger> _loggers;
        private SearchFilter _filter;

        public LoggerAdapter(Context context, List<IncidentLogger> cards) : base(context)
        {
            _loggers = cards;
            _filter = new SearchFilter(this);
        }

        protected override IncidentLogger GetItemAt(int position) => _loggers[position];

        /*public void ClearSelectedCards()
        {
            foreach (var infoCard in SelectedItems)
            {
                DBTable.Create(DBAccess.ParseTableName(infoCard.Name), true);
                infoCard.RowCount = 0;
            }
        }*/

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_logger, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var logger = _loggers[position];

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);

            viewHolder.Button.SetImageResource(logger.ImageResourceID);

            viewHolder.Label.Text = logger.Name;
            viewHolder.Label.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public ImageButton Button { get; set; }
            public TextView Label { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Button = itemView.FindViewById<ImageButton>(Resource.Id.logger_button);
                Label = itemView.FindViewById<TextView>(Resource.Id.logger_label);
            }
        }

        private class SearchFilter : SearchFilter<IncidentLogger>
        {
            private LoggerAdapter _adapter;

            public SearchFilter(LoggerAdapter adapter) => _adapter = adapter;

            protected override List<IncidentLogger> GetItemsToFilter() => _adapter._loggers;
            protected override string GetSearchStringFromItem(IncidentLogger item) => item.Name;

            protected override void SetItemsFromResults(List<IncidentLogger> items)
            {
                _adapter._loggers = items;
                _adapter.NotifyDataSetChanged();
            }
        }
    }
}