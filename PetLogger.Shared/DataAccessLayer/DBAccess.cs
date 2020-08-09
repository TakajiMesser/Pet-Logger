using PetLogger.Shared.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace PetLogger.Shared.DataAccessLayer
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
            typeof(PetType),
            typeof(IncidentType),
            typeof(Incident),
            typeof(Modifier),
            typeof(Schedule),
            typeof(LoggerDefinition),
            typeof(Reminder)
        };

        public static void InitializeDatabase()
        {
            //DBAccess.BackUpDatabase();
            DBAccess.InitializeTables();

            // TODO - Set up database values for testing purposes
            /*DBAccess.ResetTables();
            BuiltIn.InitializeBuiltInTypes();

            var dog = DBTable.Get<PetType>(p => p.Name == "Dog");
            var cat = DBTable.Get<PetType>(p => p.Name == "Cat");

            DBTable.Insert(new Pet()
            {
                Name = "Cooper",
                PetTypeID = dog.ID
            });

            DBTable.Insert(new Pet()
            {
                Name = "Kora",
                PetTypeID = cat.ID
            });

            var cooper = DBTable.Get<Pet>(p => p.Name == "Cooper");
            var pee = DBTable.Get<IncidentType>(i => i.Name == "Pee");
            var poo = DBTable.Get<IncidentType>(i => i.Name == "Poo");

            DBTable.Insert(new Incident()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                Time = DateTime.Now - TimeSpan.FromHours(27),
            });

            DBTable.Insert(new Incident()
            {
                PetID = cooper.ID,
                IncidentTypeID = poo.ID,
                Time = DateTime.Now - TimeSpan.FromHours(10),
            });

            DBTable.Insert(new LoggerDefinition()
            {
                PetID = cooper.ID,
                IncidentTypeID = pee.ID,
                IncludeDays = true,
                IncludeHours = true,
                IncludeMinutes = true,
                IncludeSeconds = true,
            });

            DBTable.Insert(new LoggerDefinition()
            {
                PetID = cooper.ID,
                IncidentTypeID = poo.ID,
                IncludeDays = true,
                IncludeHours = true,
                IncludeMinutes = true,
                IncludeSeconds = true,
            });*/
        }

        public static Type ParseTableName(string tableName)
        {
            switch (tableName)
            {
                case "Pet":
                    return typeof(Pet);
                case "PetType":
                    return typeof(PetType);
                case "IncidentType":
                    return typeof(IncidentType);
                case "Incident":
                    return typeof(Incident);
                case "Modifier":
                    return typeof(Modifier);
                case "Schedule":
                    return typeof(Schedule);
                case "LoggerDefinition":
                    return typeof(LoggerDefinition);
                case "Reminder":
                    return typeof(Reminder);
            }

            throw new ArgumentException("Could not find table name " + tableName);
        }

        public static TableMapping GetMapping<T>() => Connection.GetMapping<T>();

        public static TableMapping GetMapping(Type type) => Connection.GetMapping(type);

        public static void InitializeTables()
        {
            foreach (var type in EntityTypes)
            {
                DBTable.Create(type, false);
            }
        }

        public static void ResetTables()
        {
            foreach (var type in EntityTypes)
            {
                DBTable.Create(type, true);
            }
        }

        public static void Execute(string query) => Connection.Execute(query);

        public static List<T> Query<T>(string query) where T : class, IEntity, new() => Connection.Query<T>(query);

        public static void BackUpDatabase()
        {
            var directoryPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "PetLogger Database Backups");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Attempt to find a valid backup file name that isn't already taken
            var suffix = 1;
            var filePath = Path.Combine(directoryPath, "petlogger-backup-" + suffix + ".db");

            while (File.Exists(filePath))
            {
                suffix++;
                filePath = Path.Combine(directoryPath, "petlogger-backup-" + suffix + ".db");
            }

            File.Copy(DatabasePath, filePath);
        }

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