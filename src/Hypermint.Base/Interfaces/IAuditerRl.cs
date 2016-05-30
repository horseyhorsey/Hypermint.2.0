using Hs.RocketLauncher.AuditBase;

namespace Hypermint.Base.Interfaces
{
    public interface IAuditerRl
    {
        RocketLauncherAudits RlAuditsDefault { get; set; }

        RocketLauncherAudits RlAudits { get; set; }

        void ScanRocketLaunchMedia(string systemName, string rlMediaPath);

        void ScanForBezels(string systemName, string rlMediaPath);
        void ScanForCards(string systemName, string rlMediaPath);
        string[] GetFilesForMedia(string systemName, string romName, string rlMediaPath, string mediaType, string addFolder = "");
        string[] GetFoldersForMediaColumn(string systemName, string romName, string rlMediaPath, string mediaType);        
        void ScanForMultiGame(string _selectedSystem, string rlMediaPath);
        void ScanForController(string _selectedSystem, string rlMediaPath);
        void ScanForGuides(string _selectedSystem, string rlMediaPath);
        void ScanForManuals(string _selectedSystem, string rlMediaPath);
        void ScanForMusic(string _selectedSystem, string rlMediaPath);
        void ScanSaves(string _selectedSystem, string rlMediaPath);
        void ScanVideos(string _selectedSystem, string rlMediaPath);
        void ScanFadeLayers(string _selectedSystem, string rlMediaPath);
        void ScanScreenshots(string _selectedSystem, string rlMediaPath);
        void ScanArtwork(string _selectedSystem, string rlMediaPath);
    }
}
