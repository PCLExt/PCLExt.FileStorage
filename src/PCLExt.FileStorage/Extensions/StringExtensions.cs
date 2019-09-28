using System.Linq;

namespace PCLExt.FileStorage.Extensions
{
    internal static class StringExtensions
    {
        public static string EnsureEndsWith(this string text, params char[] endVals) =>
            endVals.All(val => !text.EndsWith(val.ToString())) ? text + endVals[0] : text;
    }
}