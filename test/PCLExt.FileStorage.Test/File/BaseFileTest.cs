#if WINDOWS_UWP
using SetUpAttr = Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute;
using TearDownAttr = Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute;
#else
using SetUpAttr = NUnit.Framework.SetUpAttribute;
using TearDownAttr = NUnit.Framework.TearDownAttribute;
#endif

namespace PCLExt.FileStorage.Test.File
{
    public abstract class BaseFileTest
    {
        protected const string FileName1 = "TestFile1";
        protected const string FileName2 = "TestFile2";

        protected static IFolder TestFolder => new TestFolder();

        [SetUpAttr]
        [TearDownAttr]
        public void Clean()
        {
            if (TestFolder.CheckExists(FileName1) == ExistenceCheckResult.FileExists)
                TestFolder.GetFile(FileName1).Delete();

            if (TestFolder.CheckExists(FileName2) == ExistenceCheckResult.FileExists)
                TestFolder.GetFile(FileName2).Delete();

            TestFolder.Delete();
        }
    }
}