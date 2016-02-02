using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hs.HyperSpin.Database;
using Hypermint.Base;
using Hypermint.Base.Interfaces;
using System.IO;
using System.Xml;

namespace Hs.Hypermint.Services
{
    public class MainMenuRepo : IMainMenuRepo
    {
        private Systems _systems;

        public Systems BuildMainMenuItems(string mainMenuXml, string iconsPath = "")
        {
            _systems = new Systems();
            //Create a databaseMenu object to reference
            foreach (string system in GetSystems(mainMenuXml))
            {
                if (iconsPath != string.Empty && Directory.Exists(iconsPath))
                {
                    Uri iconImage = new Uri(Path.Combine(iconsPath, system + ".png"));
                    _systems.Add(new MainMenu(system, iconImage));
                }
                else
                    _systems.Add(new MainMenu(system, 1));
            }

            return _systems;       
        }

        private string[] GetSystems(string MainMenuXml)
        {
            string[] sysName;

            using (XmlTextReader reader = new XmlTextReader(MainMenuXml))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(MainMenuXml);
                int sysCount = xdoc.SelectNodes("menu/game").Count + 1;
                sysName = new string[sysCount];
                int i = 0;
                sysName[i] = "Main Menu";
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
