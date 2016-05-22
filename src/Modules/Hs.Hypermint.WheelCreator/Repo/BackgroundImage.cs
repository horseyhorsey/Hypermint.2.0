using Hs.Hypermint.WheelCreator.Services;
using ImageMagick;

namespace Hs.Hypermint.WheelCreator.Repo
{
    public class BackgroundImage : IBackgroundImageService
    {
        public MagickImage FilledBackground(MagickColor bgColor, int width, int height)
        {
            var image = new MagickImage(bgColor, width, height);

            return image;
        }

        public MagickImage PlasmaBackground(string color1 = "steelblue", string color2 = "black",
        int width = 1024, int height = 768)
        {
            string plasmaImageText = "plasma:";
            plasmaImageText += color1 + "-";
            plasmaImageText += color2;

            var plasmaImage = new MagickImage(plasmaImageText, width, height);

            plasmaImage.Alpha(AlphaOption.Set);

            plasmaImage.BackgroundColor = MagickColors.Transparent;

            //plasmaImage.Vignette(0.5, 100, width, height);           

            return plasmaImage;

        }

        public MagickImage FeatherImage(MagickImage image, MorphologyMethod method, Kernel kernel)
        {
            var feathered = new MagickImage(image);            

            feathered.VirtualPixelMethod = VirtualPixelMethod.Transparent;

            feathered.Morphology(method, kernel);

            return feathered;
        }

    }
}
