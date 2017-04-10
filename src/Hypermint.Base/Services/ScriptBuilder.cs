using Hypermint.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Services
{
    /// <summary>
    /// Base abstract class for building scripts.
    /// </summary>
    public abstract class ScriptBuilder
    {
        protected Script _script = null;
        public void InitScript()
        {
            //_script = new Script(new ScriptOptions());
        }
        protected virtual string SetTrimName(int index, string audioDub) => "Trim" + index + " = " + audioDub;
        protected internal abstract void CreateScriptOptions();
        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="options"></param>
        protected internal abstract string BuildScript(ScriptOptions options);
    }
}
