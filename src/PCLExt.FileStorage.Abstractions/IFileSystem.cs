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
    /// Represents a file system.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// A folder representing storage which is where the app is running.
        /// </summary>
        IFolder BaseStorage { get; }
        /// <summary>
        /// A folder representing storage which is local to the current device.
        /// </summary>
        IFolder LocalStorage { get; }
        /// <summary>
        /// A folder representing storage which may be synced with other devices for the same user.
        /// </summary>
        IFolder RoamingStorage { get; }
        /// <summary>
        /// Depending on OS, it will return BaseStorage on Desktop platforms and LocalStorage on Mobile platforms.
        /// </summary>
        IFolder SpecialStorage { get; }
    }
}
