using NUnit.Framework;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

using System;
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
    public class FileCreateTest : BaseFileTest
    {
        [TestAttr]
        public void CreateOpenIfExists() => CreateOpenIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateOpenIfExistsAsync() => await CreateOpenIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CreateOpenIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var file1 = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.OpenIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.OpenIfExists, cancellationToken);

            Assert.IsTrue(file1.Exists);
        }

        [TestAttr]
        public void CreateGenerateUniqueName() => CreateGenerateUniqueNameCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateGenerateUniqueNameAsync() => await CreateGenerateUniqueNameCoreAsync(false, CancellationToken.None);
        private async Task CreateGenerateUniqueNameCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var file1 = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.GenerateUniqueName)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.GenerateUniqueName, cancellationToken);
            Assert.IsTrue(file1.Exists);

            Assert.IsFalse(string.Equals(file.Path, file1.Path, StringComparison.Ordinal));

            if (sync)
                file1.Delete();
            else
                await file1.DeleteAsync(cancellationToken);
        }

        [TestAttr]
        public void CreateReplaceExisting() => CreateReplaceExistingCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateReplaceExistingAsync() => await CreateReplaceExistingCoreAsync(false, CancellationToken.None);
        private async Task CreateReplaceExistingCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            Assert.IsTrue(file.Exists);

            var file1 = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.ReplaceExisting)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.ReplaceExisting, cancellationToken);
            Assert.IsTrue(file1.Exists);

            Assert.IsTrue(string.Equals(file.Path, file1.Path, StringComparison.Ordinal));
        }

        [TestAttr]
        public void CreateFailIfExists() => CreateFailIfExistsCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFailIfExistsAsync() => await CreateFailIfExistsCoreAsync(false, CancellationToken.None);
        private async Task CreateFailIfExistsCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            Assert.IsTrue(file.Exists);
        }

        [TestAttr]
        public void CreateFailIfExistsTwice() => CreateFailIfExistsTwiceCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task CreateFailIfExistsTwiceAsync() => await CreateFailIfExistsTwiceCoreAsync(false, CancellationToken.None);
        private async Task CreateFailIfExistsTwiceCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);
            Assert.IsTrue(file.Exists);

            if(sync)
                Assert.That(() => TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists), Throws.TypeOf<FileExistException>());
            else
                Assert.That(() => TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken), Throws.TypeOf<FileExistException>());
        }
    }
}