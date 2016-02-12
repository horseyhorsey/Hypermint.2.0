﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hs.HyperSpin.Database;
using System.Xml.Serialization;
using System.IO;
using Hypermint.Base.Constants;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public class HyperspinXmlService : IHyperspinXmlService
    {
        public bool SerializeHyperspinXml(Games gamesList, string systemName, 
            string hyperspinPath, string dbName = "")
        {
            if (dbName != "Favorites")
                gamesList.RemoveAt(0);

            if (string.IsNullOrEmpty(dbName))
                dbName = systemName;

            var xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("", "");

            var xmlRootAttr = new XmlRootAttribute("menu");
            XmlSerializer serializer = new XmlSerializer(typeof(Games), xmlRootAttr);

            var databasePath = Path.Combine(hyperspinPath, Root.Databases, systemName);
            
            if (!Directory.Exists(databasePath))
                Directory.CreateDirectory(databasePath);

            var dbXmlFileName = dbName + ".xml";

            string finalPath = Path.Combine(hyperspinPath, Root.Databases, systemName, dbXmlFileName);

            TextWriter textWriter = new StreamWriter(finalPath);
            serializer.Serialize(textWriter, gamesList, xmlNameSpace);
            textWriter.Close();

            return true;

        }
    }
}
