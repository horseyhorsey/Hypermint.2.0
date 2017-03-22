using Hypermint.Base.Interfaces;
using Hypermint.Base.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System;

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
                Image tempImage = null;
                Image finalImage = null;

                if (preset.TileEnabled)
                {
                    int tw = preset.TileWidth;
                    int th = preset.TileHeight;

                    if (preset.FlipOn)
                    {
                        tempImage = FlipImage(imgIn, preset.FlipL);
                    }

                    if (tempImage != null)
                        finalImage = ResizeImageTile(tempImage, new Size(preset.Width, preset.Height));
                    else
                        finalImage = ResizeImageTile(imgIn, new Size(preset.Width, preset.Height));

                    //NOT USED?
                    //tempImage = ResizeImage(imgIn, new Size(tw, th));

                }
                else if (preset.StretchImage)
                {
                    if (preset.FlipOn)                    
                        tempImage = FlipImage(imgIn, preset.FlipL);

                    if (tempImage != null)
                        finalImage = ResizeImageEdit(tempImage, new Size(preset.Width, preset.Height));
                    else
                        finalImage = ResizeImageEdit(imgIn, new Size(preset.Width, preset.Height));
                }
                else if (preset.ResizeImage)
                {
                    if (preset.FlipOn)
                        tempImage = FlipImage(imgIn, preset.FlipL);

                    if (tempImage != null)
                        finalImage = ResizeImage(tempImage, new Size(preset.Width, preset.Height));
                    else
                        finalImage = ResizeImage(imgIn, new Size(preset.Width, preset.Height));
                }
                else
                {
                    //If this is hit only allow image flips to be rendered.
                    if (preset.FlipOn)
                    {
                        tempImage = FlipImage(imgIn, preset.FlipL);

                        if (tempImage != null)
                            finalImage = ResizeImage(tempImage, new Size(imgIn.Width, imgIn.Height));
                        else
                            finalImage = ResizeImage(imgIn, new Size(imgIn.Width, imgIn.Height));
                    }                     
                }

                try
                {
                    if (finalImage == null)
                        finalImage = imgIn;

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

        /// <summary>
        /// Rotates image 90 or 270
        /// </summary>
        /// <param name="img"></param>
        /// <param name="flipL">true for left, false for right</param>
        private Image FlipImage(Image img, bool flipL)
        {
            if (flipL)
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);

            return img;
        }

        public bool ConvertImageFormat(string inputImage,
            string outputFileName, bool isJpg)
        {
            using (var imgIn = Image.FromFile(inputImage))
            {
                if (isJpg)
                    imgIn.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Png);
                else
                    imgIn.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            return true;
        }
    }
}
