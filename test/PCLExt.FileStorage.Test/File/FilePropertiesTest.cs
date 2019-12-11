using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using PCLExt.FileStorage.Extensions;

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
    public class FilePropertiesTest : BaseFileTest
    {
        [TestAttr]
        public void Exists() => ExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
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

        [TestAttr]
        public void Size() => SizeCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
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

        [TestAttr]
        public void CreationTime() => CreationTimeCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreationTimeAsync() => await CreationTimeCoreAsync(false, CancellationToken.None);
        private async Task CreationTimeCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var timeBeforeFileCreaton = DateTime.UtcNow.AddSeconds(-1);
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            Assert.IsTrue(timeBeforeFileCreaton <= file.CreationTime.UtcDateTime, $"{timeBeforeFileCreaton:yyyyMMddHHmmss.fff} <= {file.CreationTime.UtcDateTime:yyyyMMddHHmmss.fff}");
        }

        [TestAttr]
        public void LastAccessTime() => LastAccessTimeCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
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
                await Task.Delay(100, cancellationToken);

            var content = sync
                ? file.ReadAllBytes()
                : await file.ReadAllBytesAsync(cancellationToken);

            Assert.IsTrue(lastAccessTime < file.LastAccessTime);
            Assert.IsTrue(file.LastAccessTime < DateTime.UtcNow);
        }

        [TestAttr]
        public void LastWriteTimeTime() => LastWriteTimeCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
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
                await Task.Delay(100, cancellationToken);

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