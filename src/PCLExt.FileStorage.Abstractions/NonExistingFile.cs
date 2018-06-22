using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    public sealed class NonExistingFile : IFile
    {
        public string Name => throw new FileNotFoundException();
        public string Path => throw new FileNotFoundException();
        public bool Exists => false;
        public long Size => throw new FileNotFoundException();
        public DateTime CreationTime => throw new FileNotFoundException();
        public DateTime CreationTimeUTC => throw new FileNotFoundException();
        public DateTime LastAccessTime => throw new FileNotFoundException();
        public DateTime LastAccessTimeUTC => throw new FileNotFoundException();
        public DateTime LastWriteTime => throw new FileNotFoundException();
        public DateTime LastWriteTimeUTC => throw new FileNotFoundException();

        public Stream Open(FileAccess fileAccess) => throw new FileNotFoundException();
        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public void Delete() => throw new FileNotFoundException();
        public Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public void WriteAllBytes(byte[] bytes) => throw new FileNotFoundException();
        public Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public byte[] ReadAllBytes() => throw new FileNotFoundException();
        public Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) => throw new FileNotFoundException();
        public Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists,
            CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public void Move(IFile newFile) => throw new FileNotFoundException();
        public Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public void Copy(IFile newFile) => throw new FileNotFoundException();
        public Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => throw new FileNotFoundException();
        public Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();

        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => throw new FileNotFoundException();
        public Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken)) => throw new FileNotFoundException();
    }
}