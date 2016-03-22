using Hs.HyperSpin.Database;
using System.Xml.Serialization;
using System.IO;
using Hypermint.Base.Constants;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using System.Xml;
using System.Linq;

namespace Hs.Hypermint.DatabaseDetails.Services
{
    public class HyperspinXmlService : IHyperspinXmlService
    {
        public bool SerializeGenreXml(Games gamesList, string systemName, string hyperspinPath)
        {
            var genreNames = GetGenreNames(ref gamesList);

            SerializeGenreMainMenu(genreNames, hyperspinPath, systemName);

            foreach (string dbName in genreNames)
            {                
                var tempGamesBuilder = new Games();

                if (dbName == systemName) return false;

                if (dbName != "")
                {
                    foreach (var game in gamesList)
                    {
                        if (game.Genre == dbName)
                            tempGamesBuilder.Add(game);
                    }

                    SerializeHyperspinXml(tempGamesBuilder, systemName, hyperspinPath, dbName);
                }
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
                if (item != "")
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
                {
                    game.Genre = game.Genre.Replace("/", " ").Replace(@"\", " ");
                    genreNames.Add(game.Genre);
                }
            }

            genreNames.Sort();

            return genreNames;
        }

        public bool SerializeHyperspinXml(Games gamesList, string systemName,
            string hyperspinPath, string dbName = "",bool isMultiSystem = false)
        {
            if (dbName != "Favorites")
            {
                if (string.IsNullOrEmpty(dbName))
                    dbName = systemName;
            }

            var dbXmlFileName = dbName + ".xml";            

            string finalPath = Path.Combine(hyperspinPath, Root.Databases, systemName, dbXmlFileName);
            var databasePath = Path.Combine(hyperspinPath, Root.Databases, systemName);

            if (isMultiSystem)
            {
                File.Create(databasePath + "\\_multisystem");
            }

            if (!File.Exists(finalPath))
            {
                Directory.CreateDirectory(databasePath);

                using (var file = File.Create(finalPath))
                {
                    file.Close();
                }

            }

            var games = new List<Game>(gamesList);

            games.Sort();            

            TextWriter textWriter = new StreamWriter(finalPath);

            if (!Directory.Exists(databasePath))
                Directory.CreateDirectory(databasePath);

            XmlSerializer serializer;

            var xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add("", "");

            var xmlRootAttr = new XmlRootAttribute("menu");

            if (!systemName.Contains("Main Menu"))
            {
                try
                {
                    serializer = new XmlSerializer(typeof(List<Game>), xmlRootAttr);
                    serializer.Serialize(textWriter, games, xmlNameSpace);
                }
                catch (Exception e) { }

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


        public Game SearchGameFromXml(string romName, string system, string hyperspinPath)
        {
            var fetchedGame = new Game();

            string mainMenuXml = Path.Combine(hyperspinPath, Root.Databases, system, system + ".xml");

            XDocument xdoc = null;

            using (var xmlreader = XmlReader.Create(mainMenuXml))
            {
                try { xdoc = XDocument.Load(xmlreader); }
                catch (XmlException e) { return fetchedGame; }
            }

            try
            {
                var gameVars =
                from item in xdoc.Descendants("game")
                where (item.Attribute("name").Value.Contains(romName) && (item.Element("cloneof").Value == ""))

                select new
                {
                    RomName = item.Attribute("name").Value,
                    Enabled = 1,
                    Description = item.Element("description").Value,
                    CloneOf = item.Element("cloneof").Value,
                    CRC = item.Element("crc").Value,
                    Manufacturer = item.Element("manufacturer").Value,
                    Genre = item.Element("genre").Value,
                    Year = Convert.ToInt32(item.Element("year").Value),
                    Rating = item.Element("rating").Value,
                    System = system
                };

                try
                {
                    foreach (var game in gameVars)
                    {
                        if (game != null)
                        {
                            fetchedGame = new Game()
                            {
                                RomName = game.RomName,
                                Description = game.Description,
                                CloneOf = game.CloneOf,
                                Crc = game.CRC,
                                Manufacturer = game.Manufacturer,
                                Year = game.Year,
                                Genre = game.Genre,
                                Rating = game.Rating,
                                Enabled = game.Enabled,
                                System = game.System,
                                IsFavorite = true
                            };
                        }
                    }
                }
                catch (NullReferenceException e) { }
            }
            catch { }            
            
            return fetchedGame;
        }

        public List<Game> SearchRomStringsListFromXml(List<string> romsNamesList, string system, string hyperspinPath)
        {
            var fetchedGames = new List<Game>();

            string mainMenuXml = Path.Combine(hyperspinPath, Root.Databases, system, system + ".xml");

            XDocument xdoc = null;

            using (var xmlreader = XmlReader.Create(mainMenuXml))
            {
                try { xdoc = XDocument.Load(xmlreader); }
                catch (XmlException e) { return fetchedGames; }
            }

            try
            {
                foreach (string romName in romsNamesList)
                {
                    var gameVars =
               from item in xdoc.Descendants("game")
               where (item.Attribute("name").Value.Contains(romName) && (item.Element("cloneof").Value == ""))

               select new
               {
                   RomName = item.Attribute("name").Value,
                   Enabled = 1,
                   Description = item.Element("description").Value,
                   CloneOf = item.Element("cloneof").Value,
                   CRC = item.Element("crc").Value,
                   Manufacturer = item.Element("manufacturer").Value,
                   Genre = item.Element("genre").Value,
                   Year = Convert.ToInt32(item.Element("year").Value),
                   Rating = item.Element("rating").Value,
                   System = system
               };
                    
                    try
                    {
                        Game fetchedGame = new Game();

                        foreach (var game in gameVars)
                        {

                            if (game != null && !string.IsNullOrEmpty(game.RomName))
                            {
                                fetchedGame = new Game()
                                {
                                    RomName = game.RomName,
                                    Description = game.Description,
                                    CloneOf = game.CloneOf,
                                    Crc = game.CRC,
                                    Manufacturer = game.Manufacturer,
                                    Year = game.Year,
                                    Genre = game.Genre,
                                    Rating = game.Rating,
                                    Enabled = game.Enabled,
                                    System = game.System,
                                    IsFavorite = true
                                };
                            }

                            if (string.IsNullOrEmpty(game.Description))
                                fetchedGame.Description = game.RomName;

                            fetchedGames.Add(fetchedGame);
                            
                        }

                    }
                    catch (NullReferenceException e) { }                
            catch { }
        }
            }
            catch { }

            return fetchedGames;
        }
    }
}
