using PottyLogger.Droid.Helpers;
using PottyLogger.Shared.Data;
using PottyLogger.Shared.DataAccessLayer;
using System;

namespace PottyLogger.Shared.Models
{
    public class IncidentLogger
    {
        private LoggerDefinition _definition;

        public IncidentLogger(LoggerDefinition definition)
        {
            _definition = definition;
            ImageResourceID = ImageHelper.GetImageResourceID(definition.ImageName);
        }

        public string Name => _definition.Name;
        public int ImageResourceID { get; }

        public void LogIncident() => DBTable.Insert(new Incident()
        {
            Time = DateTime.Now,
            IncidentTypeID = _definition.IncidentTypeID,
            PetID = _definition.PetID
        });
    }
}