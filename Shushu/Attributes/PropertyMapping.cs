using System;

namespace Shushu.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyMapping : Attribute
    {
        public Enums.IndexField IndexField { get; }
        public string Property { get; set; }

        public PropertyMapping(Enums.IndexField indexField)
        {
            IndexField = indexField;
        }
    }
}
