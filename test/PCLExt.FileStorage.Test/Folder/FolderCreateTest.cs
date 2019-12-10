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
        public void CreateFolder()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [TestAttr]
        public void CreateFolderTwiceOpenIfExists()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(folder.Exists);

            folder.Delete();
        }

        [TestAttr]
        public void CreateFolderTwiceGenerateUniqueName()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder1 = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(!string.Equals(folder.Path, folder1.Path, System.StringComparison.Ordinal));

            folder.Delete();
            folder1.Delete();
        }

        [TestAttr]
        public void CreateFolderTwiceReplaceExisting()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);
            var folder1 = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(folder.Exists);
            Assert.IsTrue(folder1.Exists);
            Assert.IsTrue(string.Equals(folder.Path, folder1.Path, System.StringComparison.Ordinal));

            folder.Delete();
        }

        [TestAttr]
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

        [TestAttr]
        public void CreateFolderTwiceUnknown()
        {
            var folder = new TestFolder().CreateFolder(FolderName1, CreationCollisionOption.FailIfExists);

#if WINDOWS_UWP
            Assert.ThrowsException<System.ArgumentException>(() => new TestFolder().CreateFolder(FolderName1, (CreationCollisionOption) 4));
#else
            Assert.That(() => new TestFolder().CreateFolder(FolderName1, (CreationCollisionOption)4), Throws.ArgumentException);
#endif

            Assert.IsTrue(folder.Exists);
            folder.Delete();
        }

        [TestAttr]
        public void CreateFile()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestAttr]
        public void CreateFileTwiceOpenIfExists()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.OpenIfExists);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestAttr]
        public void CreateFileTwiceReplaceExisting()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            new TestFolder().CreateFile(FileName1, CreationCollisionOption.ReplaceExisting);
            Assert.IsTrue(file.Exists);

            file.Delete();
        }

        [TestAttr]
        public void CreateFileTwiceGenerateUniqueName()
        {
            var file = new TestFolder().CreateFile(FileName1, CreationCollisionOption.FailIfExists);
            var newFile = new TestFolder().CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(!string.Equals(file.Path, newFile.Path, System.StringComparison.Ordinal));

            file.Delete();
        }

        [TestAttr]
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

        [TestAttr]
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
    }
}
