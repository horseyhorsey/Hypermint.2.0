using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Models
{
    public class ScriptOptions
    {
        #region Properties
        public string[] vidname;
        public string[] audioname;
        public string[] wheelName;
        public string[] wheelNameAlpha;
        public string[] audioDub;
        public string[] trimName;

        public AviSynthOption avisynthOption { get; }
        public string SystemName { get; }
        public bool Overlay = false;
        public bool overlayResize = false;
        public bool ResizeOverlay;
        public string currentSystem;        
        public string[] VideoFiles { get; }

        public string WheelPath { get; }
        public string ExportScriptPath { get; }
        #endregion

        public ScriptOptions(
            string[] videos, AviSynthOption aviSynthOptions,
            string currentSystem, bool overlay, bool resizeOverlay,
            string wheelPath, string exportPath)
        {
            VideoFiles = videos;
            avisynthOption = aviSynthOptions;
            SystemName = currentSystem;
            Overlay = overlay;
            ResizeOverlay = resizeOverlay;
            WheelPath = wheelPath;
            ExportScriptPath = exportPath;
        }
    }
}
