using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shushu.Attributes;
using System.IO;

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

            var s0 = new Shu("1", "nenecchi", 130);
            var s1 = new Shu("2", "umiko", 150);

            _shushu.IndexDocument(s0);
            _shushu.IndexDocument(s1);
        }

        [TestMethod]
        public void NoMappingPoco()
        {
            Assert.AreEqual(null, null);            
        }
    }
}
