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
            string result = img.rename("GGGG.testdata");
            Assert.AreEqual(result, "GGGG_0.testdata");
        }


        [TestMethod]
        public void TestRenameIncrement()
        {
            // temporary file create
            File.Create("GGGG_0.testdata");

            Image img = new Image("aaa", "bbb");
            string result = img.rename("GGGG_0.testdata");
            Assert.AreEqual(result, "GGGG_1.testdata");

            // temporary file delete
            File.Delete("GGGG_0.testdata");
        }

        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            System.Console.WriteLine("Test Start");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            System.IO.FileInfo[] files =
                di.GetFiles("*.testdata", System.IO.SearchOption.AllDirectories);
        }
    }
}
