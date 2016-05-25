using System;
using System.Xml.Serialization;

namespace Hs.Hypermint.ImageEdit.Preset
{
    [Serializable]
    [XmlRoot(ElementName = "Preset")]
    public class ImageEditPreset
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Png { get; set; }

        public bool ResizeImage { get; set; }
        public bool StretchImage { get; set; }

        public string Description { get; set; }
        
        public bool TileEnabled { get; set; }
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }

        public bool FlipOn { get; set; }
        public bool FlipL { get; set; }
    }
}
