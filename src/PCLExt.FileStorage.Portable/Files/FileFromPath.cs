namespace PCLExt.FileStorage.Files
{
    public class FileFromPath : BaseFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public FileFromPath(string path) : base(GetFileFromPath(path)) { }
        private static IFile GetFileFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC
            return System.IO.File.Exists(path) ? new NET4FileImplementation(path) : null;
#elif CORE
            return System.IO.File.Exists(path) ? new NETCOREFileImplementation(path) : null;
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
