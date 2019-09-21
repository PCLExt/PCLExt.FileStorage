//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to access a folder that does not exist on disk fails.
    /// </summary>
    public class FolderNotFoundException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.DirectoryNotFoundException
#endif
    {
        /// <exclude/>
        public FolderNotFoundException() : base() { }

        /// <exclude/>
        public FolderNotFoundException(string message) : base(message) { }

        /// <exclude/>
        public FolderNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}