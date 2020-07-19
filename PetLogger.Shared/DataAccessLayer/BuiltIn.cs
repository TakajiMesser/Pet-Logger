using PetLogger.Shared.Data;

namespace PetLogger.Shared.DataAccessLayer
{
    public static class BuiltIn
    {
        private readonly static string[] PET_TYPES = new []
        {
            "Dog",
            "Cat"
        };

        private readonly static string[] INCIDENT_TYPES = new[]
        {
            "Pee",
            "Poo",
            "Bath"
        };

        private readonly static string[] MODIFIERS = new[]
        {
            "Inside",
            "Outside",
            "Day",
            "Night",
            "Correct",
            "Incorrect"
        };

        public static void InitializeBuiltInTypes()
        {
            foreach (var petType in PET_TYPES)
            {
                InsertPetType(petType);
            }

            foreach (var incidentType in INCIDENT_TYPES)
            {
                InsertIncidentType(incidentType);
            }

            foreach (var modifier in MODIFIERS)
            {
                InsertModifier(modifier);
            }
        }

        private static void InsertPetType(string name)
        {
            if (DBTable.GetFirstOrDefault<PetType>(p => p.Name == name) == null)
            {
                DBTable.Insert(new PetType()
                {
                    Name = name
                });
            }
        }

        private static void InsertIncidentType(string name)
        {
            if (DBTable.GetFirstOrDefault<IncidentType>(p => p.Name == name) == null)
            {
                DBTable.Insert(new IncidentType()
                {
                    Name = name,
                    ImageName = GetImageNameForIncidentType(name)
                });
            }
        }

        private static void InsertModifier(string name)
        {
            if (DBTable.GetFirstOrDefault<Modifier>(m => m.Name == name) == null)
            {
                DBTable.Insert(new Modifier()
                {
                    Name = name
                });
            }
        }

        private static string GetImageNameForIncidentType(string name)
        {
            switch (name)
            {
                case "Pee":
                    return "umbrella";
                case "Poo":
                    return "wc";
                case "Bath":
                    return "bath";
            }

            return "";
        }
    }
}