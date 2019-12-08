namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> where the application is running.
    /// </summary>
    public class ApplicationRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the <see cref="BaseFolder"/> where the application is running.
        /// Does not exist in Android and UWP
        /// </summary>
#if !PORTABLE
        public ApplicationRootFolder() : base(GetApplicationFolder()) { }
        private static IFolder GetApplicationFolder()
        {
#if ANDROID
            return new NonExistingFolder("");
#elif __IOS__
            return new DefaultFolderImplementation(Foundation.NSBundle.MainBundle.BundlePath);
#elif NETFX45 || __MACOS__
            return new DefaultFolderImplementation(System.AppDomain.CurrentDomain.BaseDirectory);
#elif NETSTANDARD2_0 || NETCOREAPP2_0
            // As of today, when using -p:PublishSingleFile=True, System.AppContext.BaseDirectory will point to %TEMP%/.net where the unpacked
            // application is stored. We should get the path to the actual packed .exe instead
            var moduleFilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            return new DefaultFolderImplementation(System.IO.Path.GetDirectoryName(moduleFilePath));
            //return new DefaultFolderImplementation(System.AppContext.BaseDirectory);
#elif WINDOWS_UWP
            return new UWP.StorageFolderImplementation(
                Windows.ApplicationModel.Package.Current.InstalledLocation);
#endif
        }
#else
        public ApplicationRootFolder() : base(new NonExistingFolder(string.Empty)) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}