namespace PCLExt.FileStorage.Folders
{
    public class FolderFromPath : BaseFolder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public FolderFromPath(string path) : base(GetFolderFromPath(path)) { }
        private static IFolder GetFolderFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC
            return System.IO.Directory.Exists(path) ? new NET4FolderImplementation(path, true) : null;

#elif CORE
            return System.IO.Directory.Exists(path) ? new NETCOREFolderImplementation(path, true) : null;
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
