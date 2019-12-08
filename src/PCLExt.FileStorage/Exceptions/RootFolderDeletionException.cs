using System;

namespace PCLExt.FileStorage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an attempt to delete a root folder fails.
    /// </summary>
#if !PORTABLE
    [Serializable]
#endif
    public class RootFolderDeletionException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.IOException
#endif
    {
        public string? FolderName { get; }

        /// <exclude/>
        public RootFolderDeletionException() : base() { }

        /// <exclude/>
        public RootFolderDeletionException(string message) : base(message) { }

        /// <exclude/>
        public RootFolderDeletionException(string message, Exception innerException) : base(message, innerException) { }

        /// <exclude/>
        public RootFolderDeletionException(string message, string folderName) : base(message)
        {
            FolderName = folderName;
        }

        /// <exclude/>
        public RootFolderDeletionException(string message, string folderName, Exception innerException) : base(message, innerException)
        {
            FolderName = folderName;
        }

#if !PORTABLE
        protected RootFolderDeletionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            FolderName = serializationInfo.GetString("PCLExt_RootFolderDeletion_FolderName");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PCLExt_RootFolderDeletion_FolderName", FolderName, typeof(string));
        }
#endif
    }
}