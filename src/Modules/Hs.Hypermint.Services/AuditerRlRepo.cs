﻿using Hs.RocketLauncher.AuditBase;
using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.Services
{
    public class AuditerRlRepo : IAuditerRl
    {
        public RocketLauncherAudits RlAudits { get; set; }
    }
}
