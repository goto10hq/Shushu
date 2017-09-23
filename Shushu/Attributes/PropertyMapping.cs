using System;

namespace Shushu.Attributes
{
    /// <summary>
    /// Property mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyMapping : Attribute
    {
        /// <summary>
        /// Gets the index field.
        /// </summary>
        /// <value>The index field.</value>
        public Enums.IndexField IndexField { get; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Shushu.Attributes.PropertyMapping"/> class.
        /// </summary>
        /// <param name="indexField">Index field.</param>
        public PropertyMapping(Enums.IndexField indexField)
        {
            IndexField = indexField;
        }
    }
}
