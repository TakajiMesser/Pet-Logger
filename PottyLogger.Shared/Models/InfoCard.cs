using PottyLogger.Shared.Data;
using PottyLogger.Shared.DataAccessLayer;
using System;

namespace PottyLogger.Shared.Models
{
    public class InfoCard
    {
        private InfoDefinition _definition;

        public InfoCard(InfoDefinition definition)
        {
            _definition = definition;
            Update();
        }

        public string Title => _definition.Title;
        public string Description => _definition.Description;

        public bool IncludeDays => _definition.IncludeDays;
        public bool IncludeHours => _definition.IncludeHours;
        public bool IncludeMinutes => _definition.IncludeMinutes;
        public bool IncludeSeconds => _definition.IncludeSeconds;

        public DateTime? LatestIncidentTime { get; private set; }

        public void Update()
        {
            var latestIncident = DBTable.GetAll<Incident>(i => i.PetID == _definition.PetID && i.IncidentTypeID == _definition.IncidentTypeID)
                .OrderByDescending(i => i.Time)
                .FirstOrDefault();

            LatestIncidentTime = latestIncident != null
                ? latestIncident.Time
                : (DateTime?)null;
        }
    }
}
