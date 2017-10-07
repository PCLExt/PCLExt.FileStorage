using PCLExt.FileStorage.Folders;

namespace PCLExt.FileStorage.Test
{
    internal class TestFolder : BaseFolder
    {
#if __ANDROID__
        public TestFolder() : base(new LocalStorageFolder().CreateFolder("Testing", CreationCollisionOption.OpenIfExists)) { }
#else
        public TestFolder() : base(new ApplicationFolder().CreateFolder("Testing", CreationCollisionOption.OpenIfExists)) { }
#endif
    }
}