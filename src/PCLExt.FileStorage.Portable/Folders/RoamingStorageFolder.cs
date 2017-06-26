using System;

using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder representing storage which may be synced with other devices for the same user.
    /// </summary>
    public class RoamingStorageFolder : BaseFolder
    {
        /// <summary>
        /// Creates a folder representing storage which may be synced with other devices for the same user.
        /// </summary>
#if DESKTOP || ANDROID || __IOS__ || MAC || NETSTANDARD2_0
        public RoamingStorageFolder() : base(GetRoamingFolder()) { }
        private static IFolder GetRoamingFolder()
        {
#if ANDROID || __IOS__
            return null;
#elif DESKTOP || MAC || NETSTANDARD2_0
            return new DefaultFolderImplementation(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).GetDataFolder();
#endif
        }
#else
        public RoamingStorageFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}