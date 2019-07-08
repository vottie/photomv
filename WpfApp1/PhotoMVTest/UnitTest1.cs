using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using photomv;

namespace PhotoMVTest
{
    [TestClass]
    public class ImageTest
    {
        [TestMethod]
        public void TestRename()
        {
            Image img = new Image("aaa", "bbb");
            string result = img.rename("GGGG");
            Assert.AreEqual(result, "GGGG_0");
        }

#if false
        [TestMethod]
        public void TestRenameIncrement()
        {
            // temporary file create
            File.Create("./GGGG_0");

            Image img = new Image("aaa", "bbb");
            string result = img.rename("GGGG");
            Assert.AreEqual(result, "GGGG_1");

            // temporary file delete
            File.Delete("./GGGG_0");
        }
#endif
        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            System.Console.WriteLine("Test Start");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            File.Delete("*.testdata");
        }
    }
}
