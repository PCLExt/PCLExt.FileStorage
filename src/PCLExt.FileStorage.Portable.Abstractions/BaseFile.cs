using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents an abstract file system file
    /// </summary>
    public abstract class BaseFile : IFile
    {
        private readonly IFile _file;

        /// <inheritdoc />
        public string Name => _file.Name;
        /// <inheritdoc />
        public string Path => _file.Path;
        /// <inheritdoc />
        public long Size => _file.Size;

        /// <summary>
        /// Shows if the file actually exist. Controversial property.
        /// </summary>
        public bool Exists => _file != null;

        /// <summary>
        /// Wraps an <see cref="IFile"/>
        /// </summary>
        /// <param name="file"></param>
        public BaseFile(IFile file) { _file = file; }

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess) => _file.Open(fileAccess);
        /// <inheritdoc />
        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken)) => _file.OpenAsync(fileAccess, cancellationToken);

        /// <inheritdoc />
        public void Delete() => _file.Delete();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken)) => _file.DeleteAsync(cancellationToken);

        /// <inheritdoc />
        public void Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) => _file.Rename(newName, collisionOption);
        /// <inheritdoc />
        public Task RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default(CancellationToken)) => _file.RenameAsync(newName, collisionOption, cancellationToken);

        /// <inheritdoc />
        public void Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => _file.Move(newPath, collisionOption);
        /// <inheritdoc />
        public Task MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken)) => _file.MoveAsync(newPath, collisionOption, cancellationToken);

        /// <inheritdoc />
        public void Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) => _file.Copy(newPath, collisionOption);
        /// <inheritdoc />
        public Task CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken)) => _file.CopyAsync(newPath, collisionOption, cancellationToken);
    }
}