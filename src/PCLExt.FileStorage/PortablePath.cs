//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using PCLExt.FileStorage.Exceptions;

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
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
                return System.IO.Path.DirectorySeparatorChar;
#endif

                throw ExceptionsHelper.NotImplementedInReferenceAssembly();
            }
        }

        /// <summary>
        /// The alterrnative character used to separate elements in a file system path.
        /// </summary>
        public static char AltDirectorySeparatorChar
        {
            get
            {
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
                return System.IO.Path.AltDirectorySeparatorChar;
#endif

                throw ExceptionsHelper.NotImplementedInReferenceAssembly();
            }
        }

        /// <summary>
        /// Combines multiple strings into a path.
        /// </summary>
        /// <param name="paths">Path elements to combine.</param>
        /// <returns>A combined path.</returns>
        public static string Combine(params string[] paths)
        {
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
			return System.IO.Path.Combine(paths);
#endif

            throw ExceptionsHelper.NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Returns the extension for the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to get the extension.</param>
        /// <returns>
        /// A <see cref="string" /> containing the extension of the specified 
        /// <paramref name="path" /> (including the "."), or an empty 
        /// <see cref="string" /> if <paramref name="path" /> does not have 
        /// extension information.
        /// </returns>
        /// <exception cref="System.ArgumentException"><paramref name="path" /> contains one or more invalid characters.</exception>
        public static string GetExtension(string path)
        {
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
            return System.IO.Path.GetExtension(path);
#endif

            throw ExceptionsHelper.NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Returns the filename for the specified path string.
        /// </summary>
        /// <param name="path">The path string from which to obtain the file name and extension.</param>
        /// <returns>
        /// <para>
        /// A <see cref="string" /> consisting of the characters after the last 
        /// directory character in path. 
        /// </para>
        /// <para>
        /// If the last character of <paramref name="path" /> is a directory or 
        /// volume separator character, an empty <see cref="string" /> is returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.ArgumentException"><paramref name="path" /> contains one or more invalid characters.</exception>
        public static string GetFileName(string path)
        {
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
            return System.IO.Path.GetFileName(path);
#endif

            throw ExceptionsHelper.NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Returns the filename without extension for the specified path string.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>
        /// A <see cref="string" /> containing the <see cref="string" /> returned 
        /// by <see cref="GetFileName" />, minus the last period (.) and all 
        /// characters following it.
        /// </returns>
        /// <exception cref="System.ArgumentException"><paramref name="path" /> contains one or more invalid characters.</exception>
        public static string GetFileNameWithoutExtension(string path)
        {
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
            return System.IO.Path.GetFileNameWithoutExtension(path);
#endif

            throw ExceptionsHelper.NotImplementedInReferenceAssembly();
        }

        /// <summary>
        /// Determines whether a path string includes an extension.
        /// </summary>
        /// <param name="path">The path to search for an extension.</param>
        /// <returns>
        /// <see langword="true" />. if the characters that follow the last 
        /// directory separator or volume separator in the <paramref name="path" /> 
        /// include a period (.) followed by one or more characters; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="System.ArgumentException"><paramref name="path" /> contains one or more invalid characters.</exception>
        public static bool HasExtension(string path)
        {
#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
            return System.IO.Path.HasExtension(path);
#endif

            throw ExceptionsHelper.NotImplementedInReferenceAssembly();
        }
    }
}
