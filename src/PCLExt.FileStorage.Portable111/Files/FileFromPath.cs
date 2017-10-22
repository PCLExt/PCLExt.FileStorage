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
            if(System.IO.File.Exists(path))
                return new DefaultFileImplementation(path);
            else
                throw new Exceptions.FileNotFoundException($"File does not exists on {path}");
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
