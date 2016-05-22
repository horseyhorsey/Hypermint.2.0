using Hs.Hypermint.WheelCreator.Models;
using ImageMagick;
using System.Threading.Tasks;

namespace Hs.Hypermint.WheelCreator.Services
{
    public interface ITextImageService
    {
        Task<MagickImage> GenerateCaptionAsync(WheelTextSetting setting);

        string[] GetTextPresets();

        WheelTextSetting DeserializePreset(string presetFile);        
    }
}
