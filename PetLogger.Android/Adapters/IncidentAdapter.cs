using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Components;
using PetLogger.Shared.Data;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class IncidentAdapter : MultiSelectListAdapter<Incident>
    {
        public IncidentAdapter(Context context, List<Incident> loggers) : base(context, loggers) { }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_logger, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var incident = GetItemAt(position);
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }
            public FloatingActionButton LogButton {get;set;}
            public CountDownView TimeSince { get; set; }
            public ImageButton AddScheduleButton { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.title);
                LogButton = itemView.FindViewById<FloatingActionButton>(Resource.Id.fam_log_button);
                TimeSince = itemView.FindViewById<CountDownView>(Resource.Id.time_since);
                AddScheduleButton = itemView.FindViewById<ImageButton>(Resource.Id.button_add_schedule);
            }
        }
    }
}