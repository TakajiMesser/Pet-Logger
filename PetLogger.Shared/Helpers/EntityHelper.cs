using Android.App;
using Android.Content;
using Android.Support.V7.Preferences;
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
                if (Attribute.GetCustomAttribute(property, typeof(IdentifierAttribute)) is IdentifierAttribute identifierAttribute)
                {
                    return property;
                }
            }

            return null;
        }
    }
}