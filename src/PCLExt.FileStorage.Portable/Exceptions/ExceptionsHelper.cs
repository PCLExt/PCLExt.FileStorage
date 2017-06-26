using System;

namespace PCLExt.FileStorage.Exceptions
{
    internal static class ExceptionsHelper
    {
        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException(@"This functionality is not implemented in the portable version of this assembly.
You should reference the PCLExt.FileStorage NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
}
