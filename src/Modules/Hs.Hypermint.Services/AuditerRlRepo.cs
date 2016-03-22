using Hs.RocketLauncher.AuditBase;
using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
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

        private bool CheckForMedia(string path, string ext)
        {
            return _fileManagement.CheckMediaFolderFiles(path, ext);
        }
    }
}
