using PetLogger.Shared.Data;
using System;

namespace PetLogger.Droid.Models
{
    public class HistoryItem
    {
        public const int HEADER_VIEW_TYPE = 0;
        public const int INCIDENT_VIEW_TYPE = 1;

        private HistoryItem(int viewType) => ViewType = viewType;

        public int ViewType { get; }
        public DateTime Date { get; private set; }
        public Incident Incident { get; private set; }

        public bool IsHeader => ViewType == HEADER_VIEW_TYPE;

        public static HistoryItem CreateHeaderItem(DateTime date) => new HistoryItem(HEADER_VIEW_TYPE)
        {
            Date = date
        };

        public static HistoryItem CreateIncidentItem(Incident incident) => new HistoryItem(INCIDENT_VIEW_TYPE)
        {
            Incident = incident
        };
    }
}
