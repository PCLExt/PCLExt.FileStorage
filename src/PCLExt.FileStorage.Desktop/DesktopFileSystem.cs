//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

namespace PCLExt.FileStorage
{
	/// <summary>
	/// Implementation of <see cref="IFileSystem"/> over classic .NET file I/O APIs.
	/// </summary>
	public class DesktopFileSystem : IFileSystem
	{
#if MAC
		[System.Runtime.InteropServices.DllImport(ObjCRuntime.Constants.FoundationLibrary)]
		static extern System.IntPtr NSHomeDirectory(); // Under the sandbox, need to read the HomeDirectory

		static string MacHomeDirectory => ((Foundation.NSString)ObjCRuntime.Runtime.GetNSObject(NSHomeDirectory())).ToString();
#endif

        /// <inheritdoc />
        public IFolder BaseStorage
		{
			get
			{
#if ANDROID || __IOS__
				var storage = "";
				return null;
#elif DESKTOP || MAC
				var storage = System.AppDomain.CurrentDomain.BaseDirectory;
#endif
				return new FileSystemFolder(storage);
			}
		}

        /// <inheritdoc />
        public IFolder LocalStorage
		{
			get
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
				return new FileSystemFolder(storage);
			}
		}

        /// <inheritdoc />
        public IFolder RoamingStorage
		{
			get
			{
#if ANDROID 
				var storage = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

#elif __IOS__ || MAC
				var storage = "";
				return null;
#elif DESKTOP
				var storage = System.Windows.Forms.Application.UserAppDataPath; // SpecialFolder.ApplicationData is not app-specific, so use the Windows Forms API to get the app data path.
#endif
				return new FileSystemFolder(storage);
			}
		}

	    /// <inheritdoc />
	    public IFolder SpecialStorage
        {
            get
            {
#if DESKTOP || MAC
				return BaseStorage;

#elif ANDROID || __IOS__
                return LocalStorage;
#endif

                return null;
            }
        }
    }
}
