using System.Linq;

namespace PCLExt.FileStorage.Extensions
{
    public static class StringExtensions
    {
        public static string PathEnsureDirectorySeparator(this string path) =>
            path.EnsureEndsWith(PortablePath.DirectorySeparatorChar, PortablePath.AltDirectorySeparatorChar);

        internal static string EnsureEndsWith(this string path, params char[] endVals) =>
            endVals.All(val => !path.EndsWith(val.ToString())) ? path + endVals[0] : path;
    }
}