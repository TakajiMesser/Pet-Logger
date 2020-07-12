using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PottyLogger.Droid.Components;
using PottyLogger.Droid.Helpers;
using PottyLogger.Shared.Models;
using System;
using System.Collections.Generic;

namespace PottyLogger.Droid.Adapters
{
    public class InfoCardAdapter : MultiSelectAdapter<InfoCard>, IFilterable
    {
        public Filter Filter => _filter;
        public override int ItemCount => _cards.Count;
        public IEnumerable<InfoCard> SelectedItems
        {
            get
            {
                foreach (var position in SelectedPositions)
                {
                    yield return _cards[position];
                }
            }
        }

        private List<InfoCard> _cards;
        private SearchFilter _filter;

        public InfoCardAdapter(Context context, List<InfoCard> cards) : base(context)
        {
            _cards = cards;
            _filter = new SearchFilter(this);
        }

        protected override InfoCard GetItemAt(int position) => _cards[position];

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
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_info, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var infoCard = _cards[position];

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);

            viewHolder.Title.Text = infoCard.Title;
            viewHolder.Title.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            viewHolder.Description.Text = infoCard.Description;
            viewHolder.Description.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            if (infoCard.LatestIncidentTime.HasValue)
            {
                viewHolder.TimeSince.IncludeDays = infoCard.IncludeDays;
                viewHolder.TimeSince.IncludeHours = infoCard.IncludeHours;
                viewHolder.TimeSince.IncludeMinutes = infoCard.IncludeMinutes;
                viewHolder.TimeSince.IncludeSeconds = infoCard.IncludeSeconds;
                viewHolder.TimeSince.TimeSpan = DateTime.Now - infoCard.LatestIncidentTime.Value;
            }
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }
            public TextView Description { get; set; }
            public TimeSpanView TimeSince { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.title);
                Description = itemView.FindViewById<TextView>(Resource.Id.description);
                TimeSince = itemView.FindViewById<TimeSpanView>(Resource.Id.time_since);
            }
        }

        private class SearchFilter : SearchFilter<InfoCard>
        {
            private InfoCardAdapter _adapter;

            public SearchFilter(InfoCardAdapter adapter) => _adapter = adapter;

            protected override List<InfoCard> GetItemsToFilter() => _adapter._cards;
            protected override string GetSearchStringFromItem(InfoCard item) => item.Title;

            protected override void SetItemsFromResults(List<InfoCard> items)
            {
                _adapter._cards = items;
                _adapter.NotifyDataSetChanged();
            }
        }
    }
}