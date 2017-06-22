using System;
using System.Linq;

namespace PCLExt.FileStorage.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IFile"/> and <see cref="IFolder"/> class
    /// </summary>
    public static class FolderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static IFolder GetFolderFromPath(this IFolder folder, string folderPath)
        {
            var folders = folderPath.Split(new[]
            {
#if DESKTOP || ANDROID || __IOS__ || MAC || PORTABLE
                PortablePath.DirectorySeparatorChar
#elif CORE
                System.IO.Path.DirectorySeparatorChar
#endif
            }, StringSplitOptions.RemoveEmptyEntries);

            return GetFolderFromPath(folder, folders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folders"></param>
        /// <returns></returns>
        public static IFolder GetFolderFromPath(this IFolder folder, params string[] folders)
        {
            if (folders != null)
                foreach (var folderName in folders)
                    folder = folder.CreateFolder(folderName, CreationCollisionOption.OpenIfExists);

            return folder;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IFile GetFileFromPath(this IFolder folder, string filePath)
        {
            var foldersWithFile = filePath.Split(new[]
            {
#if DESKTOP || ANDROID || __IOS__ || MAC || PORTABLE
                PortablePath.DirectorySeparatorChar
#elif CORE
                System.IO.Path.DirectorySeparatorChar
#endif
            }, StringSplitOptions.RemoveEmptyEntries);

            return GetFileFromPath(folder, foldersWithFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="foldersWithFile"></param>
        /// <returns></returns>
        public static IFile GetFileFromPath(this IFolder folder, params string[] foldersWithFile)
        {
            var fileName = foldersWithFile.Last();
            var folders = foldersWithFile.Length > 1 ? foldersWithFile.Take(foldersWithFile.Length - 1).ToArray() : null;

            if (folders != null)
                foreach (var folderName in folders)
                    folder = folder.CreateFolder(folderName, CreationCollisionOption.OpenIfExists);

            return folder.CreateFile(fileName, CreationCollisionOption.OpenIfExists);
        }
    }
}
