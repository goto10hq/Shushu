using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Spatial;
using Microsoft.Azure.Search.Models;

namespace Shushu.Tokens
{
    /// <summary>
    /// Shushu index.
    /// </summary>
    [SerializePropertyNamesAsCamelCase]
    public class ShushuIndex
    {
        [Key]
        [IsFilterable]
        public string Id { get; set; }

        [IsFilterable]
        public string Entity { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text0 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text1 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text2 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text3 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text4 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text5 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text6 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text7 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text8 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text9 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text10 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text11 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text12 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text13 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text14 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text15 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text16 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text17 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text18 { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer("standardasciifolding.lucene")]
        [IsFilterable]
        public string Text19 { get; set; }

        [IsSortable]
        [IsFilterable]
        [IsFacetable]
        public DateTimeOffset? Date0 { get; set; }

        [IsSortable]
        [IsFilterable]
        [IsFacetable]
        public DateTimeOffset? Date1 { get; set; }

        [IsSortable]
        [IsFilterable]
        [IsFacetable]
        public DateTimeOffset? Date2 { get; set; }

        [IsSortable]
        [IsFilterable]
        [IsFacetable]
        public DateTimeOffset? Date3 { get; set; }

        [IsSortable]
        [IsFilterable]
        [IsFacetable]
        public DateTimeOffset? Date4 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags0 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags1 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags2 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags3 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags4 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags5 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags6 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags7 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags8 { get; set; }

        [IsSearchable]
        [IsFilterable]
        [IsFacetable]
        public List<string> Tags9 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number0 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number1 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number2 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number3 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number4 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number5 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number6 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number7 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number8 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public Int64? Number9 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double0 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double1 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double2 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double3 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double4 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double5 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double6 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double7 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double8 { get; set; }

        [IsFilterable]
        [IsSortable]
        [IsFacetable]
        public double? Double9 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag0 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag1 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag2 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag3 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag4 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag5 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag6 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag7 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag8 { get; set; }

        [IsFilterable]
        [IsFacetable]
        public bool? Flag9 { get; set; }

        [IsFilterable]
        [IsSortable]
        public GeographyPoint Point0 { get; set; }
    }
}