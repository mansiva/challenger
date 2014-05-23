using System;
using System.Collections.Generic;
using System.IO;

namespace Hibernum.Core
{
    /// <summary>
    /// Utility class for IO operations on directories and files.
    /// </summary>
    public static class IOUtil
    {
        public delegate bool TestFileDelegate(FileInfo file);

        /// <summary>
        /// Copies the items that are in the source directory to the destination directory.
        /// </summary>
        public static void DirectoryCopy(DirectoryInfo source, DirectoryInfo destination, bool recursive, bool overwrite, TestFileDelegate fileTest)
        {
            if(!source.Exists)
            {
                return;
            }

            if(!destination.Exists)
            {
                destination.Create();
            }

            foreach(FileInfo file in source.GetFiles())
            {
                if(fileTest == null || fileTest(file))
                {
                    string path = Path.Combine(destination.FullName, file.Name);
                    file.CopyTo(path, overwrite);
                }
            }
            
            if(recursive)
            {
                foreach(DirectoryInfo directory in source.GetDirectories())
                {
                    string path = Path.Combine(destination.FullName, directory.Name);
                    DirectoryCopy(directory, new DirectoryInfo(path), recursive, overwrite, fileTest);
                }
            }
        }

        /// <summary>
        /// Recursively searches for directories that match searchPattern.
        /// </summary>
        public static DirectoryInfo[] DirectoryRecursiveSearch(DirectoryInfo source, string searchPattern)
        {
            List<DirectoryInfo> results = new List<DirectoryInfo>();

            if(source.Exists)
            {
                // Add the results of the current directory.
                results.AddRange(source.GetDirectories(searchPattern));
                foreach(DirectoryInfo child in source.GetDirectories())
                {
                    // Add the results of the child directory.
                    results.AddRange(DirectoryRecursiveSearch(child, searchPattern));
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Predicate that returns <c>true</c> when the file passed as parameter is not a meta file.
        /// </summary>
        public static bool IsNotMeta(FileInfo file)
        {
            return file.Exists && !file.FullName.ToLower().EndsWith(".meta");
        }

        /// <summary>
        /// Searches the source directory for files that match the predicate.
        /// </summary>
        public static FileInfo[] FileSearch(DirectoryInfo source, bool recursive, TestFileDelegate fileTest)
        {
            List<FileInfo> results = new List<FileInfo>();

            if(source.Exists && fileTest != null)
            {
                foreach(FileInfo file in source.GetFiles())
                {
                    if(fileTest(file))
                    {
                        results.Add(file);
                    }
                }

                if(recursive)
                {
                    foreach(DirectoryInfo child in source.GetDirectories())
                    {
                        results.AddRange(FileSearch(child, recursive, fileTest));
                    }
                }
            }

            return results.ToArray();
        }
    }
}
