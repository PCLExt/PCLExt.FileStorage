using NUnit.Framework;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if WINDOWS_UWP
using TestFixtureAttr = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestAttr = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
using TestFixtureAttr = NUnit.Framework.TestFixtureAttribute;
using TestAttr = NUnit.Framework.TestAttribute;
#endif

#if __ANDROID__ || __IOS__
using RootFolder = PCLExt.FileStorage.Folders.LocalRootFolder;
#else
using RootFolder = PCLExt.FileStorage.Folders.ApplicationRootFolder;
#endif

namespace PCLExt.FileStorage.Test.Folder
{
    [TestFixtureAttr]
    public class FolderDeleteTest : BaseFolderTest
    {
        [TestAttr]
        public void CreateFolder() => CreateFolderCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderAsync() => await CreateFolderCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderCoreAsync(bool sync, CancellationToken cancellationToken)
        {
        }

        public void Delete()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);
        }

        public void DeleteTwice()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);

            Assert.That(() => folder.Delete(), Throws.TypeOf<FolderNotFoundException>());
        }

        public void DeleteRootStorage()
        {
            var folder = new RootFolder();

            Assert.That(() => folder.Delete(), Throws.TypeOf<RootFolderDeletionException>());
        }
    }
}
