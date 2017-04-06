//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents a folder in the <see cref="DesktopFileSystem"/>.
    /// </summary>
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public class FileSystemFolder : IFolder
    {
        private readonly bool _canDelete;

        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Path { get; }

        /// <summary>
        /// Creates a new <see cref="FileSystemFolder" /> corresponding to a specified path.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <param name="canDelete">Specifies whether the folder can be deleted (via <see cref="DeleteAsync"/>).</param>
        public FileSystemFolder(string path, bool canDelete = false) { Name = System.IO.Path.GetFileName(path); Path = path; _canDelete = canDelete; }

        /// <inheritdoc />
        public IFile CreateFile(string desiredName, CreationCollisionOption option)
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            EnsureExists();

            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (File.Exists(newPath))
            {
                if (option == CreationCollisionOption.GenerateUniqueName)
                {
                    var desiredRoot = System.IO.Path.GetFileNameWithoutExtension(desiredName);
                    var desiredExtension = System.IO.Path.GetExtension(desiredName);
                    for (var num = 2; File.Exists(newPath); num++)
                    {
                        nameToUse = $"{desiredRoot} ({num}){desiredExtension}";
                        newPath = System.IO.Path.Combine(Path, nameToUse);
                    }
                    InternalCreateFile(newPath);
                }
                else if (option == CreationCollisionOption.ReplaceExisting)
                {
                    File.Delete(newPath);
                    InternalCreateFile(newPath);
                }
                else if (option == CreationCollisionOption.FailIfExists)
                    throw new IOException($"File already exists: {newPath}");
                else if (option == CreationCollisionOption.OpenIfExists)
                {
                    //	No operation.
                }
                else
                    throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
            }
            else
            {
                //	Create file.
                InternalCreateFile(newPath);
            }

            return new FileSystemFile(newPath);
        }
        /// <inheritdoc />
        public async Task<IFile> CreateFileAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);
            EnsureExists();

            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (File.Exists(newPath))
            {
                if (option == CreationCollisionOption.GenerateUniqueName)
                {
                    var desiredRoot = System.IO.Path.GetFileNameWithoutExtension(desiredName);
                    var desiredExtension = System.IO.Path.GetExtension(desiredName);
                    for (var num = 2; File.Exists(newPath); num++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        nameToUse = $"{desiredRoot} ({num}){desiredExtension}";
                        newPath = System.IO.Path.Combine(Path, nameToUse);
                    }
                    InternalCreateFile(newPath);
                }
                else if (option == CreationCollisionOption.ReplaceExisting)
                {
                    File.Delete(newPath);
                    InternalCreateFile(newPath);
                }
                else if (option == CreationCollisionOption.FailIfExists)
                    throw new IOException($"File already exists: {newPath}");
                else if (option == CreationCollisionOption.OpenIfExists)
                {
                    //	No operation.
                }
                else
                    throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
            }
            else
            {
                //	Create file.
                InternalCreateFile(newPath);
            }

            return new FileSystemFile(newPath);
        }
        void InternalCreateFile(string path)
        {
            using (var stream = File.Create(path)) { }
        }

        /// <inheritdoc />
        public IFile GetFile(string name)
        {
            var path = System.IO.Path.Combine(Path, name);
            if (!File.Exists(path))
                throw new FileNotFoundException($"File does not exist: {path}");
            return new FileSystemFile(path);
        }
        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var path = System.IO.Path.Combine(Path, name);
            if (!File.Exists(path))
                throw new FileNotFoundException($"File does not exist: {path}");
            return new FileSystemFile(path);
        }

        /// <inheritdoc />
        public IList<IFile> GetFiles(string searchPattern = "", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly)
        {
            EnsureExists();
            return Directory.GetFiles(Path, searchPattern, (SearchOption) searchOption).Select(f => new FileSystemFile(f)).ToList<IFile>().AsReadOnly();
        }
        /// <inheritdoc />
        public async Task<IList<IFile>> GetFilesAsync(string searchPattern = "", FolderSearchOption searchOption = FolderSearchOption.TopFolderOnly, CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();
            return Directory.GetFiles(Path, searchPattern, (SearchOption) searchOption).Select(f => new FileSystemFile(f)).ToList<IFile>().AsReadOnly();
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
                if (option == CreationCollisionOption.GenerateUniqueName)
                {
                    for (var num = 2; Directory.Exists(newPath); num++)
                    {
                        nameToUse = $"{desiredName} ({num})";
                        newPath = System.IO.Path.Combine(Path, nameToUse);
                    }
                    Directory.CreateDirectory(newPath);
                }
                else if (option == CreationCollisionOption.ReplaceExisting)
                {
                    Directory.Delete(newPath, true);
                    Directory.CreateDirectory(newPath);
                }
                else if (option == CreationCollisionOption.FailIfExists)
                    throw new IOException($"Directory already exists: {newPath}");
                
                else if (option == CreationCollisionOption.OpenIfExists)
                {
                    //	No operation.
                }
                else
                    throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
            }
            else
                Directory.CreateDirectory(newPath);
            
            return new FileSystemFolder(newPath, true);
        }
        /// <inheritdoc />
        public async Task<IFolder> CreateFolderAsync(string desiredName, CreationCollisionOption option, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNullOrEmpty(desiredName, nameof(desiredName));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();
            var nameToUse = desiredName;
            var newPath = System.IO.Path.Combine(Path, nameToUse);
            if (Directory.Exists(newPath))
            {
                if (option == CreationCollisionOption.GenerateUniqueName)
                {
                    for (var num = 2; Directory.Exists(newPath); num++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        nameToUse = $"{desiredName} ({num})";
                        newPath = System.IO.Path.Combine(Path, nameToUse);
                    }
                    Directory.CreateDirectory(newPath);
                }
                else if (option == CreationCollisionOption.ReplaceExisting)
                {
                    Directory.Delete(newPath, true);
                    Directory.CreateDirectory(newPath);
                }
                else if (option == CreationCollisionOption.FailIfExists)
                    throw new IOException($"Directory already exists: {newPath}");

                else if (option == CreationCollisionOption.OpenIfExists)
                {
                    //	No operation.
                }
                else
                    throw new ArgumentException($"Unrecognized CreationCollisionOption: {option}");
            }
            else
                Directory.CreateDirectory(newPath);

            return new FileSystemFolder(newPath, true);
        }

        /// <inheritdoc />
        public IFolder GetFolder(string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            var path = System.IO.Path.Combine(Path, name);
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory does not exist: {path}");
            return new FileSystemFolder(path, true);
        }
        /// <inheritdoc />
        public async Task<IFolder> GetFolderAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var path = System.IO.Path.Combine(Path, name);
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Directory does not exist: {path}");
            return new FileSystemFolder(path, true);
        }

        /// <inheritdoc />
        public IList<IFolder> GetFolders()
        {
            EnsureExists();
            return Directory.GetDirectories(Path).Select(d => new FileSystemFolder(d, true)).ToList<IFolder>().AsReadOnly();
        }
        /// <inheritdoc />
        public async Task<IList<IFolder>> GetFoldersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();
            return Directory.GetDirectories(Path).Select(d => new FileSystemFolder(d, true)).ToList<IFolder>().AsReadOnly();
        }

        /// <inheritdoc />
        public ExistenceCheckResult CheckExists(string name)
        {
            Requires.NotNullOrEmpty(name, "name");

            var checkPath = System.IO.Path.Combine(Path, name);
            if (File.Exists(checkPath))
                return ExistenceCheckResult.FileExists;
            else if (Directory.Exists(checkPath))
                return ExistenceCheckResult.FolderExists;
            else
                return ExistenceCheckResult.NotFound;
        }
        /// <inheritdoc />
        public async Task<ExistenceCheckResult> CheckExistsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNullOrEmpty(name, "name");

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
                throw new IOException("Cannot delete root storage folder.");

            EnsureExists();
            Directory.Delete(Path, true);
        }
        /// <inheritdoc />
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            if (!_canDelete)
                throw new IOException("Cannot delete root storage folder.");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();
            Directory.Delete(Path, true);
        }

        /// <inheritdoc />
        public IFolder Move(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            Requires.NotNull(folder, nameof(folder));

            EnsureExists();

            var files = GetFiles();
            foreach (var file in files)
                file.Move(System.IO.Path.Combine(folder.Path, file.Name), option);

            var folders = GetFolders();
            foreach (var nFolder in folders)
                nFolder.Move(folder.CreateFolder(nFolder.Name, CreationCollisionOption.OpenIfExists), option);

            Delete();
            return folder;
        }
        /// <inheritdoc />
        public async Task<IFolder> MoveAsync(IFolder folder, NameCollisionOption option = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default(CancellationToken))
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

            await DeleteAsync(cancellationToken);
            return folder;
        }

        void EnsureExists()
        {
            if (!Directory.Exists(Path))
                throw new DirectoryNotFoundException("Directory does not exist: " + Path);
        }
    }
}
