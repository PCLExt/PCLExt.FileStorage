using PCLExt.FileStorage.Exceptions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    public class NonExistingFolder : IFolder
    {
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Path { get; }
        /// <inheritdoc />
        public bool Exists => false;
        /// <inheritdoc />
        public DateTime CreationTime => throw new FolderNotFoundException();
        /// <inheritdoc />
        public DateTime CreationTimeUTC => throw new FolderNotFoundException();
        /// <inheritdoc />
        public DateTime LastAccessTime => throw new FolderNotFoundException();
        /// <inheritdoc />
        public DateTime LastAccessTimeUTC => throw new FolderNotFoundException();
        /// <inheritdoc />
        public DateTime LastWriteTime => throw new FolderNotFoundException();
        /// <inheritdoc />
        public DateTime LastWriteTimeUTC => throw new FolderNotFoundException();

        public NonExistingFolder(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }

        /// <inheritdoc />
        public ExistenceCheckResult CheckExists(string name) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public void Copy(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task CopyAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IFile CreateFile(string desiredName, CreationCollisionOption option) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IFolder CreateFolder(string desiredName, CreationCollisionOption option) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public void Delete() => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IFile GetFile(string name) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IList<IFile> GetFiles(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IList<IFile>> GetFilesAsync(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IFolder GetFolder(string name) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IList<IFolder> GetFolders() => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public void Move(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task MoveAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();

        /// <inheritdoc />
        public IFolder Rename(string newName) => throw new FolderNotFoundException();
        /// <inheritdoc />
        public Task<IFolder> RenameAsync(string newName, CancellationToken cancellationToken = default) => throw new FolderNotFoundException();
    }
}