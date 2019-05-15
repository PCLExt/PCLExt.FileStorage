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
        public FileFromPath(string path) : base(GetFileFromPath(path)) { }
        private static IFile GetFileFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, nameof(path));

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || ANDROID || __IOS__ || __MACOS__
            if (System.IO.File.Exists(path))
                return new DefaultFileImplementation(path);
            else
                throw new Exceptions.FileNotFoundException($"File does not exists on {path}");
#elif WINDOWS_UWP
            var result = new UWP.StorageFileImplementation(path);
            if(!result.Exists)
                throw new Exceptions.FileNotFoundException($"File does not exists on {path}");
            return result;
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
