﻿using PetLogger.Shared.DataAccessLayer;
using SQLite;
using System;

namespace PetLogger.Shared.Data
{
    public class Incident : Entity
    {
        [Identifier]
        public DateTime Time { get; set; }

        [ForeignKey(typeof(Pet))]
        public int PetID { get; set; }

        [ForeignKey(typeof(IncidentType))]
        public int IncidentTypeID { get; set; }

        [Ignore]
        public Pet Pet => DBTable.Get<Pet>(PetID);

        [Ignore]
        public IncidentType IncidentType => DBTable.Get<IncidentType>(IncidentTypeID);
    }
}
