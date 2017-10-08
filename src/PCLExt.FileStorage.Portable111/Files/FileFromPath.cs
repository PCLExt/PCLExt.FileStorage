namespace PCLExt.FileStorage.Files
{
    /// <summary>
    /// Represents a folder created by a given path
    /// </summary>
    public class FileFromPath : BaseFile
    {
        /// <summary>
        /// Creates a new <see cref="IFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public FileFromPath(string path) : base(GetFileFromPath(path)) { }
        private static IFile GetFileFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC || NETSTANDARD2_0
            return System.IO.File.Exists(path) ? new DefaultFileImplementation(path) : null;
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
