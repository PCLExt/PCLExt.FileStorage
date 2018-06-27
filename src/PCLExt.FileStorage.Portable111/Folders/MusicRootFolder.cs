using PCLExt.FileStorage.Extensions;
using System.Linq;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> where the app is running.
    /// </summary>
    public class MusicRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the <see cref="BaseFolder"/> where the app is running.
        /// </summary>
#if !PORTABLE
        public MusicRootFolder() : base(GetMusicFolder()) { }
        private static IFolder GetMusicFolder()
        {
#if ANDROID
            return new DefaultFolderImplementation(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath);
#elif __IOS__
            return new DefaultFolderImplementation(Foundation.NSSearchPath.GetDirectories(Foundation.NSSearchPathDirectory.MusicDirectory, Foundation.NSSearchPathDomain.User).FirstOrDefault());
#elif DESKTOP || __MACOS__ || NETSTANDARD2_0
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic));
#elif WINDOWS_UWP
            return null;
#endif
        }
#else
        public MusicRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}
