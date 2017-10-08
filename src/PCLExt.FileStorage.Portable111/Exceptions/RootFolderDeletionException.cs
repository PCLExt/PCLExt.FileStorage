using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <exclude/>
    public class RootFolderDeletionException
#if PORTABLE
        : System.IO.IOException
#elif NETSTANDARD2_0
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