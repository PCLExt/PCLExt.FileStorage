namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> representing storage that deals with temporary files.
    /// </summary>
    public class TempRootFolder : BaseFolder
    {
        /// <summary>
        /// Creates a <see cref="BaseFolder"/> representing storage that deals with temporary files.
        /// </summary>
#if !PORTABLE
        public TempRootFolder() : base(GetTempFolder()) { }
        private static IFolder GetTempFolder()
        {
#if WINDOWS_UWP
             return new UWP.StorageFolderImplementation(Windows.Storage.ApplicationData.Current.TemporaryFolder);
#else
            return new DefaultFolderImplementation(System.IO.Path.GetTempPath());
#endif
        }

        /// <summary>
        /// Creates a unique temporary file with given or default extension.
        /// </summary>
        /// <param name="extension">Name of the extension, without the dot at the beginning.</param>
        /// <returns></returns>
        public IFile CreateTempFile(string extension = "tmp")
        {
            string fileName;
            do
            {
                fileName = $"{System.Guid.NewGuid()}.{extension}";
            } while (System.IO.File.Exists(System.IO.Path.Combine(Path, fileName)));
            return CreateFile(fileName, CreationCollisionOption.FailIfExists);  // -- Potential race condition?
        }
#else
        public TempRootFolder() : base(new NonExistingFolder(string.Empty)) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}