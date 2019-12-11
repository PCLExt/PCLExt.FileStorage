using System.IO;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
#endif

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

#if __ANDROID__ || __IOS__

using RootFolder = PCLExt.FileStorage.Folders.LocalRootFolder;

#else

using RootFolder = PCLExt.FileStorage.Folders.ApplicationRootFolder;

#endif

namespace PCLExt.FileStorage.Test
{
#if WINDOWS_UWP
    [TestClass]
#else
    [TestFixture]
#endif
    public class FoldersTest
    {
        private const string FolderName1 = "TestFolder1";
        private const string FolderName2 = "TestFolder2";
        private const string FileName1 = "TestFile1";
        private const string FileName2 = "TestFile2";

#if WINDOWS_UWP
        [TestCleanup]
        [TestInitialize]
#else
        [SetUp]
        [TearDown]
#endif
        public void Clean() => new TestFolder().Delete();




    }
}