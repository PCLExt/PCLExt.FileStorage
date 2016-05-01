//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace PCLExt.FileStorage.Exceptions
{
    /// <exclude/>
    public class FileNotFoundException
#if PORTABLE
        : IOException
#else
        : System.IO.FileNotFoundException
#endif
    {
        /// <exclude/>
        public FileNotFoundException(string message) : base(message) { }

        /// <exclude/>
        public FileNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <exclude/>
    public class DirectoryNotFoundException
#if PORTABLE
        : IOException
#elif NETFX_CORE
        : System.IO.FileNotFoundException
#else
        : System.IO.DirectoryNotFoundException
#endif
    {
        /// <exclude/>
        public DirectoryNotFoundException(string message) : base(message) { }

        /// <exclude/>
        public DirectoryNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
