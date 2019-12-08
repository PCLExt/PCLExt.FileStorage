using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.UWP.Extensions;
using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage.UWP
{
    internal sealed class StorageFolderImplementation : IFolder
    {
        public string Name => _storageFolder.Name;
        public string Path => _storageFolder.Path;

        public bool Exists
        {
            get
            {
                try
                {
                    var storageFolder = StorageFolder.GetFolderFromPathAsync(_storageFolder.Path).RunSync();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return false;
                }
                return true;
            }
        }

        public DateTimeOffset CreationTime => _storageFolder.DateCreated;
        public DateTimeOffset LastAccessTime => GetStorageFolderProperties().ItemDate;
        public DateTimeOffset LastWriteTime => GetStorageFolderProperties().DateModified;

        private StorageFolder _storageFolder;

        public StorageFolderImplementation(StorageFolder storageFolder)
        {
            _storageFolder = storageFolder;
        }
        public StorageFolderImplementation(string path)
        {
            _storageFolder = StorageFolder.GetFolderFromPathAsync(path).RunSync();
        }

        private BasicProperties GetStorageFolderProperties() => _storageFolder.GetBasicPropertiesAsync().RunSync();

        public ExistenceCheckResult CheckExists(string name) => CheckExistsAsync(name).RunSync();
        public async Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var finded = await _storageFolder.GetItemAsync(name)
                    .AsTask(cancellationToken);
                if (finded == null)
                {
                    return ExistenceCheckResult.NotFound;
                }

                if (finded.Attributes == Windows.Storage.FileAttributes.Directory)
                {
                    return ExistenceCheckResult.FolderExists;
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return ExistenceCheckResult.NotFound;
            }

            return ExistenceCheckResult.FileExists;
        }

        public void Copy(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting) => CopyAsync(folder, option).RunSync();
        public Task CopyAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default) =>
            CopyAsync(_storageFolder, folder, option, cancellationToken);

        private async Task CopyAsync(StorageFolder source, IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
        {
            // Get all files (shallow) from source
            var queryOptions = new QueryOptions
            {
                IndexerOption = IndexerOption.DoNotUseIndexer,  // Avoid problems cause by out of sync indexer
                FolderDepth = FolderDepth.Shallow,
            };
            var queryFiles = source.CreateFileQueryWithOptions(queryOptions);
            var files = await queryFiles.GetFilesAsync();

            // Copy files into target folder
            foreach (var storageFile in files)
            {
                try
                {
                    var targetStorageFolder = await StorageFolder.GetFolderFromPathAsync(folder.Path);
                    var windowsOption = option.ConvertToNameCollision();
                    if (windowsOption == Windows.Storage.NameCollisionOption.ReplaceExisting)
                    {
                        var findedFile = await targetStorageFolder.GetFileAsync(storageFile.Name);
                        if (null != findedFile && string.Equals(findedFile.Path, storageFile.Path, StringComparison.Ordinal))
                            continue;
                    }

                    var nameCollisionCounter = 2;
                    var filename = storageFile.Name;
                    while (true)
                    {
                        try
                        {
                            var copiedFile = await storageFile.CopyAsync(targetStorageFolder, filename, windowsOption);
                            break;
                        }
                        catch (Exception)
                        {
                            if (option != NameCollisionOption.GenerateUniqueName)
                                throw;
                            filename = $"{filename} ({nameCollisionCounter++})";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new FileExistException(storageFile.Path, ex);
                }
            }

            // Get all folders (shallow) from source
            var queryFolders = source.CreateFolderQueryWithOptions(queryOptions);
            var folders = await queryFolders.GetFoldersAsync();

            // For each folder call CopyAsync with new destination as destination
            foreach (var storageFolder in folders)
            {
                var nameCollisionCounter = 2;
                var subfolderName = storageFolder.Name;
                IFolder targetFolder = null;
                while (true)
                {
                    try
                    {

                        targetFolder = await folder.CreateFolderAsync(subfolderName, option.ConvertToCreationCollision(), cancellationToken);
                        break;
                    }
                    catch (FolderExistException)
                    {
                        if (option != NameCollisionOption.GenerateUniqueName)
                            throw;
                        subfolderName = $"{storageFolder.Name} ({nameCollisionCounter++})";
                    }
                }

                if (targetFolder == null)
                    throw new Exception("Can't create StorageFolder");
                await CopyAsync(storageFolder, targetFolder, option, cancellationToken);
            }
        }

        public IFile CreateFile(string desiredName, CreationCollisionOption option) => CreateFileAsync(desiredName, option).RunSync();
        public async Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default)
        {
            try
            {
                var file = await _storageFolder.CreateFileAsync(desiredName, option.ConvertToWindowsCreationCollisionOption())
                    .AsTask(cancellationToken);
                return new StorageFileImplementation(file);
            }
            catch (Exception ex)
            {
                throw new FileExistException(desiredName, ex);
            }
        }

        public IFolder CreateFolder(string desiredName, CreationCollisionOption option) => CreateFolderAsync(desiredName, option).RunSync();
        public async Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default)
        {
            try
            {
                var folder = await _storageFolder.CreateFolderAsync(desiredName, option.ConvertToWindowsCreationCollisionOption())
                    .AsTask(cancellationToken);
                return new StorageFolderImplementation(folder);
            }
            catch (Exception ex)
            {
                throw new FolderExistException(desiredName, ex);
            }
        }

        public void Delete() => DeleteAsync().RunSync();
        public async Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            if (string.Equals(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, _storageFolder.Path, StringComparison.Ordinal))
            {
                throw new RootFolderDeletionException("Cannot delete root storage folder.");
            }

            try
            {
                await _storageFolder.DeleteAsync()
                    .AsTask(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new FolderNotFoundException(_storageFolder.Path, ex);
            }
        }

        public IFile GetFile(string name) => GetFileAsync(name).RunSync();
        public async Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var storageFile = await _storageFolder.GetFileAsync(name)
                    .AsTask(cancellationToken);
                return new StorageFileImplementation(storageFile);
            }
            catch (Exception ex)
            {
                throw new Exceptions.FileNotFoundException(name, ex);
            }

        }

        public IList<IFile> GetFiles(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly) =>
            GetFilesAsync(searchPattern, searchOption).RunSync();
        public async Task<IList<IFile>> GetFilesAsync(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly, CancellationToken cancellationToken = default)
        {
            var fileTypeFilter = new List<string>();
            fileTypeFilter.Add("*");

            var queryOptions = new QueryOptions(
                CommonFileQuery.OrderBySearchRank, fileTypeFilter);

            if (searchOption == FolderSearchOption.AllFolders)
                queryOptions.FolderDepth = FolderDepth.Deep;
            else if (searchOption == FolderSearchOption.TopFolderOnly)
                queryOptions.FolderDepth = FolderDepth.Shallow;
            else
                throw new NotSupportedException(
                    $"Not supported {nameof(searchOption)}: {searchOption}");

            queryOptions.UserSearchFilter = searchPattern;
            var queryResult = _storageFolder.
                CreateFileQueryWithOptions(queryOptions);
            var storageFiles = await queryResult.GetFilesAsync()
                .AsTask(cancellationToken);

            return storageFiles.Select(
                o => new StorageFileImplementation(o)).ToList<IFile>();
        }

        public IFolder GetFolder(string name) => GetFolderAsync(name).RunSync();
        public async Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var storageFolder = await _storageFolder.GetFolderAsync(name)
                    .AsTask(cancellationToken);
                return new StorageFolderImplementation(storageFolder);
            }
            catch (Exception ex)
            {
                throw new FolderNotFoundException(name, ex);
            }

        }

        public IList<IFolder> GetFolders() => GetFoldersAsync().RunSync();
        public async Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default)
        {
            var storageFolders = await _storageFolder.GetFoldersAsync()
                .AsTask(cancellationToken);
            return storageFolders.Select(o => new StorageFolderImplementation(o)).ToList<IFolder>();
        }

        public void Move(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting) => MoveAsync(folder, option).RunSync();
        public async Task MoveAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
        {
            var subfolders = await GetFoldersAsync(cancellationToken);
            foreach (var subfolder in subfolders)
            {
                var moveToSubfolderName = System.IO.Path.Combine(
                    folder.Path, subfolder.Name);
                CreationCollisionOption creationCollisionOption;
                switch (option)
                {
                    case NameCollisionOption.ReplaceExisting:
                        creationCollisionOption = CreationCollisionOption.ReplaceExisting;
                        break;
                    case NameCollisionOption.GenerateUniqueName:
                        creationCollisionOption = CreationCollisionOption.GenerateUniqueName;
                        break;
                    case NameCollisionOption.FailIfExists:
                        creationCollisionOption = CreationCollisionOption.FailIfExists;
                        break;
                    default:
                        throw new NotSupportedException($"Not supported {nameof(option)}: {option}");
                }

                var moveToSubfoler = await folder.CreateFolderAsync(subfolder.Name, creationCollisionOption);
                await subfolder.MoveAsync(moveToSubfoler, option, cancellationToken);
            }

            var files = await GetFilesAsync(cancellationToken: cancellationToken);
            foreach (var file in files)
            {
                var newFilePath = PortablePath.Combine(folder.Path, file.Name);
                var movedFile = await file.MoveAsync(newFilePath, option, cancellationToken);
            }
            if (folder.Path.StartsWith(_storageFolder.Path) &&
               (folder.Path.Length == _storageFolder.Path.Length ||
                folder.Path[_storageFolder.Path.Length] == PortablePath.DirectorySeparatorChar))
                return;
            await DeleteAsync(cancellationToken);
        }

        public IFolder Rename(string newName) => RenameAsync(newName).RunSync();
        public async Task<IFolder> RenameAsync(string newName, CancellationToken cancellationToken = default)
        {
            try
            {
                var oldStorageFolder = await StorageFolder.GetFolderFromPathAsync(_storageFolder.Path);
                await _storageFolder.RenameAsync(newName)
                    .AsTask(cancellationToken);
                var result = new StorageFolderImplementation(_storageFolder);
                _storageFolder = oldStorageFolder;
                return result;
            }
            catch (Exception ex)
            {
                throw new FolderExistException(newName, ex);
            }
        }
    }
}
