using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to create a file fails because it already exists on disk.
    /// </summary>
#if !PORTABLE
    [Serializable]
#endif
    public class FileExistException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.IOException
#endif
    {
        public string? FileName { get; }

        /// <exclude/>
        public FileExistException() { }

        /// <exclude/>
        public FileExistException(string message) : base(message) { }

        /// <exclude/>
        public FileExistException(string message, Exception innerException) : base(message, innerException) { }

        /// <exclude/>
        public FileExistException(string message, string fileName) : base(message)
        {
            FileName = fileName;
        }

        /// <exclude/>
        public FileExistException(string message, string fileName, Exception innerException) : base(message, innerException)
        {
            FileName = fileName;
        }

#if !PORTABLE
        protected FileExistException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            FileName = serializationInfo.GetString("PCLExt_FileExist_FileName");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PCLExt_FileExist_FileName", FileName, typeof(string));
        }
#endif
    }
}