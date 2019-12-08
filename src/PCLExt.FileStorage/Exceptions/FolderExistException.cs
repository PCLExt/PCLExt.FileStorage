using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to create a folder fails because it already exists on disk.
    /// </summary>
#if !PORTABLE
    [Serializable]
#endif
    public class FolderExistException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.IOException
#endif
    {
        public string? FolderName { get; }

        /// <exclude/>
        public FolderExistException() : base() { }

        /// <exclude/>
        public FolderExistException(string message) : base(message) { }

        /// <exclude/>
        public FolderExistException(string message, Exception innerException) : base(message, innerException) { }

        /// <exclude/>
        public FolderExistException(string message, string folderName) : base(message)
        {
            FolderName = folderName;
        }

        /// <exclude/>
        public FolderExistException(string message, string folderName, Exception innerException) : base(message, innerException)
        {
            FolderName = folderName;
        }

#if !PORTABLE
        protected FolderExistException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            FolderName = serializationInfo.GetString("PCLExt_FolderExist_FolderName");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PCLExt_FolderExist_FolderName", FolderName, typeof(string));
        }
#endif
    }
}