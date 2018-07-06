using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to create a folder fails because it already exists on disk.
    /// </summary>
    public class FolderExistException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.IOException
#endif
    {
        /// <exclude/>
        public FolderExistException(string message) : base(message) { }

        /// <exclude/>
        public FolderExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}