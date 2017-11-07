namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder where the app is running.
    /// </summary>
    public class ApplicationRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the folder where the app is running.
        /// </summary>
#if !PORTABLE
        public ApplicationRootFolder() : base(GetApplicationFolder()) { }
        private static IFolder GetApplicationFolder()
        {
#if ANDROID
            return null;
#elif __IOS__
            return new DefaultFolderImplementation(Foundation.NSBundle.MainBundle.BundlePath);
#elif DESKTOP || __MACOS__
            return new DefaultFolderImplementation(System.AppDomain.CurrentDomain.BaseDirectory);
#elif NETSTANDARD2_0
            return new DefaultFolderImplementation(System.AppContext.BaseDirectory);
#elif WINDOWS_UWP
            return null;
#endif
        }
#else
        public ApplicationRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}