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

namespace PCLExt.FileStorage.Test.Folder
{
    [TestFixtureAttr]
    public class FolderCreateTest : BaseFolderTest
    {
        [TestAttr]
        public void CreateFolder() => CreateFolderCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderAsync() => await CreateFolderCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var folder = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken);
            Assert.IsTrue(folder.Exists);
        }

        [TestAttr]
        public void CreateFolderTwiceOpenIfExists() => CreateFolderTwiceOpenIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderTwiceOpenIfExistsAsync() => await CreateFolderTwiceOpenIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderTwiceOpenIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var folder = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken);
            _ = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.OpenIfExists, cancellationToken);
            Assert.IsTrue(folder.Exists);
        }

        [TestAttr]
        public void CreateFolderTwiceGenerateUniqueName() => CreateFolderTwiceGenerateUniqueNameCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderTwiceGenerateUniqueNameAsync() => await CreateFolderTwiceGenerateUniqueNameCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderTwiceGenerateUniqueNameCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var folder = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken);

            IFolder? folder1 = null;
            try
            {
                folder1 = sync
                    ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.GenerateUniqueName)
                    : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.GenerateUniqueName, cancellationToken);
                Assert.IsTrue(folder.Exists);
                Assert.IsTrue(folder1.Exists);
                Assert.IsFalse(string.Equals(folder.Path, folder1.Path, StringComparison.Ordinal));
            }
            finally
            {
                folder1?.Delete();
            }
        }

        [TestAttr]
        public void CreateFolderTwiceReplaceExisting() => CreateFolderTwiceReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderTwiceReplaceExistingAsync() => await CreateFolderTwiceReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderTwiceReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var folder = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken);
            var folder1 = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.ReplaceExisting)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.ReplaceExisting, cancellationToken);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(string.Equals(folder.Path, folder1.Path, StringComparison.Ordinal));
        }

        [TestAttr]
        public void CreateFolderTwiceFailIfExists() => CreateFolderTwiceFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderTwiceFailIfExistsAsync() => await CreateFolderTwiceFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderTwiceFailIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var folder = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FolderExistException>());
            else
                Assert.That(() => TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken), Throws.TypeOf<FolderExistException>());

            Assert.IsTrue(folder.Exists);
        }

        [TestAttr]
        public void CreateFolderTwiceUnknown() => CreateFolderTwiceUnknownCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderTwiceUnknownAsync() => await CreateFolderTwiceUnknownCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderTwiceUnknownCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var folder = sync
                ? TestFolder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFolderAsync(FolderName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if(sync)
                Assert.That(() => TestFolder.CreateFolder(FolderName1, (CreationCollisionOption) 4), Throws.ArgumentException);
            else
                Assert.That(() => TestFolder.CreateFolderAsync(FolderName1, (CreationCollisionOption) 4, cancellationToken), Throws.ArgumentException);

            Assert.IsTrue(folder.Exists);
        }

        [TestAttr]
        public void CreateFile() => CreateFileCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFileAsync() => await CreateFileCoreAsync(false, CancellationToken.None);
        private async Task CreateFileCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            Assert.IsTrue(file.Exists);
        }

        [TestAttr]
        public void CreateFileTwiceOpenIfExists() => CreateFileTwiceOpenIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFileTwiceOpenIfExistsAsync() => await CreateFileTwiceOpenIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CreateFileTwiceOpenIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            _ = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);
            Assert.IsTrue(file.Exists);
        }

        [TestAttr]
        public void CreateFileTwiceReplaceExisting() => CreateFileTwiceReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFileTwiceReplaceExistingAsync() => await CreateFileTwiceReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task CreateFileTwiceReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            _ = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);
            Assert.IsTrue(file.Exists);
        }

        [TestAttr]
        public void CreateFileTwiceGenerateUniqueName() => CreateFileTwiceGenerateUniqueNameCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFileTwiceGenerateUniqueNameAsync() => await CreateFileTwiceGenerateUniqueNameCoreAsync(false, CancellationToken.None);
        private async Task CreateFileTwiceGenerateUniqueNameCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            var newFile = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.GenerateUniqueName, cancellationToken);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsFalse(string.Equals(file.Path, newFile.Path, StringComparison.Ordinal));
        }

        [TestAttr]
        public void CreateFileTwiceFailIfExists() => CreateFileTwiceFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFileTwiceFailIfExistsAsync() => await CreateFileTwiceFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CreateFileTwiceFailIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            else
                Assert.That(() => TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken), Throws.TypeOf<FileExistException>());

            Assert.IsTrue(file.Exists);
        }

        [TestAttr]
        public void CreateFileTwiceUnknown() => CreateFileTwiceUnknownCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFileTwiceUnknownAsync() => await CreateFileTwiceUnknownCoreAsync(false, CancellationToken.None);
        private async Task CreateFileTwiceUnknownCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => TestFolder.CreateFile(FileName1, (CreationCollisionOption) 4), Throws.ArgumentException);
            else
                Assert.That(() => TestFolder.CreateFileAsync(FileName1, (CreationCollisionOption) 4), Throws.ArgumentException);
        }
    }
}