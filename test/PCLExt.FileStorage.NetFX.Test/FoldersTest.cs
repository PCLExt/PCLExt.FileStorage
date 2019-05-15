using System.IO;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
#endif

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

#if __ANDROID__ || __IOS__

using RootFolder = PCLExt.FileStorage.Folders.LocalRootFolder;

#else

using RootFolder = PCLExt.FileStorage.Folders.ApplicationRootFolder;

#endif

namespace PCLExt.FileStorage.Test
{
#if WINDOWS_UWP
    [TestClass]
#else
    [TestFixture]
#endif
    public class FoldersTest
    {
        private const string FolderName1 = "TestFolder1";
        private const string FolderName2 = "TestFolder2";
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

#if WINDOWS_UWP
        [TestCleanup]
        [TestInitialize]
#else
        [SetUp]
        [TearDown]
#endif
        public void Clean() => new TestFolder().Delete();

        #region Create

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFolder()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFolderTwiceOpenIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFolderTwiceGenerateUniqueName()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder1 = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(folder.Path != folder1.Path);

            folder.Delete();
            folder1.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFolderTwiceReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder1 = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(folder.Path == folder1.Path);

            folder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFolderTwiceFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FolderExistException>(() => new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists));
#else
            Assert.That(() => new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FolderExistException>());
#endif

            Assert.IsTrue(folder.Exists);
            folder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFolderTwiceUnknown()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<System.ArgumentException>(() => new TestFolder().CreateFolder(FolderName1, (CreationCollisionOption) 4));
#else
            Assert.That(() => new TestFolder().CreateFolder(FolderName1, (CreationCollisionOption) 4), Throws.ArgumentException);
#endif

            Assert.IsTrue(folder.Exists);
            folder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFile()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFileTwiceOpenIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFileTwiceReplaceExisting()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFileTwiceGenerateUniqueName()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFile = new TestFolder().CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(file.Path != newFile.Path);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFileTwiceFailIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FileExistException>(() => new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists));
#else
            Assert.That(() => new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
#endif

            Assert.IsTrue(file.Exists);
            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFileTwiceUnknown()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<System.ArgumentException>(() => new TestFolder().CreateFile(FileName1, (CreationCollisionOption) 4));
#else
            Assert.That(() => new TestFolder().CreateFile(FileName1, (CreationCollisionOption)4), Throws.ArgumentException);
#endif

            file.Delete();
        }

#endregion

#region Delete

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void Delete()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void DeleteTwice()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);

#if WINDOWS_UWP
            Assert.ThrowsException<FolderNotFoundException>(() => folder.Delete());
#else
            Assert.That(() => folder.Delete(), Throws.TypeOf<FolderNotFoundException>());
#endif
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void DeleteRootStorage()
        {
            var folder = new RootFolder();

#if WINDOWS_UWP
            Assert.ThrowsException<RootFolderDeletionException>(() => folder.Delete());
#else
            Assert.That(() => folder.Delete(), Throws.TypeOf<RootFolderDeletionException>());
#endif
        }

#endregion

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void Rename()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFolder = folder.Rename(FolderName2);
                Assert.IsFalse(folder.Exists);
                Assert.IsTrue(newFolder.Exists);
                Assert.IsFalse(folder.Path == newFolder.Path);
                newFolder.Delete();
            }
            catch
            {
                folder.Delete();

                throw;
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void RenameAlreadyExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = new TestFolder().CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FolderExistException>(() => folder.Rename(FolderName2));
#else
            Assert.That(() => folder.Rename(FolderName2), Throws.TypeOf<FolderExistException>());
#endif

            folder.Delete();
            folder2.Delete();
        }

#region GetFile

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void GetFile()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = folder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var getFiles = folder.GetFile(FileName1);
            Assert.IsTrue(file1.Path == getFiles.Path);
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void GetFileNotExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<Exceptions.FileNotFoundException>(() => folder.GetFile(FileName1));
#else
            Assert.That(() => folder.GetFile(FileName1), Throws.TypeOf<Exceptions.FileNotFoundException>());
#endif
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void GetFiles()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = folder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var getFiles = folder.GetFiles();
            Assert.IsTrue(getFiles.Count == 2);
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void GetFolderNotExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FolderNotFoundException>(() => folder.GetFolder(FolderName1));
#else
            Assert.That(() => folder.GetFolder(FolderName1), Throws.TypeOf<FolderNotFoundException>());
#endif
        }

#endregion

#region GetFolder

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void GetFolder()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = folder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var getFolder = folder.GetFolder(FolderName1);
            Assert.IsTrue(folder1.Path == getFolder.Path);
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#endregion

#region Copy

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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
#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
            Assert.ThrowsException<IOException>(СopyAction);
#else
            Assert.That(СopyAction, Throws.InstanceOf<IOException>());
#endif

            folder.Delete();
            newFolder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
            Assert.ThrowsException<IOException>(СopyAction);
#else
            Assert.That(СopyAction, Throws.InstanceOf<IOException>());
#endif

            folder.Delete();
        }

#endregion

#region Move

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
            Assert.ThrowsException<IOException>(MoveFolderAction);
#else
            Assert.That(MoveFolderAction, Throws.InstanceOf<IOException>());
#endif

            folder.Delete();
            newFolder.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
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

#if WINDOWS_UWP
            Assert.ThrowsException<IOException>(MoveSelfAction);
#else
            Assert.That(MoveSelfAction, Throws.InstanceOf<IOException>());
#endif

            folder.Delete();
        }

#endregion

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void FolderFromPath()
        {
            var folder = new TestFolder().GetFolderFromPath(Path.Combine(FolderName1, FolderName2));
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder.Path == Path.Combine(new TestFolder().Path, FolderName1, FolderName2));
        }
    }
}