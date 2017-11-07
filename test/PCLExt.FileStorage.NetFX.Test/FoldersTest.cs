using System.IO;

using NUnit.Framework;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

#if __ANDROID__ || __IOS__
    
using RootFolder = PCLExt.FileStorage.Folders.LocalRootFolder;

#else

using RootFolder = PCLExt.FileStorage.Folders.ApplicationRootFolder;

#endif

namespace PCLExt.FileStorage.Test
{
    [TestFixture] 
    public class FoldersTest
    {
        private const string FolderName1 = "TestFolder1";
        private const string FolderName2 = "TestFolder2";
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

        [SetUp]
        [TearDown]
        public void Clean()
        {
            new TestFolder().Delete();
        }

        #region Create

        [Test]
        public void CreateFolder()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [Test]
        public void CreateFolderTwiceOpenIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [Test]
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

        [Test]
        public void CreateFolderTwiceReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder1 = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(folder.Path == folder1.Path);

            folder.Delete();
        }

        [Test]
        public void CreateFolderTwiceFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FolderExistException>());

            Assert.IsTrue(folder.Exists);
            folder.Delete();
        }

        [Test]
        public void CreateFolderTwiceUnknown()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => new TestFolder().CreateFolder(FolderName1, (CreationCollisionOption) 4), Throws.ArgumentException);

            Assert.IsTrue(folder.Exists);
            folder.Delete();
        }

        [Test]
        public void CreateFile()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [Test]
        public void CreateFileTwiceOpenIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [Test]
        public void CreateFileTwiceReplaceExisting()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [Test]
        public void CreateFileTwiceGenerateUniqueName()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFile = new TestFolder().CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(file.Path != newFile.Path);

            file.Delete();
        }

        [Test]
        public void CreateFileTwiceFailIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());

            Assert.IsTrue(file.Exists);
            file.Delete();
        }

        [Test]
        public void CreateFileTwiceUnknown()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => new TestFolder().CreateFile(FileName1, (CreationCollisionOption) 4), Throws.ArgumentException);

            file.Delete();
        }

        #endregion

        #region Delete

        [Test]
        public void Delete()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);
        }

        [Test]
        public void DeleteTwice()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);

            Assert.That(() => folder.Delete(), Throws.TypeOf<FolderNotFoundException>());
        }

        [Test]
        public void DeleteRootStorage()
        {
            var folder = new RootFolder();

            Assert.That(() => folder.Delete(), Throws.TypeOf<RootFolderDeletionException>());
        }

        #endregion

        [Test]
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

        [Test]
        public void RenameAlreadyExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = new TestFolder().CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            Assert.That(() => folder.Rename(FolderName2), Throws.TypeOf<FolderExistException>());

            folder.Delete();
            folder2.Delete();
        }

        #region GetFile

        [Test]
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

        [Test]
        public void GetFileNotExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => folder.GetFile(FileName1), Throws.TypeOf<Exceptions.FileNotFoundException>());
        }

        [Test]
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

        [Test]
        public void GetFolderNotExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => folder.GetFolder(FolderName1), Throws.TypeOf<FolderNotFoundException>());
        }

        #endregion

        #region GetFolder

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
        [Test]
        public void CopyFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            Assert.That(delegate
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
            }, Throws.InstanceOf<IOException>());

            folder.Delete();
            newFolder.Delete();
        }

        [Test]
        public void CopySelfFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(delegate
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
            }, Throws.InstanceOf<IOException>());

            folder.Delete();
        }

        #endregion

        #region Move

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void MoveFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            Assert.That(delegate
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
            }, Throws.InstanceOf<IOException>());

            folder.Delete();
            newFolder.Delete();
        }

        [Test]
        public void MoveSelfFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(delegate
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
            }, Throws.InstanceOf<IOException>());

            folder.Delete();
        }

        #endregion

        [Test]
        public void FolderFromPath()
        {
            var folder = new TestFolder().GetFolderFromPath(Path.Combine(FolderName1, FolderName2));
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder.Path == Path.Combine(new TestFolder().Path, FolderName1, FolderName2));
        }
    }
}