using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Hypermint.Modules.HyperSpin.Database.Tools
{
    public class DatabaseTools
    {
        public List<DatabaseMenu> GetMainMenuItemsFromXml(string xmlPath, string pathToIcons = "")
        {
            if (!System.IO.File.Exists(xmlPath))
                return null;

            List<DatabaseMenu> tempMenuList = new List<DatabaseMenu>();

            using (XmlTextReader reader = new XmlTextReader(xmlPath))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(xmlPath);

                int sysCount = xdoc.SelectNodes("menu/game").Count;
                Uri img;

                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "game"))
                        if (reader.HasAttributes)
                        {
                            string name = reader.GetAttribute("name");
                            int enabled = Convert.ToInt32(reader.GetAttribute("enabled"));
                            string icon = pathToIcons + "\\" + name + ".png";

                            if (!System.IO.File.Exists(icon))
                                tempMenuList.Add(new DatabaseMenu(name, enabled));
                            else
                            {
                                img = new Uri(icon);
                                tempMenuList.Add(new DatabaseMenu(name, img, enabled));
                            }
                        }
                }
            }

            return tempMenuList;
        }
    }
}
