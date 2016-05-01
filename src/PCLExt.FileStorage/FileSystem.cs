//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Provides access to an implementation of <see cref="IFileSystem"/> for the current platform.
    /// </summary>
    public static class FileSystem
    {
		private static IFileSystem _fileSystem;

		/// <summary>
		/// The implementation of <see cref="IFileSystem"/> for the current platform.
		/// </summary>
		public static IFileSystem Current
		{
			get
			{
#if DESKTOP || ANDROID || __IOS__ || MAC
				if(_fileSystem == null)
				_fileSystem = new DesktopFileSystem();

				return _fileSystem;
#endif

				throw NotImplementedInReferenceAssembly();
			}
		}

        internal static Exception NotImplementedInReferenceAssembly() =>
			new NotImplementedException(@"This functionality is not implemented in the portable version of this assembly.
You should reference the PCLExt.FileStorage NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
