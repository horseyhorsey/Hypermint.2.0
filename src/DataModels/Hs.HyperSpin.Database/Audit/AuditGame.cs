namespace Hs.HyperSpin.Database.Audit
{
    /// <summary>
    /// Media audits for games
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
}
