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
        private static readonly Lazy<IFileSystem> _instance = new Lazy<IFileSystem>(CreateInstance, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        private static IFileSystem CreateInstance()
        {
#if COMMON
            return new DesktopFileSystem();
#else
            return null;
#endif
        }

        /// <summary>
        /// The implementation of <see cref="IFileSystem"/> for the current platform.
        /// </summary>
        public static IFileSystem Current
        {
            get
            {
                var ret = _instance.Value;
                if (ret == null)
                    throw NotImplementedInReferenceAssembly();
                
                return ret;
            }
        }

        internal static Exception NotImplementedInReferenceAssembly() => new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the PCLStorage NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
