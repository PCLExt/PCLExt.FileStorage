namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// Represents a folder created by a given path
    /// </summary>
    public class FolderFromPath : BaseFolder
    {
        /// <summary>
        /// Creates a new <see cref="IFolder"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public FolderFromPath(string path) : base(GetFolderFromPath(path)) { }
        private static IFolder GetFolderFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || __MACOS__ || NETSTANDARD2_0
            return System.IO.Directory.Exists(path) ? new DefaultFolderImplementation(path, true) : null;
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
