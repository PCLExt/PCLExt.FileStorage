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
    public class FolderRenameTest : BaseFolderTest
    {
        [TestAttr]
        public void CreateFolder() => CreateFolderCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderAsync() => await CreateFolderCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderCoreAsync(bool sync, CancellationToken cancellationToken)
        {
        }

        public void Rename()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFolder = folder.Rename(FolderName2);
                Assert.IsFalse(folder.Exists);
                Assert.IsTrue(newFolder.Exists);
                Assert.IsFalse(string.Equals(folder.Path, newFolder.Path, System.StringComparison.Ordinal));
                newFolder.Delete();
            }
            catch
            {
                folder.Delete();

                throw;
            }
        }

        public void RenameAlreadyExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = new TestFolder().CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            Assert.That(() => folder.Rename(FolderName2), Throws.TypeOf<FolderExistException>());

            folder.Delete();
            folder2.Delete();
        }
    }
}