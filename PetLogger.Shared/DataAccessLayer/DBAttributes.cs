using System;

namespace PetLogger.Shared.DataAccessLayer
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IdentifierAttribute : Attribute
    {
        public IdentifierAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        public Type Type { get; }

        public ForeignKeyAttribute(Type type) => Type = type;
    }
}