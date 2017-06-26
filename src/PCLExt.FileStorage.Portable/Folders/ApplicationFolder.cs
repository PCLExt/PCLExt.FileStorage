namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder where the app is running.
    /// </summary>
    public class ApplicationFolder : BaseFolder
    {
        /// <summary>
        /// Returns the folder where the app is running.
        /// </summary>
#if DESKTOP || ANDROID || __IOS__ || MAC || NETSTANDARD2_0
        public ApplicationFolder() : base(GetApplicationFolder()) { }
        private static IFolder GetApplicationFolder()
        {
#if ANDROID || __IOS__
            return null;
#elif DESKTOP || MAC
            return new DefaultFolderImplementation(System.AppDomain.CurrentDomain.BaseDirectory);
#elif NETSTANDARD2_0
            return new DefaultFolderImplementation(System.AppContext.BaseDirectory);
#endif
        }
#else
        public ApplicationFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}