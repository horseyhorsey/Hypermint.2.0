using Hs.HyperSpin.Database;
using System.Xml.Serialization;
using System.IO;
using Hypermint.Base.Constants;
using System.Collections.Generic;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public class HyperspinXmlService : IHyperspinXmlService
    {
        public bool SerializeHyperspinXml(IList<Game> gamesList, string systemName, 
            string hyperspinPath, string dbName = "")
        {            
            // This is to allow setting the database name
            // Used for favorites ..atm
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
