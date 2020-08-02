using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;

namespace PetLogger.Shared.Models
{
    public class IncidentLogger
    {
        private LoggerDefinition _definition;

        public IncidentLogger(LoggerDefinition definition)
        {
            _definition = definition;
            ImageResourceID = ThemeHelper.GetImageResourceID(definition.IncidentType.ImageName, Themes.Dark);
        }

        public string Title => _definition.Title;
        public int ImageResourceID { get; }

        public int PetID => _definition.PetID;
        public int IncidentTypeID => _definition.IncidentTypeID;

        public DateTime? LatestIncidentTime
        {
            get
            {
                var latestIncident = DBTable.GetAll<Incident>(i => i.PetID == _definition.PetID && i.IncidentTypeID == _definition.IncidentTypeID)
                .OrderByDescending(i => i.Time)
                .FirstOrDefault();

                return latestIncident != null
                    ? latestIncident.Time
                    : (DateTime?)null;
            }
        }

        public void Delete() => _definition.Delete();

        public void LogIncident() => DBTable.Insert(new Incident()
        {
            Time = DateTime.Now,
            IncidentTypeID = _definition.IncidentTypeID,
            PetID = _definition.PetID
        });
    }
}