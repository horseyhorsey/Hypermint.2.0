using System.Collections.Generic;

namespace Hs.Hypermint.Services
{
    public class RocketMediaFolderScanResult
    {
        private string _scanFolderName;

        public List<string> MatchedFolders { get; set; }
        public List<string> UnMatchedFolders { get; set; }

        public RocketMediaFolderScanResult(string scanPath)
        {
            _scanFolderName = scanPath;
            MatchedFolders = new List<string>();
            UnMatchedFolders = new List<string>();
        }
    }
}
