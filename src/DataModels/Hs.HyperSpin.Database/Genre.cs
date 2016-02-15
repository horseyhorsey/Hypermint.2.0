using System.Xml.Serialization;

namespace Hs.HyperSpin.Database
{
    [XmlType(TypeName = "game")]
    public class Genre
    {        
        [XmlAttribute("name")]
        public string GenreName { get; set; }
    }
}
