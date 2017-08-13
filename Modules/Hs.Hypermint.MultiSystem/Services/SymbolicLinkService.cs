using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.MultiSystem.Services
{
    public static class SymbolicLinkService
    {
        [DllImport("kernel32.dll")]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        public static string SymbolicLinkName { get; set; }
        public static string FileName { get; set; }
        public static SymbolicLink SymLinkType { get; set; }

        public static void CheckThenCreate(string FileToLink, string tempSymlinkFile)
        {
            if (File.Exists(FileToLink))
            {
                SymbolicLinkName = tempSymlinkFile;
                FileName = FileToLink;
                SymLinkType = SymbolicLink.File ;
                CreateSymbolicLink(SymbolicLinkName, FileName, SymLinkType);
            }
        }

        public static void CreateDirectory(string tempSymlinkFile)
        {
            if (!Directory.Exists(tempSymlinkFile))
                Directory.CreateDirectory(tempSymlinkFile);
        }

    }

    public enum SymbolicLink
    {
        File = 0,
        Directory = 1
    }
}
