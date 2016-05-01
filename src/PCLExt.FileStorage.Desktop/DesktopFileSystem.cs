//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

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

		/// <summary>
		/// A folder representing storage which is where the app is running.
		/// </summary>
		public IFolder BaseStorage
		{
			get
			{
#if ANDROID || __IOS__ || OSX
				var storage = "";
				return null;
#elif DESKTOP || MAC
				var storage = System.AppDomain.CurrentDomain.BaseDirectory;
#endif
				return new FileSystemFolder(storage);
			}
		}

		/// <summary>
		/// A folder representing storage which is local to the current device.
		/// </summary>
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

		/// <summary>
		/// A folder representing storage which may be synced with other devices for the same user.
		/// </summary>
		public IFolder RoamingStorage
		{
			get
			{
#if ANDROID 
				var storage = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

#elif __IOS__ || OSX || MAC
				var storage = "";
				return null;
#elif DESKTOP
				var storage = System.Windows.Forms.Application.UserAppDataPath; // SpecialFolder.ApplicationData is not app-specific, so use the Windows Forms API to get the app data path.
#endif
				return new FileSystemFolder(storage);
			}
		}

		/// <summary>
		/// Gets a file, given its path.  Returns null if the file does not exist.
		/// </summary>
		/// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A file for the given path, or null if it does not exist.</returns>
		public async Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken)
		{
			Requires.NotNullOrEmpty(path, "path");

			await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
			if (System.IO.File.Exists(path))
				return new FileSystemFile(path);

			return null;
		}

		/// <summary>
		/// Gets a folder, given its path.  Returns null if the folder does not exist.
		/// </summary>
		/// <param name="path">The path to a folder, as returned from the <see cref="IFolder.Path"/> property.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A folder for the specified path, or null if it does not exist.</returns>
		public async Task<IFolder> GetFolderFromPathAsync(string path, CancellationToken cancellationToken)
		{
			Requires.NotNullOrEmpty(path, "path");

			await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
			if (System.IO.Directory.Exists(path))
				return new FileSystemFolder(path, true);

			return null;
		}
	}
}
