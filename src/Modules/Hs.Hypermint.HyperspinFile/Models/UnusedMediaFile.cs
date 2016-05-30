
namespace Hs.Hypermint.HyperspinFile.Models
{
    public class MediaFile
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

    }

    public class UnusedMediaFile : MediaFile
    {

    }
}
