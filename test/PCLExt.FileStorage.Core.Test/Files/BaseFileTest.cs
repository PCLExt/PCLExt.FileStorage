using System.Threading.Tasks;

using NUnit.Framework;

namespace PCLExt.FileStorage.Test
{
    public abstract class BaseFileTest
    {
        protected const string FileName1 = "TestFile1";
        protected const string FileName2 = "TestFile2";

        protected static IFolder TestFolder => new TestFolder();

        [SetUp]
        [TearDown]
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