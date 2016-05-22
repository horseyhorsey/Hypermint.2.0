using Hs.HyperSpin.Database;
using System.IO;
using System.Linq;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using Hs.HyperSpin.Database.Audit;

namespace Hs.Hypermint.Services
{
    /// <summary>
    /// Scan for Hyperspin media
    /// </summary>
    public class Auditer : IAuditer
    {
        public AuditsMenu AuditsMenuList { get; set; }

        public AuditsGame AuditsGameList { get; set; }

        /// <summary>
        /// Scans Game list for media and sets whether media exits,
        /// Sets all values to an Audit list for each game
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="databaseGameList"></param>
        public void ScanForMedia(string hyperspinPath, string systemName, Games databaseGameList)
        {            
            string FullPath;
            var hsPath = hyperspinPath;

            //Shouldn't be here for main menu??
            if (systemName.Contains("Main Menu"))
                return;

            AuditsGameList = new AuditsGame();

            for (int i = 0; i < databaseGameList.Count; i++)
            {                           
                string tempPath = "";                

                AuditsGameList.Add(new AuditGame()
                {
                    RomName = databaseGameList[i].RomName
                });

                if (systemName != "_Default")
                {                    

                    tempPath = Path.Combine(hsPath, Root.Media, systemName);

                    FullPath = Path.Combine(tempPath, Images.Artwork1,
                        databaseGameList.ElementAt(i).RomName + ".png");
                    AuditsGameList[i].HaveArt1 = CheckForFile(FullPath);
                    FullPath = Path.Combine(tempPath, Images.Artwork2,
                        databaseGameList.ElementAt(i).RomName + ".png");
                    AuditsGameList[i].HaveArt2 = CheckForFile(FullPath);
                    FullPath = Path.Combine(tempPath, Images.Artwork3,
                        databaseGameList.ElementAt(i).RomName + ".png");
                    AuditsGameList[i].HaveArt3 = CheckForFile(FullPath);
                    FullPath = Path.Combine(tempPath, Images.Artwork4,
                        databaseGameList.ElementAt(i).RomName + ".png");
                    AuditsGameList[i].HaveArt4 = CheckForFile(FullPath);

                    FullPath = Path.Combine(tempPath, Images.Backgrounds,
                        databaseGameList.ElementAt(i).RomName + ".png");
                    AuditsGameList[i].HaveBackground = CheckForFile(FullPath);

                    FullPath = Path.Combine(tempPath, Sound.BackgroundMusic,
                        databaseGameList.ElementAt(i).RomName + ".mp3");
                    AuditsGameList[i].HaveBGMusic = CheckForFile(FullPath);

                    FullPath = Path.Combine(tempPath, Sound.SystemStart);
                    AuditsGameList[i].HaveS_Start = CheckMediaFolderFiles(FullPath, "*.mp3");

                    FullPath = Path.Combine(tempPath, Sound.SystemExit);
                    AuditsGameList[i].HaveS_Exit = CheckMediaFolderFiles(FullPath, "*.mp3");
                    
                    tempPath = Path.Combine(hsPath, Root.Media, systemName);

                    FullPath = Path.Combine(tempPath, Images.Wheels, databaseGameList.ElementAt(i).RomName + ".png");
                    AuditsGameList[i].HaveWheel = CheckForFile(FullPath);

                    FullPath = Path.Combine(tempPath, Root.Themes, databaseGameList.ElementAt(i).RomName + ".zip");
                    AuditsGameList[i].HaveTheme = CheckForFile(FullPath);

                    //Video slightly different, where you have flvs & pngs
                    FullPath = Path.Combine(tempPath, Root.Video, databaseGameList.ElementAt(i).RomName + ".mp4");
                    if (!CheckForFile(FullPath))
                        FullPath = Path.Combine(tempPath, Root.Video, databaseGameList.ElementAt(i).RomName + ".flv");
                    if (!CheckForFile(FullPath))
                        FullPath = Path.Combine(tempPath, Root.Video, databaseGameList.ElementAt(i).RomName + ".png");
                    if (!CheckForFile(FullPath))
                        AuditsGameList[i].HaveVideo = false;
                    else
                        AuditsGameList[i].HaveVideo = true;
                }
                
            }

        }

        public void ScanForMediaMainMenu(string hyperspinPath, Games databaseGameList)
        {            
            string FullPath;
            var hsPath = hyperspinPath;
            string tempPath = "";

            AuditsMenuList = new AuditsMenu();

            for (int i = 0; i < databaseGameList.Count; i++)
            {
                AuditsMenuList.Add(new AuditMenu()
                {
                    
                    RomName = databaseGameList[i].RomName
                });

                tempPath = Path.Combine(hsPath, Root.Media);

                FullPath = Path.Combine(tempPath, AuditsMenuList[i].RomName, Images.Letters);
                AuditsMenuList[i].HaveLetters = CheckMediaFolderFiles(FullPath, "*.png");

                FullPath = Path.Combine(tempPath, AuditsMenuList[i].RomName, Images.Special);
                AuditsMenuList[i].HaveSpecial = CheckMediaFolderFiles(FullPath, "*.png");

                FullPath = Path.Combine(tempPath, AuditsMenuList[i].RomName, Images.GenreWheel);
                AuditsMenuList[i].HaveGenreWheel = CheckMediaFolderFiles(FullPath, "*.png");

                FullPath = Path.Combine(tempPath, AuditsMenuList[i].RomName, Images.GenreBackgrounds);
                AuditsMenuList[i].HaveGenreBG = CheckMediaFolderFiles(FullPath, "*.png");

                FullPath = Path.Combine(tempPath, AuditsMenuList[i].RomName, Images.Pointer);
                AuditsMenuList[i].HavePointer = CheckMediaFolderFiles(FullPath, "*.png");

                FullPath = Path.Combine(tempPath, AuditsMenuList[i].RomName, Root.Sound);
                AuditsMenuList[i].HaveS_Click = CheckMediaFolderFiles(FullPath, "Wheel Click.mp3");

                FullPath = Path.Combine(hsPath, Root.Media, AuditsMenuList[i].RomName, Sound.WheelSounds);
                AuditsMenuList[i].HaveS_Wheel = CheckMediaFolderFiles(FullPath, "*.mp3");

                FullPath = Path.Combine(tempPath, "Main Menu", Sound.BackgroundMusic, databaseGameList.ElementAt(i).RomName + ".mp3");
                AuditsMenuList[i].HaveBGMusic = CheckForFile(FullPath);                

                FullPath = Path.Combine(tempPath, "Main Menu", Images.Wheels, databaseGameList.ElementAt(i).RomName + ".png");
                AuditsMenuList[i].HaveWheel = CheckForFile(FullPath);

                FullPath = Path.Combine(tempPath, "Main Menu", Root.Themes, databaseGameList.ElementAt(i).RomName + ".zip");
                AuditsMenuList[i].HaveTheme = CheckForFile(FullPath);

                FullPath = Path.Combine(tempPath, "Main Menu", Root.Video, databaseGameList.ElementAt(i).RomName + ".mp4");
                AuditsMenuList[i].HaveVideo = CheckForFile(FullPath);


            }
        }

        #region THESE TWO METHODS NEED TO BE MOVED
        /// <summary>
        /// Check all files in directory with given Extension
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="extFilter"></param>
        /// <returns></returns>
        private bool CheckMediaFolderFiles(string fullpath, string extFilter)
        {
            if (!Directory.Exists(fullpath))
                return false;

            string[] getFiles;
            getFiles = Directory.GetFiles(fullpath, extFilter);
            if (getFiles.Length != 0)
                return true;
            else return false;
        }

        /// <summary>
        /// Check a given filename to exist
        /// </summary>
        /// <param name="filenamePath"></param>
        /// <returns></returns>
        private bool CheckForFile(string filenamePath)
        {
            if (File.Exists(filenamePath))
                return true;
            else
                return false;
        }

        #endregion

    }
}
