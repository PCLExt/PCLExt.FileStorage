using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to create a file fails because it already exists on disk.
    /// </summary>
    public class FileExistException
#if PORTABLE
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
