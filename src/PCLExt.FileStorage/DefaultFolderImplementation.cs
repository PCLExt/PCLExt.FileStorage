//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage
{
    /// <inheritdoc />
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    internal class DefaultFolderImplementation : IFolder
    {
        private readonly bool _canDelete;

        /// <inheritdoc />
        public string Name => System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(Path));
        /// <inheritdoc />
        public string Path { get; }
        /// <inheritdoc />
        public bool Exists => Directory.Exists(Path);
        /// <inheritdoc />
        public DateTime CreationTime => Directory.GetCreationTime(Path);
        /// <inheritdoc />
        public DateTime CreationTimeUTC => Directory.GetCreationTimeUtc(Path);
        /// <inheritdoc />
        public DateTime LastAccessTime => Directory.GetLastAccessTime(Path);
        /// <inheritdoc />
        public DateTime LastAccessTimeUTC => Directory.GetLastAccessTimeUtc(Path);
        /// <inheritdoc />
        public DateTime LastWriteTime => Directory.GetLastWriteTime(Path);
        /// <inheritdoc />
        public DateTime LastWriteTimeUTC => Directory.GetLastWriteTimeUtc(Path);

        /// <summary>
        /// Creates a new <see cref="IFolder" /> corresponding to a specified path.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <param name="canDelete">Specifies whether the folder can be deleted (via <see cref="DeleteAsync"/>).</param>
        public DefaultFolderImplementation(string path, bool canDelete = false)
        {
            Path = path.EnsureEndsWith(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            _canDelete = canDelete;
        }

        /// <inheritdoc />
        public IFile CreateFile(string desiredName, CreationCollisionOption option)
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            EnsureExists();

            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (File.Exists(newPath))
            {
                switch (option)
                {
                    case CreationCollisionOption.GenerateUniqueName:
                        var desiredRoot = System.IO.Path.GetFileNameWithoutExtension(desiredName);
                        var desiredExtension = System.IO.Path.GetExtension(desiredName);
                        for (var num = 2; File.Exists(newPath); num++)
                        {
                            nameToUse = $"{desiredRoot} ({num}){desiredExtension}";
                            newPath = System.IO.Path.Combine(Path, nameToUse);
                        }
                        InternalCreateFile(newPath);
                        break;
                    case CreationCollisionOption.ReplaceExisting:
                        File.Delete(newPath);
                        InternalCreateFile(newPath);
                        break;
                    case CreationCollisionOption.FailIfExists:
                        throw new FileExistException($"File already exists: {newPath}");
                    case CreationCollisionOption.OpenIfExists:
                        //	No operation.
                        break;
                    default:
                        throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
                }
            }
            else
            {
                //	Create file.
                InternalCreateFile(newPath);
            }

            return new DefaultFileImplementation(newPath);
        }
        /// <inheritdoc />
        public async Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (File.Exists(newPath))
            {
                switch (option)
                {
                    case CreationCollisionOption.GenerateUniqueName:
                        var desiredRoot = System.IO.Path.GetFileNameWithoutExtension(desiredName);
                        var desiredExtension = System.IO.Path.GetExtension(desiredName);
                        for (var num = 2; File.Exists(newPath); num++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            nameToUse = $"{desiredRoot} ({num}){desiredExtension}";
                            newPath = System.IO.Path.Combine(Path, nameToUse);
                        }
                        InternalCreateFile(newPath);
                        break;
                    case CreationCollisionOption.ReplaceExisting:
                        File.Delete(newPath);
                        InternalCreateFile(newPath);
                        break;
                    case CreationCollisionOption.FailIfExists:
                        throw new FileExistException($"File already exists: {newPath}");
                    case CreationCollisionOption.OpenIfExists:
                        //	No operation.
                        break;
                    default:
                        throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
                }
            }
            else
            {
                //	Create file.
                InternalCreateFile(newPath);
            }

            return new DefaultFileImplementation(newPath);
        }
        private void InternalCreateFile(string path)
        {
            using (var stream = File.Create(path)) { }
        }

        /// <inheritdoc />
        public IFile GetFile(string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            var path = System.IO.Path.Combine(Path, name);
            if (!File.Exists(path))
                throw new Exceptions.FileNotFoundException($"File does not exist: {path}");
            return new DefaultFileImplementation(path);
        }
        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var path = System.IO.Path.Combine(Path, name);
            if (!File.Exists(path))
                throw new Exceptions.FileNotFoundException($"File does not exist: {path}");
            return new DefaultFileImplementation(path);
        }

        /// <inheritdoc />
        public IList<IFile> GetFiles(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly)
        {
            EnsureExists();

            return Directory.GetFiles(Path, searchPattern, (SearchOption) searchOption).Select(f => new DefaultFileImplementation(f)).ToList<IFile>().AsReadOnly();
        }
        /// <inheritdoc />
        public async Task<IList<IFile>> GetFilesAsync(string searchPattern = "*", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly, CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            return Directory.GetFiles(Path, searchPattern, (SearchOption) searchOption).Select(f => new DefaultFileImplementation(f)).ToList<IFile>().AsReadOnly();
        }

        /// <inheritdoc />
        public IFolder CreateFolder(string desiredName, CreationCollisionOption option)
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            EnsureExists();

            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (Directory.Exists(newPath))
            {
                switch (option)
                {
                    case CreationCollisionOption.GenerateUniqueName:
                        for (var num = 2; Directory.Exists(newPath); num++)
                        {
                            nameToUse = $"{desiredName} ({num})";
                            newPath = System.IO.Path.Combine(Path, nameToUse);
                        }
                        Directory.CreateDirectory(newPath);
                        break;
                    case CreationCollisionOption.ReplaceExisting:
                        Directory.Delete(newPath, true);
                        Directory.CreateDirectory(newPath);
                        break;
                    case CreationCollisionOption.FailIfExists:
                        throw new FolderExistException($"Directory already exists: {newPath}");
                    case CreationCollisionOption.OpenIfExists:
                        //	No operation.
                        break;
                    default:
                        throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
                }
            }
            else
                Directory.CreateDirectory(newPath);

            return new DefaultFolderImplementation(newPath, true);
        }
        /// <inheritdoc />
        public async Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (Directory.Exists(newPath))
            {
                switch (option)
                {
                    case CreationCollisionOption.GenerateUniqueName:
                        for (var num = 2; Directory.Exists(newPath); num++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            nameToUse = $"{desiredName} ({num})";
                            newPath = System.IO.Path.Combine(Path, nameToUse);
                        }
                        Directory.CreateDirectory(newPath);
                        break;
                    case CreationCollisionOption.ReplaceExisting:
                        Directory.Delete(newPath, true);
                        Directory.CreateDirectory(newPath);
                        break;
                    case CreationCollisionOption.FailIfExists:
                        throw new FolderExistException($"Directory already exists: {newPath}");
                    case CreationCollisionOption.OpenIfExists:
                        //	No operation.
                        break;
                    default:
                        throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
                }
            }
            else
                Directory.CreateDirectory(newPath);

            return new DefaultFolderImplementation(newPath, true);
        }

        /// <inheritdoc />
        public IFolder GetFolder(string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            var path = System.IO.Path.Combine(Path, name);
            if (!Directory.Exists(path))
                throw new FolderNotFoundException($"Directory does not exist: {path}");
            return new DefaultFolderImplementation(path, true);
        }
        /// <inheritdoc />
        public async Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var path = System.IO.Path.Combine(Path, name);
            if (!Directory.Exists(path))
                throw new FolderNotFoundException($"Directory does not exist: {path}");
            return new DefaultFolderImplementation(path, true);
        }

        /// <inheritdoc />
        public IList<IFolder> GetFolders()
        {
            EnsureExists();

            return Directory.GetDirectories(Path).Select(d => new DefaultFolderImplementation(d, true)).ToList<IFolder>().AsReadOnly();
        }
        /// <inheritdoc />
        public async Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            return Directory.GetDirectories(Path).Select(d => new DefaultFolderImplementation(d, true)).ToList<IFolder>().AsReadOnly();
        }

        /// <inheritdoc />
        public ExistenceCheckResult CheckExists(string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            var checkPath = System.IO.Path.Combine(Path, name);
            if (File.Exists(checkPath))
                return ExistenceCheckResult.FileExists;
            else if (Directory.Exists(checkPath))
                return ExistenceCheckResult.FolderExists;
            else
                return ExistenceCheckResult.NotFound;
        }
        /// <inheritdoc />
        public async Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var checkPath = System.IO.Path.Combine(Path, name);
            if (File.Exists(checkPath))
                return ExistenceCheckResult.FileExists;
            else if (Directory.Exists(checkPath))
                return ExistenceCheckResult.FolderExists;
            else
                return ExistenceCheckResult.NotFound;
        }

        /// <inheritdoc />
        public void Delete()
        {
            if (!_canDelete)
                throw new RootFolderDeletionException("Cannot delete root storage folder.");

            EnsureExists();

            Directory.Delete(Path, true);
        }
        /// <inheritdoc />
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            if (!_canDelete)
                throw new RootFolderDeletionException("Cannot delete root storage folder.");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            Directory.Delete(Path, true);
        }

        /// <inheritdoc />
        public IFolder Rename(string newName)
        {
            Requires.NotNullOrEmpty(newName, nameof(newName));

            EnsureExists();

            var parentFolder = new DirectoryInfo(Path).Parent.FullName;
            var newPath = System.IO.Path.Combine(parentFolder, newName);

            if(Directory.Exists(newPath))
                throw new FolderExistException($"Can't rename to {newName}! A folder with this name already exists.");

            Directory.Move(Path, newPath);

            return new DefaultFolderImplementation(newPath, true);
        }
        /// <inheritdoc />
        public async Task<IFolder> RenameAsync(string newName, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newName, nameof(newName));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var parentFolder = new DirectoryInfo(Path).Parent.FullName;
            var newPath = System.IO.Path.Combine(parentFolder, newName);

            if (Directory.Exists(newPath))
                throw new FolderExistException($"Can't rename to {newName}! A folder with this name already exists.");

            Directory.Move(Path, newPath);

            return new DefaultFolderImplementation(newPath, true);
        }

        /// <inheritdoc />
        public void Move(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            Requires.NotNull(folder, nameof(folder));

            EnsureExists();

            var files = GetFiles();
            foreach (var file in files)
                file.Move(System.IO.Path.Combine(folder.Path, file.Name), option);

            var folders = GetFolders();
            foreach (var nFolder in folders)
                nFolder.Move(folder.CreateFolder(nFolder.Name, CreationCollisionOption.OpenIfExists), option);

            if(Path != folder.Path)
                Delete();
        }
        /// <inheritdoc />
        public async Task MoveAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(folder, nameof(folder));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var files = await GetFilesAsync(cancellationToken: cancellationToken);
            foreach (var file in files)
                await file.MoveAsync(System.IO.Path.Combine(folder.Path, file.Name), option, cancellationToken);

            var folders = await GetFoldersAsync(cancellationToken);
            foreach (var nFolder in folders)
                await nFolder.MoveAsync(await folder.CreateFolderAsync(nFolder.Name, CreationCollisionOption.OpenIfExists, cancellationToken), option, cancellationToken);

            if (Path != folder.Path)
                await DeleteAsync(cancellationToken);
        }

        /// <inheritdoc />
        public void Copy(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            Requires.NotNull(folder, nameof(folder));

            EnsureExists();

            var files = GetFiles();
            foreach (var file in files)
                file.Copy(System.IO.Path.Combine(folder.Path, file.Name), option);

            var folders = GetFolders();
            foreach (var nFolder in folders)
                nFolder.Copy(folder.CreateFolder(nFolder.Name, CreationCollisionOption.OpenIfExists), option);
        }
        /// <inheritdoc />
        public async Task CopyAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(folder, nameof(folder));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var files = await GetFilesAsync(cancellationToken: cancellationToken);
            foreach (var file in files)
                await file.CopyAsync(System.IO.Path.Combine(folder.Path, file.Name), option, cancellationToken);

            var folders = await GetFoldersAsync(cancellationToken);
            foreach (var nFolder in folders)
                await nFolder.CopyAsync(await folder.CreateFolderAsync(nFolder.Name, CreationCollisionOption.OpenIfExists, cancellationToken), option, cancellationToken);
        }

        private void EnsureExists()
        {
            if (!Directory.Exists(Path))
                throw new FolderNotFoundException("Directory does not exist: " + Path);
        }
    }
}
#endif
