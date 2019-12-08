namespace PCLExt.FileStorage
{
    /// <summary>
    /// Specifies whether a file should be opened for write access or not
    /// </summary>
    public enum FileAccess
    {
        /// <summary>
        /// Specifies that a file should be opened for read-only access
        /// </summary>
        Read,

        /// <summary>
        /// Specifies that a file should be opened for read/write access
        /// </summary>
        ReadAndWrite,
    }
}