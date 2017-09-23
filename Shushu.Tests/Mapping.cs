using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shushu.Attributes;
using Shushu.Tokens;
using System;
using System.Collections.Generic;

namespace Shushu.Tests
{
    [TestClass]
    public class Mapping
    {
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
            
            [PropertyMapping(Enums.IndexField.Tags7)]
            public List<string> Tags { get; set; }

            [PropertyMapping(Enums.IndexField.Double2)]
            public double Money { get; set; } = 3.14;

            [PropertyMapping(Enums.IndexField.Number0)]
            public int Iq { get; set; } = 220;

            [PropertyMapping(Enums.IndexField.Point0)]
            public GeoPoint Location { get; set; }
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
            var a = new Aoba { Id = "id-6502", IgnoreMe = false, Title = "Umiko", Tags = new List<string> { "hi", "ho" }, Location = new GeoPoint(130.56, 220.44), Clever = true };

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
            Assert.AreEqual(3.14, search.Double2);
            Assert.AreEqual(GeographyPoint.Create(130.56, 220.44), search.Point0);
        }

        [TestMethod]
        public void MappingAzureSearchDirectly()
        {
            var dt = DateTime.Now;
            var a = new AzureSearch { Id = "foo", Text0 = "bar", Date0 = dt };

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
