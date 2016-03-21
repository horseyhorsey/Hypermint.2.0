using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.RocketLauncher.AuditBase
{
    public class RocketLaunchAudit
    {
        public string RomName { get; set; }
        public string Description { get; set; }
        public string Publisher { get; set; }

        public bool HaveArtwork { get; set; }
        public bool HaveBackgrounds { get; set; }
        public bool HaveBezels { get; set; }
        public bool HaveCards { get; set; }
        public bool HaveController { get; set; }
        public bool Developer { get; set; }
        public bool HaveFade { get; set; }
        public bool HaveGenre { get; set; }
        public bool HaveGuide { get; set; }
        public bool HaveManual { get; set; }
        public bool HaveMultiGame { get; set; }
        public bool HaveMusic { get; set; }
        public bool HaveRating { get; set; }
        public bool HaveYear { get; set; }
        public bool HaveVideo { get; set; }

    }
}
