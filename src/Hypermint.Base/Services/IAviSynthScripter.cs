using Hypermint.Base.Models;

namespace Hypermint.Base.Services
{
    public interface IAviSynthScripter
    {
        string CreateScript(ScriptOptions scriptOptions);
    }
}
