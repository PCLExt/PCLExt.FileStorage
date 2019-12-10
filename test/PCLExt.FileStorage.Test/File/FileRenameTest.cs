using NUnit.Framework;

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
    public class FileRenameTest : BaseFileTest
    {
        [TestAttr]
        public void Rename() => RenameCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task RenameAsync() => await RenameCoreAsync(false, CancellationToken.None);
        private async Task RenameCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = sync
                ? TestFolder.CreateFile(FileName1, CreationCollisionOption.FailIfExists)
                : await TestFolder.CreateFileAsync(FileName1, CreationCollisionOption.FailIfExists, cancellationToken);

            var newFile = sync ? file.Rename(FileName2) : await file.RenameAsync(FileName2);
            Assert.IsFalse(file.Exists);
            Assert.IsTrue(newFile.Exists);
            Assert.IsFalse(string.Equals(file.Name, newFile.Name, StringComparison.Ordinal));
        }
    }
}