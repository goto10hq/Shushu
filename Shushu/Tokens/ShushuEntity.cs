using System;
using System.Collections.Generic;
using Microsoft.Spatial;
using Microsoft.Azure.Search.Models;
using Shushu.Attributes;

namespace Shushu.Tokens
{
    /// <summary>
    /// Shushu entity.
    /// </summary>
    [SerializePropertyNamesAsCamelCase]
    public class ShushuEntity
    {
        [PropertyMapping(Enums.IndexField.Id)]
        public string Id { get; set; }

        [PropertyMapping(Enums.IndexField.Entity)]
        public string Entity { get; set; }

        [PropertyMapping(Enums.IndexField.Text0)]
        public string Text0 { get; set; }

        [PropertyMapping(Enums.IndexField.Text1)]
        public string Text1 { get; set; }

        [PropertyMapping(Enums.IndexField.Text2)]
        public string Text2 { get; set; }

        [PropertyMapping(Enums.IndexField.Text3)]
        public string Text3 { get; set; }

        [PropertyMapping(Enums.IndexField.Text4)]
        public string Text4 { get; set; }

        [PropertyMapping(Enums.IndexField.Text5)]
        public string Text5 { get; set; }

        [PropertyMapping(Enums.IndexField.Text6)]
        public string Text6 { get; set; }

        [PropertyMapping(Enums.IndexField.Text7)]
        public string Text7 { get; set; }

        [PropertyMapping(Enums.IndexField.Text8)]
        public string Text8 { get; set; }

        [PropertyMapping(Enums.IndexField.Text9)]
        public string Text9 { get; set; }

        [PropertyMapping(Enums.IndexField.Text10)]
        public string Text10 { get; set; }

        [PropertyMapping(Enums.IndexField.Text11)]
        public string Text11 { get; set; }

        [PropertyMapping(Enums.IndexField.Text12)]
        public string Text12 { get; set; }

        [PropertyMapping(Enums.IndexField.Text13)]
        public string Text13 { get; set; }

        [PropertyMapping(Enums.IndexField.Text14)]
        public string Text14 { get; set; }

        [PropertyMapping(Enums.IndexField.Text15)]
        public string Text15 { get; set; }

        [PropertyMapping(Enums.IndexField.Text16)]
        public string Text16 { get; set; }

        [PropertyMapping(Enums.IndexField.Text17)]
        public string Text17 { get; set; }

        [PropertyMapping(Enums.IndexField.Text18)]
        public string Text18 { get; set; }

        [PropertyMapping(Enums.IndexField.Text19)]
        public string Text19 { get; set; }

        [PropertyMapping(Enums.IndexField.Date0)]
        public DateTimeOffset? Date0 { get; set; }

        [PropertyMapping(Enums.IndexField.Date1)]
        public DateTimeOffset? Date1 { get; set; }

        [PropertyMapping(Enums.IndexField.Date2)]
        public DateTimeOffset? Date2 { get; set; }

        [PropertyMapping(Enums.IndexField.Date3)]
        public DateTimeOffset? Date3 { get; set; }

        [PropertyMapping(Enums.IndexField.Date4)]
        public DateTimeOffset? Date4 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags0)]
        public IList<string> Tags0 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags1)]
        public IList<string> Tags1 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags2)]
        public IList<string> Tags2 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags3)]
        public IList<string> Tags3 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags4)]
        public IList<string> Tags4 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags5)]
        public IList<string> Tags5 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags6)]
        public IList<string> Tags6 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags7)]
        public IList<string> Tags7 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags8)]
        public IList<string> Tags8 { get; set; }

        [PropertyMapping(Enums.IndexField.Tags9)]
        public IList<string> Tags9 { get; set; }

        [PropertyMapping(Enums.IndexField.Number0)]
        public long? Number0 { get; set; }

        [PropertyMapping(Enums.IndexField.Number1)]
        public long? Number1 { get; set; }

        [PropertyMapping(Enums.IndexField.Number2)]
        public long? Number2 { get; set; }

        [PropertyMapping(Enums.IndexField.Number3)]
        public long? Number3 { get; set; }

        [PropertyMapping(Enums.IndexField.Number4)]
        public long? Number4 { get; set; }

        [PropertyMapping(Enums.IndexField.Number5)]
        public long? Number5 { get; set; }

        [PropertyMapping(Enums.IndexField.Number6)]
        public long? Number6 { get; set; }

        [PropertyMapping(Enums.IndexField.Number7)]
        public long? Number7 { get; set; }

        [PropertyMapping(Enums.IndexField.Number8)]
        public long? Number8 { get; set; }

        [PropertyMapping(Enums.IndexField.Number9)]
        public long? Number9 { get; set; }

        [PropertyMapping(Enums.IndexField.Double0)]
        public double? Double0 { get; set; }

        [PropertyMapping(Enums.IndexField.Double1)]
        public double? Double1 { get; set; }

        [PropertyMapping(Enums.IndexField.Double2)]
        public double? Double2 { get; set; }

        [PropertyMapping(Enums.IndexField.Double3)]
        public double? Double3 { get; set; }

        [PropertyMapping(Enums.IndexField.Double4)]
        public double? Double4 { get; set; }

        [PropertyMapping(Enums.IndexField.Double5)]
        public double? Double5 { get; set; }

        [PropertyMapping(Enums.IndexField.Double6)]
        public double? Double6 { get; set; }

        [PropertyMapping(Enums.IndexField.Double7)]
        public double? Double7 { get; set; }

        [PropertyMapping(Enums.IndexField.Double8)]
        public double? Double8 { get; set; }

        [PropertyMapping(Enums.IndexField.Double9)]
        public double? Double9 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag0)]
        public bool? Flag0 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag1)]
        public bool? Flag1 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag2)]
        public bool? Flag2 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag3)]
        public bool? Flag3 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag4)]
        public bool? Flag4 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag5)]
        public bool? Flag5 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag6)]
        public bool? Flag6 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag7)]
        public bool? Flag7 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag8)]
        public bool? Flag8 { get; set; }

        [PropertyMapping(Enums.IndexField.Flag9)]
        public bool? Flag9 { get; set; }

        [PropertyMapping(Enums.IndexField.Point0)]
        public GeographyPoint Point0 { get; set; }
    }
}