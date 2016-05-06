namespace PCLExt.FileStorage
{
    public static class Storage
    {
        public static IFolder BaseFolder { get; }
        
        public static IFolder SettingsFolder { get; }

        public static IFolder LogFolder { get; }
        public static IFolder CrashLogFolder { get; }

        public static IFolder LuaFolder { get; }

        public static IFolder DatabaseFolder { get; }

        public static IFolder ContentFolder { get; }

        static Storage()
        {
            IFolder BaseFolder = null;
            if (FileSystem.Current.BaseStorage != null)
                BaseFolder = FileSystem.Current.BaseStorage;
            else if (FileSystem.Current.LocalStorage != null)
                BaseFolder = FileSystem.Current.LocalStorage;
            else if (FileSystem.Current.RoamingStorage != null)
                BaseFolder = FileSystem.Current.RoamingStorage;



            SettingsFolder  = BaseFolder.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists).Result;
            LogFolder       = BaseFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists).Result;
            CrashLogFolder  = LogFolder.CreateFolderAsync("CrashLogs", CreationCollisionOption.OpenIfExists).Result;
            LuaFolder       = BaseFolder.CreateFolderAsync("Lua", CreationCollisionOption.OpenIfExists).Result;
            DatabaseFolder  = BaseFolder.CreateFolderAsync("Database", CreationCollisionOption.OpenIfExists).Result;
            ContentFolder   = BaseFolder.CreateFolderAsync("Content", CreationCollisionOption.OpenIfExists).Result;
        }
    }
}
