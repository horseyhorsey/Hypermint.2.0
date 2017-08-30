using Hypermint.Base.Interfaces;
using Hypermint.Base.Model;
using System.IO;

namespace Hypermint.Base
{
    public class SettingsRepo : ISettingsHypermint
    {
        private Setting hypermintSettings = new Setting();
        public Setting HypermintSettings
        {
            get { return hypermintSettings; }
            set { hypermintSettings = value; }
        }
    }

}
