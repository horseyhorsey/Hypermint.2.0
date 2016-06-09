using Hs.RocketLauncher.AuditBase;
using Hypermint.Base.Interfaces;
using Hypermint.Base.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.Services
{
    public class AuditerRlRepo : IAuditerRl
    {
        public RocketLauncherAudits RlAudits { get; set; }

        public RocketLauncherAudits RlAuditsDefault { get; set; }

        private IFileFolderChecker _fileManagement;

        public void ScanRocketLaunchMedia(string systemName, string rlMediaPath)
        {
            _fileManagement = new FileFolderChecker();

            ScanDefaultFolders(ref systemName, ref rlMediaPath);
        }

        private void ScanDefaultFolders(ref string systemName, ref string rlMediaPath)
        {
            RlAuditsDefault.ElementAt(0).HaveArtwork =
                CheckForMedia(rlMediaPath + "\\Artwork\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveBackgrounds =
                CheckForMedia(rlMediaPath + "\\Backgrounds\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveBezels =
                CheckForMedia(rlMediaPath + "\\Bezels\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveCards =
                CheckForMedia(rlMediaPath + "\\Bezels\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveController =
                CheckForMedia(rlMediaPath + "\\Controller\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveFade =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveFadeLayer1 =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Layer 1*.*");

            RlAuditsDefault.ElementAt(0).HaveFadeLayer2 =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Layer 2*.*");

            RlAuditsDefault.ElementAt(0).HaveFadeLayer3 =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Layer 3*.*");

            RlAuditsDefault.ElementAt(0).HaveFadeLayer4 =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Layer 4*.*");

            RlAuditsDefault.ElementAt(0).HaveExtraLayer1 =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Extra Layer*.*");

            RlAuditsDefault.ElementAt(0).HaveInfoBar =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Info Bar*.*");

            RlAuditsDefault.ElementAt(0).HaveProgressBar =
                CheckForMedia(rlMediaPath + "\\Fade\\" + systemName + "\\_Default" + "\\", "Progress Bar*.*");

            RlAuditsDefault.ElementAt(0).HaveGuide =
                CheckForMedia(rlMediaPath + "\\Guides\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveManual =
                CheckForMedia(rlMediaPath + "\\Manuals\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveMultiGame =
                CheckForMedia(rlMediaPath + "\\MultiGame\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveMusic =
                CheckForMedia(rlMediaPath + "\\Music\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveSaves =
                CheckForMedia(rlMediaPath + "\\Saved Games\\" + systemName + "\\_Default" + "\\", "*.*");

            RlAuditsDefault.ElementAt(0).HaveVideo =
                CheckForMedia(rlMediaPath + "\\Videos\\" + systemName + "\\_Default" + "\\", "*.*");
        }

        private bool CheckForMedia(string path, string ext) => _fileManagement.CheckMediaFolderFiles(path, ext);

        public void ScanForBezels(string systemName, string rlMediaPath)
        {
            for (int i = 0; i < RlAudits.Count; i++)
            {

                RlAudits[i].HaveBezels =
                    CheckForMedia(
                        rlMediaPath + "\\Bezels\\" + systemName + "\\" + RlAudits[i].RomName + "\\",
                        "Bezel*.*");

                RlAudits[i].HaveBezelBg =
                    CheckForMedia(
                        rlMediaPath + "\\Bezels\\" + systemName + "\\" + RlAudits[i].RomName + "\\",
                        "Background*.*");
            }
        }
        public void ScanForCards(string systemName, string rlMediaPath)
        {
            for (int i = 0; i < RlAudits.Count; i++)
            {
                if (RlAudits[i].RomName == "1942 (Europe)")
                    Console.WriteLine("ss");

                RlAudits[i].HaveCards =
                    CheckForMedia(
                        rlMediaPath + "\\Bezels\\" + systemName + "\\" + RlAudits[i].RomName + "\\",
                        "Instruction Card *.*");
            }
        }

        public void ScanForController(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Controller\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveController = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanForMultiGame(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\MultiGame\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            try
            {
                var dirs = Directory.EnumerateDirectories(scanPath);

                foreach (var item in dirs)
                {
                    try
                    {
                        var dirName = Path.GetFileNameWithoutExtension(item);

                        var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                        var index = RlAudits.IndexOf(s);
                        RlAudits[index].HaveMultiGame = true;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }

        }

        public void ScanForGuides(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Guides\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveGuide = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanForManuals(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Manuals\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveManual = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanForManualFiles(string _selectedSystem, string rlMediaPath)
        {
            for (int i = 1; i < RlAudits.Count; i++)
            {

                RlAudits[i].HaveManual =
                    CheckForMedia(
                        rlMediaPath + "\\Manuals\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\",
                        "*.*");
            }
        }

        public void ScanForMusic(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Music\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveMusic = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanSaves(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Saved Games\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveSaves = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanVideos(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Videos\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveVideo = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanArtwork(string _selectedSystem, string rlMediaPath)
        {
            var scanPath = rlMediaPath + "\\Artwork\\" + _selectedSystem;
            if (!Directory.Exists(scanPath)) return;

            var dirs = Directory.EnumerateDirectories(scanPath);

            foreach (var item in dirs)
            {
                try
                {
                    var dirName = Path.GetFileNameWithoutExtension(item);

                    var s = RlAudits.Where(x => x.RomName == dirName).FirstOrDefault();
                    var index = RlAudits.IndexOf(s);
                    RlAudits[index].HaveArtwork = true;
                }
                catch (Exception)
                {

                }
            }
        }

        public void ScanScreenshots(string _selectedSystem, string rlMediaPath)
        {
            for (int i = 1; i < RlAudits.Count; i++)
            {
                RlAudits[i].HaveScreenshots =
                    CheckForMedia(
                        rlMediaPath + "\\Artwork\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\Screenshots\\",
                        "*.*");
            }
        }

        public void ScanFadeLayers(string _selectedSystem, string rlMediaPath)
        {
            for (int i = 0; i < RlAudits.Count; i++)
            {
                RlAudits[i].HaveFadeLayer1 =
                    CheckForMedia(
                        rlMediaPath + "\\Fade\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\",
                        "Layer 1*.*");
                RlAudits[i].HaveFadeLayer2 =
                    CheckForMedia(
                        rlMediaPath + "\\Fade\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\",
                        "Layer 2*.*");
                RlAudits[i].HaveFadeLayer3 =
                    CheckForMedia(
                        rlMediaPath + "\\Fade\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\",
                        "Layer 3*.*");
                RlAudits[i].HaveExtraLayer1 =
                                    CheckForMedia(
                                        rlMediaPath + "\\Fade\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\",
                                        "Extra Layer 1.*");
                RlAudits[i].HaveExtraLayer1 =
                    CheckForMedia(
                        rlMediaPath + "\\Fade\\" + _selectedSystem + "\\" + RlAudits[i].RomName + "\\",
                        "Extra Layer 1*.*");
            }
        }

        public string[] GetFilesForMedia(string systemName, string romName, string rlMediaPath, string mediaType, string addFolder = "")
        {
            string[] files = null;

            switch (mediaType)
            {
                case "Artwork":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Artwork\\" + systemName + "\\" + romName + "\\" + addFolder,
                        "*.*");
                    break;
                case "Bezel":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Bezels\\" + systemName + "\\" + romName + "\\",
                    "Bezel*.*");
                    break;
                case "Bezels":
                    if (romName == "_Default")
                        files = _fileManagement.GetFiles(rlMediaPath + "\\Bezels\\" + systemName + "\\" + romName + "\\" + addFolder,
                        "*.*");
                    break;
                case "BezelBg":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Bezels\\" + systemName + "\\" + romName + "\\",
                        "Background*.*");
                    break;
                case "Cards":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Bezels\\" + systemName + "\\" + romName + "\\",
                        "Instruction Card*.*");
                    break;
                case "Controller":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Controller\\" + systemName + "\\" + romName + "\\" + addFolder,
                        "*.*");
                    break;
                case "Guides":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Guides\\" + systemName + "\\" + romName + "\\" + addFolder,
                        "*.*");
                    break;
                case "Fade":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Fade\\" + systemName + "\\" + romName + "\\" + addFolder,
                    "*.*");
                    break;
                case "Layer 1":
                case "Layer 2":
                case "Layer 3":
                case "Layer 4":
                case "Extra Layer 1":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Fade\\" + systemName + "\\" + romName + "\\",
                         mediaType + "*.*");
                    break;
                case "Manuals":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Manuals\\" + systemName + "\\" + romName + "\\" + addFolder,
                        "*.*");
                    break;
                case "MultiGame":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\MultiGame\\" + systemName + "\\" + romName + "\\",
                        "*.*");
                    break;
                case "Music":
                    if (romName == "Default")
                        files = _fileManagement.GetFiles(rlMediaPath + "\\Music\\" + systemName + "\\_Default\\",
                            "*.*");
                    else
                        files = _fileManagement.GetFiles(rlMediaPath + "\\Music\\" + systemName + "\\" + romName + "\\" + addFolder,
                        "*.*");
                    break;
                case "Saved Game":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Saved Games\\" + systemName + "\\" + romName + "\\" + addFolder,
                    "*.*");
                    break;
                case "Screenshots":
                    files = _fileManagement.GetFiles(rlMediaPath + "\\Artwork\\" + systemName + "\\" + romName + "\\Screenshots\\",
                        "*.*");
                    break;
                case "Videos":
                    if (romName == "Default")
                        files = _fileManagement.GetFiles(rlMediaPath + "\\Videos\\" + systemName + "\\_Default\\",
                        "*.*");
                    else
                        files = _fileManagement.GetFiles(rlMediaPath + "\\Videos\\" + systemName + "\\" + romName + "\\" + addFolder, 
                        "*.*");
                    break;
                default:
                    break;
            }

            return files;
        }

        public string[] GetFoldersForMediaColumn(string systemName, string romName, string rlMediaPath, string mediaType)
        {
            string[] folders = null;

            if (isFadeColumn(mediaType))
                mediaType = "Fade";

            switch (mediaType)
            {
                case "Artwork":
                    folders = _fileManagement.GetFolders(rlMediaPath + "\\Artwork\\" + systemName + "\\" + romName);
                    break;
                case "Bezels":
                    if (romName == "_Default")
                        folders = _fileManagement.GetFolders(rlMediaPath + "\\Bezels\\" + systemName + "\\" + romName + "\\");
                    break;
                case "Guides":
                    folders = _fileManagement.GetFolders(rlMediaPath + "\\Guides\\" + systemName + "\\" + romName + "\\");
                    break;
                case "Fade":
                    if (romName == "_Default")
                        folders = _fileManagement.GetFolders(rlMediaPath + "\\Fade\\" + systemName + "\\" + romName + "\\");
                    break;
                case "Manuals":
                    folders = _fileManagement.GetFolders(rlMediaPath + "\\Manuals\\" + systemName + "\\" + romName + "\\");
                    break;
                case "MultiGame":
                    if (romName == "_Default")
                        folders = _fileManagement.GetFolders(rlMediaPath + "\\MultiGame\\" + systemName + "\\" + romName + "\\");
                    break;
                default:
                    break;
            }

            return folders;
        }

        private bool isFadeColumn(string columnHeader)
        {
            switch (columnHeader)
            {
                case "Layer 1":
                case "Layer 2":
                case "Layer 3":
                case "Layer 4":
                case "Info Bar":
                case "Progress bar":
                case "7z Extracting":
                case "7z Complete":
                case "_Default Folder":
                    return true;
                default:
                    return false;
            }
        }

    }
}
