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
        RocketLauncherAudits RlAudits { get; set; }
    }
}
