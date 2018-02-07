using PCLExt.FileStorage.Folders;

namespace PCLExt.FileStorage.Test
{
    internal class TestFolder : BaseFolder
    {
#if __ANDROID__ || __IOS__
        public TestFolder() : base(new LocalRootFolder().CreateFolder("Testing", CreationCollisionOption.OpenIfExists)) { }
#else
        public TestFolder() : base(new TempRootFolder().CreateFolder("Testing", CreationCollisionOption.OpenIfExists)) { }
#endif
    }
}