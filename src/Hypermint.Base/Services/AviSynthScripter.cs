using Hypermint.Base.Models;

namespace Hypermint.Base.Services
{
    /// <summary>
    /// Creates avi synth scripts from a builder.
    /// </summary>
    public class AviSynthScripter : IAviSynthScripter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptOptions"></param>
        /// <returns>Name of exported filename with no ext</returns>
        public string CreateScript(ScriptOptions scriptOptions)
        {
            var builder = new AviSynthScriptBuilder();

            builder.InitScript();

            builder.CreateScriptOptions();

            return builder.BuildScript(scriptOptions);
        }
    }    
}
