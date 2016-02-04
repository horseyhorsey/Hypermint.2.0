using System.Collections.ObjectModel;

namespace Hs.HyperSpin.Database
{
    /// <summary>
    /// Global media audits
    /// </summary>
    public abstract class Audit
    {
        #region Properties
        public bool HaveWheel { get; set; }
        public bool HaveTheme { get; set; }
        public bool HaveVideo { get; set; }
        public bool HaveBGMusic { get; set; }
        #endregion
    }

    /// <summary>
    /// Media for Main Menu audit store
    /// </summary>
    public class AuditMainMenu : Audit
    {
        #region Properties
        public bool HaveLetters { get; set; }
        public bool HaveSpecial { get; set; }
        public bool HaveGenreBG { get; set; }
        public bool HaveGenreWheel { get; set; }
        public bool HavePointer { get; set; }
        public bool HaveS_Wheel { get; set; }
        public bool HaveS_Click { get; set; }
        #endregion
    }

    /// <summary>
    /// Media for game audit store
    /// </summary>
    public class AuditGame : Audit
    {
        #region Properties
        /// <summary>
        /// Artwork1 folder
        /// </summary>
        public bool HaveArt1 { get; set; }
        /// <summary>
        /// Artwork2 folder
        /// </summary>
        public bool HaveArt2 { get; set; }
        /// <summary>
        /// Artwork3 folder
        /// </summary>
        public bool HaveArt3 { get; set; }
        /// <summary>
        /// Artwork4 folder
        /// </summary>
        public bool HaveArt4 { get; set; }
        /// <summary>
        /// Backgrounds folder
        /// </summary>
        public bool HaveBackground { get; set; }
        public bool HaveS_Start { get; set; }
        public bool HaveS_Exit { get; set; }
        #endregion
    }

    public class AuditGames : ObservableCollection<AuditGame>
    {

    }

}
