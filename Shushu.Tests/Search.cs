using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shushu.Attributes;
using System.IO;
using System.Collections.Generic;
using Microsoft.Azure.Search.Models;

namespace Shushu.Tests
{
    [TestClass]
    public class Search
    {
        Shushu _shushu;

        [ClassMapping(Enums.IndexField.Entity, "shushu")]        
        public class Shu
        {
            [PropertyMapping(Enums.IndexField.Id)]
            public string Id { get; set; }

            [PropertyMapping(Enums.IndexField.Text0)]
            public string Name { get; set; }

            [PropertyMapping(Enums.IndexField.Number0)]
            public int Iq { get; set; }

            public Shu()
            {
            }

            public Shu(string id, string name, int iq)
            {
                Id = id;
                Name = name;
                Iq = iq;
            }
        }

        [TestInitialize]
        public void Init()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .AddUserSecrets("shushu")
                .AddEnvironmentVariables()
                .Build();

            _shushu = new Shushu(appSettings["Shushu:Name"], appSettings["Shushu:ServiceApiKey"], appSettings["Shushu:SearchApiKey"], appSettings["Shushu:Index"]);

            var s0 = new Shu("1", "nene", 130);
            var s1 = new Shu("2", "umiko", 150);

            _shushu.IndexDocument(s0);
            _shushu.IndexDocument(s1);

            var shushus = new List<Shu>
            {
                new Shu("3", "aoba", 140),
                new Shu("4", "momiji", 110),
                new Shu("5", "hifumi", 115),
            };

            _shushu.IndexDocuments(shushus);
        }

        [TestMethod]
        public void CountAllDocuments()
        {
            var n = _shushu.CountAllDocuments();
            Assert.AreEqual(5, n);
        }

        [TestMethod]
        public void GetDocument()
        {
            var shu = _shushu.GetDocument<Shu>("3");

            Assert.AreEqual("3", shu.Id);
            Assert.AreEqual("aoba", shu.Name);
            Assert.AreEqual(140, shu.Iq);
        }

        [TestMethod]
        public void SearchDocumentsSimpleWithOrderBy()
        {
            var sp = new SearchParameters
            {
                Filter = "@Entity eq 'shushu'",
                Top = 1,
                OrderBy = new List<string> { "@Iq" }
            };

            var result = _shushu.SearchDocuments<Shu>("*", sp);

            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(4, result.Results[0].Document.Id);
        }

        [TestMethod]
        public void SearchDocumentsSimpleWithOrderByDesc()
        {
            var sp = new SearchParameters
            {
                Filter = "@Entity eq 'shushu'",
                Top = 2,
                OrderBy = new List<string> { "@Iq desc" }
            };

            var result = _shushu.SearchDocuments<Shu>("*", sp);

            Assert.AreEqual(2, result.Results.Count);
            Assert.AreEqual(2, result.Results[0].Document.Id);
            Assert.AreEqual(3, result.Results[1].Document.Id);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //_shushu.DeleteIndex();
        }
    }
}
