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
using System.IO;
#endif

#if __ANDROID__ || __IOS__
using RootFolder = PCLExt.FileStorage.Folders.LocalRootFolder;
#else
using RootFolder = PCLExt.FileStorage.Folders.ApplicationRootFolder;
#endif

namespace PCLExt.FileStorage.Test.Folder
{
    [TestFixtureAttr]
    public class FolderMoveTest : BaseFolderTest
    {
        [TestAttr]
        public void CreateFolder() => CreateFolderCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderAsync() => await CreateFolderCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderCoreAsync(bool sync, CancellationToken cancellationToken)
        {
        }

        public void MoveGenerateUniqueName()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Move(newFolder, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(newFolder.CheckExists($"{FileName1} (2)") == ExistenceCheckResult.FileExists);
                Assert.IsFalse(folder.Exists);
                Assert.IsFalse(file1.Exists);
                Assert.IsFalse(folder1.Exists);
                Assert.IsTrue(newFolder.Exists);
                Assert.IsTrue(newFile1.Exists);
                Assert.IsTrue(newFolder1.Exists);
                newFolder.Delete();
            }
            catch
            {
                folder.Delete();

                throw;
            }
        }

        public void MoveSelfGenerateUniqueName()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Move(folder, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(folder.CheckExists($"{FileName1} (2)") == ExistenceCheckResult.FileExists);
                Assert.IsTrue(folder.Exists);
                Assert.IsFalse(file1.Exists);
            }
            finally
            {
                folder.Delete();
            }
        }

        public void MoveReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Move(newFolder, NameCollisionOption.ReplaceExisting);
                Assert.IsFalse(folder.Exists);
                Assert.IsFalse(file1.Exists);
                Assert.IsFalse(folder1.Exists);
                Assert.IsTrue(newFolder.Exists);
                Assert.IsTrue(newFile1.Exists);
                Assert.IsTrue(newFolder1.Exists);
                newFolder.Delete();
            }
            catch
            {
                folder.Delete();

                throw;
            }
        }

        public void MoveSelfReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Move(folder, NameCollisionOption.ReplaceExisting);
                Assert.IsTrue(folder.Exists);
                Assert.IsTrue(file1.Exists);
            }
            finally
            {
                folder.Delete();
            }
        }

        public void MoveFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            void MoveFolderAction()
            {
                try
                {
                    folder.Move(newFolder, NameCollisionOption.FailIfExists);
                    Assert.Fail();
                }
                catch (FileExistException e)
                {
                    throw new IOException("", e);
                }
                catch (FolderExistException e)
                {
                    throw new IOException("", e);
                }
            }

            Assert.That(MoveFolderAction, Throws.InstanceOf<IOException>());

            folder.Delete();
            newFolder.Delete();
        }

        public void MoveSelfFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            void MoveSelfAction()
            {
                try
                {
                    folder.Move(folder, NameCollisionOption.FailIfExists);
                    Assert.Fail();
                }
                catch (FileExistException e)
                {
                    throw new IOException("", e);
                }
                catch (FolderExistException e)
                {
                    throw new IOException("", e);
                }
            }

            Assert.That(MoveSelfAction, Throws.InstanceOf<IOException>());

            folder.Delete();
        }
    }
}