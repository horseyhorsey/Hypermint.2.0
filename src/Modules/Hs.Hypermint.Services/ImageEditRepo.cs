using Hypermint.Base.Interfaces;
using Hypermint.Base.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Hs.Hypermint.Services
{
    public class ImageEditRepo : IImageEditService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="rocketGameMediaPath">Full path to games rocketlauncher folder I:\RL\Media\MediaType\System\Romname</param>
        /// <param name="outputPath"></param>
        /// <param name="outputFileName"></param>
        /// <param name="isJpg"></param>
        /// <returns></returns>
        public string ConvertImageUsingPreset(
            ImageEditPreset preset,
            string inputImage,
            string outputFileName,
            bool isPng)
        {            

            using (var imgIn = Image.FromFile(inputImage))
            {
                Image finalImage;

                if (preset.TileEnabled)
                {
                    int tw = preset.TileWidth;
                    int th = preset.TileHeight;

                    Image tileImage;

                    tileImage = ResizeImage(imgIn, new Size(tw, th));

                    if (preset.FlipOn)
                    {
                        if (preset.FlipL)
                            tileImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        else
                            tileImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }

                    finalImage = ResizeImageTile(tileImage, new Size(preset.Width, preset.Height));
                }

                else if (preset.StretchImage)
                {
                    finalImage = ResizeImageEdit(imgIn, new Size(preset.Width, preset.Height));
                }

                else if (preset.ResizeImage)
                {
                    finalImage = ResizeImage(imgIn, new Size(preset.Width, preset.Height));
                }

                else
                {
                    finalImage = ResizeImageEdit(imgIn, new Size(preset.Width, preset.Height));
                }

                try
                {
                    if (isPng)
                        finalImage.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Png);
                    else
                        finalImage.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (System.Exception)
                {
                    
                }
                
                return outputFileName;
            }
                                               
        }

        /// <summary>
        /// Resize an image
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Image ResizeImage(Image imgToResize, Size size)
        {

            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = size.Width / (float)sourceWidth;
            nPercentH = size.Height / (float)sourceHeight;

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            var bitMap = new Bitmap(destWidth, destHeight);
            
            using (var g = Graphics.FromImage(bitMap))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();
            }

            using (TextureBrush brush = new TextureBrush(bitMap, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(bitMap))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.FillRectangle(brush, 0, 0, 1920, 1080);
                g.Dispose();
            }

            return bitMap;
        }

        public Image ResizeImageTile(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            int destWidth = (int)(size.Width);
            int destHeight = (int)(size.Height);

            var bitMap = new Bitmap(destWidth, destHeight);

            using (TextureBrush brush = new TextureBrush(imgToResize, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(bitMap))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.FillRectangle(brush, 0, 0, destWidth, destHeight);
                g.Dispose();
            }
            return bitMap;
        }

        public Image ResizeImageEdit(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            int destWidth = (int)(size.Width);
            int destHeight = (int)(size.Height);

            var bitMap = new Bitmap(destWidth, destHeight);

            using (TextureBrush brush = new TextureBrush(imgToResize, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(bitMap))
            {                
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
             
                g.Dispose();
            }

            return bitMap;
        }
    }
}
