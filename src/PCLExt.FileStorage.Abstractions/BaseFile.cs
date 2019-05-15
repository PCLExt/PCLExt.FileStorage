using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <inheritdoc />
    public abstract class BaseFile : IFile
    {
        private readonly IFile _file;

        /// <inheritdoc />
        public string Name => _file.Name;
        /// <inheritdoc />
        public string Path => _file.Path;
        /// <inheritdoc />
        public bool Exists => _file.Exists;
        /// <inheritdoc />
        public long Size => _file.Size;
        /// <inheritdoc />
        public DateTime CreationTime => _file.CreationTime;
        /// <inheritdoc />
        public DateTime CreationTimeUTC => _file.CreationTimeUTC;
        /// <inheritdoc />
        public DateTime LastAccessTime => _file.LastAccessTime;
        /// <inheritdoc />
        public DateTime LastAccessTimeUTC => _file.LastAccessTimeUTC;
        /// <inheritdoc />
        public DateTime LastWriteTime => _file.LastWriteTime;
        /// <inheritdoc />
        public DateTime LastWriteTimeUTC => _file.LastWriteTimeUTC;

        /// <summary>
        /// Wraps an <see cref="IFile"/>
        /// </summary>
        /// <param name="file"></param>
        protected BaseFile(IFile file) { _file = file ?? new NonExistingFile(); }

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess) => _file.Open(fileAccess);
        /// <inheritdoc />
        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default) => _file.OpenAsync(fileAccess, cancellationToken);

        /// <inheritdoc />
        public void Delete() => _file.Delete();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default) => _file.DeleteAsync(cancellationToken);

        /// <inheritdoc />
        public void WriteAllBytes(byte[] bytes) => _file.WriteAllBytes(bytes);
        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken) => _file.WriteAllBytesAsync(bytes, cancellationToken);

        /// <inheritdoc />
        public byte[] ReadAllBytes() => _file.ReadAllBytes();
        /// <inheritdoc />
        public Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken) => _file.ReadAllBytesAsync(cancellationToken);

        /// <inheritdoc />
        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) => _file.Rename(newName, collisionOption);
        /// <inheritdoc />
        public Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default) => _file.RenameAsync(newName, collisionOption, cancellationToken);

        /// <inheritdoc />
        public void Move(IFile newFile) => _file.Move(newFile);
        /// <inheritdoc />
        public Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default) => _file.MoveAsync(newFile, cancellationToken);

        /// <inheritdoc />
        public void Copy(IFile newFile) => _file.Copy(newFile);
        /// <inheritdoc />
        public Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default) => _file.CopyAsync(newFile, cancellationToken);

        /// <inheritdoc />
        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => _file.Move(newPath, collisionOption);
        /// <inheritdoc />
        public Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => _file.MoveAsync(newPath, collisionOption, cancellationToken);

        /// <inheritdoc />
        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => _file.Copy(newPath, collisionOption);
        /// <inheritdoc />
        public Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => _file.CopyAsync(newPath, collisionOption, cancellationToken);

        /// <inheritdoc />
        public override string ToString() => Path;
    }
}