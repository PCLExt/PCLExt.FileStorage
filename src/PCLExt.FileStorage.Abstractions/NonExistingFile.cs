using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents a non existing file.
    /// </summary>
    public sealed class NonExistingFile : IFile
    {
        /// <inheritdoc />
        public string Name => throw new FileNotFoundException();
        /// <inheritdoc />
        public string Path => throw new FileNotFoundException();
        /// <inheritdoc />
        public bool Exists => false;
        /// <inheritdoc />
        public long Size => throw new FileNotFoundException();
        /// <inheritdoc />
        public DateTime CreationTime => throw new FileNotFoundException();
        /// <inheritdoc />
        public DateTime CreationTimeUTC => throw new FileNotFoundException();
        /// <inheritdoc />
        public DateTime LastAccessTime => throw new FileNotFoundException();
        /// <inheritdoc />
        public DateTime LastAccessTimeUTC => throw new FileNotFoundException();
        /// <inheritdoc />
        public DateTime LastWriteTime => throw new FileNotFoundException();
        /// <inheritdoc />
        public DateTime LastWriteTimeUTC => throw new FileNotFoundException();

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public void Delete() => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public void WriteAllBytes(byte[] bytes) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public byte[] ReadAllBytes() => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public void Move(IFile newFile) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public void Copy(IFile newFile) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw new FileNotFoundException();

        /// <inheritdoc />
        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => throw new FileNotFoundException();
        /// <inheritdoc />
        public Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw new FileNotFoundException();
    }
}