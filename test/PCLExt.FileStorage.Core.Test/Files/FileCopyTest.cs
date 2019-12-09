using NUnit.Framework;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;
using PCLExt.FileStorage.Test;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage.Core.Test.Files
{
    [TestFixture]
    public class FileCopyTest : BaseFileTest
    {
        [Test]
        public void Copy() => CopyCoreAsync(true, CancellationToken.None).Wait();
        [Test]
        public async Task CopyAsync() => await CopyCoreAsync(false, CancellationToken.None);
        private async Task CopyCoreAsync(bool sync, CancellationToken cancellationToken)
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
                file.Copy(newFile);
            else
                await file.CopyAsync(newFile, cancellationToken);

            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
            var newData = sync ? newFile.ReadAllBytes() : await newFile.ReadAllBytesAsync(cancellationToken);
            Assert.IsTrue(data.SequenceEqual(newData));
        }

        [Test]
        public void CopySelf() => CopySelfCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopySelfAsync() => await CopySelfCoreAsync(false, CancellationToken.None);
        private async Task CopySelfCoreAsync(bool sync, CancellationToken cancellationToken)
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

            file.Copy(file);
            Assert.IsTrue(file.Exists);
            var newData = sync ? file.ReadAllBytes() : await file.ReadAllBytesAsync(cancellationToken);
            Assert.IsTrue(data.SequenceEqual(newData));
        }

        [Test]
        public void CopyGenerateUniqueName() => CopyGenerateUniqueNameCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopyGenerateUniqueNameAsync() => await CopyGenerateUniqueNameCoreAsync(false, CancellationToken.None);
        private async Task CopyGenerateUniqueNameCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync 
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var newFile = sync
                ? file.Copy(file.Path, NameCollisionOption.GenerateUniqueName)
                : await file.CopyAsync(file.Path, NameCollisionOption.GenerateUniqueName, cancellationToken);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(!string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
        }

        [Test]
        public void CopyReplaceExisting() => CopyReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopyReplaceExistingAsync() => await CopyReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task CopyReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var newFile = sync
                ? file.Copy($"{file.Path} (1)", NameCollisionOption.ReplaceExisting)
                : await file.CopyAsync($"{file.Path} (1)", NameCollisionOption.ReplaceExisting, cancellationToken);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(!string.Equals(file.Path, newFile.Path, StringComparison.Ordinal));
        }

        [Test]
        public void CopySelfReplaceExisting() => CopySelfReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopySelfReplaceExistingAsync() => await CopySelfReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task CopySelfReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var newFile = sync
                 ? file.Copy(file.Path, NameCollisionOption.ReplaceExisting)
                 : await file.CopyAsync(file.Path, NameCollisionOption.ReplaceExisting);
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsTrue(string.Equals(file.Path, newFile.Path, StringComparison.Ordinal));
        }

        [Test]
        public void CopyFailIfExists() => CopyFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopyFailIfExistsAsync() => await CopyFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CopyFailIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file1 = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            var file2 = sync
                ? TestFolder.CreateFile(FileName2, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName2, CreationCollisionOption.FailIfExists, cancellationToken);

            if(sync)
                Assert.That(() => file1.Copy(file2.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            else
                Assert.That(() => file1.CopyAsync(file2.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            Assert.IsTrue(file1.Exists);
            Assert.IsTrue(file2.Exists);
        }

        [Test]
        public void CopySelfFailIfExists() => CopySelfFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopySelfFailIfExistsAsync() => await CopySelfFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CopySelfFailIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => file.Copy(file.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            else
                Assert.That(() => file.CopyAsync(file.Path, NameCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            Assert.IsTrue(file.Exists);
        }

        [Test]
        public void CopyUnknown() => CopyUnknownCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CopyUnknownAsync() => await CopyUnknownCoreAsync(false, CancellationToken.None);
        private async Task CopyUnknownCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                Assert.That(() => file.Copy(file.Path, (NameCollisionOption) 3), Throws.ArgumentException);
            else
                Assert.That(() => file.CopyAsync(file.Path, (NameCollisionOption) 3), Throws.ArgumentException);
        }
    }
}