using Hs.HyperSpin.Database;
using System.Xml.Serialization;
using System.IO;
using Hypermint.Base.Constants;
using System.Collections.Generic;
using System;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public class HyperspinXmlService : IHyperspinXmlService
    {
        public bool SerializeGenreXml(Games gamesList, string systemName, string hyperspinPath)
        {
            var genreNames = GetGenreNames(ref gamesList);

            SerializeGenreMainMenu(genreNames, hyperspinPath,systemName);
            
            foreach (var item in genreNames)
            {
                var tempGamesBuilder = new Games();

                foreach (var game in gamesList)
                {
                    if (game.Genre == item)
                        tempGamesBuilder.Add(game);
                }

                SerializeHyperspinXml(tempGamesBuilder, systemName, hyperspinPath, item);                             
            }

            return true;
        }

        private void SerializeGenreMainMenu(List<string> genres, string hyperspinPath,
             string systemName)
        {
            var genreXml = "genre.xml";
            var finalPath = Path.Combine(hyperspinPath, Root.Databases, systemName, genreXml);
            var databasePath = Path.Combine(hyperspinPath, Root.Databases, systemName);
            TextWriter textWriter = new StreamWriter(finalPath);

            if (!Directory.Exists(databasePath))
                Directory.CreateDirectory(databasePath);

            XmlSerializer serializer;

            var xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("", "");

            var xmlRootAttr = new XmlRootAttribute("menu");

            var menuItems = new List<MainMenu>();
            foreach (var item in genres)
            {
                menuItems.Add(new MainMenu(item, 1));
            }

            try
            {
                serializer = new XmlSerializer(typeof(List<MainMenu>), xmlRootAttr);
                serializer.Serialize(textWriter, menuItems, xmlNameSpace);
            }
            catch (Exception) { }

            textWriter.Close();
        }

        /// <summary>
        /// Pull all genre names from the incoming gamesList
        /// </summary>
        /// <param name="gamesList"></param>
        /// <returns></returns>
        private List<string> GetGenreNames(ref Games gamesList)
        {
            var genreNames = new List<string>();

            foreach (var game in gamesList)
            {
            
                if (!genreNames.Contains(game.Genre))
                    genreNames.Add(game.Genre);
            }

            genreNames.Sort();

            return genreNames;
        }

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

            if (!File.Exists(finalPath))
            {
                Directory.CreateDirectory(databasePath);

                using (var file = File.Create(finalPath))
                {
                    file.Close();
                }
                
            }
                
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

        public bool SerializeMainMenuXml(Systems systemList, string hyperspinPath, string mainMenuName = "Main Menu")
        {            
            var finalPath = Path.Combine(hyperspinPath, Root.Databases, "Main Menu", mainMenuName + ".xml");
            var databasePath = Path.Combine(hyperspinPath, Root.Databases, "Main Menu");
            TextWriter textWriter = new StreamWriter(finalPath);

            systemList.Remove(new MainMenu("Main Menu"));

            if (!Directory.Exists(databasePath))
                Directory.CreateDirectory(databasePath);

            XmlSerializer serializer;

            if (File.Exists(finalPath))
            {
                if (File.Exists(finalPath + "BACKUP"))
                    File.Delete(finalPath + "BACKUP");

                File.Copy(finalPath, finalPath + "BACKUP");
            }

            var xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("", "");

            var xmlRootAttr = new XmlRootAttribute("menu");

            var menuItems = new List<MainMenu>();
            foreach (var item in systemList)
            {
                if (!item.Name.Contains("Main Menu"))
                    menuItems.Add(new MainMenu(item.Name, item.Enabled));
            }            

            try
            {
                serializer = new XmlSerializer(typeof(List<MainMenu>), xmlRootAttr);
                serializer.Serialize(textWriter, menuItems, xmlNameSpace);
            }
            catch (Exception) { }

            textWriter.Close();

            return true;
        }
    }
}
