using Hypermint.Base.Interfaces;
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontends.Models.Hyperspin;

namespace Hs.Hypermint.Services
{
    public class GameRepo : IGameRepo
    {
        /// <summary>
        /// Shared gameslist for the view models
        /// </summary>
        public Games GamesList { get; set; }
       
        /// <summary>
        /// Scan system xml for games
        /// </summary>
        /// <param name="systemXml"></param>
        /// <param name="systemName"></param>
        public void GetGames(string systemXml, string systemName = "Main Menu")
        {
            if (!File.Exists(systemXml)) return;

            var tempGamesList = new List<Game>();            
            
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(systemXml);

            string name = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
                manu = string.Empty, genre = string.Empty, rating = string.Empty;
            int enabled = 0;
            int year = 0;
            string index = string.Empty;
            var i = 0;
            var lastRom = string.Empty;

            //GamesList.Add(new Game("_Default", "_Default"));

            try
            {
                foreach (XmlNode node in xdoc.SelectNodes("menu/game"))
                {
                    name = node.SelectSingleNode("@name").InnerText;

                    char s = name[0];
                    char t;

                    if (lastRom != string.Empty)
                    {
                        t = lastRom[0];
                        if (char.ToLower(s) == char.ToLower(t))
                        {
                            index = string.Empty;
                            image = string.Empty;
                        }
                        else
                        {
                            index = "true";
                            image = char.ToLower(s).ToString();
                        }
                    }

                    if (node.SelectSingleNode("@enabled") != null)
                    {
                        if (node.SelectSingleNode("@enabled").InnerText != null)
                        {
                            enabled = Convert.ToInt32(node.SelectSingleNode("@enabled").InnerText);
                        }
                    }
                    else
                        enabled = 1;

                    if (!systemName.Contains("Main Menu"))
                    {
                        desc = node.SelectSingleNode("description").InnerText;

                        if (node.SelectSingleNode("cloneof") != null)
                            cloneof = node.SelectSingleNode("cloneof").InnerText;
                        if (node.SelectSingleNode("crc") != null)
                            crc = node.SelectSingleNode("crc").InnerText;
                        if (node.SelectSingleNode("manufacturer") != null)
                            manu = node.SelectSingleNode("manufacturer").InnerText;
                        if (node.SelectSingleNode("year") != null)
                            if (!string.IsNullOrEmpty(node.SelectSingleNode("year").InnerText))
                                Int32.TryParse(node.SelectSingleNode("year").InnerText, out year);

                        if (node.SelectSingleNode("genre") != null)
                        {
                            var genreName = node.SelectSingleNode("genre").InnerText;

                            genreName = genreName.Replace(@"\", "-");
                            genreName = genreName.Replace(@"/", "-");

                            genre = genreName;
                        }
                        if (node.SelectSingleNode("rating") != null)
                            rating = node.SelectSingleNode("rating").InnerText;

                    }

                    tempGamesList.Add(new Game(name, index, image, desc, cloneof, crc, manu, year, genre, rating, enabled,systemName));

                    lastRom = name;
                    i++;
                }

                //Only sort by romname if it isn't main menu
                if(!systemName.Contains("Main Menu"))
                    tempGamesList.Sort((x, y) => x.RomName.CompareTo(y.RomName));

                GamesList = new Games();

                foreach (var item in tempGamesList)
                {
                    GamesList.Add(item);
                }
                      

            }
            catch (Exception e) { var msg = e.Message; }

            
            
        }

        public async Task GetGamesAsync(string systemXml, string systemName = "Main Menu")
        {
            if (!File.Exists(systemXml)) return;

            var tempGamesList = new List<Game>();

            await Task.Run(() =>
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(systemXml);

                string name = string.Empty, image = string.Empty, desc = string.Empty, cloneof = string.Empty, crc = string.Empty,
                    manu = string.Empty, genre = string.Empty, rating = string.Empty;
                int enabled = 0;
                int year = 0;
                string index = string.Empty;
                var i = 0;
                var lastRom = string.Empty;

                //GamesList.Add(new Game("_Default", "_Default"));

                try
                {
                    foreach (XmlNode node in xdoc.SelectNodes("menu/game"))
                    {
                        name = node.SelectSingleNode("@name").InnerText;

                        char s = name[0];
                        char t;

                        if (lastRom != string.Empty)
                        {
                            t = lastRom[0];
                            if (char.ToLower(s) == char.ToLower(t))
                            {
                                index = string.Empty;
                                image = string.Empty;
                            }
                            else
                            {
                                index = "true";
                                image = char.ToLower(s).ToString();
                            }
                        }

                        if (node.SelectSingleNode("@enabled") != null)
                        {
                            if (node.SelectSingleNode("@enabled").InnerText != null)
                            {
                                enabled = Convert.ToInt32(node.SelectSingleNode("@enabled").InnerText);
                            }
                        }
                        else
                            enabled = 1;

                        if (!systemName.Contains("Main Menu"))
                        {
                            desc = node.SelectSingleNode("description").InnerText;

                            if (node.SelectSingleNode("cloneof") != null)
                                cloneof = node.SelectSingleNode("cloneof").InnerText;
                            if (node.SelectSingleNode("crc") != null)
                                crc = node.SelectSingleNode("crc").InnerText;
                            if (node.SelectSingleNode("manufacturer") != null)
                                manu = node.SelectSingleNode("manufacturer").InnerText;
                            if (node.SelectSingleNode("year") != null)
                                if (!string.IsNullOrEmpty(node.SelectSingleNode("year").InnerText))
                                    Int32.TryParse(node.SelectSingleNode("year").InnerText, out year);

                            if (node.SelectSingleNode("genre") != null)
                            {
                                var genreName = node.SelectSingleNode("genre").InnerText;

                                genreName = genreName.Replace(@"\", "-");
                                genreName = genreName.Replace(@"/", "-");

                                genre = genreName;
                            }
                            if (node.SelectSingleNode("rating") != null)
                                rating = node.SelectSingleNode("rating").InnerText;

                        }

                        tempGamesList.Add(new Game(name, index, image, desc, cloneof, crc, manu, year, genre, rating, enabled, systemName));

                        lastRom = name;
                        i++;
                    }

                    //Only sort by romname if it isn't main menu
                    if (!systemName.Contains("Main Menu"))
                        tempGamesList.Sort((x, y) => x.RomName.CompareTo(y.RomName));

                    GamesList = new Games();

                    foreach (var item in tempGamesList)
                    {
                        GamesList.Add(item);
                    }


                }
                catch (Exception e) { var msg = e.Message; }
            });

        }

        public void ScanForRoms(string rlPath, string systemName)
        {
            var emuName = RocketSettingsRepo.GetDefaultEmulator(rlPath, systemName);
            var paths = RocketSettingsRepo.GetRomPaths(rlPath, systemName);
            var exts = RocketSettingsRepo.GetRomExtensions(rlPath, emuName);

            for (int i = 0; i < GamesList.Count; i++)
            {
                GamesList[i].RomExists = 
                    RomCheck.RomExists(paths, exts, GamesList[i].RomName);
            }
        }

    }
}
