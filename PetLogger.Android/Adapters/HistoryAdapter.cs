using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Droid.Models;
using PetLogger.Shared.Data;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class HistoryAdapter : ListAdapter<HistoryItem>, IFilterable
    {
        public const string DATE_FORMAT = "MMMM dd yyyy (dddd)";
        public const string TIME_FORMAT = "{0,8:h:mm tt}";

        private DateTime? _oldestDate;

        public HistoryAdapter(Context context) : base(context) => Filter = CreateSearchFilter(i => i.IsHeader 
            ? i.Date.ToString(DATE_FORMAT)
            : i.Incident.Pet.Name + i.Incident.IncidentType.Name + string.Format(TIME_FORMAT, i.Incident.Time));

        public Filter Filter { get; }

        public void AddIncidents(IEnumerable<Incident> incidents)
        {
            foreach (var incident in incidents)
            {
                if (!_oldestDate.HasValue || _oldestDate.Value != incident.Time.Date)
                {
                    _oldestDate = incident.Time.Date;
                    AddItem(HistoryItem.CreateHeaderItem(_oldestDate.Value));
                }

                AddItem(HistoryItem.CreateIncidentItem(incident));
            }
        }

        public override int GetItemViewType(int position) => GetItemAt(position).ViewType;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view;

            if (viewType == HistoryItem.HEADER_VIEW_TYPE)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_history_header, parent, false);
            }
            else
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_history_incident, parent, false);
            }

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var item = GetItemAt(position);

            viewHolder.ItemView.Tag = position;

            if (item.IsHeader)
            {
                viewHolder.DateLabel.Text = item.Date.ToString(DATE_FORMAT);
                viewHolder.DateLabel.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);
            }
            else
            {
                viewHolder.TimeLabel.Text = string.Format(TIME_FORMAT, item.Incident.Time);
                viewHolder.TimeLabel.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

                viewHolder.PetIncidentLabel.Text = item.Incident.Pet.Name + " " + item.Incident.IncidentType.Name;
                viewHolder.PetIncidentLabel.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);
            }
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView DateLabel { get; set; }
            public TextView TimeLabel { get; set; }
            public TextView PetIncidentLabel { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                DateLabel = itemView.FindViewById<TextView>(Resource.Id.date_label);
                TimeLabel = itemView.FindViewById<TextView>(Resource.Id.time_label);
                PetIncidentLabel = itemView.FindViewById<TextView>(Resource.Id.pet_incident_label);
            }
        }
    }
}