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
    public class FolderCopyTest : BaseFolderTest
    {
        [TestAttr]
        public void CreateFolder() => CreateFolderCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFolderAsync() => await CreateFolderCoreAsync(false, CancellationToken.None);
        private async Task CreateFolderCoreAsync(bool sync, CancellationToken cancellationToken)
        {
        }

        public void CopyGenerateUniqueName()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Copy(newFolder, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(newFolder.CheckExists($"{FileName1} (2)") == ExistenceCheckResult.FileExists);
            }
            finally
            {
                folder.Delete();
                newFolder.Delete();
            }
        }

        public void CopySelfGenerateUniqueName()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Copy(folder, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(folder.CheckExists($"{FileName1} (2)") == ExistenceCheckResult.FileExists);
            }
            finally
            {
                folder.Delete();
            }
        }

        public void CopyReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Copy(newFolder, NameCollisionOption.ReplaceExisting);
                Assert.IsFalse(newFolder.CheckExists(FolderName2) == ExistenceCheckResult.FileExists);
            }
            finally
            {
                folder.Delete();
                newFolder.Delete();
            }
        }

        public void CopySelfReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                folder.Copy(folder, NameCollisionOption.ReplaceExisting);
                Assert.IsFalse(folder.CheckExists(FolderName2) == ExistenceCheckResult.FileExists);
            }
            finally
            {
                folder.Delete();
            }
        }

        // TODO: Check
        public void CopyFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            void СopyAction()
            {
                try
                {
                    folder.Copy(newFolder, NameCollisionOption.FailIfExists);
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

            Assert.That(СopyAction, Throws.InstanceOf<IOException>());

            folder.Delete();
            newFolder.Delete();
        }

        public void CopySelfFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            void СopyAction()
            {
                try
                {
                    folder.Copy(folder, NameCollisionOption.FailIfExists);
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

            Assert.That(СopyAction, Throws.InstanceOf<IOException>());

            folder.Delete();
        }
    }
}