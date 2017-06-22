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
    /// Specifies what should happen when trying to create a file or folder that already exists.
    /// </summary>
    public enum CreationCollisionOption
    {
        /// <summary>
        /// Creates a new file with a unique name of the form "name (2).txt"
        /// </summary>
        GenerateUniqueName = 0,
        /// <summary>
        /// Replaces any existing file with a new (empty) one
        /// </summary>
        ReplaceExisting = 1,
        /// <summary>
        /// Throws an exception if the file exists
        /// </summary>
        FailIfExists = 2,
        /// <summary>
        /// Opens the existing file, if any
        /// </summary>
        OpenIfExists = 3,
    }
}