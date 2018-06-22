//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents a file system folder
    /// </summary>
    public interface IFolder
    {
        /// <summary>
        /// The name of the folder
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The "full path" of the folder
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Shows if the folder actually exist. Controversial property.
        /// </summary>
        bool Exists { get; }

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
        /// Creates a file in this folder
        /// </summary>
        /// <param name="desiredName">The name of the file to create</param>
        /// <param name="option">Specifies how to behave if the specified file already exists</param>
        /// <returns>The newly created file</returns>
        IFile CreateFile(string desiredName, CreationCollisionOption option);
        /// <summary>
        /// Creates a file in this folder
        /// </summary>
        /// <param name="desiredName">The name of the file to create</param>
        /// <param name="option">Specifies how to behave if the specified file already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created file</returns>
        Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a file in this folder
        /// </summary>
        /// <param name="name">The name of the file to get</param>
        /// <returns>The requested file, or null if it does not exist</returns>
        IFile GetFile(string name);
        /// <summary>
        /// Gets a file in this folder
        /// </summary>
        /// <param name="name">The name of the file to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested file, or null if it does not exist</returns>
        Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of the files in this folder
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <returns>A list of the files in the folder</returns>
        IList<IFile> GetFiles(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly);

        /// <summary>
        /// Gets a list of the files in this folder
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of the files in the folder</returns>
        Task<IList<IFile>> GetFilesAsync(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a subfolder in this folder
        /// </summary>
        /// <param name="desiredName">The name of the folder to create</param>
        /// <param name="option">Specifies how to behave if the specified folder already exists</param>
        /// <returns>The newly created folder</returns>
        IFolder CreateFolder(string desiredName, CreationCollisionOption option);
        /// <summary>
        /// Creates a subfolder in this folder
        /// </summary>
        /// <param name="desiredName">The name of the folder to create</param>
        /// <param name="option">Specifies how to behave if the specified folder already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly created folder</returns>
        Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a subfolder in this folder
        /// </summary>
        /// <param name="name">The name of the folder to get</param>
        /// <returns>The requested folder, or null if it does not exist</returns>
        IFolder GetFolder(string name);
        /// <summary>
        /// Gets a subfolder in this folder
        /// </summary>
        /// <param name="name">The name of the folder to get</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The requested folder, or null if it does not exist</returns>
        Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of subfolders in this folder
        /// </summary>
        /// <returns>A list of subfolders in the folder</returns>
        IList<IFolder> GetFolders();
        /// <summary>
        /// Gets a list of subfolders in this folder
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of subfolders in the folder</returns>
        Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Checks whether a folder or file exists at the given location.
        /// </summary>
        /// <param name="name">The name of the file or folder to check for.</param>
        /// <returns>A task whose result is the result of the existence check.</returns>
        ExistenceCheckResult CheckExists(string name);
        /// <summary>
        /// Checks whether a folder or file exists at the given location.
        /// </summary>
        /// <param name="name">The name of the file or folder to check for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task whose result is the result of the existence check.</returns>
        Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes this folder and all of its contents
        /// </summary>
        /// <returns>A task which will complete after the folder is deleted</returns>
        void Delete();
        /// <summary>
        /// Deletes this folder and all of its contents
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which will complete after the folder is deleted</returns>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        IFolder Rename(string newName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IFolder> RenameAsync(string newName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Moves this folder and all of its contents to the given folder, current folder will be deleted.
        /// </summary>
        /// <param name="folder">The folder in which content will be moved</param>
        /// <param name="option">Specifies how to behave if the specified folder/file already exists</param>
        /// <returns>The folder with moved content.</returns>
        void Move(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting);
        /// <summary>
        /// Moves this folder and all of its contents to the given folder, current folder will be deleted.
        /// </summary>
        /// <param name="folder">The folder in which content will be moved</param>
        /// <param name="option">Specifies how to behave if the specified folder/file already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The folder with moved content.</returns>
        Task MoveAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Copies this folder and all of its contents to the given folder.
        /// </summary>
        /// <param name="folder">The folder in which content will be copied</param>
        /// <param name="option">Specifies how to behave if the specified folder/file already exists</param>
        void Copy(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting);
        /// <summary>
        /// Copies this folder and all of its contents to the given folder.
        /// </summary>
        /// <param name="folder">The folder in which content will be copied</param>
        /// <param name="option">Specifies how to behave if the specified folder/file already exists</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task CopyAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken));
    }
}
