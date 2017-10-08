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
    /// <exclude/>
    public class FileNotFoundException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.FileNotFoundException
#endif
    {
        /// <exclude/>
        public FileNotFoundException(string message) : base(message) { }

        /// <exclude/>
        public FileNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}