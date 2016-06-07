using Hypermint.Base.Models;

namespace Hypermint.Base.Services
{
    public interface IAviSynthScripter
    {
        string CreateScript(string[] VideoFiles, AviSynthOption options, string systemName,
            bool overlay = false, bool overlayResize = false, 
            string wheelPath = "", string exportScriptPath = @"exports\aviSynth");
    }
}
