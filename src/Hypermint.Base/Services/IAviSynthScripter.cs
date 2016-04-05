using Hypermint.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Services
{
    public interface IAviSynthScripter
    {
        void CreateScript(string[] VideoFiles, AviSynthOption options, string systemName,
            bool overlay = false, bool overlayResize = false, 
            string wheelPath = "", string exportScriptPath = @"exports\aviSynth");
    }
}
