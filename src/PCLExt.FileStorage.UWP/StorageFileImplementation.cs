using PCLExt.FileStorage.Extensions;
using PCLExt.FileStorage.UWP.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace PCLExt.FileStorage.UWP
{
    /// <inheritdoc />
    sealed class StorageFileImplementation : IFile
    {
        public string Name => _storageFile.Name;

        public string Path => _storageFile.Path;

        private bool _exists = false;
        public bool Exists => _exists;

        public long Size =>
            (long)_storageFile.GetBasicPropertiesAsync().AsTask().Result.Size;

        public DateTime CreationTime =>
            _storageFile.DateCreated.ToLocalTime().DateTime;

        public DateTime CreationTimeUTC =>
            _storageFile.DateCreated.ToUniversalTime().DateTime;

        public DateTime LastAccessTime => _storageFile.GetBasicPropertiesAsync().
            AsTask().Result.ItemDate.ToLocalTime().DateTime;

        public DateTime LastAccessTimeUTC => _storageFile.GetBasicPropertiesAsync().
            AsTask().Result.ItemDate.ToUniversalTime().DateTime;

        public DateTime LastWriteTime => _storageFile.GetBasicPropertiesAsync().
            AsTask().Result.DateModified.ToLocalTime().DateTime;

        public DateTime LastWriteTimeUTC => _storageFile.GetBasicPropertiesAsync().
            AsTask().Result.DateModified.ToUniversalTime().DateTime;

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
            try
            {
                _storageFile = StorageFile.GetFileFromPathAsync(path).AsTask().Result;
            }
            catch (FileNotFoundException)
            {
                _exists = false;
                return;
            }

            _exists = true;
        }

        public async void Copy(IFile newFile)
        {
            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                System.IO.Path.GetDirectoryName(newFile.Path));
            await _storageFile.CopyAsync(newFolder, newFile.Name);
        }

        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting)
        {
            return CopyAsync(newPath, collisionOption).Result;
        }

        public async Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                 System.IO.Path.GetDirectoryName(newFile.Path)).
                 AsTask(cancellationToken);
            await _storageFile.CopyAsync(newFolder, newFile.Name).
                AsTask(cancellationToken);
        }

       

        public async Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                System.IO.Path.GetDirectoryName(newPath));
            var newName = System.IO.Path.GetDirectoryName(newPath);
            var windowsNameCollision = StorageExtensions. ConvertToNameCollision(collisionOption);
            var newFile = await _storageFile.CopyAsync(
                newFolder, newName, windowsNameCollision);
            return new StorageFileImplementation(newFile.Path);
        }

        public void Delete()
        {
            _storageFile.DeleteAsync().AsTask().Wait();
        }

        public async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            await _storageFile.DeleteAsync().AsTask(cancellationToken);
        }

        public async void Move(IFile newFile)
        {
            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                 System.IO.Path.GetDirectoryName(newFile.Path)).
                 AsTask().ConfigureAwait(false);
            await _storageFile.MoveAsync(newFolder);
        }

        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting)
        {
            return MoveAsync(
                newPath: newPath,
                collisionOption: collisionOption).Result;
        }

        public async Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default(CancellationToken))
        {
            await MoveAsync(
                newPath: newFile.Path,
                cancellationToken: cancellationToken);
        }

        public async Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            var newFolder = await StorageFolder.GetFolderFromPathAsync(
                  System.IO.Path.GetDirectoryName(newPath));
            var newName = System.IO.Path.GetFileName(newPath);
            var windowsCollisionOption = StorageExtensions.ConvertToNameCollision(collisionOption);
            await _storageFile.MoveAsync(newFolder, newName, windowsCollisionOption);
            return new StorageFileImplementation(newPath);
        }

        public Stream Open(FileAccess fileAccess)
        {
            return OpenAsync(fileAccess).Result;
        }

        private FileAccessMode Convert(FileAccess fileAccess)
        {
            if (fileAccess == FileAccess.Read)
                return FileAccessMode.Read;
            if (fileAccess == FileAccess.ReadAndWrite)
                return FileAccessMode.ReadWrite;
            throw new NotSupportedException(fileAccess.ToString());
        }

        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsFileAccess = Convert(fileAccess);
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            var randomStream = await _storageFile.OpenAsync(windowsFileAccess).
                AsTask(cancellationToken);
            return randomStream.AsStream();
        }

        public byte[] ReadAllBytes()
        {
            return ReadAllBytesAsync().Result;
        }

        public async Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
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
            return RenameAsync(newName, collisionOption).Result;
        }

        public async Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsCollisionOption = StorageExtensions.ConvertToNameCollision(collisionOption);
            var newFileName = System.IO.Path.GetFileName(newName);
            await _storageFile.RenameAsync(newFileName, windowsCollisionOption);
            return new StorageFileImplementation(_storageFile.Path);
        }

        public void WriteAllBytes(byte[] bytes)
        {
            WriteAllBytesAsync(bytes).Wait();
        }

        public async Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            using (Stream stream = await _storageFile.OpenStreamForWriteAsync())
            {
                using (var sw = new BinaryWriter(stream))
                    sw.Write(bytes);
            }
        }
    }
}
