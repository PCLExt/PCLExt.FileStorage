using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> representing storage which may be synced with other devices for the same user.
    /// Does not exist in Android and iOS.
    /// </summary>
    public class RoamingRootFolder : BaseFolder
    {
        //public bool IsSynchronizing { get; }

        /// <summary>
        /// Creates a <see cref="BaseFolder"/> representing storage which may be synced with other devices for the same user.
        /// </summary>
#if !PORTABLE
        public RoamingRootFolder() : base(GetRoamingFolder()) { }
        private static IFolder GetRoamingFolder()
        {
#if ANDROID || __IOS__
            return null;
#elif DESKTOP || __MACOS__ || NETSTANDARD2_0
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)).GetDataFolder();
#elif WINDOWS_UWP
            return new UWP.StorageFolderImplementation(
                Windows.Storage.ApplicationData.Current.RoamingFolder);
#endif
        }
#else
        public RoamingRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}