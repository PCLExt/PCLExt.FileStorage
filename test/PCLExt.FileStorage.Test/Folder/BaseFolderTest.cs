#if WINDOWS_UWP
using SetUpAttr = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using TearDownAttr = Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute;
#else
using SetUpAttr = NUnit.Framework.SetUpAttribute;
using TearDownAttr = NUnit.Framework.TearDownAttribute;
#endif

namespace PCLExt.FileStorage.Test.Folder
{
    public class BaseFolderTest
    {
        protected const string FolderName1 = "TestFolder1";
        protected const string FolderName2 = "TestFolder2";
        protected const string FileName1 = "TestFile1";
        protected const string FileName2 = "TestFile2";

        [SetUpAttr]
        [TearDownAttr]
        public void Clean() => new TestFolder().Delete();
    }
}
