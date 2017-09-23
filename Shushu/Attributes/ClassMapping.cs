using System;

namespace Shushu.Attributes
{
    /// <summary>
    /// Class mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ClassMapping : Attribute
    {
        /// <summary>
        /// Gets the index field.
        /// </summary>
        /// <value>The index field.</value>
        public Enums.IndexField IndexField { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Shushu.Attributes.ClassMapping"/> class.
        /// </summary>
        /// <param name="indexField">Index field.</param>
        /// <param name="value">Value.</param>
        public ClassMapping(Enums.IndexField indexField, object value)
        {            
            IndexField = indexField;
            Value = value;
        }
    }
}
