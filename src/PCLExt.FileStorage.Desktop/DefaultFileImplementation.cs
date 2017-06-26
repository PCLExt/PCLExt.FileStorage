//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using PCLExt.FileStorage.Extensions;

namespace PCLExt.FileStorage
{
    /// <summary>
    /// Represents a file
    /// </summary>
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    internal class DefaultFileImplementation : IFile
    {
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public string Path { get; private set; }
        /*
        /// <inheritdoc />
        public long Size => new FileInfo(Path).Length;
        */

        /// <summary>
        /// Creates a new <see cref="IFile"/> corresponding to the specified path.
        /// </summary>
        /// <param name="path">The file path</param>
        public DefaultFileImplementation(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }

        /// <inheritdoc />
        public Stream Open(FileAccess fileAccess)
        {
            if (fileAccess == FileAccess.Read)
                return File.OpenRead(Path);
            else if (fileAccess == FileAccess.ReadAndWrite)
                return File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite);
            else
                throw new ArgumentException($"Unrecognized FileAccess value: {fileAccess}");
        }
        /// <inheritdoc />
        public async Task<Stream> OpenAsync(FileAccess fileAccess, CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (fileAccess == FileAccess.Read)
                return File.OpenRead(Path);
            else if (fileAccess == FileAccess.ReadAndWrite)
                return File.Open(Path, FileMode.Open, System.IO.FileAccess.ReadWrite);
            else
                throw new ArgumentException($"Unrecognized FileAccess value: {fileAccess}");
        }

        /// <inheritdoc />
        public void Delete()
        {
            if (!File.Exists(Path))
                throw new FileNotFoundException($"File does not exist: {Path}");

            File.Delete(Path);
        }
        /// <inheritdoc />
        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            if (!File.Exists(Path))
                throw new FileNotFoundException($"File does not exist: {Path}");

            File.Delete(Path);
        }

        /// <inheritdoc />
        public void Rename(string newName, NameCollisionOption collisionOption)
        {
            Requires.NotNullOrEmpty(newName, "newName");

            Move(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), newName), collisionOption);
        }
        /// <inheritdoc />
        public async Task RenameAsync(string newName, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newName, "newName");

            await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            await MoveAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), newName), collisionOption, cancellationToken);
        }

        /// <inheritdoc />
        public void Move(string newPath, NameCollisionOption collisionOption)
        {
            Requires.NotNullOrEmpty(newPath, "newPath");

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
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Move(Path, candidatePath);
                Path = candidatePath;
                Name = candidateName;
                return;
            }
        }
        /// <inheritdoc />
        public async Task MoveAsync(string newPath, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, "newPath");

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
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Move(Path, candidatePath);
                Path = candidatePath;
                Name = candidateName;
                return;
            }
        }

        /// <inheritdoc />
        public void Copy(string newPath, NameCollisionOption collisionOption)
        {
            Requires.NotNullOrEmpty(newPath, "newPath");

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
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Copy(Path, candidatePath);
                Path = candidatePath;
                Name = candidateName;
                return;
            }
        }
        /// <inheritdoc />
        public async Task CopyAsync(string newPath, NameCollisionOption collisionOption, CancellationToken cancellationToken)
        {
            Requires.NotNullOrEmpty(newPath, "newPath");

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
                            throw new IOException("File already exists.");
                        case NameCollisionOption.GenerateUniqueName:
                            continue; // try again with a new name.
                        case NameCollisionOption.ReplaceExisting:
                            File.Delete(candidatePath);
                            break;
                    }
                }

                File.Copy(Path, candidatePath);
                Path = candidatePath;
                Name = candidateName;
                return;
            }
        }
    }
}
