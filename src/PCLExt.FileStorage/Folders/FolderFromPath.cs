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
        /// <param name="createIfNotExisting">Create folder automatically</param>
        public FolderFromPath(string path, bool createIfNotExisting = false) : base(GetFolderFromPath(path, createIfNotExisting)) { }
        private static IFolder GetFolderFromPath(string path, bool createIfNotExisting = false)
        {
            Requires.NotNullOrEmpty(path, nameof(path));

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || ANDROID || __IOS__ || __MACOS__
            var folder = new DefaultFolderImplementation(path, true);

            if (!folder.Exists && createIfNotExisting)
                System.IO.Directory.CreateDirectory(path);

            return folder.Exists ? (IFolder) folder : (IFolder) new NonExistingFolder(path);
#elif WINDOWS_UWP
            var uwpFolder = new UWP.StorageFolderImplementation(path);

            if (!uwpFolder.Exists && createIfNotExisting)
                System.IO.Directory.CreateDirectory(path); // TODO: Test, will it work on UWP?

            return uwpFolder.Exists ? (IFolder) uwpFolder : (IFolder) new NonExistingFolder(path);
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
