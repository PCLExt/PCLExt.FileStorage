using System;
using System.IO;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;
#if NUNIT

using NUnit.Framework;

using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

#if __ANDROID__ || __IOS__
    
using RootFolder = PCLExt.FileStorage.Folders.LocalRootFolder;

#else

using RootFolder = PCLExt.FileStorage.Folders.ApplicationRootFolder;

#endif

namespace PCLExt.FileStorage.Test
{
    [TestClass] 
    public class FoldersTest
    {
        private const string FolderName1 = "TestFolder1";
        private const string FolderName2 = "TestFolder2";
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

        [TestInitialize]
        [TestCleanup]
        public void Clean()
        {
            new TestFolder().Delete();
        }

        #region Create

        [TestMethod]
        public void CreateFolder()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [TestMethod]
        public void CreateFolderTwiceOpenIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [TestMethod]
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

        [TestMethod]
        public void CreateFolderTwiceReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder1 = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(folder.Path == folder1.Path);

            folder.Delete();
        }

        [TestMethod]
        [ExpectedException(typeof(FolderExistException))]
        public void CreateFolderTwiceFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFolderTwiceUnknown()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            try
            {
                new TestFolder().CreateFolder(FolderName1, (CreationCollisionOption)4);
                Assert.IsTrue(folder.Exists);
            }
            finally
            {
                folder.Delete();
            }
        }

        [TestMethod]
        public void CreateFile()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestMethod]
        public void CreateFileTwiceOpenIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestMethod]
        public void CreateFileTwiceReplaceExisting()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestMethod]
        public void CreateFileTwiceGenerateUniqueName()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFile = new TestFolder().CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(file.Path != newFile.Path);

            file.Delete();
        }

        [TestMethod]
        [ExpectedException(typeof(FileExistException))]
        public void CreateFileTwiceFailIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFileTwiceUnknown()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                new TestFolder().CreateFile(FileName1, (CreationCollisionOption)4);
                Assert.Fail();
            }
            finally
            {
                file.Delete();
            }       
        }

        #endregion

        #region Delete

        [TestMethod]
        public void Delete()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);
        }

        [TestMethod]
        [ExpectedException(typeof(FolderNotFoundException))]
        public void DeleteTwice()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            folder.Delete();
            Assert.IsFalse(folder.Exists);

            folder.Delete();
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(RootFolderDeletionException))]
        public void DeleteRootStorage()
        {
            var folder = new RootFolder();

            folder.Delete();
        }

        #endregion

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(FolderExistException))]
        public void RenameAlreadyExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder2 = new TestFolder().CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            try
            {
                var newFolder = folder.Rename(FolderName2);
                Assert.Fail();
            }
            catch
            {
                folder.Delete();
                folder2.Delete();

                throw;
            }
        }

        #region GetFile

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(Exceptions.FileNotFoundException))]
        public void GetFileNotExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            var getFiles = folder.GetFile(FileName1);
            Assert.Fail();
        }

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(FolderNotFoundException))]
        public void GetFolderNotExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

            var getFiles = folder.GetFolder(FolderName1);
            Assert.Fail();
        }

        #endregion

        #region GetFolder

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void CopyFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

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
            finally
            {
                folder.Delete();
                newFolder.Delete();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void CopySelfFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

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
            finally
            {
                folder.Delete();
            }
        }

        #endregion

        #region Move

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void MoveFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var folder1 = folder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

            var newFolder = new TestFolder().CreateFolder($"{FolderName1} (2)", CreationCollisionOption.FailIfExists);
            var newFile1 = newFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFolder1 = newFolder.CreateFolder(FolderName2, CreationCollisionOption.FailIfExists);

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
            finally
            {
                folder.Delete();
                newFolder.Delete();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void MoveSelfFailIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var file1 = folder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

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
            finally
            {
                folder.Delete();
            }
        }

        #endregion

        [TestMethod]
        public void FolderFromPath()
        {
            var folder = new TestFolder().GetFolderFromPath(Path.Combine(FolderName1, FolderName2));
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder.Path == Path.Combine(new TestFolder().Path, FolderName1, FolderName2));
        }
    }
}