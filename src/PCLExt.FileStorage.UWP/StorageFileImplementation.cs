using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.FileProperties;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;
using PCLExt.FileStorage.UWP.Extensions;

namespace PCLExt.FileStorage.UWP
{
    /// <inheritdoc />
    internal sealed class StorageFileImplementation : IFile
    {
        public string Name => _storageFile.Name;
        public string Path => _storageFile.Path;

        public bool Exists
        {
            get
            {
                try
                {
                    var storageFolder = StorageFile.GetFileFromPathAsync(_storageFile.Path).RunSync();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return false;
                }
                return true;
            }
        }

        public ulong Size => GetStorageFileProperties().Size;

        public DateTimeOffset CreationTime => _storageFile.DateCreated;
        public DateTimeOffset LastAccessTime => GetStorageFileProperties().ItemDate;
        public DateTimeOffset LastWriteTime => GetStorageFileProperties().DateModified;

        private StorageFile _storageFile;

        /// <summary>
        /// Creates a new <see cref="IFile"/> from StorageFile
        /// </summary>
        /// <param name="storageFile">StorageFile to keep in field</param>
        public StorageFileImplementation(StorageFile storageFile)
        {
            _storageFile = storageFile;
        }
        /// <summary>
        /// Creates a new <see cref="IFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public StorageFileImplementation(string path)
        {
            _storageFile = StorageFile.GetFileFromPathAsync(path).RunSync();
        }

        private BasicProperties GetStorageFileProperties() => _storageFile.GetBasicPropertiesAsync().RunSync();

        public void Copy(IFile newFile) => CopyAsync(newFile).RunSync();
        public async Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (string.Equals(_storageFile.Path, newFile.Path, StringComparison.Ordinal))
                return;

            var newFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(newFile.Path))
                .AsTask(cancellationToken);
            if (newFile.Exists)
                await newFile.DeleteAsync(cancellationToken);

            await _storageFile.CopyAsync(newFolder, newFile.Name)
                .AsTask(cancellationToken);
        }

        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) =>
            CopyAsync(newPath, collisionOption).RunSync();
        public async Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (string.Equals(_storageFile.Path, newPath, StringComparison.Ordinal))
            {
                if (collisionOption == NameCollisionOption.FailIfExists)
                    throw new FileExistException(newPath);

                if (collisionOption == NameCollisionOption.ReplaceExisting)
                    return this;
            }

            var newFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(newPath));
            var initialNewName = System.IO.Path.GetFileName(newPath);
            var newName = initialNewName;
            var nameCollisionCounter = 2;
            StorageFile newFile = null;
            while (true)
            {
                try
                {
                    newFile = await _storageFile.CopyAsync(newFolder, newName, collisionOption.ConvertToNameCollision());
                    break;
                }
                catch (Exception ex)
                {
                    if (collisionOption != NameCollisionOption.GenerateUniqueName)
                    {
                        if (ex.Message.Contains("HRESULT: 0x800700B7"))
                            throw new Exceptions.FileExistException(newPath, ex);
                        else
                            throw;
                    }
                    newName = $"{initialNewName} ({nameCollisionCounter++})";
                }

            }
            return new StorageFileImplementation(newFile.Path);
        }

        public void Delete() => DeleteAsync().RunSync();
        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            try
            {
                await _storageFile.DeleteAsync().AsTask(cancellationToken);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                throw new Exceptions.FileNotFoundException(_storageFile.Path, ex);
            }
        }

        public void Move(IFile newFile) => MoveAsync(newFile).RunSync();
        public Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default) =>
            MoveAsync(newPath: newFile.Path, cancellationToken: cancellationToken);

        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting) =>
            MoveAsync(newPath, collisionOption).RunSync();
        public async Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (collisionOption == NameCollisionOption.ReplaceExisting && string.Equals(_storageFile.Path, newPath, StringComparison.Ordinal))
                return this;

            var newFolder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(newPath));

            var oldFile = await StorageFile.GetFileFromPathAsync(_storageFile.Path);
            var initialName = System.IO.Path.GetFileName(newPath);
            var newName = initialName;
            var nameCollisionCounter = 2;
            while (true)
            {
                try
                {
                    await _storageFile.MoveAsync(newFolder, newName, collisionOption.ConvertToNameCollision());
                    break;
                }
                catch (Exception ex)
                {
                    if (collisionOption != NameCollisionOption.GenerateUniqueName)
                        throw new FileExistException(newPath, ex);
                    newName = $"{initialName} ({nameCollisionCounter++})";
                }
            }
            var result = new StorageFileImplementation(_storageFile);
            _storageFile = oldFile;
            return result;
        }

        public Stream Open(FileAccess fileAccess) => OpenAsync(fileAccess).RunSync();
        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var randomStream = await _storageFile.OpenAsync(fileAccess.ConvertToFileAccessMode())
                .AsTask(cancellationToken);
            return randomStream.AsStream();
        }

        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists) =>
            RenameAsync(newName, collisionOption).RunSync();
        public async Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var newFileName = System.IO.Path.GetFileName(newName);
            var oldStorageFile = await StorageFile.GetFileFromPathAsync(_storageFile.Path);
            await _storageFile.RenameAsync(newFileName, collisionOption.ConvertToNameCollision());
            var result = new StorageFileImplementation(_storageFile);
            _storageFile = oldStorageFile;
            return result;
        }

        public void WriteAllBytes(byte[] bytes) => WriteAllBytesAsync(bytes).RunSync();
        public async Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            using var stream = await _storageFile.OpenStreamForWriteAsync();
            using var sw = new BinaryWriter(stream);
            sw.Write(bytes);
        }

        public byte[] ReadAllBytes() => ReadAllBytesAsync().RunSync();
        public async Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            using var stream = await _storageFile.OpenStreamForReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}