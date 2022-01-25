using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to delete a root folder fails.
    /// </summary>
    public class RootFolderDeletionException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.IOException
#endif
    {
        /// <exclude/>
        public RootFolderDeletionException(string message) : base(message) { }

        /// <exclude/>
        public RootFolderDeletionException(string message, Exception innerException) : base(message, innerException) { }
    }
}