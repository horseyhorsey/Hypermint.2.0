using Frontends.Models.Hyperspin;

namespace Hypermint.Base.Model
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class GameItemViewModel
    {
        public Game Game { get; set; }

        /// <summary>
        /// Data bind wraps the Game model from Frontends.Models.Hyperspin <para/>
        /// *Fody propertychanged not available on .Net std 2.0
        /// * Is available now for 2.0
        /// </summary>
        /// <param name="game">The game.</param>       
        public GameItemViewModel(Game game)
        {
            this.Game = game;
        }

        public int GameEnabled { get { return Game.GameEnabled; } set { Game.GameEnabled = value; } }
        public string Name { get { return Game.Name; } set { Game.Name = value; } }
        public string RomName { get { return Game.RomName; } set { Game.RomName = value; } }
        public string System { get { return Game.System; } set { Game.System = value; } }
        public string Description { get { return Game.Description; } set { Game.Description = value; } }
        public string Manufacturer { get { return Game.Manufacturer; } set { Game.Manufacturer = value; } }
        public int Year { get { return Game.Year; } set { Game.Year = value; } }
        public string Genre { get { return Game.Genre; } set { Game.Genre = value; } }
        public string CloneOf { get { return Game.CloneOf; } set { Game.CloneOf = value; } }
        public string Rating { get { return Game.Rating; } set { Game.Rating = value; } }
        public string Crc { get { return Game.Crc; } set { Game.Crc = value; } }

        //Extra props
        public bool RomExists { get { return Game.RomExists; } set { Game.RomExists = value; } }
        public bool IsFavorite { get { return Game.IsFavorite; } set { Game.IsFavorite = value; } }

        //AuditsHs
        public bool HaveWheel { get { return Game.MenuAudit.HaveWheel; } set { Game.MenuAudit.HaveWheel = value; } }
        public bool HaveArt1 { get { return Game.MenuAudit.HaveArt1; } set { Game.MenuAudit.HaveArt1 = value; } }
        public bool HaveArt2 { get { return Game.MenuAudit.HaveArt2; } set { Game.MenuAudit.HaveArt2 = value; } }
        public bool HaveArt3 { get { return Game.MenuAudit.HaveArt3; } set { Game.MenuAudit.HaveArt3 = value; } }
        public bool HaveArt4 { get { return Game.MenuAudit.HaveArt4; } set { Game.MenuAudit.HaveArt4 = value; } }
        public bool HaveBackground { get { return Game.MenuAudit.HaveBackground; } set { Game.MenuAudit.HaveBackground = value; } }
        public bool HaveTheme { get { return Game.MenuAudit.HaveTheme; } set { Game.MenuAudit.HaveTheme = value; } }
        public bool HaveVideo { get { return Game.MenuAudit.HaveVideo; } set { Game.MenuAudit.HaveVideo = value; } }
        public bool HaveBGMusic { get { return Game.MenuAudit.HaveBGMusic; } set { Game.MenuAudit.HaveBGMusic = value; } }
        public bool HaveS_Exit { get { return Game.MenuAudit.HaveS_Exit; } set { Game.MenuAudit.HaveS_Exit = value; } }
        public bool HaveS_Start { get { return Game.MenuAudit.HaveS_Start; } set { Game.MenuAudit.HaveS_Start = value; } }

        //Audit HsMenu
        public bool HaveS_Wheel { get { return Game.MenuAudit.HaveWheelSounds; } set { Game.MenuAudit.HaveWheelSounds = value; } }
        public bool HaveLetters { get { return Game.MenuAudit.HaveLetters; } set { Game.MenuAudit.HaveLetters = value; } }
        public bool HaveSpecial { get { return Game.MenuAudit.HaveSpecial; } set { Game.MenuAudit.HaveSpecial = value; } }
        public bool HaveGenreBG { get { return Game.MenuAudit.HaveGenreBG; } set { Game.MenuAudit.HaveGenreBG = value; } }
        public bool HaveGenreWheel { get { return Game.MenuAudit.HaveGenreWheel; } set { Game.MenuAudit.HaveGenreWheel = value; } }
        public bool HavePointer { get { return Game.MenuAudit.HavePointer; } set { Game.MenuAudit.HavePointer = value; } }
        public bool HaveWheelClick { get { return Game.MenuAudit.HaveWheelClick; } set { Game.MenuAudit.HaveWheelClick = value; } }

        //AuditsRL Pause
        public bool HaveArtwork { get { return Game.RlAudit.HaveArtwork; } set { Game.RlAudit.HaveArtwork = value; } }
        public bool HaveBackgrounds { get { return Game.RlAudit.HaveBackgrounds; } set { Game.RlAudit.HaveBackgrounds = value; } }        
        public bool HaveVideoRl { get { return Game.RlAudit.HaveVideo; } set { Game.RlAudit.HaveVideo = value; } }
        public bool HaveController { get { return Game.RlAudit.HaveController; } set { Game.RlAudit.HaveController = value; } }
        public bool HaveGuide { get { return Game.RlAudit.HaveGuide; } set { Game.RlAudit.HaveGuide = value; } }
        public bool HaveManual { get { return Game.RlAudit.HaveManual; } set { Game.RlAudit.HaveManual = value; } }
        public bool HaveMultiGame { get { return Game.RlAudit.HaveMultiGame; } set { Game.RlAudit.HaveMultiGame = value; } }
        public bool HaveScreenshots { get { return Game.RlAudit.HaveScreenshots; } set { Game.RlAudit.HaveScreenshots = value; } }
        public bool HaveMusic { get { return Game.RlAudit.HaveMusic; } set { Game.RlAudit.HaveMusic = value; } }
        public bool HaveSavedGames { get { return Game.RlAudit.HaveSavedGames; } set { Game.RlAudit.HaveSavedGames = value; } }

        //AuditsRl Bezels        
        public bool HaveBezels { get { return Game.RlAudit.HaveBezels; } set { Game.RlAudit.HaveBezels = value; } }
        public bool HaveBezelBg { get { return Game.RlAudit.HaveBezelBg; } set { Game.RlAudit.HaveBezelBg = value; } }
        public bool HaveCards { get { return Game.RlAudit.HaveCards; } set { Game.RlAudit.HaveCards = value; } }

        //AuditRl Fade
        public bool HaveFadeLayer1 { get { return Game.RlAudit.HaveFadeLayer1; } set { Game.RlAudit.HaveFadeLayer1 = value; } }
        public bool HaveFadeLayer2 { get { return Game.RlAudit.HaveFadeLayer2; } set { Game.RlAudit.HaveFadeLayer2 = value; } }
        public bool HaveFadeLayer3 { get { return Game.RlAudit.HaveFadeLayer3; } set { Game.RlAudit.HaveFadeLayer3 = value; } }
        public bool HaveExtraLayer1 { get { return Game.RlAudit.HaveExtraLayer1; } set { Game.RlAudit.HaveExtraLayer1 = value; } }
    }
}

