using PottyLogger.Shared.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace PottyLogger.Shared.DataAccessLayer
{
    public static class DBAccess
    {
        // Change with sqlite3_limit(db,SQLITE_LIMIT_VARIABLE_NUMBER,size)
        public const int SQLITE_LIMIT_VARIABLE_NUMBER = 999;

        public static string DatabasePath => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "petlogger.db");

        private static SQLiteConnection _connection = null;
        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(DatabasePath);
                }

                return _connection;
            }
        }

        public static int LastInsertRowID => (int)SQLite3.LastInsertRowid(Connection.Handle);

        public static List<Type> EntityTypes => new List<Type>
        {
            typeof(Pet),
            typeof(IncidentType),
            typeof(Incident),
            typeof(InfoDefinition),
            typeof(LoggerDefinition)
        };

        public static Type ParseTableName(string tableName)
        {
            switch (tableName)
            {
                case "Pet":
                    return typeof(Pet);
                case "IncidentType":
                    return typeof(IncidentType);
                case "Incident":
                    return typeof(Incident);
                case "InfoDefinition":
                    return typeof(InfoDefinition);
                case "LoggerDefinition":
                    return typeof(LoggerDefinition);
            }

            throw new ArgumentException("Could not find table name " + tableName);
        }

        public static TableMapping GetMapping<T>() => Connection.GetMapping<T>();

        public static TableMapping GetMapping(Type type) => Connection.GetMapping(type);

        public static void InitializeTables()
        {
            DBTable.Create<Pet>(false);
            DBTable.Create<IncidentType>(false);
            DBTable.Create<Incident>(false);
            DBTable.Create<InfoDefinition>(false);
            DBTable.Create<LoggerDefinition>(false);
        }

        public static void ResetTables()
        {
            DBTable.Create<Pet>(true);
            DBTable.Create<IncidentType>(true);
            DBTable.Create<Incident>(true);
            DBTable.Create<InfoDefinition>(true);
            DBTable.Create<LoggerDefinition>(true);
        }

        public static void Execute(string query) => Connection.Execute(query);

        public static TimeSpan? GetTimeSinceLastIncident(string petName, string incidentName)
        {
            var pet = DBTable.GetFirstOrDefault<Pet>(p => p.Name == petName);
            if (pet == null) throw new KeyNotFoundException("No record found for pet with name " + petName);

            var incidentType = DBTable.GetFirstOrDefault<IncidentType>(i => i.Name == incidentName);
            if (incidentType == null) throw new KeyNotFoundException("No record found for incident type with name " + incidentName);

            var incident = DBTable.GetAll<Incident>(i => i.PetID == pet.ID && i.IncidentTypeID == incidentType.ID)
                .OrderByDescending(i => i.Time)
                .FirstOrDefault();

            return incident != null
                ? DateTime.Now - incident.Time
                : (TimeSpan?)null;
        }
    }
}