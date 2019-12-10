using NUnit.Framework;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

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
    public class FileDeleteTest : BaseFileTest
    {
        [TestAttr]
        public void Delete() => DeleteCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task DeleteAsync() => await DeleteCoreAsync(false, CancellationToken.None);
        private async Task DeleteCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                file.Delete();
            else
                await file.DeleteAsync(cancellationToken);
            Assert.IsFalse(file.Exists);
        }

        [TestAttr]
        public void DeleteTwice() => DeleteTwiceCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task DeleteTwiceAsync() => await DeleteTwiceCoreAsync(false, CancellationToken.None);
        private async Task DeleteTwiceCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            if (sync)
                file.Delete();
            else
                await file.DeleteAsync(cancellationToken);
            Assert.IsFalse(file.Exists);

            if (sync)
                Assert.That(() => file.Delete(), Throws.TypeOf<FileNotFoundException>());
            else
                Assert.That(() => file.DeleteAsync(), Throws.TypeOf<FileNotFoundException>());
        }
    }
}