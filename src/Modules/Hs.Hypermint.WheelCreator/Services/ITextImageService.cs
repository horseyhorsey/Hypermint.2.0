using Hs.Hypermint.WheelCreator.Models;
using ImageMagick;
using System.Threading.Tasks;

namespace Hs.Hypermint.WheelCreator.Services
{
    public interface ITextImageService
    {
        WheelTextSetting DeserializePreset(string presetFile);
        Task<MagickImage> GenerateCaptionAsync(WheelTextSetting setting);
        string[] GetTextPresets();             
    }
}
