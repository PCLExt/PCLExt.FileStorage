using PCLExt.FileStorage.UWP.Extensions;
using System;
using System.Collections.Generic;
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

        public bool Exists => true;

        private readonly StorageFolder _storageFolder;

        public StorageFolderImplementation(StorageFolder storageFolder)
        {
            _storageFolder = storageFolder;
        }

        public StorageFolderImplementation(String path)
        {
            _storageFolder = StorageFolder.GetFolderFromPathAsync(path).GetResults();
        }

        public ExistenceCheckResult CheckExists(string name)
        {
            return CheckExistsAsync(name).Result;
        }

        public async Task<ExistenceCheckResult> CheckExistsAsync(
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var finded = await _storageFolder.GetItemAsync(name).AsTask(cancellationToken);
            if (finded == null)
            {
                return ExistenceCheckResult.NotFound;
            }

            if (finded.Attributes == FileAttributes.Directory)
            {
                return ExistenceCheckResult.FolderExists;
            }

            return ExistenceCheckResult.FileExists;
        }

        public void Copy(
            IFolder folder, 
            NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            CopyAsync(folder, option).Wait();
        }

        public async Task CopyAsync(
            IFolder folder, 
            NameCollisionOption option = NameCollisionOption.ReplaceExisting, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var creationCollisionOption = StorageExtensions.ConvertToCreationCollision(option);
            
            var targetFolder = await folder.CreateFolderAsync(Name,
                creationCollisionOption, cancellationToken) as StorageFolderImplementation;

            if (targetFolder == null)
                throw new Exception("Can't create StorageFolder");

            // Get all files (shallow) from source
            var queryOptions = new QueryOptions
            {
                IndexerOption = IndexerOption.DoNotUseIndexer,  // Avoid problems cause by out of sync indexer
                FolderDepth = FolderDepth.Shallow,
            };
            var queryFiles = _storageFolder.CreateFileQueryWithOptions(queryOptions);
            var files = await queryFiles.GetFilesAsync();

            // Copy files into target folder
            foreach (var storageFile in files)
            {
                await storageFile.CopyAsync(targetFolder._storageFolder, storageFile.Name,
                    StorageExtensions.ConvertToNameCollision(option));
            }

            // Get all folders (shallow) from source
            var queryFolders = _storageFolder.CreateFolderQueryWithOptions(queryOptions);
            var folders = await queryFolders.GetFoldersAsync();

            // For each folder call CopyAsync with new destination as destination
            foreach (var storageFolder in folders)
            {
                await CopyAsync(GetFolder(storageFolder.Name), option, cancellationToken);
            }
        }

        public IFile CreateFile(
            string desiredName,
            CreationCollisionOption option)
        {
            return CreateFileAsync(desiredName, option).Result;
        }

        public async Task<IFile> CreateFileAsync(
            string desiredName, 
            CreationCollisionOption option,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsOption = StorageExtensions.
                ConvertToWindowsCreationCollisionOption(option);
            var file = await _storageFolder.CreateFileAsync(
                desiredName, windowsOption).AsTask(cancellationToken);
            return new StorageFileImplementation(file);
        }

        public IFolder CreateFolder(
            string desiredName,
            CreationCollisionOption option)
        {
            return CreateFolderAsync(desiredName, option).Result;
        }

        public async Task<IFolder> CreateFolderAsync(
            string desiredName, 
            CreationCollisionOption option, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsOption = StorageExtensions.
                ConvertToWindowsCreationCollisionOption(option);
            var folder = await _storageFolder.CreateFolderAsync(
                desiredName, windowsOption).AsTask(cancellationToken);
            return new StorageFolderImplementation(folder);
        }

        public void Delete()
        {
            DeleteAsync().Wait();
        }

        public async Task DeleteAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await _storageFolder.DeleteAsync().AsTask(cancellationToken);
        }

        public IFile GetFile(string name)
        {
            return GetFileAsync(name).Result;
        }

        public async Task<IFile> GetFileAsync(
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var storageFile = await _storageFolder.GetFileAsync(name).
                AsTask(cancellationToken);
            return new StorageFileImplementation(storageFile);
        }

        public IList<IFile> GetFiles(
            string searchPattern = "*", 
            FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly)
        {
            return GetFilesAsync(searchPattern, searchOption).Result;
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
            return GetFolderAsync(name).Result;
        }

        public async Task<IFolder> GetFolderAsync(
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var storageFolder = await _storageFolder.
                GetFolderAsync(name).AsTask(cancellationToken);
            return new StorageFolderImplementation(storageFolder);
        }

        public IList<IFolder> GetFolders()
        {
            return GetFoldersAsync().Result;
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
            MoveAsync(folder, option).Wait();
        }

        public async Task MoveAsync(
            IFolder folder,
            NameCollisionOption option = NameCollisionOption.ReplaceExisting,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var subfolders = await folder.GetFoldersAsync(cancellationToken);
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

            var files = await folder.GetFilesAsync(
                cancellationToken: cancellationToken);
            foreach(var file in files)
            {
                var newFilePath = System.IO.Path.Combine(folder.Path, file.Name);
                await file.MoveAsync(newFilePath, option, cancellationToken);
            }
        }

        public IFolder Rename(string newName)
        {
            return RenameAsync(newName).Result;
        }

        public async Task<IFolder> RenameAsync(
            string newName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await _storageFolder.RenameAsync(newName).AsTask(cancellationToken);
            return this;
        }
    }
}
