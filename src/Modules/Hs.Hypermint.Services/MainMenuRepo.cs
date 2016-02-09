using System;
using System.Linq;
using Hs.HyperSpin.Database;
using Hypermint.Base.Interfaces;
using System.IO;
using System.Xml;

namespace Hs.Hypermint.Services
{
    public class MainMenuRepo : IMainMenuRepo
    {        
        public Systems Systems { get; set; }

        public void BuildMainMenuItems(string mainMenuXml, string iconsPath = "")
        {
            if (!File.Exists(mainMenuXml))
                return;

            Systems = new Systems();
            //Create a databaseMenu object to reference
            foreach (string system in GetSystems(mainMenuXml))
            {
                if (iconsPath != string.Empty && Directory.Exists(iconsPath))
                {
                    Uri iconImage = new Uri(Path.Combine(iconsPath, system + ".png"));
                    Systems.Add(new MainMenu(system, iconImage));
                }
                else
                    Systems.Add(new MainMenu(system, 1));
            }
     
        }

        private string[] GetSystems(string MainMenuXml)
        {
            string[] sysName;

            using (XmlTextReader reader = new XmlTextReader(MainMenuXml))
            {
                var menuName = Path.GetFileNameWithoutExtension(MainMenuXml);
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(MainMenuXml);
                int sysCount = xdoc.SelectNodes("menu/game").Count + 1;
                sysName = new string[sysCount];
                int i = 0;
                sysName[i] = menuName;
                i++;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                        if (reader.HasAttributes)
                        {
                            sysName[i] = reader.GetAttribute("name");
                            i++;
                        }
                }
            }
            return sysName;
        }

        public string[] GetMainMenuDatabases(string MainMenuFolder)
        {
            var xmlPath = MainMenuFolder;
            string[] menuXmls;

            if (Directory.Exists(xmlPath))
            {
                int i = 0;
                menuXmls = new string[Directory.GetFiles(xmlPath, "*.xml").Count()];
                foreach (var item in Directory.GetFiles(xmlPath, "*.xml"))
                {
                    menuXmls[i] = Path.GetFileNameWithoutExtension(item);
                    i++;
                }
                return menuXmls;
            }
            else
                return null;
        }

    }
}
