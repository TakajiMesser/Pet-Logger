using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using PetLogger.Droid.Helpers;
using PetLogger.Shared.Models;
using System;
using System.Collections.Generic;

namespace PetLogger.Droid.Adapters
{
    public class PetIncidentAdapter : ListAdapter<PetIncident>
    {
        public PetIncidentAdapter(Context context, List<PetIncident> petIncidents) : base(context, petIncidents) { }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_pet_incident, parent, false);
            SetUpItemViewClickEvents(view);

            var viewHolder = new ViewHolder(view);
            viewHolder.ToggleLoggerButton.Click += ToggleLoggerButton_Click;

            return viewHolder;
        }

        private void ToggleLoggerButton_Click(object sender, EventArgs e)
        {
            var position = (int)((View)sender).Tag;
            var petIncident = GetItemAt(position);

            if (petIncident.HasIncidentLogger)
            {
                petIncident.RemoveIncidentLogger();
            }
            else
            {
                petIncident.AddIncidentLogger();
            }

            NotifyItemChanged(position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ViewHolder;
            var petIncident = GetItemAt(position);

            viewHolder.ItemView.Tag = position;
            viewHolder.ToggleLoggerButton.Tag = position;

            viewHolder.Title.Text = petIncident.Title;
            viewHolder.Title.Typeface = FontHelper.GetTypeface(Context, CustomFonts.RobotoCondensedRegular);

            viewHolder.IncidentIcon.SetImageResource(petIncident.ImageResourceID);
            viewHolder.IncidentIcon.SetScaleType(ImageView.ScaleType.FitCenter);

            var incidentCount = petIncident.IncidentCount;

            if (incidentCount > 0)
            {
                viewHolder.CountSection.Visibility = ViewStates.Visible;
                viewHolder.LastSection.Visibility = ViewStates.Visible;
                viewHolder.NoneLabel.Visibility = ViewStates.Gone;

                viewHolder.CountValue.Text = incidentCount.ToString();

                var timeSince = DateTime.Now - petIncident.LastIncidentTime;

                // TODO - 5 is somewhat arbitrary here...
                if (timeSince.TotalDays >= 2)
                {
                    var nDays = (int)Math.Round(timeSince.TotalDays);

                    viewHolder.LastValue.Text = nDays.ToString();
                    viewHolder.LastLabel.Text = "Day" + (nDays == 1 ? "" : "s") + " Ago";
                }
                else if (timeSince.TotalHours >= 2)
                {
                    var nHours = (int)Math.Round(timeSince.TotalHours);

                    viewHolder.LastValue.Text = nHours.ToString();
                    viewHolder.LastLabel.Text = "Hour" + (nHours == 1 ? "" : "s") + " Ago";
                }
                else if (timeSince.TotalMinutes >= 1)
                {
                    var nMinutes = (int)Math.Round(timeSince.TotalMinutes);

                    viewHolder.LastValue.Text = nMinutes.ToString();
                    viewHolder.LastLabel.Text = "Minute" + (nMinutes == 1 ? "" : "s") + " Ago";
                }
                else
                {
                    viewHolder.LastValue.Text = "<";
                    viewHolder.LastLabel.Text = "a Minute Ago";
                }
            }
            else
            {
                viewHolder.CountSection.Visibility = ViewStates.Gone;
                viewHolder.LastSection.Visibility = ViewStates.Gone;
                viewHolder.NoneLabel.Visibility = ViewStates.Visible;
            }

            if (petIncident.HasIncidentLogger)
            {
                viewHolder.ToggleLoggerLabel.Text = "Remove Logger";
                viewHolder.ToggleLoggerButton.SetImageResource(Resource.Drawable.minus_black);
            }
            else
            {
                viewHolder.ToggleLoggerLabel.Text = "Add Logger";
                viewHolder.ToggleLoggerButton.SetImageResource(Resource.Drawable.plus_black);
            }
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Title { get; set; }
            public ImageView IncidentIcon { get; set; }
            public LinearLayout CountSection { get; set; }
            public TextView CountValue { get; set; }
            public LinearLayout LastSection { get; set; }
            public TextView LastValue { get; set; }
            public TextView LastLabel { get; set; }
            public TextView NoneLabel { get; set; }
            public TextView ToggleLoggerLabel { get; set; }
            public ImageButton ToggleLoggerButton { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Title = itemView.FindViewById<TextView>(Resource.Id.title);
                IncidentIcon = itemView.FindViewById<ImageView>(Resource.Id.incident_icon);
                CountSection = itemView.FindViewById<LinearLayout>(Resource.Id.count_section);
                CountValue = itemView.FindViewById<TextView>(Resource.Id.count_value);
                LastSection = itemView.FindViewById<LinearLayout>(Resource.Id.last_section);
                LastValue = itemView.FindViewById<TextView>(Resource.Id.last_value);
                LastLabel = itemView.FindViewById<TextView>(Resource.Id.last_label);
                NoneLabel = itemView.FindViewById<TextView>(Resource.Id.none_label);
                ToggleLoggerLabel = itemView.FindViewById<TextView>(Resource.Id.toggle_logger_label);
                ToggleLoggerButton = itemView.FindViewById<ImageButton>(Resource.Id.toggle_logger_button);
            }
        }
    }
}