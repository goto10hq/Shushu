using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shushu.Attributes;
using Shushu.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shushu.Tests
{
    [TestClass]
    public class Mapping
    {
        public enum Color
        {
            Red = 0,
            Green = 1,
            Blue = 2
        }

        [ClassMapping(Enums.IndexField.Entity, "myproject/aoba")]
        [ClassMapping(Enums.IndexField.Flag8, true)]
        public class Aoba
        {
            [PropertyMapping(Enums.IndexField.Id)]
            public string Id { get; set; }

            public bool IgnoreMe { get; set; }

            [PropertyMapping(Enums.IndexField.Flag3)]
            public bool? DoNotIgnoreMe { get; set; }

            [PropertyMapping(Enums.IndexField.Flag4)]
            public bool Clever { get; set; } = true;

            [PropertyMapping(Enums.IndexField.Text0)]
            public string Title { get; set; }

            [PropertyMapping(Enums.IndexField.Text1)]
            public string AutoTitle => Title?.ToLower() ?? "?";

            [PropertyMapping(Enums.IndexField.Tags7)]
            public List<string> Tags { get; set; }

            [PropertyMapping(Enums.IndexField.Double2)]
            public double Money { get; set; } = 3.14;

            [PropertyMapping(Enums.IndexField.Number0)]
            public int Iq { get; set; } = 220;

            [PropertyMapping(Enums.IndexField.Number1)]
            public int? RealIq { get; set; } = 120;

            [PropertyMapping(Enums.IndexField.Number2)]
            public Color Color { get; set; } = Color.Blue;

            [PropertyMapping(Enums.IndexField.Point0)]
            public GeoPoint Location { get; set; }

            [PropertyMapping(Enums.IndexField.Date0)]
            public DateTime Work { get; set; }

            [PropertyMapping(Enums.IndexField.Complex0)]
            public IList<ComplexItem> Bees { get; set; }
        }
        
        public class Umiko
        {
            public string Test { get; set; }
        }

        [ClassMapping(Enums.IndexField.Number6, 3)]
        [ClassMapping(Enums.IndexField.Number7, 4)]
        [ClassMapping(Enums.IndexField.Number6, 5)]
        public class ErrorClassPoco
        {
            public string Test { get; set; }
        }

        public class ErrorPropertyPoco
        {
            [PropertyMapping(Enums.IndexField.Number4)]
            public string Test { get; set; }

            [PropertyMapping(Enums.IndexField.Number5)]
            public string Test2 { get; set; }

            [PropertyMapping(Enums.IndexField.Number4)]
            public string Test3 { get; set; }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Object has not Id field index defined but has been accepted.")]
        public void NoMappingPoco()
        {
            var u = new Umiko { Test = "test" };
            var search = u.MapToIndex();
        }

        [TestMethod]
        public void MappingPoco()
        {
            var now = DateTime.Now;
            // TODO: ms = 0 ??
            var dt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var a = new Aoba 
            { 
                Id = "id-6502", 
                IgnoreMe = false, 
                Title = "Umiko", 
                Tags = new List<string> { "hi", "ho" }, 
                Location = new GeoPoint(130.56, 220.44), 
                Clever = true, 
                Work = dt,
                Bees = new List<ComplexItem> { new ComplexItem { Text = "Maya", Number = 100, Flag = false }, 
                    new ComplexItem { Text = "Hornet", Number = 130, Flag = true } }
            };

            var search = a.MapToIndex();

            // class mappings
            Assert.AreEqual("myproject/aoba", search.Entity);
            Assert.AreEqual(true, search.Flag8);
            Assert.AreEqual(null, search.Flag1);

            // property mappins
            Assert.AreEqual("id-6502", search.Id);
            Assert.AreEqual("Umiko", search.Text0);
            Assert.AreEqual(null, search.Flag3);
            Assert.AreEqual(true, search.Flag4);
            Assert.AreEqual(2, search.Tags7.Count);
            Assert.AreEqual("ho", search.Tags7[1]);
            Assert.AreEqual(220, search.Number0);
            Assert.AreEqual(120, search.Number1);
            Assert.AreEqual((Int64)(Int32)Color.Blue, search.Number2);
            Assert.AreEqual(3.14, search.Double2);
            Assert.AreEqual(dt, search.Date0);
            Assert.AreEqual(GeographyPoint.Create(130.56, 220.44).Latitude, search.Point0.Latitude);
            Assert.AreEqual(GeographyPoint.Create(130.56, 220.44).Longitude, search.Point0.Longitude);
            Assert.AreEqual("Maya", search.Complex0.First().Text);
            Assert.AreEqual(100, search.Complex0.First().Number);
            Assert.AreEqual(false, search.Complex0.First().Flag);
            Assert.AreEqual("Hornet", search.Complex0.Last().Text);
            Assert.AreEqual(130, search.Complex0.Last().Number);
            Assert.AreEqual(true, search.Complex0.Last().Flag);

            var a2 = search.MapFromIndex<Aoba>();

            Assert.AreEqual("id-6502", a2.Id);
            Assert.AreEqual("Umiko", a2.Title);
            Assert.AreEqual(null, a2.DoNotIgnoreMe);
            Assert.AreEqual(true, a2.Clever);
            Assert.AreEqual(2, a2.Tags.Count);
            Assert.AreEqual("ho", a2.Tags[1]);
            Assert.AreEqual(220, a2.Iq);
            Assert.AreEqual(120, a2.RealIq);
            Assert.AreEqual(3.14, a2.Money);
            Assert.AreEqual(dt, a2.Work);
            Assert.AreEqual(new GeoPoint(130.56, 220.44).Coordinates[0], a2.Location.Coordinates[0]);
            Assert.AreEqual(new GeoPoint(130.56, 220.44).Coordinates[1], a2.Location.Coordinates[1]);
            Assert.AreEqual(new GeoPoint(130.56, 220.44).Type, a2.Location.Type);
            Assert.AreEqual("Maya", a2.Bees.First().Text);
            Assert.AreEqual(100, a2.Bees.First().Number);
            Assert.AreEqual(false, a2.Bees.First().Flag);
            Assert.AreEqual("Hornet", a2.Bees.Last().Text);
            Assert.AreEqual(130, a2.Bees.Last().Number);
            Assert.AreEqual(true, a2.Bees.Last().Flag);
        }

        [TestMethod]
        public void MappingShushuIndexDirectly()
        {
            var dt = DateTime.Now;
            var a = new ShushuIndex { Id = "foo", Text0 = "bar", Date0 = dt };

            var search = a.MapToIndex();

            Assert.AreEqual(a.Id, search.Id);
            Assert.AreEqual(a.Text0, search.Text0);
            Assert.AreEqual(a.Text1, search.Text1);
            Assert.AreEqual(a.Date0, search.Date0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Duplicated class mappings were inappropriately allowed.")]
        public void TestDuplicatedClassMappings()
        {
            var poco = new ErrorClassPoco();
            var search = poco.MapToIndex();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Duplicated property mappings were inappropriately allowed.")]
        public void TestDuplicatedPropertyMappings()
        {
            var poco = new ErrorPropertyPoco();
            var search = poco.MapToIndex();
        }

        [TestMethod]
        public void TestSearchParameterMappingsForOrderBy()
        {
            var p = new SearchParameters
            {
                OrderBy = new List<string> { "@Iq", "@Location", "foo" }
            };

            p = p.MapSearchParameters<Aoba>();

            Assert.AreEqual(p.OrderBy.Count, 3);
            Assert.AreEqual("number0", p.OrderBy[0]);
            Assert.AreEqual("point0", p.OrderBy[1]);
            Assert.AreEqual("foo", p.OrderBy[2]);
        }

        [TestMethod]
        public void TestSearchParameterMappingsForSelect()
        {
            var p = new SearchParameters
            {
                Select = new List<string> { "@Iq", "@Location", "foo" }
            };

            p = p.MapSearchParameters<Aoba>();

            Assert.AreEqual(p.Select.Count, 3);
            Assert.AreEqual("number0", p.Select[0]);
            Assert.AreEqual("point0", p.Select[1]);
            Assert.AreEqual("foo", p.Select[2]);
        }

        [TestMethod]
        public void TestSearchParameterMappingsForHighlightFields()
        {
            var p = new SearchParameters
            {
                HighlightFields = new List<string> { "@Iq", "@Location", "foo" }
            };

            p = p.MapSearchParameters<Aoba>();

            Assert.AreEqual(p.HighlightFields.Count, 3);
            Assert.AreEqual("number0", p.HighlightFields[0]);
            Assert.AreEqual("point0", p.HighlightFields[1]);
            Assert.AreEqual("foo", p.HighlightFields[2]);
        }

        [TestMethod]
        public void TestSearchParameterMappingsForSearchFields()
        {
            var p = new SearchParameters
            {
                SearchFields = new List<string> { "@Iq", "@Location", "foo" }
            };

            p = p.MapSearchParameters<Aoba>();

            Assert.AreEqual(p.SearchFields.Count, 3);
            Assert.AreEqual("number0", p.SearchFields[0]);
            Assert.AreEqual("point0", p.SearchFields[1]);
            Assert.AreEqual("foo", p.SearchFields[2]);
        }

        [TestMethod]
        public void TestSearchParameterMappingsForFacets()
        {
            var p = new SearchParameters
            {
                Facets = new List<string> { "@Iq:day", "@Location", "foo" }
            };

            p = p.MapSearchParameters<Aoba>();

            Assert.AreEqual(p.Facets.Count, 3);
            Assert.AreEqual("number0:day", p.Facets[0]);
            Assert.AreEqual("point0", p.Facets[1]);
            Assert.AreEqual("foo", p.Facets[2]);
        }

        [TestMethod]
        public void TestSearchParameterMappingsForFilter()
        {
            var p = new SearchParameters
            {
                Filter = "entity eq 'something' and @Title eq 'test'",
            };

            p = p.MapSearchParameters<Aoba>();

            Assert.AreEqual("entity eq 'something' and text0 eq 'test'", p.Filter);
        }

        [TestMethod]
        public void CamelCasePropertyNameResolver()
        {
            Assert.AreEqual("foo", "foo".ToCamelCase());
            Assert.AreEqual("foo", "Foo".ToCamelCase());
            Assert.AreEqual("fooBar", "FooBar".ToCamelCase());
            Assert.AreEqual("foo_Bar", "Foo_Bar".ToCamelCase());
            Assert.AreEqual("myid", "MYID".ToCamelCase());
            Assert.AreEqual("myiDoh", "MYIDoh".ToCamelCase());
        }
    }
}