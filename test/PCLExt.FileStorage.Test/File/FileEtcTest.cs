using NUnit.Framework;

using PCLExt.FileStorage.Extensions;

using System;
using System.IO;
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
    public class FileEtcTest : BaseFileTest
    {
        [TestAttr]
        public void FileFromPath() => FileFromPathCoreAsync(true, CancellationToken.None).RunSync();
        [TestAttr]
        public async Task FileFromPathAsync() => await FileFromPathCoreAsync(false, CancellationToken.None);
        private async Task FileFromPathCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            var file = TestFolder.GetFolderFromPath(Path.Combine("Folder1", FileName2));
            Assert.IsTrue(file.Exists);
            Assert.IsTrue(string.Equals(file.Path, Path.Combine(new TestFolder().Path, "Folder1", FileName2), StringComparison.Ordinal));
        }
    }
}