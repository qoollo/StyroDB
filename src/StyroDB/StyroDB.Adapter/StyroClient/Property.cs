using System;

namespace StyroDB.Adapter.StyroClient
{
    class Property
    {
        public Property(string name, int ordinal, Type type, Object value)
        {
            Name = name;
            Ordinal = ordinal;
            TypeValue = type;
            Value = value;
        }

        public readonly string Name;
        public int Ordinal;
        public readonly Type TypeValue;
        public readonly Object Value;
    }
}