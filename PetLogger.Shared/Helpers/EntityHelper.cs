using PetLogger.Shared.DataAccessLayer;
using System;
using System.Reflection;

namespace PetLogger.Shared.Helpers
{
    public static class EntityHelper
    {
        public static PropertyInfo GetIdentifierProperty(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (Attribute.GetCustomAttribute(property, typeof(IdentifierAttribute)) is IdentifierAttribute)
                {
                    return property;
                }
            }

            return null;
        }
    }
}