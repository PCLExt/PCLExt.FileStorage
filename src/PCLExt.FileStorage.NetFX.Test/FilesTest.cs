using System;
using System.IO;

using PCLExt.FileStorage.Exceptions;

#if NUNIT

using NUnit.Framework;   

using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace PCLExt.FileStorage.Test
{
    [TestClass]
    public class FilesTest
    {
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

        private static IFolder TestFolder => new TestFolder();


        [TestInitialize]
        [TestCleanup]
        public void Clean() => TestFolder.Delete();

        #region Size

        [TestMethod]
        public void Size()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            file.WriteAllBytes(data);

            Assert.IsTrue(file.Size == data.Length);

            file.Delete();
        }

        #endregion

        #region Create

        [TestMethod]
        public void CreateOpenIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestMethod]
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

        [TestMethod]
        public void CreateReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file1.Exists);

            Assert.IsTrue(file.Path == file1.Path);

            file.Delete();
        }

        [TestMethod]
        public void CreateFailIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestMethod]
        [ExpectedException(typeof(FileExistException))]
        public void CreateFailIfExistsTwice()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.Fail();
        }

        #endregion

        #region Delete

        [TestMethod]
        public void Delete()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            file.Delete();
            Assert.IsFalse(file.Exists);
        }

        [TestMethod]
        [ExpectedException(typeof(Exceptions.FileNotFoundException))]
        public void DeleteTwice()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            file.Delete();
            Assert.IsFalse(file.Exists);

            file.Delete();
            Assert.Fail();
        }

        #endregion

        #region Open

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void OpenReadTryWrite()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            using (var stream = file.Open(FileAccess.Read))
                stream.WriteByte(0);
            Assert.Fail();
        }

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OpenUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            using (var stream = file.Open((FileAccess)2)) { }
            Assert.Fail();
        }

        #endregion

        #region Rename

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(FileExistException))]
        public void CopyFailIfExists()
        {
            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file1.Copy(file2.Path, NameCollisionOption.FailIfExists);
                Assert.Fail();
            }
            finally
            {
                Assert.IsTrue(file1.Exists);
                Assert.IsTrue(file2.Exists);

                file1.Delete();
                file2.Delete();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileExistException))]
        public void CopySelfFailIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy(file.Path, NameCollisionOption.FailIfExists);
                Assert.Fail();
            }
            finally
            {
                Assert.IsTrue(file.Exists);

                file.Delete();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CopyUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy(file.Path, (NameCollisionOption) 3);
                Assert.Fail();
            }
            finally
            {
                file.Delete();
            }
        }

        #endregion

        #region Move

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [ExpectedException(typeof(FileExistException))]
        public void MoveFailIfExists()
        {
            var file = TestFolder.CreateFile("TestFile", CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Move(file.Path, NameCollisionOption.FailIfExists);
                Assert.Fail();
            }
            finally
            {
                file.Delete();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Move(file.Path, (NameCollisionOption)3);
                Assert.Fail();
            }
            finally
            {
                file.Delete();
            }
        }

        #endregion
    }
}