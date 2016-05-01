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
    /// Provides portable versions of APIs such as Path.Combine.
    /// </summary>
    public static class PortablePath
    {
        /// <summary>
        /// The character used to separate elements in a file system path.
        /// </summary>
        public static char DirectorySeparatorChar
        {
            get
            {
#if DESKTOP || ANDROID || __IOS__ || MAC
				return System.IO.Path.DirectorySeparatorChar;
#endif

				throw FileSystem.NotImplementedInReferenceAssembly();
            }
        }

        /// <summary>
        /// Combines multiple strings into a path.
        /// </summary>
        /// <param name="paths">Path elements to combine.</param>
        /// <returns>A combined path.</returns>
        public static string Combine(params string[] paths)
        {
#if DESKTOP || ANDROID || __IOS__ || MAC
			return System.IO.Path.Combine(paths);
#endif

			throw FileSystem.NotImplementedInReferenceAssembly();
        }
    }
}
