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
    /// The exception that is thrown when an attempt to access a file that does not exist on disk fails.
    /// </summary>
#if !PORTABLE
    [Serializable]
#endif
    public class FileNotFoundException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.FileNotFoundException
#endif
    {
        /// <exclude/>
        public FileNotFoundException() : base() { }

        /// <exclude/>
        public FileNotFoundException(string message) : base(message) { }

        /// <exclude/>
        public FileNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <exclude/>
        public FileNotFoundException(string message, string fileName)
#if !PORTABLE
            : base(message, fileName)
#endif
        { }

        /// <exclude/>
        public FileNotFoundException(string message, string fileName, Exception innerException)
#if !PORTABLE
            : base(message, fileName, innerException)
#endif
        { }

#if !PORTABLE
        protected FileNotFoundException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) { }
#endif
    }
}