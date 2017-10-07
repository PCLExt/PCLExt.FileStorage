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
#if __ANDROID__
            Assert.IsTrue(new LocalStorageFolder().Exists);
#else
            Assert.IsTrue(new LocalStorageFolder().Exists);
#endif


        [TestMethod]
        public void RoamingStorageFolder() =>
#if __ANDROID__
            Assert.IsFalse(new RoamingStorageFolder().Exists);
#else
            Assert.IsTrue(new RoamingStorageFolder().Exists);
#endif

        [TestMethod]
        public void ApplicationFolder() =>
#if __ANDROID__
            Assert.IsFalse(new ApplicationFolder().Exists);
#else
            Assert.IsTrue(new ApplicationFolder().Exists);
#endif
    }
}