using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Hypermint.Modules.HyperSpin.Database
{
    public class DatabaseMenu
    {
        #region Properties
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("enabled")]
        public int Enabled { get; set; }
        [XmlIgnore]
        public Uri SysIcon { get; set; }
        [XmlIgnore]
        public bool XmlExists { get; set; }
        [XmlIgnore]
        public bool GenreExists { get; set; }
        #endregion

        //public DatabaseMenu()
        //{ }
        public DatabaseMenu(string _name)
        {
            Name = _name;

        }
        public DatabaseMenu(string _name, int _enabled = 1)
        {
            Name = _name;
            Enabled = _enabled;
        }
        public DatabaseMenu(string _name, Uri pathToIcon, int _enabled = 1)
        {
            Name = _name;
            Enabled = _enabled;
            if (pathToIcon != null)
               SysIcon = pathToIcon;
        }

    }
}
