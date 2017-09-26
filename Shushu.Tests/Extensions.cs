using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shushu.Tests
{
    [TestClass]
    public class Extensions
    {
        [TestMethod]        
        public void NoMappingPoco()
        {
            string n = null;

            Assert.AreEqual(string.Empty, n.ToEscapedSearchString());
            Assert.AreEqual("yo \\* \\~ \\*", "yo * ~ *".ToEscapedSearchString());
        }
    }
}
