using ImageMagick;

namespace Hs.Hypermint.WheelCreator.Services
{
    public interface IBackgroundImageService
    {
        MagickImage FilledBackground(MagickColor bgColor, int width, int height);

        MagickImage PlasmaBackground(string color1 = "steelblue", string color2 = "black",
        int width = 1024, int height = 768);

        MagickImage FeatherImage(MagickImage image, MorphologyMethod method, Kernel kernel);
    }
}
