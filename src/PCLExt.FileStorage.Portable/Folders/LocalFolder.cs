namespace PCLExt.FileStorage.Folders
{
    /// <summary>
    /// A folder representing storage which is local to the current device.
    /// </summary>
    public class LocalFolder : BaseFolder
    {
#if MAC
		[System.Runtime.InteropServices.DllImport(ObjCRuntime.Constants.FoundationLibrary)]
		static extern System.IntPtr NSHomeDirectory(); // Under the sandbox, need to read the HomeDirectory

		static string MacHomeDirectory => ((Foundation.NSString)ObjCRuntime.Runtime.GetNSObject(NSHomeDirectory())).ToString();
#endif

        /// <summary>
        /// 
        /// </summary>
#if DESKTOP || ANDROID || __IOS__ || MAC
        public LocalFolder() : base(GetLocalFolder()) { }
        private static IFolder GetLocalFolder()
        {
#if ANDROID
				var storage = Android.App.Application.Context.GetExternalFilesDir(null)?.ParentFile?.AbsolutePath;
				if(string.IsNullOrEmpty(storage))
				    return null;
#elif __IOS__
				var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
				var storage = System.IO.Path.Combine(documents, "..", "Library");
#elif MAC
                // (non-sandboxed) /Users/foo/Library/Application Support/ProcessName/
                // (sandboxed) /Users/foo/Library/Containers/<AppId>/Data/Library/Application Support/ProcessName/
				var name = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
				var storage = System.IO.Path.Combine(MacHomeDirectory, "Library", "Application Support");
				if(!string.IsNullOrEmpty(name))
				{
				storage = System.IO.Path.Combine(storage, name);
				if(!System.IO.Directory.Exists(storage)) // Ensure it exists to stope FileSystemFolder from throwing exception.
				System.IO.Directory.CreateDirectory(storage);
				}
#elif DESKTOP
            var storage = System.Windows.Forms.Application.LocalUserAppDataPath; // SpecialFolder.LocalApplicationData is not app-specific, so use the Windows Forms API to get the app data path.
#endif
            return new NET4FolderImplementation(storage);
        }
#elif CORE
        public LocalFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInNetStandard();
#else
        public LocalFolder() : base(null) => throw Exceptions.ExceptionsHelper.NotImplementedInReferenceAssembly();
#endif
    }
}