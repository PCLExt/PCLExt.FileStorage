using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PCLExt.FileStorage
{
    public static class AsyncIO
    {
#if !PORTABLE
        private static int BufferSize { get; } = 4096;

        private static FileOptions CopyFileOptions { get; } = FileOptions.Asynchronous | FileOptions.SequentialScan;
        private static FileOptions MoveFileOptions { get; } = FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.DeleteOnClose;
        private static FileOptions DeleteFileOptions { get; } = FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.DeleteOnClose;

        public static void Copy(string sourceFile, string destinationFile, bool overwrite = false) => File.Copy(sourceFile, destinationFile, overwrite);
        public static Task CopyAsync(string sourceFile, string destinationFile, bool overwrite = false, CancellationToken cancellationToken = default) => Task.Run(() => File.Copy(sourceFile, destinationFile, overwrite), cancellationToken);

        public static void Move(string sourceFile, string destinationFile) => File.Move(sourceFile, destinationFile);
        public static Task MoveAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken = default) => Task.Run(() => File.Move(sourceFile, destinationFile), cancellationToken);



        // this is 2x slower
        public static async Task CopyAsync1(string sourceFile, string destinationFile, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            //await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            using var sourceStream = new FileStream(sourceFile, FileMode.Open, System.IO.FileAccess.Read, FileShare.Read, BufferSize, CopyFileOptions);
            using var destinationStream = new FileStream(destinationFile, overwrite ? FileMode.OpenOrCreate : FileMode.CreateNew, System.IO.FileAccess.Write, FileShare.None, BufferSize, CopyFileOptions);
            await sourceStream.CopyToAsync(destinationStream, BufferSize, cancellationToken);
        }

        public static void Rename(string sourceFile, string newFileName)
        {
            var newFilePath = Path.Combine(Path.GetDirectoryName(sourceFile), newFileName + Path.GetExtension(sourceFile));
            File.Move(sourceFile, newFilePath);
        }
        public static async Task RenameAsync(string sourceFile, string newFileName, CancellationToken cancellationToken)
        {
            //await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            await Task.Factory.StartNew(path =>
            {
                var tuple = (System.Tuple<string, string>) path;
                Rename(tuple.Item1, tuple.Item2);
            }, System.Tuple.Create(sourceFile, newFileName), cancellationToken);
        }

        public static void Move1(string sourceFile, string destinationFile) => File.Move(sourceFile, destinationFile);
        public static async Task MoveAsync1(string sourceFile, string destinationFile, CancellationToken cancellationToken)
        {
            //await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            using var sourceStream = new FileStream(sourceFile, FileMode.Open, System.IO.FileAccess.Read, FileShare.Delete, BufferSize, MoveFileOptions);
            using var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, System.IO.FileAccess.Write, FileShare.None, BufferSize, CopyFileOptions);
            await sourceStream.CopyToAsync(destinationStream, BufferSize, cancellationToken);
        }

        public static void Delete(string sourceFile) => File.Delete(sourceFile);
        public static async Task DeleteAsync(string sourceFile, CancellationToken cancellationToken)
        {
            //await AwaitExtensions.SwitchOffMainThreadAsync(cancellationToken);

            await Task.Factory.StartNew(path => File.Delete(path as string), sourceFile, cancellationToken);
        }
#endif
    }
}
