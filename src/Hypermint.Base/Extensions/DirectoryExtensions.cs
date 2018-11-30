using System.Diagnostics;
using System.IO;

namespace Hypermint.Base.Extensions
{
    public static class DirectoryExtensions
    {
        /// <summary>
        /// Opens a directory with the default process
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        public static bool Open(this DirectoryInfo directoryInfo)
        {
            if (Directory.Exists(directoryInfo.FullName))
            {
                Process.Start(directoryInfo.FullName);
                return true;
            }
            else
                return false;
        }
    }
}
