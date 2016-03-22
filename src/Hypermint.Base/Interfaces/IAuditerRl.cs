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

        
    }
}
