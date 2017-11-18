using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder representing storage which is local to the current device.
    /// </summary>
    public class LocalRootFolder : BaseFolder
    {
        //public bool IsBackuping { get; }

#if __MACOS__
		[System.Runtime.InteropServices.DllImport(ObjCRuntime.Constants.FoundationLibrary)]
		static extern System.IntPtr NSHomeDirectory(); // Under the sandbox, need to read the HomeDirectory

		static string MacHomeDirectory => ((Foundation.NSString) ObjCRuntime.Runtime.GetNSObject(NSHomeDirectory())).ToString();
#endif

        /// <summary>
        /// Creates a folder representing storage which is local to the current device
        /// </summary>
#if !PORTABLE
        public LocalRootFolder() : base(GetLocalFolder()) { }
        private static IFolder GetLocalFolder()
        {
#if ANDROID
            var storage = Android.App.Application.Context.GetExternalFilesDir(null)?.ParentFile?.AbsolutePath;
            return string.IsNullOrEmpty(storage) ? null : new DefaultFolderImplementation(storage);
#elif __IOS__
            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var storage = System.IO.Path.Combine(documents, "..", "Library");
            return new DefaultFolderImplementation(storage);
#elif __MACOS__
            // (non-sandboxed) /Users/foo/Library/Application Support/ProcessName/
            // (sandboxed) /Users/foo/Library/Containers/<AppId>/Data/Library/Application Support/ProcessName/
            var name = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            var storage = System.IO.Path.Combine(MacHomeDirectory, "Library", "Application Support");
            if (!string.IsNullOrEmpty(name))
            {
                storage = System.IO.Path.Combine(storage, name);
                if (!System.IO.Directory.Exists(storage)) // Ensure it exists to stope FileSystemFolder from throwing exception.
                    System.IO.Directory.CreateDirectory(storage);
            }
            return new DefaultFolderImplementation(storage);
#elif DESKTOP || NETSTANDARD2_0
            return new DefaultFolderImplementation(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)).GetDataFolder();
#elif WINDOWS_UWP
            return new UWP.StorageFolderImplementation(
                Windows.Storage.ApplicationData.Current.LocalFolder);
#endif
        }
#else
        public LocalRootFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}