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
    internal sealed class DefaultFileImplementation : IFile
    {
        /// <inheritdoc />
        public string Name => System.IO.Path.GetFileName(Path);
        /// <inheritdoc />
        public string Path { get; } // -- This class is a pointer to a place, so we cant change it
        /// <inheritdoc />
        public bool Exists => File.Exists(Path);
        /// <inheritdoc />
        public ulong Size => (ulong) new FileInfo(Path).Length;
        /// <inheritdoc />
        public DateTimeOffset CreationTime
        {
            get
            {
                var creationTime = File.GetCreationTimeUtc(Path);
                if (creationTime == DateTime.MinValue)
                    EnsureExists();

                return new DateTimeOffset(creationTime);
            }
        }
        /// <inheritdoc />
        public DateTimeOffset LastAccessTime
        {
            get
            {
                var lastAccessTime = File.GetLastAccessTimeUtc(Path);
                if (lastAccessTime == DateTime.MinValue)
                    EnsureExists();

                return new DateTimeOffset(lastAccessTime);
            }
        }

        /// <inheritdoc />
        public DateTimeOffset LastWriteTime
        {
            get
            {
                var lastWriteTime = File.GetLastWriteTimeUtc(Path);
                if (lastWriteTime == DateTime.MinValue)
                    EnsureExists();

                return new DateTimeOffset(lastWriteTime);
            }
        }

        /// <summary>
        /// Creates a new <see cref="IFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public DefaultFileImplementation(string path) => Path = path;

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess)
            => OpenCoreAsync(true, fileAccess, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken = default)
            => OpenCoreAsync(false, fileAccess, cancellationToken);
        private async Task<Stream> OpenCoreAsync(bool sync, FileAccess fileAccess, CancellationToken cancellationToken)
        {
            if (!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            switch (fileAccess)
            {
                case FileAccess.Read:
                    {
                        if (sync)
                            return File.OpenRead(Path);
                        else
                        {

                        }
                        break;
                    }
            }

            FileStream fileStream = fileAccess switch
            {
                FileAccess.Read => sync
                    ? File.OpenRead(Path)
                    : File.OpenRead(Path),
                FileAccess.ReadAndWrite => sync
                    ? File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite)
                    : File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite),
                _ => throw new ArgumentException($"Unrecognized FileAccess value: {fileAccess}", nameof(fileAccess)),
            };
            return fileStream;
        }

        /// <inheritdoc />
        public void Delete()
            => DeleteCoreAsync(true, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task DeleteAsync(CancellationToken cancellationToken = default)
            => DeleteCoreAsync(false, cancellationToken);
        private async Task DeleteCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            if (!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            if(sync)
                File.Delete(Path);
            else
                await AsyncIO.DeleteAsync(Path, cancellationToken);
        }

        /// <inheritdoc />
        public IFile Rename(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists)
            => RenameCoreAsync(true, newName, collisionOption, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task<IFile> RenameAsync(string newName, NameCollisionOption collisionOption = NameCollisionOption.FailIfExists, CancellationToken cancellationToken = default)
            => RenameCoreAsync(false, newName, collisionOption, cancellationToken);
        private async Task<IFile> RenameCoreAsync(bool sync, string newName, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newName, nameof(newName));

            if (!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            var newNamePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), newName);
            return sync
                ? Move(newNamePath, collisionOption)
                : await MoveAsync(newNamePath, collisionOption, cancellationToken);
        }

        /// <inheritdoc />
        public void Move(IFile newFile)
            => MoveCoreAsync(true, newFile, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task MoveAsync(IFile newFile, CancellationToken cancellationToken = default)
            => MoveCoreAsync(false, newFile, cancellationToken);
        private async Task MoveCoreAsync(bool sync, IFile newFile, CancellationToken cancellationToken)
        {
            Requires.NotNull(newFile, nameof(newFile));

            if (!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            if (newFile.Exists && !string.Equals(Path, newFile.Path, StringComparison.Ordinal))
            {
                if (sync)
                    newFile.Delete();
                else
                    await newFile.DeleteAsync(cancellationToken);
            }

            if (sync)
                File.Move(Path, newFile.Path);
            else
                await AsyncIO.MoveAsync(Path, newFile.Path, cancellationToken);
        }

        /// <inheritdoc />
        public void Copy(IFile newFile)
            => CopyCoreAsync(true, newFile, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task CopyAsync(IFile newFile, CancellationToken cancellationToken = default)
            => CopyCoreAsync(false, newFile, cancellationToken);
        public async Task CopyCoreAsync(bool sync, IFile newFile, CancellationToken cancellationToken)
        {
            if (string.Equals(Path, newFile.Path, StringComparison.Ordinal))
                return; // -- Windows is refusing to do it when its the same file. Guess it kinda makes sense.

            if(!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            if (sync)
                File.Copy(Path, newFile.Path);
            else
                AsyncIO.CopyAsync(Path, newFile.Path, cancellationToken);
        }

        /// <inheritdoc />
        public IFile Move(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting)
            => MoveCoreAsync(true, newPath, collisionOption, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task<IFile> MoveAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
            => MoveCoreAsync(false, newPath, collisionOption, cancellationToken);
        public async Task<IFile> MoveCoreAsync(bool sync, string newPath, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, nameof(newPath));

            if (!sync)
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
                            {
                                if (sync)
                                    File.Delete(candidatePath);
                                else
                                    await AsyncIO.DeleteAsync(candidatePath, cancellationToken);
                            }
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized NameCollisionOption value: {collisionOption}", nameof(collisionOption));
                    }
                }

                if(sync)
                    File.Move(Path, candidatePath);
                else
                    AsyncIO.MoveAsync(Path, candidatePath, cancellationToken);

                return new FileFromPath(candidatePath);
            }
        }

        /// <inheritdoc />
        public IFile Copy(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting)
            => CopyCoreAsync(true, newPath, collisionOption, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task<IFile> CopyAsync(string newPath, NameCollisionOption collisionOption = NameCollisionOption.ReplaceExisting, CancellationToken cancellationToken = default)
            => CopyCoreAsync(false, newPath, collisionOption, cancellationToken);
        public async Task<IFile> CopyCoreAsync(bool sync, string newPath, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, nameof(newPath));

            if (!sync)
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
                            if (string.Equals(Path, newPath, StringComparison.Ordinal))
                                return new FileFromPath(Path); // -- Windows is refusing to do it when its the same file. Guess it kinda makes sense.

                            File.Delete(candidatePath);
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized NameCollisionOption value: {collisionOption}", nameof(collisionOption));
                    }
                }

                File.Copy(Path, candidatePath, true);

                return new FileFromPath(candidatePath);
            }
        }

        /// <inheritdoc />
        public void WriteAllBytes(byte[] bytes)
            => WriteAllBytesCoreAsync(true, bytes, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task WriteAllBytesAsync(byte[] bytes, CancellationToken cancellationToken = default)
            => WriteAllBytesCoreAsync(false, bytes, cancellationToken);
        public async Task WriteAllBytesCoreAsync(bool sync, byte[] bytes, CancellationToken cancellationToken)
        {
            if (!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            File.WriteAllBytes(Path, bytes);
        }

        /// <inheritdoc />
        public byte[] ReadAllBytes()
            => ReadAllBytesCoreAsync(true, CancellationToken.None).RunSync();
        /// <inheritdoc />
        public Task<byte[]> ReadAllBytesAsync(CancellationToken cancellationToken = default)
            => ReadAllBytesCoreAsync(false, cancellationToken);
        public async Task<byte[]> ReadAllBytesCoreAsync(bool sync, CancellationToken cancellationToken)
        {
            if (!sync)
                await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            EnsureExists();

            return File.ReadAllBytes(Path);
        }

        private void EnsureExists()
        {
            if (!Exists)
                throw new Exceptions.FileNotFoundException($"File does not exist: {Path}", System.IO.Path.GetFileName(Path));
        }
    }
}
#endif