using Frontends.Models.Hyperspin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hs.Hypermint.Services
{
    public class RocketMediaFolderScanner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RocketMediaFolderScanner"/> class.
        /// </summary>
        /// <param name="rocketMediaFolder">The rocket media folder.</param>
        /// <param name="hyperSpinfolder">The hyper spinfolder.</param>
        /// <exception cref="System.NullReferenceException">
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// </exception>
        public RocketMediaFolderScanner(string rocketMediaFolder, string hyperSpinfolder)
        {
            if (string.IsNullOrEmpty(rocketMediaFolder))
                throw new NullReferenceException();
            if (string.IsNullOrEmpty(hyperSpinfolder))
                throw new NullReferenceException();
            if (!Directory.Exists(rocketMediaFolder))
                throw new DirectoryNotFoundException();
            if (!Directory.Exists(hyperSpinfolder))
                throw new DirectoryNotFoundException();

            RocketLaunchMediaFolder = rocketMediaFolder;
            HyperSpinPath = hyperSpinfolder;
        }

        #region Properties

        public string RocketLaunchMediaFolder { get; private set; }
        public string HyperSpinPath { get; private set; }

        List<string> MediaFolders = new List<string>
            {
                "Backgrounds", "Artwork", "Fade"
            };

        #endregion

        #region Public Methods

        /// <summary>
        /// Matches the rocketlaunch media folder to games.
        /// </summary>
        /// <param name="directories">The directories.</param>
        /// <param name="gameRepo">The game repo.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">No games exist in gameRepo</exception>
        public RocketMediaFolderScanResult MatchFoldersToGames(string[] directories, IEnumerable<Game> gamesList)
        {
            if (gamesList.Count() == 0)
                throw new NullReferenceException("No games exist in games list");

            int matchedFolderCount = 0;
            int[] results = new int[4];
            RocketMediaFolderScanResult result = new RocketMediaFolderScanResult("");

            //If a directory matches a game in the list , increment the matched count
            foreach (var directory in directories)
            {
                //var dirName = Path.GetFileNameWithoutExtension(directory);
                var dirName = Path.GetFileName(directory);

                if (gamesList.Any(x => x.RomName.ToLower() == dirName.ToLower()))
                {
                    matchedFolderCount++;
                    result.MatchedFolders.Add(dirName);
                }
                else
                {
                    result.UnMatchedFolders.Add(dirName);
                }
            }

            results[0] = directories.Count();
            results[1] = matchedFolderCount;
            results[2] = gamesList.Count() - matchedFolderCount;
            results[3] = directories.Count() - matchedFolderCount;

            return result;
        }

        #endregion

        #region Support Methods        

        /// <summary>
        /// Gets all folders from a directory
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns></returns>
        public string[] GetAllFolders(string dir)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return Directory.GetDirectories(dir);
        }

        #endregion
    }
}
