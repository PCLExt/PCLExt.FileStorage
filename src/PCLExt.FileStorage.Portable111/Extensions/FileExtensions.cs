//-----------------------------------------------------------------------
// <copyright company="Daniel Plaisted">
//     Copyright (c) Daniel Plaisted. All rights reserved.
// </copyright>
// This file is a derivation of:
// https://github.com/dsplaisted/PCLStorage
// Which is released under the MS-PL license.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="IFile"/> class
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Reads the contents of a file as a string
        /// </summary>
        /// <param name="file">The file to read </param>
        /// <returns>The content of the file</returns>
        public static string ReadAllText(this IFile file)
        {
            using (var stream = file.Open(FileAccess.Read))
            using (var sr = new StreamReader(stream))
                return sr.ReadToEnd();
        }

        /// <summary>
        /// Reads the contents of a file as a string
        /// </summary>
        /// <param name="file">The file to read </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The content of the file</returns>
        public static async Task<string> ReadAllTextAsync(this IFile file, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await file.OpenAsync(FileAccess.Read, cancellationToken).ConfigureAwait(false))
            using (var sr = new StreamReader(stream))
                return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Writes text to a file, overwriting any existing data
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="content">The content to write to the file</param>
        public static void WriteAllText(this IFile file, string content)
        {
            using (var stream = file.Open(FileAccess.ReadAndWrite))
            {
                stream.SetLength(0);
                using (var sw = new StreamWriter(stream))
                    sw.Write(content);
            }
        }

        /// <summary>
        /// Writes text to a file, overwriting any existing data
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="content">The content to write to the file</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which completes when the write operation finishes</returns>
        public static async Task WriteAllTextAsync(this IFile file, string contents, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite, cancellationToken).ConfigureAwait(false))
            {
                stream.SetLength(0);
                using (var sw = new StreamWriter(stream))
                    await sw.WriteAsync(content).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Reads the contents of a file as an array of strings
        /// </summary>
        /// <param name="file">The file to read </param>
        /// <returns>The content of the file</returns>
        public static string[] ReadAllLines(this IFile file)
        {
            using (var stream = file.Open(FileAccess.Read))
            using (var sr = new StreamReader(stream))
            {
                var lines = new List<string>();
                while (!sr.EndOfStream)
                    lines.Add(sr.ReadLine());
                return lines.ToArray();
            }
        }

        /// <summary>
        /// Reads the contents of a file as an array of strings
        /// </summary>
        /// <param name="file">The file to read </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The content of the file</returns>
        public static async Task<string[]> ReadAllLinesAsync(this IFile file, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await file.OpenAsync(FileAccess.Read, cancellationToken).ConfigureAwait(false))
            using (var sr = new StreamReader(stream))
            {
                var lines = new List<string>();
                while (!sr.EndOfStream)
                    lines.Add(await sr.ReadLineAsync().ConfigureAwait(false));
                return lines.ToArray();
            }
        }

        /// <summary>
        /// Writes lines to a file, overwriting any existing data
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="lines">The content to write to the file</param>
        public static void WriteAllLines(this IFile file, IEnumerable<string> lines)
        {
            using (var stream = file.Open(FileAccess.ReadAndWrite))
            {
                stream.SetLength(0);
                using (var sw = new StreamWriter(stream))
                    foreach (var line in lines)
                        sw.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes lines to a file, overwriting any existing data
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="lines">The content to write to the file</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async Task WriteAllLinesAsync(this IFile file, IEnumerable<string> lines, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite, cancellationToken).ConfigureAwait(false))
            {
                stream.SetLength(0);
                using (var sw = new StreamWriter(stream))
                    foreach (var line in lines)
                        await sw.WriteLineAsync(line).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="contents"></param>
        public static void AppendText(this IFile file, string contents)
        {
            using (var stream = file.Open(FileAccess.ReadAndWrite))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                    sw.Write(contents);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="contents"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task AppendTextAsync(this IFile file, string contents, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite, cancellationToken).ConfigureAwait(false))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                    await sw.WriteAsync(contents).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="lines"></param>
        public static void AppendLines(this IFile file, IEnumerable<string> lines)
        {
            using (var stream = file.Open(FileAccess.ReadAndWrite))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                    foreach (var line in lines)
                        sw.WriteLine(line);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="lines"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static async Task AppendLinesAsync(this IFile file, IEnumerable<string> lines, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite, cancellationToken).ConfigureAwait(false))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                    foreach (var line in lines)
                        await sw.WriteLineAsync(line).ConfigureAwait(false);
            }
        }
    }
}