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
    /// The exception that is thrown when an attempt to access a folder that does not exist on disk fails.
    /// </summary>
#if !PORTABLE
    [Serializable]
#endif
    public class FolderNotFoundException
#if PORTABLE
        : System.IO.IOException
#else
        : System.IO.DirectoryNotFoundException
#endif
    {
        public string? FolderName { get; }

        /// <exclude/>
        public FolderNotFoundException() : base() { }

        /// <exclude/>
        public FolderNotFoundException(string message) : base(message) { }

        /// <exclude/>
        public FolderNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <exclude/>
        public FolderNotFoundException(string message, string folderName) : base(message)
        {
            FolderName = folderName;
        }

        /// <exclude/>
        public FolderNotFoundException(string message, string folderName, Exception innerException) : base(message, innerException)
        {
            FolderName = folderName;
        }

#if !PORTABLE
        protected FolderNotFoundException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            FolderName = serializationInfo.GetString("PCLExt_NotFound_FolderName");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PCLExt_NotFound_FolderName", FolderName, typeof(string));
        }
#endif
    }
}