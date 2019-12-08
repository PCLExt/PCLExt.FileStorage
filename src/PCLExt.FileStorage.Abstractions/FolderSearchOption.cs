namespace PCLExt.FileStorage
{
    /// <summary>
    /// Specifies whether to search the current folder, or the current folder and all subfolders.
    /// </summary>
    public enum FolderSearchOption
    {
        /// <summary>
        /// Includes the current folder and all its subfolders in a search operation.
        /// </summary>
        TopFolderOnly,

        /// <summary>
        /// Includes only the current folder in a search operation.
        /// </summary>
        AllFolders,
    }
}