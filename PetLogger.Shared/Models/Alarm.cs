using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;

namespace PetLogger.Shared.Models
{
    public class Alarm
    {
        private AlarmDefinition _definition;

        public Alarm(AlarmDefinition definition)
        {
            _definition = definition;

            var latestIncident = DBTable.GetAll<Incident>(i => i.PetID == _definition.PetID && i.IncidentTypeID == _definition.IncidentTypeID)
                .OrderByDescending(i => i.Time)
                .FirstOrDefault();

            LatestIncidentTime = latestIncident != null
                ? latestIncident.Time
                : (DateTime?)null;
        }

        public string Title => _definition.Title;
        public string Label => "PetLogger - " + _definition.Title;

        public int PetID => _definition.PetID;
        public int IncidentTypeID => _definition.IncidentTypeID;

        public DateTime? LatestIncidentTime { get; }

        public void Delete() => DBTable.Delete(_definition);
    }
}