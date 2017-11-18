#if WINDOWS_UWP
using PCLExt.FileStorage.UWP;
#endif

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

#if DESKTOP || ANDROID || __IOS__ || __MACOS__ || NETSTANDARD2_0
            if (System.IO.File.Exists(path))
                return new DefaultFileImplementation(path);
            else
                throw new Exceptions.FileNotFoundException($"File does not exists on {path}");
#elif WINDOWS_UWP
            var result = new StorageFileImplementation(path);
            if(!result.Exists)
                throw new Exceptions.FileNotFoundException($"File does not exists on {path}");
            return result;
#endif

            throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
