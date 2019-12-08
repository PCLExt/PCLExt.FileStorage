using System;

using Windows.Storage;

namespace PCLExt.FileStorage.UWP.Extensions
{
    internal static class StorageExtensions
    {
        public static FileAccessMode ConvertToFileAccessMode(this FileAccess fileAccess) => fileAccess switch
        {
            FileAccess.Read => FileAccessMode.Read,
            FileAccess.ReadAndWrite => FileAccessMode.ReadWrite,

            _ => throw new ArgumentException(fileAccess.ToString(), nameof(fileAccess)),
        };

        public static Windows.Storage.NameCollisionOption ConvertToNameCollision(this NameCollisionOption collisionOption) => collisionOption switch
        {
            NameCollisionOption.FailIfExists => Windows.Storage.NameCollisionOption.FailIfExists,
            NameCollisionOption.GenerateUniqueName => Windows.Storage.NameCollisionOption.FailIfExists,
            NameCollisionOption.ReplaceExisting => Windows.Storage.NameCollisionOption.ReplaceExisting,
            _ => throw new ArgumentException(collisionOption.ToString(), nameof(collisionOption)),
        };

        public static CreationCollisionOption ConvertToCreationCollision(this NameCollisionOption collisionOption) => collisionOption switch
            {
                NameCollisionOption.FailIfExists => CreationCollisionOption.FailIfExists,
                NameCollisionOption.GenerateUniqueName => CreationCollisionOption.FailIfExists,
                NameCollisionOption.ReplaceExisting => CreationCollisionOption.ReplaceExisting,
                _ => throw new ArgumentException(collisionOption.ToString(), nameof(collisionOption)),
            };

        public static Windows.Storage.CreationCollisionOption ConvertToWindowsCreationCollisionOption(
            this CreationCollisionOption collisionOption) => collisionOption switch
        {
            CreationCollisionOption.FailIfExists => Windows.Storage.CreationCollisionOption.FailIfExists,
            CreationCollisionOption.GenerateUniqueName => Windows.Storage.CreationCollisionOption.GenerateUniqueName,
            CreationCollisionOption.ReplaceExisting => Windows.Storage.CreationCollisionOption.ReplaceExisting,
            CreationCollisionOption.OpenIfExists => Windows.Storage.CreationCollisionOption.OpenIfExists,
            _ => throw new ArgumentException(collisionOption.ToString(), nameof(collisionOption)),
        };
    }
}
