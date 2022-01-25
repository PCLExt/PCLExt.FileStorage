using PCLExt.FileStorage.Extensions;
using System.Linq;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> where the app is running.
    /// </summary>
    public class CacheRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the <see cref="BaseFolder"/> where the app is running.
        /// </summary>
#if !PORTABLE
        public CacheRootFolder() : base(GetCacheFolder()) { }
        private static IFolder GetCacheFolder()
        {
#if ANDROID
            return new DefaultFolderImplementation(Android.App.Application.Context.CacheDir.AbsolutePath);
#elif __IOS__
            return new DefaultFolderImplementation(Foundation.NSSearchPath.GetDirectories(Foundation.NSSearchPathDirectory.CachesDirectory, Foundation.NSSearchPathDomain.User).FirstOrDefault());
#elif NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || __MACOS__
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)).CreateFolder("Cache", CreationCollisionOption.OpenIfExists);
#elif WINDOWS_UWP
            return new UWP.StorageFolderImplementation(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path);
#endif
        }
#else
        public CacheRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}
