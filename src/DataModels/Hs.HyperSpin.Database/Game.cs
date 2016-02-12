﻿using System.Xml.Serialization;

namespace Hs.HyperSpin.Database
{
    public class Game
    {
        #region Properties
        [XmlAttribute("name")]
        public string RomName { get; set; }
        [XmlIgnore]
        public string Name
        {
            get { return RomName; }
            set { RomName = value; }
        }
              
        [XmlAttribute("enabled")]
        public int Enabled { get; set; }

        [XmlElement]
        public string System { get; set; }

        [XmlAttribute("index")]
        public string Index { get; set; }

        [XmlAttribute("image")]
        public string Image { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("cloneof")]
        public string CloneOf { get; set; }

        [XmlElement("crc")]
        public string Crc { get; set; }

        [XmlElement("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlElement("genre")]
        public string Genre { get; set; }

        [XmlElement("year")]
        public int Year { get; set; }

        [XmlElement("rating")]
        public string Rating { get; set; }

        [XmlIgnore]
        public bool IsFavorite { get; set; }        
        #endregion

        #region Constructors
        /// <summary>
        /// standard romname & desc contructor
        /// </summary>
        /// <param name="Gamename"></param>
        /// <param name="Description"></param>
        public Game(string Gamename, string Description)
        {
            this.RomName = Gamename;
            this.Description = Description;            
        }

        public Game()
        {

        }

        public Game(string name, string index, string image, string desc, string cloneof,
    string crc, string manu, int year, string genre, string rating, int enabled)
        {
            RomName = name;
            Index = index;
            Image = image;
            Description = desc;
            CloneOf = cloneof;
            Crc = crc;
            Manufacturer = manu;
            Year = year;
            Genre = genre;
            Rating = rating;
            Enabled = enabled;            
        }

        public Game(string name, string index, string image, string desc, string cloneof,
            string crc, string manu, int year, string genre, string rating, int enabled, string system)
        {
            RomName = name;
            Index = index;
            Image = image;
            Description = desc;
            CloneOf = cloneof;
            Crc = crc;
            Manufacturer = manu;
            Year = year;
            Genre = genre;
            Rating = rating;
            Enabled = enabled;
            System = system;            
        }
        #endregion

    }
}
