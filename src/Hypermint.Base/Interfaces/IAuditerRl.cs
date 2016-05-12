using Hs.RocketLauncher.AuditBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Interfaces
{
    public interface IAuditerRl
    {
        RocketLauncherAudits RlAuditsDefault { get; set; }

        RocketLauncherAudits RlAudits { get; set; }

        void ScanRocketLaunchMedia(string systemName, string rlMediaPath);

        void ScanForBezels(string systemName, string rlMediaPath);
        void ScanForCards(string systemName, string rlMediaPath);
        string[] GetFilesForMedia(string systemName, string romName, string rlMediaPath, string mediaType);
        void ScanForMultiGame(string _selectedSystem, string rlMediaPath);
        void ScanForGuides(string _selectedSystem, string rlMediaPath);
        void ScanForManuals(string _selectedSystem, string rlMediaPath);
        void ScanForMusic(string _selectedSystem, string rlMediaPath);
        void ScanSaves(string _selectedSystem, string rlMediaPath);
        void ScanVideos(string _selectedSystem, string rlMediaPath);
        void ScanFadeLayers(string _selectedSystem, string rlMediaPath);
    }
}
