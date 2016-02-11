using System.Collections.ObjectModel;

namespace Hs.HyperSpin.Database.Audit
{
    /// <summary>
    /// Global media audits
    /// </summary>
    public abstract class Audit
    {
        #region Properties

        private string romName;
        public string RomName
        {
            get { return romName; }
            set { romName = value; }
        }

        public bool HaveWheel { get; set; }
        public bool HaveTheme { get; set; }
        public bool HaveVideo { get; set; }
        public bool HaveBGMusic { get; set; }
        #endregion
    }

}
