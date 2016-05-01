//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Describes the result of a file or folder existence check.
    /// </summary>
    public enum ExistenceCheckResult
    {
        /// <summary>
        /// No file system entity was found at the given path.
        /// </summary>
        NotFound,

        /// <summary>
        /// A file was found at the given path.
        /// </summary>
        FileExists,

        /// <summary>
        /// A folder was found at the given path.
        /// </summary>
        FolderExists,
    }
}
