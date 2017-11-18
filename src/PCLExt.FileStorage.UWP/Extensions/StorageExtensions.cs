using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCLExt.FileStorage.UWP.Extensions
{
    sealed class StorageExtensions
    {
        public static Windows.Storage.NameCollisionOption ConvertToNameCollision(
           NameCollisionOption collisionOption)
        {
            if (collisionOption == NameCollisionOption.FailIfExists)
                return Windows.Storage.NameCollisionOption.FailIfExists;

            if (collisionOption == NameCollisionOption.GenerateUniqueName)
                return Windows.Storage.NameCollisionOption.GenerateUniqueName;

            if (collisionOption == NameCollisionOption.ReplaceExisting)
                return Windows.Storage.NameCollisionOption.ReplaceExisting;

            throw new NotSupportedException(collisionOption.ToString());
        }

        public static CreationCollisionOption ConvertToCreationCollision(
            NameCollisionOption collisionOption)
        {
            if (collisionOption == NameCollisionOption.FailIfExists)
                return CreationCollisionOption.FailIfExists;
            if (collisionOption == NameCollisionOption.GenerateUniqueName)
                return CreationCollisionOption.GenerateUniqueName;
            if (collisionOption == NameCollisionOption.ReplaceExisting)
                return CreationCollisionOption.ReplaceExisting;

            throw new NotSupportedException(collisionOption.ToString());
        }

        public static Windows.Storage.CreationCollisionOption ConvertToWindowsCreationCollisionOption(
            CreationCollisionOption collisionOption)
        {
            if (collisionOption == CreationCollisionOption.FailIfExists)
                return Windows.Storage.CreationCollisionOption.FailIfExists;
            if (collisionOption == CreationCollisionOption.GenerateUniqueName)
                return Windows.Storage.CreationCollisionOption.GenerateUniqueName;
            if (collisionOption == CreationCollisionOption.ReplaceExisting)
                return Windows.Storage.CreationCollisionOption.ReplaceExisting;
            if (collisionOption == CreationCollisionOption.OpenIfExists)
                return Windows.Storage.CreationCollisionOption.OpenIfExists;

            throw new NotSupportedException(collisionOption.ToString());
        }
    }
}
