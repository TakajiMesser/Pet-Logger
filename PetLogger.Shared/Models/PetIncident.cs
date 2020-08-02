using PetLogger.Droid.Helpers;
using PetLogger.Shared.Data;
using PetLogger.Shared.DataAccessLayer;
using System;
using System.Linq;

namespace PetLogger.Shared.Models
{
    public class PetIncident
    {
        private Pet _pet;
        private IncidentType _incidentType;

        public PetIncident(Pet pet, IncidentType incidentType)
        {
            _pet = pet;
            _incidentType = incidentType;
            ImageResourceID = ThemeHelper.GetImageResourceID(incidentType.ImageName, Themes.Light);
        }

        public string Title => _pet.Name + " " + _incidentType.Name;
        public int ImageResourceID { get; }

        public int IncidentCount => DBTable.GetAll<Incident>()
            .Where(i => i.PetID == _pet.ID && i.IncidentTypeID == _incidentType.ID)
            .Count();

        public DateTime LastIncidentTime => DBTable.GetAll<Incident>()
            .Where(i => i.PetID == _pet.ID && i.IncidentTypeID == _incidentType.ID)
            .OrderBy(i => i.Time)
            .Select(i => i.Time)
            .First();

        public bool HasIncidentLogger => DBTable.Any<LoggerDefinition>(l => l.PetID == _pet.ID && l.IncidentTypeID == _incidentType.ID);

        public void AddIncidentLogger() => new LoggerDefinition()
        {
            PetID = _pet.ID,
            IncidentTypeID = _incidentType.ID
        }.Insert();

        public void RemoveIncidentLogger() => DBTable.DeleteFirst<LoggerDefinition>(l => l.PetID == _pet.ID && l.IncidentTypeID == _incidentType.ID);

        /*public TextView Title { get; set; }
        public ImageView IncidentIcon { get; set; }
        public LinearLayout CountSection { get; set; }
        public TextView CountValue { get; set; }
        public LinearLayout LastSection { get; set; }
        public TextView LastValue { get; set; }
        public TextView NoneLabel { get; set; }
        public TextView ToggleLoggerLabel { get; set; }
        public ImageButton ToggleLoggerButton { get; set; }*/
    }
}