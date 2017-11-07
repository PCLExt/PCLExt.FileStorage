using PCLExt.FileStorage.Folders;

using NUnit.Framework;


namespace PCLExt.FileStorage.Test
{
    [TestFixture]
    public class StandardFoldersTest
    {
        [Test]
        public void FolderFromPath()
        {
            var folder = new FolderFromPath(new TestFolder().Path);
            var folder1 = new TestFolder();
            Assert.IsTrue(folder.Path == folder1.Path);

            folder.Delete();
        }

        [Test]
        public void LocalStorageFolder() =>
#if __ANDROID__ || __IOS__
            Assert.IsTrue(new LocalRootFolder().Exists);
#else
            Assert.IsTrue(new LocalRootFolder().Exists);
#endif


        [Test]
        public void RoamingStorageFolder() =>
#if __ANDROID__ || __IOS__
            Assert.IsFalse(new RoamingRootFolder().Exists);
#else
            Assert.IsTrue(new RoamingRootFolder().Exists);
#endif

        [Test]
        public void ApplicationFolder() =>
#if __ANDROID__
            Assert.IsFalse(new ApplicationRootFolder().Exists);
#else
            Assert.IsTrue(new ApplicationRootFolder().Exists);
#endif

        [Test]
        public void DocumentsFolder() =>
//#if __ANDROID__ || __IOS__
//            Assert.IsFalse(new DocumentsRootFolder().Exists);
//#else
            Assert.IsTrue(new DocumentsRootFolder().Exists);
//#endif

        [Test]
        public void TempFolder() =>
            Assert.IsTrue(new TempRootFolder().Exists);

        [Test]
        public void TempFolderCreateTemp() =>
            Assert.IsTrue(new TempRootFolder().CreateTempFile().Exists);

        [Test]
        public void TempFolderCreateTempExtension() =>
            Assert.IsTrue(new TempRootFolder().CreateTempFile("ps1").Exists);

        [Test]
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