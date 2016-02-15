using System;
using Hs.HyperSpin.Database;
using Hypermint.Base.Interfaces;
using System.Xml;
using System.Collections.Generic;

namespace Hs.Hypermint.Services
{
    public class GenreRepo : IGenreRepo
    {
        public List<string> GenreList { get; set; }

        /// <summary>
        /// Get genres from Hyperspins genre.xml
        /// </summary>
        /// <param name="genreXmlPath"></param>
        public void PopulateGenres(string genreXmlPath)
        {
            GenreList = new List<string>();

            XmlTextReader reader = new XmlTextReader(genreXmlPath);

            GenreList.Clear();

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                {
                    if (reader.HasAttributes)
                    {
                        var genreName = reader.GetAttribute("name");

                        GenreList.Add(genreName);
                    }
                }
            }
        }
    }
}
