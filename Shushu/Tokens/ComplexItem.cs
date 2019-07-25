using System;
using System.Collections.Generic;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Shushu.Attributes;

namespace Shushu.Tokens
{
    /// <summary>
    /// Complex item.
    /// </summary>
    [SerializePropertyNamesAsCamelCase]    
    public class ComplexItem
    {
        [PropertyMapping(Enums.IndexField.Text0)]
        public string Text { get; set; }
        
        [PropertyMapping(Enums.IndexField.Date0)]
        public DateTimeOffset? Date { get; set; }

        [PropertyMapping(Enums.IndexField.Tags0)]
        public IList<string> Tags { get; set; }

        [PropertyMapping(Enums.IndexField.Number0)]
        public long? Number { get; set; }

        [PropertyMapping(Enums.IndexField.Double0)]
        public double? Double { get; set; }

        [PropertyMapping(Enums.IndexField.Flag0)]
        public bool? Flag { get; set; }

        [PropertyMapping(Enums.IndexField.Point0)]
        public GeographyPoint Point { get; set; }
    }
}