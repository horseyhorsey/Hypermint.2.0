using System;
using System.Linq;
using Hypermint.Base.Interfaces;
using System.IO;
using System.Xml;
using Frontends.Models.Hyperspin;

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
                      
            string[,] systems = GetSystems(mainMenuXml);

            try
            {
                systems[0, 0] = "Main Menu";
                systems[0, 1] = "0";
                Systems.Add(new MainMenu(systems[0, 0], Convert.ToInt32(systems[0, 1])));

                for (int i = 1; i < systems.GetLength(0); i++)
                {
                    try
                    {
                        if (systems[i,1] == null) { systems[i, 1] = "1"; }

                        if (iconsPath != string.Empty && Directory.Exists(iconsPath))
                        {
                            Uri iconImage = new Uri(Path.Combine(iconsPath, systems[i, 0] + ".png"));
                            Systems.Add(new MainMenu(systems[i, 0], iconImage, Convert.ToInt32(systems[i, 1])));
                        }
                        else
                            Systems.Add(new MainMenu(systems[i, 0], Convert.ToInt32(systems[i, 1])));
                        
                    }
                    catch (Exception) { throw; }
                   
                }
            }
            catch (Exception)
            {

                throw;
            }
           
     
        }

        private string[,] GetSystems(string MainMenuXml)
        {
            if (!File.Exists(MainMenuXml))
                return new string[0,0];

            string[,] systemsArray;

            using (XmlTextReader reader = new XmlTextReader(MainMenuXml))
            {
                var menuName = Path.GetFileNameWithoutExtension(MainMenuXml);
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(MainMenuXml);
                int sysCount = xdoc.SelectNodes("menu/game").Count + 1;
                systemsArray = new string[sysCount,2];
                int i = 0;
                systemsArray[i,0] = menuName;
                systemsArray[i, 1] = "0";

                i++;
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                        if (reader.HasAttributes)
                        {
                            systemsArray[i, 0] = reader.GetAttribute("name");
                            systemsArray[i, 1] = reader.GetAttribute("enabled");
                            
                            i++;
                        }
                }
            }

            return systemsArray;
        }

        [Obsolete("New Hyperspin frontend")]
        public string[] GetMainMenuDatabases(string MainMenuFolder)
        {
            if (!Directory.Exists(MainMenuFolder))
                return new string[0];

            var xmlPath = MainMenuFolder;
            string[] menuXmls;

            if (Directory.Exists(xmlPath))
            {
                int i = 0;
                menuXmls = new string[Directory.GetFiles(xmlPath, "*.xml").Count()];
                foreach (var item in Directory.GetFiles(xmlPath, "*.xml"))
                {
                    menuXmls[i] = item;
                    i++;
                }
                return menuXmls;
            }
            else
                return null;
        }

    }
}
