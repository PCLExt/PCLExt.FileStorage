using System;
using System.Linq;
using System.Reflection;

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
#elif NETSTANDARD2_0
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
#elif NETSTANDARD2_0
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



#if DESKTOP || MAC || NETSTANDARD2_0
        // https://github.com/mono/mono/blob/master/mcs/class/System.Windows.Forms/System.Windows.Forms/Application.cs
        private static string CompanyName
        {
            get
            {
                var company = string.Empty;

                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

                var attrs = (AssemblyCompanyAttribute[]) assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    company = attrs[0].Company;

                // If there is no [AssemblyCompany], return the outermost namespace
                // on Main ()
                if (string.IsNullOrEmpty(company))
                    if (assembly.EntryPoint != null)
                    {
                        company = assembly.EntryPoint.DeclaringType.Namespace;

                        if (company != null)
                        {
                            int firstDot = company.IndexOf('.');
                            if (firstDot >= 0)
                                company = company.Substring(0, firstDot);
                        }
                    }

                // If that doesn't work, return the name of class containing Main ()
                if (string.IsNullOrEmpty(company))
                    if (assembly.EntryPoint != null)
                        company = assembly.EntryPoint.DeclaringType.FullName;

                return company;
            }
        }
        private static string ProductName
        {
            get
            {
                var name = string.Empty;

                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

                var attrs = (AssemblyProductAttribute[])
                    assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);

                if (attrs != null && attrs.Length > 0)
                    name = attrs[0].Product;

                // If there is no [AssemblyProduct], .NET returns the name of 
                // the innermost namespace and if that fails, resorts to the 
                // name of the class containing Main ()
                if (string.IsNullOrEmpty(name))
                    if (assembly.EntryPoint != null)
                    {
                        name = assembly.EntryPoint.DeclaringType.Namespace;

                        if (name != null)
                        {
                            var lastDot = name.LastIndexOf('.');
                            if (lastDot >= 0 && lastDot < name.Length - 1)
                                name = name.Substring(lastDot + 1);
                        }

                        if (string.IsNullOrEmpty(name))
                            name = assembly.EntryPoint.DeclaringType.FullName;
                    }

                return name;
            }
        }
        private static string ProductVersion
        {
            get
            {
                var version = string.Empty;

                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

                if (Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) is AssemblyInformationalVersionAttribute infoVersion)
                    version = infoVersion.InformationalVersion;

                // If [AssemblyFileVersion] is present it is used
                // before resorting to assembly version
                if (string.IsNullOrEmpty(version))
                {
                    if (Attribute.GetCustomAttribute(assembly, typeof(AssemblyFileVersionAttribute)) is AssemblyFileVersionAttribute fileVersion)
                        version = fileVersion.Version;
                }

                // If neither [AssemblyInformationalVersionAttribute]
                // nor [AssemblyFileVersion] are present, then use
                // the assembly version
                if (string.IsNullOrEmpty(version))
                    version = assembly.GetName().Version.ToString();

                return version;
            }
        }
        internal static IFolder GetDataFolder(this IFolder baseFolder)
        {
            return baseFolder
                .CreateFolder(CompanyName, CreationCollisionOption.OpenIfExists)
                .CreateFolder(ProductName, CreationCollisionOption.OpenIfExists)
                .CreateFolder(ProductVersion, CreationCollisionOption.OpenIfExists);
        }
#endif
    }
}
