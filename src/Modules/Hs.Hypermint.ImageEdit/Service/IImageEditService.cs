using Hs.Hypermint.ImageEdit.Preset;

namespace Hs.Hypermint.ImageEdit.Service
{
    public interface IImageEditService
    {
        string ConvertImageUsingPreset(
            ImageEditPreset preset,
            string inputImage,
            string outputFileName,
            bool isPng);
    }
}
