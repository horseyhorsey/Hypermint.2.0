using System.IO;

namespace Hs.Hypermint.Services.Helpers
{
    public static class RlStaticMethods
    {
        /// <summary>
        /// Get full RL media path from mediatype system & rom
        /// </summary>
        /// <param name="rlMediaType"></param>
        /// <returns></returns>
        public static string GetSelectedPath(string rlMediaPath, 
            string rlMediaType, string systemName, string romName)
        {
            if (rlMediaType == "Saved Game")
                rlMediaType = "Saved Games";

            string parentMediaType = GetParentMediaType(rlMediaType);

            var gameMediaRootPath = GetRlGameMediaPath(
                    rlMediaPath,
                    rlMediaType, systemName, romName, parentMediaType);

            return gameMediaRootPath;
        }

        public static string GetParentMediaType(string rlMediaType)
        {
            if (rlMediaType.ToLower().Contains("layer"))
                rlMediaType = "layer";

            switch (rlMediaType)
            {
                case "Screenshots":
                    rlMediaType = "Artwork";
                    break;
                case "_Default Folder":
                    rlMediaType = "Fade";
                    break;
                case "Bezel":
                case "BezelBg":
                case "Cards":
                    rlMediaType = "Bezels";
                    break;
                case "Info":
                case "Progress":
                case "Extract":
                case "Complete":
                case "layer":
                    rlMediaType = "Fade";
                    break;                
                default:
                    rlMediaType = "";
                    break;
            }

            return rlMediaType;
        }

        private static string GetRlGameMediaPath(string rlMediaPath, string mediaType,
            string systemName, string romName, string parentMediaType = "")
        {
            if (parentMediaType != "")
            {
                if (mediaType == "Screenshots")
                    return Path.Combine(rlMediaPath, parentMediaType, systemName, romName, mediaType);
                else
                    return Path.Combine(rlMediaPath, parentMediaType, systemName, romName);
            }
            else
                return Path.Combine(rlMediaPath, mediaType, systemName, romName);
        }

        public static string GetMediaFormatFromFile(string file)
        {
            string mediaFormat = "";

            switch (Path.GetExtension(file.ToLower()))
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                    mediaFormat = "image";
                    break;
                case ".avi":
                case ".flv":
                case ".mp4":
                case ".mpg":
                    mediaFormat = "video";
                    break;
                case ".mp3":
                case ".wav":
                    mediaFormat = "audio";
                    break;
                case ".txt":
                case ".ini":
                    mediaFormat = "text";
                    break;
                case ".pdf":
                    mediaFormat = "pdf";
                    break;
                default:
                    mediaFormat = "";
                    break;
            }

            return mediaFormat;
        }

        public static string CreateFileNameForRlImage(string hmColumnName, string ratio,
            string desc, string author)
        {
            if (author != "")
                author = " (" + author + ")";

            string spacer = " - ";
            if (string.IsNullOrWhiteSpace(ratio))
                if (string.IsNullOrWhiteSpace(author))
                    if (string.IsNullOrWhiteSpace(desc))
                        spacer = "";            

            switch (hmColumnName)
            {
                case "Background":
                case "BezelBg":
                    return "Background - " + ratio + author;
                case "Bezel":
                case "Layer 1":
                case "Layer 2":
                case "Layer 3":
                case "Extra Layer 1":
                    return hmColumnName + spacer + ratio + " " + desc + author;
                default:
                    return "";
            }
        }

        public static string CreateCardFileName(string desc, string author, string position)
        {
            if (author != "")
                author = " (" + author + ")";

            string spacer = " - ";
            if (string.IsNullOrWhiteSpace(author))
                if (string.IsNullOrWhiteSpace(desc))
                        spacer = "";

            return "Instruction Card" + spacer + desc + author + " - " + position;
        }

        public static void SaveBezelIni(double[] Inipoints, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("[General]");
                sw.WriteLine("Bezel Screen Top Left X Coordinate=" + Inipoints[0]);
                sw.WriteLine("Bezel Screen Top Left Y Coordinate=" + Inipoints[1]);
                sw.WriteLine("Bezel Screen Bottom Right X Coordinate=" + Inipoints[2]);
                sw.WriteLine("Bezel Screen Bottom Right Y Coordinate=" + Inipoints[3]);
                sw.Flush();                
            }
        }

        public static string[] LoadBezelIniValues(string bezelIni)
        {
            if (!File.Exists(bezelIni)) return null;            

            IniFileReader ini = new IniFileReader(bezelIni);

            string[] values = new string[4];

            values[0] = ini.IniReadValue("General", "Bezel Screen Top Left X Coordinate");
            values[1] = ini.IniReadValue("General", "Bezel Screen Top Left Y Coordinate");
            values[2] = ini.IniReadValue("General", "Bezel Screen Bottom Right X Coordinate");
            values[3] = ini.IniReadValue("General", "Bezel Screen Bottom Right Y Coordinate");                                   
            return values;
        }
    }
}
