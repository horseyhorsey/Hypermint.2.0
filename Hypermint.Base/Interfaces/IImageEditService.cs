using Hypermint.Base.Models;
using System.Drawing;

namespace Hypermint.Base.Interfaces
{
    public interface IImageEditService
    {
        string ConvertImageUsingPreset(
            ImageEditPreset preset,
            string inputImage,
            string outputFileName,
            bool isPng);

        bool ConvertImageFormat(string inputImage,
            string outputFileName,
            bool isPng);

        Image ResizeImage(Image imgToResize, Size size);

        Image ResizeImageTile(Image imgToResize, Size size);

        Image ResizeImageEdit(Image imgToResize, Size size);
    }
}
