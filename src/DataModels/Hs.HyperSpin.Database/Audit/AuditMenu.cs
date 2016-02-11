namespace Hs.HyperSpin.Database.Audit
{
    /// <summary>
    /// Media audits for menu
    /// </summary>
    public class AuditMenu : Audit
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
}
