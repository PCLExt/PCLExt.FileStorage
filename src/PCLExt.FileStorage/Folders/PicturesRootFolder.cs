using PCLExt.FileStorage.Extensions;
using System.Linq;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A <see cref="BaseFolder"/> where the app is running.
    /// </summary>
    public class PicturesRootFolder : BaseFolder
    {
        /// <summary>
        /// Returns the <see cref="BaseFolder"/> where the app is running.
        /// </summary>
#if !PORTABLE
        public PicturesRootFolder() : base(GetPicturesFolder()) { }
        private static IFolder GetPicturesFolder()
        {
#if ANDROID
            return new DefaultFolderImplementation(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath);
#elif __IOS__
            return new DefaultFolderImplementation(Foundation.NSSearchPath.GetDirectories(Foundation.NSSearchPathDirectory.PicturesDirectory, Foundation.NSSearchPathDomain.User).FirstOrDefault());
#elif NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || __MACOS__
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures));
#elif WINDOWS_UWP
            return null;
#endif
        }
#else
        public PicturesRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}
