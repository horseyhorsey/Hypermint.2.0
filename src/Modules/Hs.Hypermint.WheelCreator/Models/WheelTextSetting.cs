using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Hs.Hypermint.WheelCreator.Models
{
    [Serializable]
    [XmlRoot(ElementName = "Preset")]
    public class WheelTextSetting
    {        
        public string Name { get; set; }
        public string FontName { get; set; }
        public string PreviewText { get; set; }
        public Color TextColor { get; set; }
        public Color TextStrokeColor { get; set; }
        public Color ShadowColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Trim { get; set; }
        public string Gravity { get; set; }
        public double ArcAmount { get; set; }
        public double StrokeWidth { get; set; }
        public int ShadowX { get; set; }
        public int ShadowY { get; set; }
        public int ShadowPercentage { get; set; }
        public double ShadowSigma { get; set; }
        public bool Swap { get; set; }
        public bool Repage { get; set; }
        public bool Shade { get; set; }
        public bool ShadeOn { get; set; }
        public bool ColorShade { get; set; }        
        public double ShadeAzimuth { get; set; }
        public double ShadeElevation { get; set; }

    }
}
