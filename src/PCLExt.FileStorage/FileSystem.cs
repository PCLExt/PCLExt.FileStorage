//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Provides access to an implementation of <see cref="IFileSystem"/> for the current platform.
    /// </summary>
    public static class FileSystem
    {
        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException(@"This functionality is not implemented in the portable version of this assembly.
You should reference the PCLExt.FileStorage NuGet package from your main application project in order to reference the platform-specific implementation.");


        /// <summary>
        /// The implementation of <see cref="IFileSystem"/> for the current platform.
        /// </summary>
        private static IFileSystem Current
		{
			get
			{
#if DESKTOP || ANDROID || __IOS__ || MAC
			    return _fileSystem ?? (_fileSystem = new DesktopFileSystem());
#endif

				throw NotImplementedInReferenceAssembly();
			}
		}
        private static IFileSystem _fileSystem;


#if MAC
		[System.Runtime.InteropServices.DllImport(ObjCRuntime.Constants.FoundationLibrary)]
		static extern System.IntPtr NSHomeDirectory(); // Under the sandbox, need to read the HomeDirectory

		static string MacHomeDirectory => ((Foundation.NSString)ObjCRuntime.Runtime.GetNSObject(NSHomeDirectory())).ToString();
#endif

        /// <summary>
        /// A folder representing storage which is where the app is running.
        /// </summary>
        public static IFolder BaseStorage => Current.BaseStorage;

        /// <summary>
        /// A folder representing storage which is local to the current device.
        /// </summary>
        public static IFolder LocalStorage => Current.LocalStorage;

        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user.
        /// </summary>
        public static IFolder RoamingStorage => Current.RoamingStorage;




        /// <summary>
        /// Gets a file, given its path.  Returns null if the file does not exist.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <returns>A file for the given path, or null if it does not exist.</returns>
        public static IFile GetFileFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC
            if (System.IO.File.Exists(path))
                return new FileSystemFile(path);

            return null;
#endif

            throw NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Gets a file, given its path.  Returns null if the file does not exist.
        /// </summary>
        /// <param name="path">The path to a file, as returned from the <see cref="IFile.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A file for the given path, or null if it does not exist.</returns>
        public static async Task<IFile> GetFileFromPathAsync(string path, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            if (System.IO.File.Exists(path))
                return new FileSystemFile(path);

            return null;
#endif

            throw NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Gets a folder, given its path.  Returns null if the folder does not exist.
        /// </summary>
        /// <param name="path">The path to a folder, as returned from the <see cref="IFolder.Path"/> property.</param>
        /// <returns>A folder for the specified path, or null if it does not exist.</returns>
        public static IFolder GetFolderFromPath(string path)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC
            if (System.IO.Directory.Exists(path))
                return new FileSystemFolder(path, true);

            return null;
#endif

            throw NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Gets a folder, given its path.  Returns null if the folder does not exist.
        /// </summary>
        /// <param name="path">The path to a folder, as returned from the <see cref="IFolder.Path"/> property.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A folder for the specified path, or null if it does not exist.</returns>
        public static async Task<IFolder> GetFolderFromPathAsync(string path, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(path, "path");

#if DESKTOP || ANDROID || __IOS__ || MAC
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            if (System.IO.Directory.Exists(path))
                return new FileSystemFolder(path, true);

            return null;
#endif

            throw NotImplementedInReferenceAssembly();
        }
    }
}
