namespace PCLExt.FileStorage.Files
{
    /// <summary>
    /// Represents a a <see cref="BaseFile"/> created by a given path.
    /// </summary>
    public class FileFromPath : BaseFile
    {
        /// <summary>
        /// Creates a new <see cref="BaseFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="paths">An array of parts of the path.</param>
        public FileFromPath(params string[] paths) : base(GetFileFromPath(System.IO.Path.Combine(paths))) { }

        /// <summary>
        /// Creates a new <see cref="BaseFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public FileFromPath(string path, bool createIfNotExisting = false) : base(GetFileFromPath(path, createIfNotExisting)) { }
        private static IFile GetFileFromPath(string path, bool createIfNotExisting = false)
        {
            Requires.NotNullOrEmpty(path, nameof(path));

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || ANDROID || __IOS__ || __MACOS__
            if(createIfNotExisting)
                System.IO.File.Create(path).Dispose();

            return System.IO.File.Exists(path) ? (IFile) new DefaultFileImplementation(path) : (IFile) new NonExistingFile(path);
#elif WINDOWS_UWP
            if (createIfNotExisting)
                System.IO.File.Create(path).Dispose(); // TODO: Test, will it work on UWP?

            return new UWP.StorageFileImplementation(path) is IFile uwpFile && uwpFile.Exists ? uwpFile : new NonExistingFile(path);
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
