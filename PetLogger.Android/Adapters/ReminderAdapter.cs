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
        public ReminderAdapter(Context context, IList<Reminder> reminders) : base(context, reminders) => Filter = CreateSearchFilter(l => l.Title);

        public Filter Filter { get; }

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
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_reminder, parent, false);
            SetUpItemViewClickEvents(view);

            return new ViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var reminder = GetItemAt(position);

            viewHolder.ItemView.Tag = position;
            viewHolder.ItemView.Selected = IsSelected(position);

            viewHolder.ReminderLabel.Text = reminder.Title;
            viewHolder.ReminderLabel.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView ReminderLabel { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                ReminderLabel = itemView.FindViewById<TextView>(Resource.Id.reminder_label);
            }
        }
    }
}