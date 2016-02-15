using Hs.HyperSpin.Database;
using System.Xml.Serialization;
using System.IO;
using Hypermint.Base.Constants;
using System.Collections.Generic;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public class HyperspinXmlService : IHyperspinXmlService
    {
        public bool SerializeHyperspinXml(Games gamesList, string systemName, 
            string hyperspinPath, string dbName = "")
        {            
            if (dbName != "Favorites")
            {
                if (string.IsNullOrEmpty(dbName))
                    dbName = systemName;
            }
            
            var dbXmlFileName = dbName + ".xml";
            string finalPath = Path.Combine(hyperspinPath, Root.Databases, systemName, dbXmlFileName);
            var databasePath = Path.Combine(hyperspinPath, Root.Databases, systemName);

            TextWriter textWriter = new StreamWriter(finalPath);

            if (!Directory.Exists(databasePath))
                Directory.CreateDirectory(databasePath);

            XmlSerializer serializer;

            var xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("", "");            

            var xmlRootAttr = new XmlRootAttribute("menu");
            
            if (!systemName.Contains("Main Menu"))
            {
                serializer = new XmlSerializer(typeof(Games), xmlRootAttr);
                serializer.Serialize(textWriter, gamesList, xmlNameSpace);
            }
            else
            {
                var menuItems = new List<MainMenu>();

                foreach (var game in gamesList)
                {
                    menuItems.Add(new MainMenu(game.RomName, game.Enabled));
                }

                serializer = new XmlSerializer(typeof(List<MainMenu>), xmlRootAttr);
                serializer.Serialize(textWriter, menuItems, xmlNameSpace);
            }
                                                                        
            textWriter.Close();

            return true;

        }

    }
}
