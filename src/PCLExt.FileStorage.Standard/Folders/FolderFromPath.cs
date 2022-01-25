namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// Represents a <see cref="BaseFolder"/> created by a given path
    /// </summary>
    public class FolderFromPath : BaseFolder
    {
        /// <summary>
        /// Creates a new <see cref="BaseFolder"/> corresponding to the specified path.
        /// </summary>
        /// <param name="paths">An array of parts of the path.</param>
        public FolderFromPath(params string[] paths) : base(GetFolderFromPath(System.IO.Path.Combine(paths))) { }

        /// <summary>
        /// Creates a new <see cref="BaseFolder"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public FolderFromPath(string path) : base(GetFolderFromPath(path)) { }
        private static IFolder GetFolderFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || ANDROID || __IOS__ || __MACOS__
            return System.IO.Directory.Exists(path) ? new DefaultFolderImplementation(path, true) : null;
#elif WINDOWS_UWP
            return new UWP.StorageFolderImplementation(path);
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
