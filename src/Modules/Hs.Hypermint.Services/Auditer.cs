using Hs.HyperSpin.Database;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hypermint.Base.Paths;
using Hypermint.Base.Constants;
using Hypermint.Base.Interfaces;
using System;
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

        public void ScanForMediaMainMenu(string hyperspinPath, List<MainMenu> mainMenuList)
        {
            int i = 0;
            string FullPath;
            var hsPath = Installation.HsPath;
            string tempPath = "";

            foreach (MainMenu node in mainMenuList)
            {
                //mainMenuList.ElementAt(i).AuditList = new List<AuditMainMenu>();
                //tempPath = Path.Combine(hsPath, Root.Media, node.Name);

                //mainMenuList.ElementAt(i).AuditList.Add(new AuditMainMenu());

                //FullPath = Path.Combine(tempPath, Images.Letters);
                //mainMenuList.ElementAt(i).AuditList[0].HaveLetters = CheckMediaFolderFiles(FullPath, "*.png");

                //FullPath = Path.Combine(tempPath, Images.Special);
                //mainMenuList.ElementAt(i).AuditList[0].HaveSpecial = CheckMediaFolderFiles(FullPath, "*.png");

                //FullPath = Path.Combine(tempPath, Images.GenreWheel);
                //mainMenuList.ElementAt(i).AuditList[0].HaveGenreWheel = CheckMediaFolderFiles(FullPath, "*.png");

                //FullPath = Path.Combine(tempPath, Images.GenreBackgrounds);
                //mainMenuList.ElementAt(i).AuditList[0].HaveGenreBG = CheckMediaFolderFiles(FullPath, "*.png");

                //FullPath = Path.Combine(tempPath, Images.Pointer);
                //mainMenuList.ElementAt(i).AuditList[0].HavePointer = CheckMediaFolderFiles(FullPath, "*.png");

                //FullPath = Path.Combine(tempPath, Root.Sound);
                //mainMenuList.ElementAt(i).AuditList[0].HaveS_Click = CheckMediaFolderFiles(FullPath, "Wheel Click.mp3");

                //FullPath = Path.Combine(tempPath, Sound.WheelSounds);
                //mainMenuList.ElementAt(i).AuditList[0].HaveS_Wheel = CheckMediaFolderFiles(FullPath, "*.mp3");

                //FullPath = Path.Combine(tempPath, Sound.BackgroundMusic, node.Name + ".mp3");
                //mainMenuList.ElementAt(i).AuditList[0].HaveBGMusic = CheckForFile(FullPath);

                //tempPath = Path.Combine(hsPath, Root.Media, "Main Menu");

                //FullPath = Path.Combine(tempPath, Images.Wheels, mainMenuList.ElementAt(i).Name + ".png");
                //mainMenuList.ElementAt(i).AuditList[0].HaveWheel = CheckForFile(FullPath);

                //FullPath = Path.Combine(tempPath, Root.Themes, mainMenuList.ElementAt(i).Name + ".zip");
                //mainMenuList.ElementAt(i).AuditList[0].HaveTheme = CheckForFile(FullPath);

                //i++;
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
