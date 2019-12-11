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
    public class FolderGetFolderTest : BaseFolderTest
    {
        [TestAttr]
        public void CreateFolder() => CreateFolderCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderAsync() => await CreateFolderCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderCoreAsync(bool sync, CancellationToken cancellationToken)
        {
        }

        public void GetFolder()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = folder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var getFolder = folder.GetFolder(FolderName1);
            Assert.IsTrue(string.Equals(folder1.Path, getFolder.Path, System.StringComparison.Ordinal));
        }

        public void GetFolders()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = folder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var getFolders = folder.GetFolders();
            Assert.IsTrue(getFolders.Count == 2);
        }
    }
}