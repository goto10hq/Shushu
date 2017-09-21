using System;

namespace Shushu.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldMapping : Attribute
    {
        public string Field { get; }

        public FieldMapping(string field)
        {
            Field = field;
        }
    }
}
