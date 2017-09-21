using System;

namespace Shushu.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ClassMapping : Attribute
    {
        public Enums.IndexField IndexField { get; }
        public object Value { get; }

        public ClassMapping(Enums.IndexField indexField, object value)
        {            
            IndexField = indexField;
            Value = value;
        }
    }
}
