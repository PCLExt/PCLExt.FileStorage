//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents a file
    /// </summary>
    public interface IFile
    {
       /// <summary>
       /// The name of the file
       /// </summary>
        string Name { get; }
        /// <summary>
        /// The full path of the file
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Shows if the file actually exist. Controversial property.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Size of the file.
        /// </summary>
        long Size { get; }

         /// <summary>
         /// Creation time.
         /// </summary>
         /// <value>The creation time.</value>
         DateTime CreationTime { get; }
 
         /// <summary>
         /// Creation time UTC.
         /// </summary>
         /// <value>The creation time UTC.</value>
         DateTime CreationTimeUTC { get; }
 
         /// <summary>
         /// Last access time.
         /// </summary>
         /// <value>The last access time.</value>
         DateTime LastAccessTime { get; }
 
         /// <summary>
         /// Last access time UTC.
         /// </summary>
         /// <value>The last access time UTC.</value>
         DateTime LastAccessTimeUTC { get; }
 
         /// <summary>
         /// Last write time.
         /// </summary>
         /// <value>The last write time.</value>
         DateTime LastWriteTime { get; }
 
         /// <summary>
         /// Last write time UTC.
         /// </summary>
         /// <value>The last write time UTC.</value>
         DateTime LastWriteTimeUTC { get; }

        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        Stream Open(FileAccess fileAccess);
        /// <summary>
        /// Opens the file
        /// </summary>
        /// <param name="fileAccess">Specifies whether the file should be opened in read-only or read/write mode</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Stream"/> which can be used to read from or write to the file</returns>
        Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>
        /// A task which will complete after the file is deleted.
        /// </returns>
        void Delete();
        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is deleted.
        /// </returns>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        void WriteAllBytes(byte[] bytes);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] ReadAllBytes();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Renames a file without changing its directory.
        /// </summary>
        /// <param name="newName">The new name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists);
        /// <summary>
        /// Renames a file without changing its directory.
        /// </summary>
        /// <param name="newName">The new name of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task which will complete after the file is renamed.
        /// </returns>
        Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newFile">The file to move to.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        void Move(IFile newFile);
        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newFile">The file to move to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copies a file. Overwriting a file of the same name is allowed.
        /// </summary>
        /// <param name="newFile">The file to copy to.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        void Copy(IFile newFile);
        /// <summary>
        /// Copies a file. Overwriting a file of the same name is allowed.
        /// </summary>
        /// <param name="newFile">The file to copy to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting);
        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
		/// Copies a file. Overwriting a file of the same name is allowed.
		/// </summary>
		/// <param name="newPath">The new full path of the file.</param>
		/// <param name="collisionOption">How to deal with collisions with existing files.</param>
		/// <returns>A task which will complete after the file is moved.</returns>
		IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting);
        /// <summary>
        /// Copies a file. Overwriting a file of the same name is allowed.
        /// </summary>
        /// <param name="newPath">The new full path of the file.</param>
        /// <param name="collisionOption">How to deal with collisions with existing files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the file is moved.</returns>
        Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));
    }
}