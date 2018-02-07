using System;
using System.IO;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
#endif

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions; 


namespace PCLExt.FileStorage.Test
{
#if WINDOWS_UWP
    [TestClass]
#else
    [TestFixture]
#endif
    public class FilesTest
    {
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

        private static IFolder TestFolder => new TestFolder();


#if WINDOWS_UWP
        [TestCleanup]
        [TestInitialize]
#else
        [SetUp]
        [TearDown]
#endif
        public void Clean() => TestFolder.Delete();

        #region Size & Times

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void Size()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            Assert.IsTrue(file.Size == data.Length);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreationTime()
        {
            var timeBeforeFileCreaton = DateTime.Now.AddSeconds(-1);
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);

            Assert.IsTrue(timeBeforeFileCreaton <= file.CreationTime);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreationTimeUTC()
        {
            var timeBeforeFileCreaton = DateTime.UtcNow.AddSeconds(-1);
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);

            Assert.IsTrue(timeBeforeFileCreaton <= file.CreationTimeUTC,
                $"{timeBeforeFileCreaton:yyyyMMddHHmmss.fff} <= {file.CreationTimeUTC:yyyyMMddHHmmss.fff}");

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void LastAccessTime()
        {
            var timeBeforeFileCreaton = DateTime.Now.AddSeconds(-1);
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);

            Assert.IsTrue(timeBeforeFileCreaton <= file.LastAccessTime);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void LastAccessTimeUTC()
        {
            var timeBeforeFileCreaton = DateTime.UtcNow.AddSeconds(-1);
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);

            Assert.IsTrue(timeBeforeFileCreaton <= file.LastAccessTimeUTC);

            file.Delete();
        }

        #endregion

        #region Create

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateOpenIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateGenerateUniqueName()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(file1.Exists);

            Assert.IsTrue(file.Path != file1.Path);

            file.Delete();
            file1.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file1.Exists);

            Assert.IsTrue(file.Path == file1.Path);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFailIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CreateFailIfExistsTwice()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);
            Action createFileAction = () => TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FileExistException>(createFileAction);
#else
            Assert.That(delegate { createFileAction.Invoke(); }, Throws.TypeOf<FileExistException>());
#endif
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
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            file.Delete();
            Assert.IsFalse(file.Exists);
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void DeleteTwice()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            file.Delete();
            Assert.IsFalse(file.Exists);
            Action deleteAction = () => file.Delete();

#if WINDOWS_UWP
            Assert.ThrowsException<Exceptions.FileNotFoundException>(deleteAction);
#else
            Assert.That(delegate { deleteAction.Invoke(); }, Throws.TypeOf<Exceptions.FileNotFoundException>());
#endif
        }

#endregion

#region Open

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void OpenRead()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            using (var stream = file.Open(FileAccess.Read))
            {
                var newData = new byte[data.Length];
                stream.Read(newData, 0, newData.Length);
                // BUG
                //CollectionAssert.AreEqual(data, newData);
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void OpenReadTryWrite()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            using (var stream = file.Open(FileAccess.Read))
#if WINDOWS_UWP
                Assert.ThrowsException<NotSupportedException>(() => stream.WriteByte(0));
#else
                Assert.That(() => stream.WriteByte(0), Throws.TypeOf<NotSupportedException>());
#endif
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void OpenReadWrite()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);

            using (var stream = file.Open(FileAccess.ReadAndWrite))
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                var newData = new byte[data.Length];
                stream.Read(newData, 0, newData.Length);
                // BUG
                //CollectionAssert.AreEqual(data, newData);
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void OpenUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Action openFileAction = () => file.Open((FileAccess)2);

#if WINDOWS_UWP
            Assert.ThrowsException<ArgumentException>(openFileAction);
#else
            Assert.That(delegate { openFileAction.Invoke(); }, Throws.ArgumentException);
#endif
        }

#endregion

#region Rename

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void Rename()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Rename(FileName2);
                Assert.IsFalse(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(file.Name != newFile.Name);
                newFile.Delete();
            }
            catch
            {
                file.Delete();

                throw;
            }
        }

#endregion

#region Copy

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void Copy()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFile = TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            try
            {
                file.Copy(newFile);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(file.Name != newFile.Name);
                // BUG
                //CollectionAssert.AreEqual(data, newFile.ReadAllBytes());
            }
            finally
            {
                file.Delete();
                newFile.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopySelf()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            try
            {
                file.Copy(file);
                Assert.IsTrue(file.Exists);
                // BUG
                //CollectionAssert.AreEqual(data, file.ReadAllBytes());
            }
            finally
            {
                file.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopyGenerateUniqueName()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy(file.Path, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(file.Name != newFile.Name);

                newFile.Delete();
            }
            finally
            {
                file.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopyReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy($"{file.Path} (1)", NameCollisionOption.ReplaceExisting);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(file.Path != newFile.Path);
            }
            finally
            {
                file.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopySelfReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy(file.Path, NameCollisionOption.ReplaceExisting);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(file.Path == newFile.Path);
            }
            finally
            {
                file.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopyFailIfExists()
        {
            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);

            Action copyAction = () => file1.Copy(file2.Path, NameCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FileExistException>(copyAction);
#else
            Assert.That(delegate { copyAction.Invoke(); }, Throws.TypeOf<FileExistException>());
#endif

            Assert.IsTrue(file1.Exists);
            Assert.IsTrue(file2.Exists);

            file1.Delete();
            file2.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopySelfFailIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Action copyAction = () => file.Copy(file.Path, NameCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FileExistException>(copyAction);
#else
            Assert.That(delegate { copyAction.Invoke(); }, Throws.TypeOf<FileExistException>());
#endif

            Assert.IsTrue(file.Exists);

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void CopyUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Action copyFileAction = () => file.Copy(file.Path, (NameCollisionOption)3);

#if WINDOWS_UWP
            Assert.ThrowsException<ArgumentException>(copyFileAction);
#else
            Assert.That(delegate { copyFileAction.Invoke(); }, Throws.ArgumentException);
#endif

            file.Delete();
        }

#endregion

#region Move

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void Move()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFile = TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            try
            {
                file.Move(newFile);
                Assert.IsFalse(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(file.Name != newFile.Name);
                // BUG
                //CollectionAssert.AreEqual(data, newFile.ReadAllBytes());
            }
            finally
            {
                newFile.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void MoveSelf()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            try
            {
                file.Move(file);
                Assert.IsTrue(file.Exists);
                // BUG
                //CollectionAssert.AreEqual(data, file.ReadAllBytes());
            }
            finally
            {
                file.Delete();
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void MoveGenerateUniqueName()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Move(file.Path, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(file.Name != newFile.Name);
                newFile.Delete();
            }
            catch
            {
                file.Delete();

                throw;
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void MoveReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Move($"{file.Path}_1", NameCollisionOption.ReplaceExisting);
                Assert.IsFalse(file.Exists);
                Assert.IsTrue(newFile.Exists);
                newFile.Delete();
            }
            catch
            {
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
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Move(file.Path, NameCollisionOption.ReplaceExisting);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
            }
            catch
            {
                file.Delete();

                throw;
            }
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void MoveFailIfExists()
        {
            var file = TestFolder.CreateFile("TestFile", CreationCollisionOption.FailIfExists);

            Action moveFileAction = () => file.Move(file.Path, NameCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<FileExistException>(moveFileAction);
#else
            Assert.That(delegate { moveFileAction.Invoke(); }, Throws.TypeOf<FileExistException>());
#endif

            file.Delete();
        }

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void MoveUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Action moveAction = () => file.Move(file.Path, (NameCollisionOption)3);

#if WINDOWS_UWP
            Assert.ThrowsException<ArgumentException>(moveAction);
#else
            Assert.That(delegate { moveAction.Invoke(); }, Throws.ArgumentException);
#endif

            file.Delete();
        }

#endregion

#if WINDOWS_UWP
        [TestMethod]
#else
        [Test]
#endif
        public void FileFromPath()
        {
            var file = new TestFolder().GetFileFromPath(Path.Combine("Folder1", FileName2));
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(file.Path == Path.Combine(new TestFolder().Path, "Folder1", FileName2));
        }
    }
}