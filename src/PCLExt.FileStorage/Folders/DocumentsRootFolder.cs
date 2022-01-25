using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> where the app is running.
    /// </summary>
    public class DocumentsRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the <see cref="BaseFolder"/> where the app is running.
        /// </summary>
#if !PORTABLE
        public DocumentsRootFolder() : base(GetDocumentsFolder()) { }
        private static IFolder GetDocumentsFolder()
        {
#if ANDROID || __IOS__
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments));
#elif NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)).GetDataFolder();
#elif WINDOWS_UWP            
            return new UWP.StorageFolderImplementation(Windows.Storage.KnownFolders.DocumentsLibrary);
#endif
        }
#else
        public DocumentsRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}
