using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Models
{
    /// <summary>
    /// An avi synth script
    /// </summary>
    public class Script
    {
        private ScriptOptions _options;

        //string[] VideoFiles, AviSynthOption options,string systemName,
        //    bool overlay = false,bool overlayResize = false, string wheelPath = "", string exportScriptPath = @"exports\videos\"

        public Script(ScriptOptions options)
        {
            _options = options;
        }

        public string Name { get; internal set; }
    }
    
}
