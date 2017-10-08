using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <exclude/>
    public class FolderExistException
#if PORTABLE
        : System.IO.IOException
#elif NETSTANDARD2_0
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