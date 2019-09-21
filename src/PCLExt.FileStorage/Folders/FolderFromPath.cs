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
        public FolderFromPath(string path, bool createIfNotExisting = false) : base(GetFolderFromPath(path, createIfNotExisting)) { }
        private static IFolder GetFolderFromPath(string path, bool createIfNotExisting = false)
        {
            Requires.NotNullOrEmpty(path, nameof(path));

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || ANDROID || __IOS__ || __MACOS__
            if (createIfNotExisting)
                System.IO.Directory.CreateDirectory(path);

            return System.IO.Directory.Exists(path) ? (IFolder) new DefaultFolderImplementation(path, true) : (IFolder) new NonExistingFolder(path);
#elif WINDOWS_UWP
            if (createIfNotExisting)
                System.IO.Directory.CreateDirectory(path); // TODO: Test, will it work on UWP?

            return new UWP.StorageFolderImplementation(path) is IFolder uwpFile && uwpFile.Exists ? uwpFile : new NonExistingFolder(path);
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
