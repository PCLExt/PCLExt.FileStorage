namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder representing storage which is where the app is running.
    /// </summary>
    public class ApplicationFolder : BaseFolder
    {
        /// <summary>
        /// 
        /// </summary>
#if DESKTOP || ANDROID || __IOS__ || MAC
        public ApplicationFolder() : base(GetApplicationFolder()) { }
        private static IFolder GetApplicationFolder()
        {
#if ANDROID || __IOS__
				var storage = "";
				return null;
#elif DESKTOP || MAC
            var storage = System.AppDomain.CurrentDomain.BaseDirectory;
#endif
            return new NET4FolderImplementation(storage);
        }
#elif CORE
        public ApplicationFolder() : base(new NETCOREFolderImplementation(System.AppContext.BaseDirectory)) { }
#else
        public ApplicationFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}