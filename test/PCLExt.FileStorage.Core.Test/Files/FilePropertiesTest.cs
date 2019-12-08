using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Test
{
    [TestFixture]
    public class FilePropertiesTest : BaseFileTest
    {
        private static async Task<IFile> CreateFile(string fileName, bool sync, CancellationToken cancellationToken)
        {
            return sync
                ? TestFolder.CreateFile(fileName, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists, cancellationToken);
        }

        private static async Task DeleteFile(IFile file, bool sync, CancellationToken cancellationToken)
        {
            if (sync)
                file.Delete();
            else
                await file.DeleteAsync(cancellationToken);
        }

        [Test]
        public void Exists() => ExistsCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task ExistsAsync() => await ExistsCoreAsync(false, CancellationToken.None);
        private async Task ExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            Assert.IsTrue(file.Exists);

            if (sync)
                file.Delete();
            else
                await file.DeleteAsync(cancellationToken);

            Assert.IsFalse(file.Exists);
        }

        [Test]
        public void Size() => SizeCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task SizeAsync() => await SizeCoreAsync(false, CancellationToken.None);
        private async Task SizeCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            if(sync)
                file.WriteAllBytes(data);
            else
                await file.WriteAllBytesAsync(data, cancellationToken);

            Assert.IsTrue(file.Size == (ulong) data.Length);
        }

        [Test]
        public void CreationTime() => CreationTimeCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task CreationTimeAsync() => await CreationTimeCoreAsync(false, CancellationToken.None);
        private async Task CreationTimeCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var timeBeforeFileCreaton = DateTime.UtcNow.AddSeconds(-1);
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            Assert.IsTrue(timeBeforeFileCreaton <= file.CreationTime.UtcDateTime, $"{timeBeforeFileCreaton:yyyyMMddHHmmss.fff} <= {file.CreationTime.UtcDateTime:yyyyMMddHHmmss.fff}");
        }

        [Test]
        public void LastAccessTime() => LastAccessTimeCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task LastAccessTimeAsync() => await LastAccessTimeCoreAsync(false, CancellationToken.None);
        public async Task LastAccessTimeCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var timeBeforeFileCreaton = DateTime.UtcNow.AddSeconds(-1);
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            Assert.IsTrue(timeBeforeFileCreaton <= file.LastAccessTime.UtcDateTime);

            var lastAccessTime = file.LastAccessTime;

            if (sync)
                Thread.Sleep(100);
            else
                await Task.Delay(100);

            var content = file.ReadAllBytes();

            Assert.IsTrue(lastAccessTime < file.LastAccessTime);
            Assert.IsTrue(file.LastAccessTime < DateTime.UtcNow);
        }

        [Test]
        public void LastWriteTimeTime() => LastWriteTimeCoreAsync(true, CancellationToken.None).RunSync();
        [Test]
        public async Task LastWriteTimeTimeAsync() => await LastWriteTimeCoreAsync(false, CancellationToken.None);
        public async Task LastWriteTimeCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var timeBeforeFileCreaton = DateTime.UtcNow.AddSeconds(-1);
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            var lastWriteTime = file.LastWriteTime;

            if (sync)
                Thread.Sleep(100);
            else
                await Task.Delay(100);

            var data = new byte[4 * 1024 * 1024];
            new Random().NextBytes(data);
            if (sync)
                file.WriteAllBytes(data);
            else
                await file.WriteAllBytesAsync(data, cancellationToken);

            Assert.IsTrue(lastWriteTime < file.LastWriteTime);
            Assert.IsTrue(file.LastWriteTime < DateTime.UtcNow);
        }
    }
}