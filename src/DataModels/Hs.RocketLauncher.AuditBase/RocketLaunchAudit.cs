namespace Hs.RocketLauncher.AuditBase
{
    public class RocketLaunchAudit
    {
        public string RomName { get; set; }
        public string Description { get; set; }     

        public bool HaveArtwork { get; set; }
        public bool HaveBackgrounds { get; set; }
        public bool HaveBezels { get; set; }
        public bool HaveBezelBg { get; set; }
        public bool HaveCards { get; set; }
        public bool HaveController { get; set; }
        public bool Developer { get; set; }

        
        public bool HaveFade { get; set; }
        public bool HaveFadeLayer1 { get; set; }
        public bool HaveFadeLayer2 { get; set; }
        public bool HaveFadeLayer3 { get; set; }
        public bool HaveFadeLayer4 { get; set; }
        
        public bool HaveExtraLayer1 { get; set; }
        public bool Have7zExtract { get; set; }
        public bool Have7zComplete { get; set; }
        public bool HaveProgressBar { get; set; }
        public bool HaveInfoBar { get; set; }


        public bool HaveGenre { get; set; }
        public bool HaveGuide { get; set; }
        public bool HaveManual { get; set; }
        public bool HaveMultiGame { get; set; }
        public bool HaveMusic { get; set; }
        public bool HavePublisher { get; set; }
        public bool HaveRating { get; set; }
        public bool HaveSaves { get; set; }        
        public bool HaveYear { get; set; }
        public bool HaveVideo { get; set; }

    }
}
