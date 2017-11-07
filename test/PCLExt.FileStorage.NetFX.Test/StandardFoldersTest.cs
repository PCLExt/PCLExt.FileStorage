using PCLExt.FileStorage.Folders;

#if NUNIT

using NUnit.Framework;

using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace PCLExt.FileStorage.Test
{
    [TestClass]
    public class StandardFoldersTest
    {
        [TestMethod]
        public void FolderFromPath()
        {
            var folder = new FolderFromPath(new TestFolder().Path);
            var folder1 = new TestFolder();
            Assert.IsTrue(folder.Path == folder1.Path);

            folder.Delete();
        }

        [TestMethod]
        public void LocalStorageFolder() =>
#if __ANDROID__ || __IOS__
            Assert.IsTrue(new LocalRootFolder().Exists);
#else
            Assert.IsTrue(new LocalRootFolder().Exists);
#endif


        [TestMethod]
        public void RoamingStorageFolder() =>
#if __ANDROID__ || __IOS__
            Assert.IsFalse(new RoamingRootFolder().Exists);
#else
            Assert.IsTrue(new RoamingRootFolder().Exists);
#endif

        [TestMethod]
        public void ApplicationFolder() =>
#if __ANDROID__
            Assert.IsFalse(new ApplicationRootFolder().Exists);
#else
            Assert.IsTrue(new ApplicationRootFolder().Exists);
#endif

        [TestMethod]
        public void DocumentsFolder() =>
//#if __ANDROID__ || __IOS__
//            Assert.IsFalse(new DocumentsRootFolder().Exists);
//#else
            Assert.IsTrue(new DocumentsRootFolder().Exists);
//#endif

        [TestMethod]
        public void TempFolder() =>
            Assert.IsTrue(new TempRootFolder().Exists);

        [TestMethod]
        public void TempFolderCreateTemp() =>
            Assert.IsTrue(new TempRootFolder().CreateTempFile().Exists);

        [TestMethod]
        public void TempFolderCreateTempExtension() =>
            Assert.IsTrue(new TempRootFolder().CreateTempFile("ps1").Exists);

        [TestMethod]
        public void TempFolderCreateTemp1()
        {
            var t1 = new DocumentsRootFolder();
            var t2 = new ApplicationRootFolder();
            var t3 = new LocalRootFolder();
            var t4 = new RoamingRootFolder();
            var t5 = new TempRootFolder();
        }
    }
}