using PCLExt.FileStorage.Extensions;
using System.Linq;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> where the app is running.
    /// </summary>
    public class VideosRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the <see cref="BaseFolder"/> where the app is running.
        /// </summary>
#if !PORTABLE
        public VideosRootFolder() : base(GetVideosFolder()) { }
        private static IFolder GetVideosFolder()
        {
#if ANDROID
            return new DefaultFolderImplementation(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMovies).AbsolutePath);
#elif __IOS__
            return new DefaultFolderImplementation(Foundation.NSSearchPath.GetDirectories(Foundation.NSSearchPathDirectory.MoviesDirectory, Foundation.NSSearchPathDomain.User).FirstOrDefault());
#elif NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || __MACOS__
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos));
#elif WINDOWS_UWP
            return null;
#endif
        }
#else
        public VideosRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}
