using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class HistoryAdapter : ListAdapter<Incident>, IFilterable
    {
        public HistoryAdapter(Context context, IList<Incident> incidents) : base(context, incidents) => Filter = CreateSearchFilter(i => i.Pet.Name + " " + i.IncidentType.Name);

        public Filter Filter { get; }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_history, parent, false);
            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var incident = GetItemAt(position);

            viewHolder.ItemView.Tag = position;

            viewHolder.TimeLabel.Text = incident.Time.ToString();
            viewHolder.TimeLabel.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            viewHolder.PetIncidentLabel.Text = incident.Pet.Name + " " + incident.IncidentType.Name;
            viewHolder.PetIncidentLabel.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView TimeLabel { get; set; }
            public TextView PetIncidentLabel { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                TimeLabel = itemView.FindViewById<TextView>(Resource.Id.time_label);
                PetIncidentLabel = itemView.FindViewById<TextView>(Resource.Id.pet_incident_label);
            }
        }
    }
}