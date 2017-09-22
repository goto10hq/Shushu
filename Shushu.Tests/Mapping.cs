using Microsoft.Spatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shushu;
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

            [PropertyMapping(Enums.IndexField.Decimal2)]
            public decimal Money { get; set; } = 3.14m;

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
        public void NoMappingPoco()
        {
            var u = new Umiko { Test = "test" };
            var search = u.MapToSearch();

            Assert.AreEqual(null, search.Entity);
            Assert.AreEqual(null, search.Text0);
        }

        [TestMethod]
        public void MappingPoco()
        {
            var a = new Aoba { Id = "id-6502", IgnoreMe = false, Title = "Umiko", Tags = new List<string> { "hi", "ho" }, Location = new GeoPoint(130.56, 220.44), Clever = true };

            var search = a.MapToSearch();

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
            Assert.AreEqual(3.14m, search.Decimal2);
            Assert.AreEqual(GeographyPoint.Create(130.56, 220.44), search.Point0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Duplicated class mappings were inappropriately allowed.")]
        public void TestDuplicatedClassMappings()
        {
            var poco = new ErrorClassPoco();
            var search = poco.MapToSearch();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Duplicated property mappings were inappropriately allowed.")]
        public void TestDuplicatedPropertyMappings()
        {
            var poco = new ErrorPropertyPoco();
            var search = poco.MapToSearch();
        }
    }
}
