using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class ReminderAdapter : MultiSelectListAdapter<Reminder>, IFilterable
    {
        private SearchFilter<Reminder> _filter;

        public ReminderAdapter(Context context, IList<Reminder> reminders) : base(context, reminders) =>
            _filter = CreateSearchFilter(l => l.Title);

        public Filter Filter => _filter;

        public void RemoveSelectedAlarms()
        {
            foreach (var reminder in SelectedItems)
            {
                reminder.Delete();
            }

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_reminder, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var reminder = GetItemAt(position);

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);

            viewHolder.Title.Text = reminder.Title;
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