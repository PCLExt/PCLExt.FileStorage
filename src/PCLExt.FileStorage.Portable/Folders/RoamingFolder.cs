namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder representing storage which may be synced with other devices for the same user.
    /// </summary>
    public class RoamingFolder : BaseFolder
    {
        /// <summary>
        /// 
        /// </summary>
#if DESKTOP || ANDROID || __IOS__ || MAC
        public RoamingFolder() : base(GetRoamingFolder()) { }
        private static IFolder GetRoamingFolder()
        {
#if ANDROID
				var storage = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

#elif __IOS__ || MAC
				var storage = "";
				return null;
#elif DESKTOP
            var storage = System.Windows.Forms.Application.UserAppDataPath; // SpecialFolder.ApplicationData is not app-specific, so use the Windows Forms API to get the app data path.
#endif
            return new NET4FolderImplementation(storage);
        }
#elif CORE
        public RoamingFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInNetStandard();
#else
        public RoamingFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}