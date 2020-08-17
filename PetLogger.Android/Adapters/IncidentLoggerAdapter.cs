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
    public class IncidentLoggerAdapter : MultiSelectListAdapter<IncidentLogger>, IFilterable
    {
        public IncidentLoggerAdapter(Context context, IList<IncidentLogger> loggers) : base(context, loggers) => Filter = CreateSearchFilter(l => l.Title);

        public Filter Filter { get; }

        public event EventHandler<LoggerEventArgs> IncidentLogged;

        public void RemoveSelectedLoggers()
        {
            foreach (var logger in SelectedItems)
            {
                logger.Delete();
            }

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_incident_logger, parent, false);
            SetUpItemViewClickEvents(view);

            var viewHolder = new ViewHolder(view);
            viewHolder.LogButton.Click += LogButton_Click;

            return viewHolder;
        }

        private void LogButton_Click(object sender, EventArgs e)
        {
            var position = (int)((View)sender).Tag;
            var logger = GetItemAt(position);

            logger.LogIncident();

            // TODO - Why doesn't NotifyItemChanged() refresh the given item here?
            NotifyItemChanged(position);
            //NotifyDataSetChanged();

            IncidentLogged?.Invoke(this, new LoggerEventArgs(logger));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var logger = GetItemAt(position);

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);
            viewHolder.LogButton.Tag = position;

            viewHolder.Title.Text = logger.Title;
            viewHolder.Title.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            viewHolder.LogButton.SetImageResource(logger.ImageResourceID);
            viewHolder.LogButton.SetScaleType(ImageView.ScaleType.FitCenter);

            if (logger.LatestIncidentTime.HasValue)
            {
                viewHolder.TimeSince.CountDirection = LiveDurationView.CountDirections.Up;
                viewHolder.TimeSince.Time = logger.LatestIncidentTime.Value;
                viewHolder.TimeSince.Start();
            }
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }
            public FloatingActionButton LogButton {get;set;}
            public LiveDurationView TimeSince { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.title);
                LogButton = itemView.FindViewById<FloatingActionButton>(Resource.Id.fam_log_button);
                TimeSince = itemView.FindViewById<LiveDurationView>(Resource.Id.time_since);
            }
        }

        public class LoggerEventArgs : EventArgs
        {
            public IncidentLogger Logger { get; }

            public LoggerEventArgs(IncidentLogger logger) => Logger = logger;
        }
    }
}