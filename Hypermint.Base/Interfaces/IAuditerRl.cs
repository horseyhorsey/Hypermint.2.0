using Frontends.Models.RocketLauncher;

namespace Hypermint.Base.Interfaces
{
    public interface IAuditerRl
    {
        #region Properties
        Audits RlAuditsDefault { get; set; }
        Audits RlAudits { get; set; } 
        #endregion

        #region Methods
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
        void ScanBackground(string _selectedSystem, string rlMediaPath); 
        #endregion
    }
}
