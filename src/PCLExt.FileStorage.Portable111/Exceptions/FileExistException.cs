using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <exclude/>
    public class FileExistException
#if PORTABLE
        : System.IO.IOException
#elif NETSTANDARD2_0
        : System.IO.IOException
#else
        : System.IO.IOException
#endif
    {
        /// <exclude/>
        public FileExistException(string message) : base(message) { }

        /// <exclude/>
        public FileExistException(string message, Exception innerException) : base(message, innerException) { }
    }
}
