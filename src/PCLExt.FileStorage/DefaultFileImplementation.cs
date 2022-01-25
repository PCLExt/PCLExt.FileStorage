//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

#if NETSTANDARD2_0 || NET5_0 || NETFX45 || __MACOS__ || ANDROID || __IOS__ || WINDOWS_UWP
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using PCLExt.FileStorage.Exceptions;
using PCLExt.FileStorage.Extensions;
using PCLExt.FileStorage.Files;

namespace PCLExt.FileStorage
{
    /// <inheritdoc />
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    internal class DefaultFileImplementation : IFile
    {
        /// <inheritdoc />
        public string Name => System.IO.Path.GetFileName(Path);
        /// <inheritdoc />
        public string Path { get; } // -- This class is a pointer to a place, so we cant change it
        /// <inheritdoc />
        public bool Exists => File.Exists(Path);
        /// <inheritdoc />
        public long Size => new FileInfo(Path).Length;
        /// <inheritdoc />
        public DateTime CreationTime => File.GetCreationTime(Path);
        /// <inheritdoc />
        public DateTime CreationTimeUTC => File.GetCreationTimeUtc(Path);
        /// <inheritdoc />
        public DateTime LastAccessTime => File.GetLastAccessTime(Path);
        /// <inheritdoc />
        public DateTime LastAccessTimeUTC => File.GetLastAccessTimeUtc(Path);
        /// <inheritdoc />
        public DateTime LastWriteTime => File.GetLastWriteTime(Path);
        /// <inheritdoc />
        public DateTime LastWriteTimeUTC => File.GetLastWriteTimeUtc(Path);

        /// <summary>
        /// Creates a new <see cref="IFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public DefaultFileImplementation(string path) => Path = path;

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess)
        {
            EnsureExists();

            switch (fileAccess)
            {
                case FileAccess.Read:
                    return File.OpenRead(Path);
                case FileAccess.ReadAndWrite:
                    return File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite);
                default:
                    throw new ArgumentException($"Unrecognized FileAccess value: {fileAccess}");
            }
        }
        /// <inheritdoc />
        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            switch (fileAccess)
            {
                case FileAccess.Read:
                    return File.OpenRead(Path);
                case FileAccess.ReadAndWrite:
                    return File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite);
                default:
                    throw new ArgumentException($"Unrecognized FileAccess value: {fileAccess}");
            }
        }

        /// <inheritdoc />
        public void Delete()
        {
            EnsureExists();

            File.Delete(Path);
        }
        /// <inheritdoc />
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            File.Delete(Path);
        }

        /// <inheritdoc />
        public void WriteAllBytes(byte[] bytes)
        {
            EnsureExists();

            File.WriteAllBytes(Path, bytes);
        }
        /// <inheritdoc />
        public async Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            File.WriteAllBytes(Path, bytes);
        }

        /// <inheritdoc />
        public byte[] ReadAllBytes()
        {
            EnsureExists();

            return File.ReadAllBytes(Path);
        }
        /// <inheritdoc />
        public async Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            return File.ReadAllBytes(Path);
        }

        /// <inheritdoc />
        public IFile Rename(string newName, NameCollisionOption collisionOption)
        {
            Requires.NotNullOrEmpty(newName, nameof(newName));

            EnsureExists();

            return Move(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), newName), collisionOption);
        }
        /// <inheritdoc />
        public async Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newName, nameof(newName));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            return await MoveAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), newName), collisionOption, cancellationToken);
        }

        /// <inheritdoc />
        public void Move(IFile newFile)
        {
            Requires.NotNull(newFile, nameof(newFile));

            if (newFile.Exists && !string.Equals(Path, newFile.Path, StringComparison.Ordinal))
                newFile.Delete();

            File.Move(Path, newFile.Path);
        }
        /// <inheritdoc />
        public async Task MoveAsync(IFile newFile, CancellationToken cancellationToken)
        {
            Requires.NotNull(newFile, nameof(newFile));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (newFile.Exists && !string.Equals(Path, newFile.Path, StringComparison.Ordinal))
                newFile.Delete();

            File.Move(Path, newFile.Path);
        }

        /// <inheritdoc />
        public void Copy(IFile newFile)
        {
            Requires.NotNull(newFile, nameof(newFile));

            if (string.Equals(Path, newFile.Path, StringComparison.Ordinal))
                return; // -- Windows is refusing to do it when its the same file. Guess it kinda makes sense.

            File.Copy(Path, newFile.Path, true);
        }
        /// <inheritdoc />
        public async Task CopyAsync(IFile newFile, CancellationToken cancellationToken)
        {
            if (string.Equals(Path, newFile.Path, StringComparison.Ordinal))
                return; // -- Windows is refusing to do it when its the same file. Guess it kinda makes sense.

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            File.Copy(Path, newFile.Path);
        }

        /// <inheritdoc />
        public IFile Move(string newPath, NameCollisionOption collisionOption)
        {
            Requires.NotNullOrEmpty(newPath, nameof(newPath));

            EnsureExists();

            var newDirectory = System.IO.Path.GetDirectoryName(newPath);
            var newName = System.IO.Path.GetFileName(newPath);

            for (var counter = 1; ; counter++)
            {
                var candidateName = newName;
                if (counter > 1)
                    candidateName = $"{System.IO.Path.GetFileNameWithoutExtension(newName)} ({counter}){System.IO.Path.GetExtension(newName)}";

                var candidatePath = System.IO.Path.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new FileExistException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            if(!string.Equals(Path, candidatePath, StringComparison.Ordinal))
                                File.Delete(candidatePath);
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized NameCollisionOption value: {collisionOption}");
                    }
                }

                File.Move(Path, candidatePath);

                return new FileFromPath(candidatePath);
            }
        }
        /// <inheritdoc />
        public async Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, nameof(newPath));

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var newDirectory = System.IO.Path.GetDirectoryName(newPath);
            var newName = System.IO.Path.GetFileName(newPath);

            for (var counter = 1; ; counter++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var candidateName = newName;
                if (counter > 1)
                    candidateName = $"{System.IO.Path.GetFileNameWithoutExtension(newName)} ({counter}){System.IO.Path.GetExtension(newName)}";

                var candidatePath = System.IO.Path.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new FileExistException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            if (!string.Equals(Path, candidatePath, StringComparison.Ordinal))
                                File.Delete(candidatePath);
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized NameCollisionOption value: {collisionOption}");
                    }
                }

                File.Move(Path, candidatePath);

                return new FileFromPath(candidatePath);
            }
        }

        /// <inheritdoc />
        public IFile Copy(string newPath, NameCollisionOption collisionOption)
        {
            // Test if path is not same. If same, skip/abort
            // When replacing, firts copy the replaceable file to the buffer, then delete. Dont just do File.Copy guess?
            Requires.NotNullOrEmpty(newPath, nameof(newPath));

            EnsureExists();

            var newDirectory = System.IO.Path.GetDirectoryName(newPath);
            var newName = System.IO.Path.GetFileName(newPath);

            for (var counter = 1; ; counter++)
            {
                var candidateName = newName;
                if (counter > 1)
                    candidateName = $"{System.IO.Path.GetFileNameWithoutExtension(newName)} ({counter}){System.IO.Path.GetExtension(newName)}";

                var candidatePath = System.IO.Path.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new FileExistException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            if (string.Equals(Path, newPath, StringComparison.Ordinal))
                                return new FileFromPath(Path); // -- Windows is refusing to do it when its the same file. Guess it kinda makes sense.

                            File.Delete(candidatePath);
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized NameCollisionOption value: {collisionOption}");
                    }
                }

                File.Copy(Path, candidatePath, true);

                return new FileFromPath(candidatePath);
            }
        }
        /// <inheritdoc />
        public async Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, nameof(newPath));

            EnsureExists();

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            var newDirectory = System.IO.Path.GetDirectoryName(newPath);
            var newName = System.IO.Path.GetFileName(newPath);

            for (var counter = 1; ; counter++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var candidateName = newName;
                if (counter > 1)
                    candidateName = $"{System.IO.Path.GetFileNameWithoutExtension(newName)} ({counter}){System.IO.Path.GetExtension(newName)}";

                var candidatePath = System.IO.Path.Combine(newDirectory, candidateName);

                if (File.Exists(candidatePath))
                {
                    switch (collisionOption)
                    {
                        case NameCollisionOption.FailIfExists:
                            throw new FileExistException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            if (string.Equals(Path, newPath, StringComparison.Ordinal))
                                return new FileFromPath(Path); // -- Windows is refusing to do it when its the same file. Guess it kinda makes sense.

                            File.Delete(candidatePath);
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized NameCollisionOption value: {collisionOption}");
                    }
                }

                File.Copy(Path, candidatePath, true);

                return new FileFromPath(candidatePath);
            }
        }

        private void EnsureExists()
        {
            if (!Exists)
                throw new Exceptions.FileNotFoundException($"File does not exist: {Path}");
        }
    }
}
#endif