using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;
using PCLExt.FileStorage.UWP.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PCLExt.FileStorage.UWP
{
    /// <inheritdoc />
    sealed class StorageFileImplementation : IFile
    {
        public string Name => _storageFile.Name;

        public string Path => _storageFile.Path;

        public bool Exists
        {
            get
            {
                try
                {
                    var storageFolder = StorageFile.GetFileFromPathAsync(
                        _storageFile.Path).AsTask().GetAwaiter().GetResult();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return false;
                }
                return true;
            }
        }

        public long Size => (long)GetStorageFileProperties().Size;

        public DateTime CreationTime =>
            _storageFile.DateCreated.ToLocalTime().DateTime;

        public DateTime CreationTimeUTC =>
            _storageFile.DateCreated.ToUniversalTime().DateTime;

        public DateTime LastAccessTime =>
            GetStorageFileProperties().ItemDate.ToLocalTime().DateTime;

        public DateTime LastAccessTimeUTC => GetStorageFileProperties().
            ItemDate.ToUniversalTime().DateTime;

        public DateTime LastWriteTime => GetStorageFileProperties().
            DateModified.ToLocalTime().DateTime;

        public DateTime LastWriteTimeUTC => GetStorageFileProperties().
            DateModified.ToUniversalTime().DateTime;

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
            _storageFile = StorageFile.GetFileFromPathAsync(path).
                AsTask().GetAwaiter().GetResult();
        }

        private BasicProperties GetStorageFileProperties()
        {
            return _storageFile.GetBasicPropertiesAsync().
                AsTask().GetAwaiter().GetResult();
        }

        public void Copy(IFile newFile)
        {
            CopyAsync(newFile).GetAwaiter().GetResult();
        }

        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting)
        {
            return CopyAsync(newPath, collisionOption).GetAwaiter().GetResult();
        }

        public async Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_storageFile.Path == newFile.Path)
                return;

            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                 System.IO.Path.GetDirectoryName(newFile.Path)).
                 AsTask(cancellationToken);
            if (newFile.Exists)
                await newFile.DeleteAsync();

            await _storageFile.CopyAsync(newFolder, newFile.Name).
                AsTask(cancellationToken);
        }

        public async Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_storageFile.Path == newPath)
            {
                if (collisionOption == NameCollisionOption.FailIfExists)
                    throw new FileExistException(newPath);

                if (collisionOption == NameCollisionOption.ReplaceExisting)
                    return this;
            }

            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                System.IO.Path.GetDirectoryName(newPath));
            var newName = System.IO.Path.GetFileName(newPath);
            var windowsNameCollision = StorageExtensions.ConvertToNameCollision(collisionOption);
            var newFile = await _storageFile.CopyAsync(
                newFolder, newName, windowsNameCollision);
            return new StorageFileImplementation(newFile.Path);
        }

        public void Delete()
        {
            DeleteAsync().GetAwaiter().GetResult();
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _storageFile.DeleteAsync().AsTask(cancellationToken);
        }

        public void Move(IFile newFile)
        {
            MoveAsync(newFile).GetAwaiter().GetResult();
        }

        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting)
        {
            return MoveAsync(
                newPath: newPath,
                collisionOption: collisionOption).
                GetAwaiter().GetResult();
        }

        public async Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            await MoveAsync(
                newPath: newFile.Path,
                cancellationToken: cancellationToken);
        }

        public async Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (collisionOption == NameCollisionOption.ReplaceExisting &&
                _storageFile.Path == newPath)
                return this;

            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                  System.IO.Path.GetDirectoryName(newPath));

            String newName;
            if (collisionOption == NameCollisionOption.GenerateUniqueName)
            {
                newName = $"{Guid.NewGuid()}";
                var extension = PortablePath.GetExtension(newPath);
                if (!String.IsNullOrEmpty(extension))
                    newName = newName + "." + extension;
            }
            else
            {
                newName = System.IO.Path.GetFileName(newPath);
            }

            var windowsCollisionOption = StorageExtensions.
                ConvertToNameCollision(collisionOption);

            var oldFile = await StorageFile.GetFileFromPathAsync(_storageFile.Path);
            try
            {
                await _storageFile.MoveAsync(newFolder, newName, windowsCollisionOption);
            }
            catch (Exception ex)
            {
                throw new FileExistException(newPath, ex);
            }
            var result = new StorageFileImplementation(_storageFile);
            _storageFile = oldFile;
            return result;
        }

        public Stream Open(FileAccess fileAccess)
        {
            return OpenAsync(fileAccess).GetAwaiter().GetResult();
        }

        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsFileAccess = Convert(fileAccess);
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            var randomStream = await _storageFile.OpenAsync(windowsFileAccess).
                AsTask(cancellationToken);
            return randomStream.AsStream();
        }

        private FileAccessMode Convert(FileAccess fileAccess)
        {
            if (fileAccess == FileAccess.Read)
                return FileAccessMode.Read;
            if (fileAccess == FileAccess.ReadAndWrite)
                return FileAccessMode.ReadWrite;
            throw new ArgumentException(fileAccess.ToString());
        }

        public byte[] ReadAllBytes()
        {
            return ReadAllBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] result;
            using (Stream stream = await _storageFile.OpenStreamForReadAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }

        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists)
        {
            return RenameAsync(newName, collisionOption).GetAwaiter().GetResult();
        }

        public async Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsCollisionOption = StorageExtensions.ConvertToNameCollision(collisionOption);
            var newFileName = System.IO.Path.GetFileName(newName);
            var oldStorageFile = await StorageFile.GetFileFromPathAsync(_storageFile.Path);
            await _storageFile.RenameAsync(newFileName, windowsCollisionOption);
            var result = new StorageFileImplementation(_storageFile);
            _storageFile = oldStorageFile;
            return result;
        }

        public void WriteAllBytes(byte[] bytes)
        {
            WriteAllBytesAsync(bytes).Wait();
        }

        public async Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (Stream stream = await _storageFile.OpenStreamForWriteAsync())
            {
                using (var sw = new BinaryWriter(stream))
                    sw.Write(bytes);
            }
        }
    }
}
