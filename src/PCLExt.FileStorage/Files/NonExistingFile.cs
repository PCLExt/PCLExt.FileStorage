using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FileNotFoundException = PCLExt.FileStorage.Exceptions.FileNotFoundException;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents a non existing file.
    /// </summary>
    public sealed class NonExistingFile : IFile
    {
        private static Exception FileNotFoundException() => throw new FileNotFoundException();

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Path { get; }
        /// <inheritdoc />
        public bool Exists => false;
        /// <inheritdoc />
        public ulong Size => throw FileNotFoundException();
        /// <inheritdoc />
        public DateTimeOffset CreationTime => throw FileNotFoundException();
        /// <inheritdoc />
        public DateTimeOffset LastAccessTime => throw FileNotFoundException();
        /// <inheritdoc />
        public DateTimeOffset LastWriteTime => throw FileNotFoundException();

        public NonExistingFile(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
        }

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public void Delete() => throw FileNotFoundException();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public void WriteAllBytes(byte[] bytes) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public byte[] ReadAllBytes() => throw FileNotFoundException();
        /// <inheritdoc />
        public Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public void Move(IFile newFile) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public void Copy(IFile newFile) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw FileNotFoundException();

        /// <inheritdoc />
        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => throw FileNotFoundException();
        /// <inheritdoc />
        public Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) => throw FileNotFoundException();
    }
}