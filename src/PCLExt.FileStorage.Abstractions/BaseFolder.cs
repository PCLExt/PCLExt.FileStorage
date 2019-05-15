using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <inheritdoc />
    public abstract class BaseFolder : IFolder
    {
        private readonly IFolder _folder;

        /// <inheritdoc />
        public string Name => _folder.Name;
        /// <inheritdoc />
        public string Path => _folder.Path;
        /// <inheritdoc />
        public bool Exists => _folder != null;
        /// <inheritdoc />
        public DateTime CreationTime => _folder.CreationTime;
        /// <inheritdoc />
        public DateTime CreationTimeUTC => _folder.CreationTimeUTC;
        /// <inheritdoc />
        public DateTime LastAccessTime => _folder.LastAccessTime;
        /// <inheritdoc />
        public DateTime LastAccessTimeUTC => _folder.LastAccessTimeUTC;
        /// <inheritdoc />
        public DateTime LastWriteTime => _folder.LastWriteTime;
        /// <inheritdoc />
        public DateTime LastWriteTimeUTC => _folder.LastWriteTimeUTC;

        /// <summary>
        /// Wraps an <see cref="IFolder"/>
        /// </summary>
        /// <param name="folder"></param>
        protected BaseFolder(IFolder folder) { _folder = folder; }

        /// <inheritdoc />
        public IFile GetFile(string name) => _folder.GetFile(name);
        /// <inheritdoc />
        public Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default) => _folder.GetFileAsync(name, cancellationToken);

        /// <inheritdoc />
        public IList<IFile> GetFiles(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly) => _folder.GetFiles(searchPattern, searchOption);
        /// <inheritdoc />
        public Task<IList<IFile>> GetFilesAsync(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly, CancellationToken cancellationToken = default) => _folder.GetFilesAsync(searchPattern, searchOption, cancellationToken);

        /// <inheritdoc />
        public IFile CreateFile(string desiredName, CreationCollisionOption option) => _folder.CreateFile(desiredName, option);
        /// <inheritdoc />
        public Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default) => _folder.CreateFileAsync(desiredName, option, cancellationToken);

        /// <inheritdoc />
        public IFolder CreateFolder(string desiredName, CreationCollisionOption option) => _folder.CreateFolder(desiredName, option);
        /// <inheritdoc />
        public Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default) => _folder.CreateFolderAsync(desiredName, option, cancellationToken);

        /// <inheritdoc />
        public IFolder GetFolder(string name) => _folder.GetFolder(name);
        /// <inheritdoc />
        public Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default) => _folder.GetFolderAsync(name, cancellationToken);

        /// <inheritdoc />
        public IList<IFolder> GetFolders() => _folder.GetFolders();
        /// <inheritdoc />
        public Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default) => _folder.GetFoldersAsync(cancellationToken);

        /// <inheritdoc />
        public ExistenceCheckResult CheckExists(string name) => _folder.CheckExists(name);
        /// <inheritdoc />
        public Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default) => _folder.CheckExistsAsync(name, cancellationToken);

        /// <inheritdoc />
        public void Delete() => _folder.Delete();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default) => _folder.DeleteAsync(cancellationToken);

        /// <inheritdoc />
        public IFolder Rename(string newName) => _folder.Rename(newName);
        /// <inheritdoc />
        public Task<IFolder> RenameAsync(string newName, CancellationToken cancellationToken = default) => _folder.RenameAsync(newName, cancellationToken);

        /// <inheritdoc />
        public void Move(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting) => _folder.Move(folder, option);
        /// <inheritdoc />
        public Task MoveAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = new CancellationToken()) => _folder.MoveAsync(folder, option, cancellationToken);

        /// <inheritdoc />
        public void Copy(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting) => _folder.Copy(folder, option);
        /// <inheritdoc />
        public Task CopyAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = new CancellationToken()) => _folder.CopyAsync(folder, option, cancellationToken);

        /// <inheritdoc />
        public override string ToString() => Path;
    }
}