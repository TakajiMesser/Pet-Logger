using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Components;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Models;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class AlarmAdapter : MultiSelectListAdapter<Alarm>, IFilterable
    {
        private SearchFilter<Alarm> _filter;

        public AlarmAdapter(Context context, IList<Alarm> alarms) : base(context, alarms) =>
            _filter = CreateSearchFilter(l => l.Title);

        public Filter Filter => _filter;

        public void RemoveSelectedAlarms()
        {
            foreach (var alarm in SelectedItems)
            {
                alarm.Delete();
            }

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_alarm, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var alarm = GetItemAt(position);

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);

            viewHolder.Title.Text = alarm.Title;
            viewHolder.Title.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            /*if (logger.LatestIncidentTime.HasValue)
            {
                viewHolder.TimeSince.IncludeDays = logger.IncludeDays;
                viewHolder.TimeSince.IncludeHours = logger.IncludeHours;
                viewHolder.TimeSince.IncludeMinutes = logger.IncludeMinutes;
                viewHolder.TimeSince.IncludeSeconds = logger.IncludeSeconds;
                viewHolder.TimeSince.InitialTime = logger.LatestIncidentTime.Value;
                viewHolder.TimeSince.Start();
            }*/
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.title);
            }
        }
    }
}