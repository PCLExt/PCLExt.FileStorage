using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.UWP.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace PCLExt.FileStorage.UWP
{
    sealed class StorageFolderImplementation : IFolder
    {
        public string Name => _storageFolder.Name;

        public string Path => _storageFolder.Path;

        public bool Exists
        {
            get
            {
                try
                {
                    var storageFolder = StorageFolder.GetFolderFromPathAsync(
                        _storageFolder.Path).AsTask().GetAwaiter().GetResult();
                }
                catch (System.IO.FileNotFoundException)
                {
                    return false;
                }
                return true;
            }
        }

        private StorageFolder _storageFolder;

        public StorageFolderImplementation(StorageFolder storageFolder)
        {
            _storageFolder = storageFolder;
        }

        public StorageFolderImplementation(String path)
        {
            _storageFolder = StorageFolder.GetFolderFromPathAsync(path).
                AsTask().GetAwaiter().GetResult();
        }

        public ExistenceCheckResult CheckExists(string name)
        {
            return CheckExistsAsync(name).GetAwaiter().GetResult();
        }

        public async Task<ExistenceCheckResult> CheckExistsAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var finded = await _storageFolder.GetItemAsync(name).AsTask(cancellationToken);
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

        public void Copy(
            IFolder folder,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            CopyAsync(folder, option).GetAwaiter().GetResult();
        }

        public async Task CopyAsync(
            IFolder folder,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await CopyAsync(_storageFolder, folder, option, cancellationToken);
        }

        private async Task CopyAsync(
           StorageFolder source,
           IFolder folder,
           NameCollisionOption option = NameCollisionOption.ReplaceExisting,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var creationCollisionOption = StorageExtensions.ConvertToCreationCollision(option);

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
                    var targetStorageFolder = await StorageFolder.
                        GetFolderFromPathAsync(folder.Path);
                    var windowsOption = StorageExtensions.ConvertToNameCollision(option);
                    if (windowsOption == Windows.Storage.NameCollisionOption.ReplaceExisting)
                    {
                        var findedFile = await targetStorageFolder.
                            GetFileAsync(storageFile.Name);
                        if (null != findedFile &&
                            findedFile.Path == storageFile.Path)
                            continue;
                    }
                    var copiedFile = await storageFile.CopyAsync(targetStorageFolder,
                        storageFile.Name,
                        windowsOption);
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
                var targetFolder = await folder.CreateFolderAsync(storageFolder.Name,
                creationCollisionOption, cancellationToken) as StorageFolderImplementation;

                if (targetFolder == null)
                    throw new Exception("Can't create StorageFolder");
                await CopyAsync(storageFolder, targetFolder, option, cancellationToken);
            }
        }

        public IFile CreateFile(
            string desiredName,
            CreationCollisionOption option)
        {
            return CreateFileAsync(desiredName, option).GetAwaiter().GetResult();
        }

        public async Task<IFile> CreateFileAsync(
            string desiredName,
            CreationCollisionOption option,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsOption = StorageExtensions.
                ConvertToWindowsCreationCollisionOption(option);
            try
            {
                var file = await _storageFolder.CreateFileAsync(
                    desiredName, windowsOption).AsTask(cancellationToken);
                return new StorageFileImplementation(file);
            }
            catch (Exception ex)
            {
                throw new Exceptions.FileExistException(desiredName, ex);
            }


        }

        public IFolder CreateFolder(
            string desiredName,
            CreationCollisionOption option)
        {
            return CreateFolderAsync(desiredName, option).GetAwaiter().GetResult();
        }

        public async Task<IFolder> CreateFolderAsync(
            string desiredName,
            CreationCollisionOption option,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsOption = StorageExtensions.
                ConvertToWindowsCreationCollisionOption(option);
            try
            {
                var folder = await _storageFolder.CreateFolderAsync(
                    desiredName, windowsOption).AsTask(cancellationToken);
                return new StorageFolderImplementation(folder);
            }
            catch (Exception ex)
            {
                throw new Exceptions.FolderExistException(desiredName, ex);
            }
        }

        public void Delete()

        {
            DeleteAsync().GetAwaiter().GetResult();
        }

        public async Task DeleteAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if(Windows.ApplicationModel.Package.Current.InstalledLocation.Path == _storageFolder.Path)
            {
                throw new RootFolderDeletionException("Cannot delete root storage folder.");
            }

            try
            {
                await _storageFolder.DeleteAsync().AsTask(cancellationToken);
            }
            catch(Exception ex)
            {
                throw new FolderNotFoundException(_storageFolder.Path, ex);
            }
        }

        public IFile GetFile(string name)
        {
            return GetFileAsync(name).GetAwaiter().GetResult();
        }

        public async Task<IFile> GetFileAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var storageFile = await _storageFolder.GetFileAsync(name).
                    AsTask(cancellationToken);
                return new StorageFileImplementation(storageFile);
            }
            catch (Exception ex)
            {
                throw new Exceptions.FileNotFoundException(name, ex);
            }

        }

        public IList<IFile> GetFiles(
            string searchPattern = "*",
            FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly)
        {
            return GetFilesAsync(searchPattern, searchOption).GetAwaiter().GetResult();
        }

        public async Task<IList<IFile>> GetFilesAsync(
            string searchPattern = "*",
            FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly,
            CancellationToken cancellationToken = default(CancellationToken))
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
            var storageFiles = await queryResult.GetFilesAsync().
                AsTask(cancellationToken);

            return storageFiles.Select(
                o => new StorageFileImplementation(o)).ToList<IFile>();
        }

        public IFolder GetFolder(string name)
        {
            return GetFolderAsync(name).GetAwaiter().GetResult();
        }

        public async Task<IFolder> GetFolderAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var storageFolder = await _storageFolder.
                    GetFolderAsync(name).AsTask(cancellationToken);
                return new StorageFolderImplementation(storageFolder);
            }
            catch (Exception ex)
            {
                throw new FolderNotFoundException(name, ex);
            }

        }

        public IList<IFolder> GetFolders()
        {
            return GetFoldersAsync().GetAwaiter().GetResult();
        }

        public async Task<IList<IFolder>> GetFoldersAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var storageFolders = await _storageFolder.
                GetFoldersAsync().AsTask(cancellationToken);
            return storageFolders.Select(
                o => new StorageFolderImplementation(o)).ToList<IFolder>();
        }

        public void Move(
            IFolder folder,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            MoveAsync(folder, option).GetAwaiter().GetResult();
        }

        public async Task MoveAsync(
            IFolder folder,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var subfolders = await GetFoldersAsync(cancellationToken);
            foreach (var subfolder in subfolders)
            {
                var moveToSubfolderName = System.IO.Path.Combine(
                    folder.Path, subfolder.Name);
                var creationCollisionOption = default(CreationCollisionOption);
                if (option == NameCollisionOption.ReplaceExisting)
                    creationCollisionOption = CreationCollisionOption.ReplaceExisting;
                else if (option == NameCollisionOption.GenerateUniqueName)
                    creationCollisionOption = CreationCollisionOption.GenerateUniqueName;
                else if (option == NameCollisionOption.FailIfExists)
                    creationCollisionOption = CreationCollisionOption.FailIfExists;
                else
                    throw new NotSupportedException(
                        $"Not supported {nameof(option)}: {option}");

                var moveToSubfoler = await folder.CreateFolderAsync(
                    subfolder.Name, creationCollisionOption);
                await subfolder.MoveAsync(moveToSubfoler, option, cancellationToken);
            }

            var files = await GetFilesAsync(
                cancellationToken: cancellationToken);
            foreach (var file in files)
            {
                var newFilePath = PortablePath.Combine(folder.Path, file.Name);
                var movedFile = await file.MoveAsync(
                    newFilePath, option, cancellationToken);
            }
            if (folder.Path.StartsWith(_storageFolder.Path) &&
               (folder.Path.Length == _storageFolder.Path.Length ||
                folder.Path[_storageFolder.Path.Length] == PortablePath.DirectorySeparatorChar))
                return;
            await DeleteAsync();
        }

        public IFolder Rename(string newName)
        {
            return RenameAsync(newName).GetAwaiter().GetResult();
        }

        public async Task<IFolder> RenameAsync(
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var oldStorageFolder = await StorageFolder.
                    GetFolderFromPathAsync(_storageFolder.Path);
                await _storageFolder.RenameAsync(newName).AsTask(cancellationToken);
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
