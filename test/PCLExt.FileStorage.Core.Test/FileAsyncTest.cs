using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Test
{

    [TestFixture]
    public class FileAsyncTest
    {
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

        private static IFolder TestFolder => new TestFolder();


        [SetUp]
        [TearDown]
        public Task CleanAsync() => TestFolder.DeleteAsync();

        #region Size & Times



        #endregion

        #region Create

        [Test]
        public async Task CreateOpenIfExists()
        {
            var file = await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            await file.DeleteAsync();
        }

        [Test]
        public void CreateGenerateUniqueName()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(file1.Exists);

            Assert.IsTrue(!string.Equals(file.Path, file1.Path, StringComparison.Ordinal));

            file.Delete();
            file1.Delete();
        }

        [Test]
        public void CreateReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file1.Exists);

            Assert.IsTrue(string.Equals(file.Path, file1.Path, StringComparison.Ordinal));

            file.Delete();
        }

        [Test]
        public void CreateFailIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [Test]
        public void CreateFailIfExistsTwice()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            Assert.That(() => TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
        }

        #endregion

        #region Delete

        [Test]
        public void Delete()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            file.Delete();
            Assert.IsFalse(file.Exists);
        }

        [Test]
        public void DeleteTwice()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            file.Delete();
            Assert.IsFalse(file.Exists);

            Assert.That(() => file.Delete(), Throws.TypeOf<Exceptions.FileNotFoundException>());
        }

        #endregion

        #region Open

        [Test]
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

        [Test]
        public void OpenReadTryWrite()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            using var stream = file.Open(FileAccess.Read);
            Assert.That(() => stream.WriteByte(0), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
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

        [Test]
        public void OpenUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => file.Open((FileAccess)2), Throws.ArgumentException);
        }

        #endregion

        #region Rename

        [Test]
        public void Rename()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Rename(FileName2);
                Assert.IsFalse(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
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

        [Test]
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
                Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
                // BUG
                //CollectionAssert.AreEqual(data, newFile.ReadAllBytes());
            }
            finally
            {
                file.Delete();
                newFile.Delete();
            }
        }

        [Test]
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

        [Test]
        public void CopyGenerateUniqueName()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy(file.Path, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));

                newFile.Delete();
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void CopyReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy($"{file.Path} (1)", NameCollisionOption.ReplaceExisting);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(!string.Equals(file.Path, newFile.Path, StringComparison.Ordinal));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void CopySelfReplaceExisting()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Copy(file.Path, NameCollisionOption.ReplaceExisting);
                Assert.IsTrue(file.Exists);
                Assert.IsTrue(newFile.Exists);
                Assert.IsTrue(string.Equals(file.Path, newFile.Path, StringComparison.Ordinal));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void CopyFailIfExists()
        {
            var file1 = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var file2 = TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists);

            Assert.That(() => file1.Copy(file2.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());

            Assert.IsTrue(file1.Exists);
            Assert.IsTrue(file2.Exists);

            file1.Delete();
            file2.Delete();
        }

        [Test]
        public void CopySelfFailIfExists()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => file.Copy(file.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());

            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [Test]
        public void CopyUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => file.Copy(file.Path, (NameCollisionOption)3), Throws.ArgumentException);

            file.Delete();
        }

        #endregion

        #region Move

        [Test]
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
                Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
                // BUG
                //CollectionAssert.AreEqual(data, newFile.ReadAllBytes());
            }
            finally
            {
                newFile.Delete();
            }
        }

        [Test]
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

        [Test]
        public void MoveGenerateUniqueName()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            try
            {
                var newFile = file.Move(file.Path, NameCollisionOption.GenerateUniqueName);
                Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
                newFile.Delete();
            }
            catch
            {
                file.Delete();

                throw;
            }
        }

        [Test]
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

        [Test]
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

        [Test]
        public void MoveFailIfExists()
        {
            var file = TestFolder.CreateFile("TestFile", CreationCollisionOption.FailIfExists);

            Assert.That(() => file.Move(file.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());

            file.Delete();
        }

        [Test]
        public void MoveUnknown()
        {
            var file = TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists);

            Assert.That(() => file.Move(file.Path, (NameCollisionOption)3), Throws.ArgumentException);

            file.Delete();
        }

        #endregion

        [Test]
        public void FileFromPath()
        {
            var file = new TestFolder().GetFileFromPath(Path.Combine("Folder1", FileName2));
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(string.Equals(file.Path, Path.Combine(new TestFolder().Path, "Folder1", FileName2), StringComparison.Ordinal));
        }
    }
}