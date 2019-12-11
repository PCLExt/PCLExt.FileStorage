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

namespace PCLExt.FileStorage.Test.File
{
    [TestFixture]
    public class FileMoveTest : BaseFileTest
    {
        [TestAttr]
        public void Move() => MoveCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveAsync() => await MoveCoreAsync(false, CancellationToken.None);
        private async Task MoveCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            var newFile = sync
                ? TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName2, CreationCollisionOption.FailIfExists, cancellationToken);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            if (sync)
                file.WriteAllBytes(data);
            else
                await file.WriteAllBytesAsync(data, cancellationToken);

            if (sync)
                file.Move(newFile);
            else
                await file.MoveAsync(newFile);
            Assert.IsFalse(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsFalse(string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
            var newData = sync ? newFile.ReadAllBytes() : await newFile.ReadAllBytesAsync(cancellationToken);
            Assert.IsTrue(data.SequenceEqual(newData));
        }

        [TestAttr]
        public void MoveSelf() => MoveSelfCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveSelfAsync() => await MoveSelfCoreAsync(false, CancellationToken.None);
        private async Task MoveSelfCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            if (sync)
                file.WriteAllBytes(data);
            else
                await file.WriteAllBytesAsync(data, cancellationToken);

            if (sync)
                file.Move(file);
            else
                await file.MoveAsync(file, cancellationToken);
            Assert.IsTrue(file.Exists);
            var newData = sync ? file.ReadAllBytes() : await file.ReadAllBytesAsync(cancellationToken);
            Assert.IsTrue(data.SequenceEqual(newData));
        }

        [TestAttr]
        public void MoveGenerateUniqueName() => MoveGenerateUniqueNameCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveGenerateUniqueNameAsync() => await MoveGenerateUniqueNameCoreAsync(false, CancellationToken.None);
        private async Task MoveGenerateUniqueNameCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            IFile? newFile = null;
            try
            {
                newFile = sync
                    ? file.Move(file.Path, NameCollisionOption.GenerateUniqueName)
                    : await file.MoveAsync(file.Path, NameCollisionOption.GenerateUniqueName, cancellationToken);
                Assert.IsTrue(newFile.Exists);
                Assert.IsFalse(string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sync)
                    newFile?.Delete();
                else if(newFile != null)
                    await newFile.DeleteAsync(cancellationToken);
            }
        }

        [TestAttr]
        public void MoveReplaceExisting() => MoveReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveReplaceExistingAsync() => await MoveReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task MoveReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            IFile? newFile = null;
            try
            {
                newFile = sync
                    ? file.Move($"{file.Path}_1", NameCollisionOption.ReplaceExisting)
                    : await file.MoveAsync($"{file.Path}_1", NameCollisionOption.ReplaceExisting, cancellationToken);
                Assert.IsFalse(file.Exists);
                Assert.IsTrue(newFile.Exists);
                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sync)
                    newFile?.Delete();
                else if (newFile != null)
                    await newFile.DeleteAsync(cancellationToken);
            }
        }

        [TestAttr]
        public void MoveSelfReplaceExisting() => MoveSelfReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveSelfReplaceExistingAsync() => await MoveSelfReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task MoveSelfReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            if (sync)
                file.WriteAllBytes(data);
            else
                await file.WriteAllBytesAsync(data, cancellationToken);

            var newFile = sync
                ? file.Move(file.Path, NameCollisionOption.ReplaceExisting)
                : await file.MoveAsync(file.Path, NameCollisionOption.ReplaceExisting, cancellationToken);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            var newData1 = sync ? file.ReadAllBytes() : await file.ReadAllBytesAsync(cancellationToken);
            var newData2 = sync ? newFile.ReadAllBytes() : await newFile.ReadAllBytesAsync(cancellationToken);
            Assert.IsTrue(data.SequenceEqual(newData1));
            Assert.IsTrue(data.SequenceEqual(newData2));
        }

        [TestAttr]
        public void MoveFailIfExists() => MoveFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveFailIfExistsAsync() => await MoveFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task MoveFailIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            var newFile = sync
                ? TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName2, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => file.Move(newFile.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            else
                Assert.That(() => file.MoveAsync(newFile.Path, NameCollisionOption.FailIfExists, cancellationToken), Throws.TypeOf<FileExistException>());
        }

        [TestAttr]
        public void MoveUnknown() => MoveFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task MoveUnknownAsync() => await MoveFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task MoveUnknownCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => file.Move(file.Path, (NameCollisionOption) 3), Throws.ArgumentException);
            else
                Assert.That(() => file.MoveAsync(file.Path, (NameCollisionOption) 3, cancellationToken), Throws.ArgumentException);
        }
    }
}