using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder representing storage which may be synced with other devices for the same user.
    /// </summary>
    public class RoamingRootFolder : BaseFolder
    {
        //public bool IsSynchronizing { get; }

        /// <summary>
        /// Creates a folder representing storage which may be synced with other devices for the same user.
        /// </summary>
#if !PORTABLE
        public RoamingRootFolder() : base(GetRoamingFolder()) { }
        private static IFolder GetRoamingFolder()
        {
#if ANDROID || __IOS__
            return null;
#elif DESKTOP || MAC || NETSTANDARD2_0
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)).GetDataFolder();
#elif WINDOWS_UWP
            return null;
            return new DefaultFolderImplementation(Windows.Storage.ApplicationData.Current.RoamingFolder.Path);
#endif
        }
#else
        public RoamingRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}