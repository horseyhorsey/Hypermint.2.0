﻿using NUnit.Framework;
using ImageMagick;

namespace Hs.Hypermint.WheelCreator.Tools.Tests
{
    [TestFixture()]
    public class WheelGenTests
    {
        [Test()]
        public void GenerateLogoTest()
        {
            //TextOverlay();

            //Label();

            // ShrinkFit();

            ImageMagickAnnotate();
        }

        private void ImageMagickAnnotate()
        {
            using (MagickImage image = new MagickImage(MagickColors.Transparent,400,180))
            {
                image.Settings.Font = "Candice";
                image.Settings.FontPointsize = 100;
                image.Settings.FillPattern = new MagickImage("pattern:checkerboard");
                image.Annotate("Anthony\nis\ncheap", new MagickGeometry(400, 180));
                image.Write(@"c:\users\admin\desktop\font_tile.png");
            }
        }

        private void TextOverlay()
        {
            using (var image = new MagickImage(MagickColors.LightBlue, 400, 150))
            {
                image.Settings.Font = "Impact";
                image.Settings.Page = new MagickGeometry(400, 150);
                image.Settings.FillPattern = new MagickImage("pattern:checkerboard");

                image.Annotate("Anthony\nneh", Gravity.Center);

                image.Write(@"c:\users\admin\desktop\font_tile.png");
            }

        }

        private static void Label()
        {

            using (MagickImage img = new MagickImage())
            {

                img.Settings.FontStyle = FontStyleType.Italic;
                img.Settings.FontWeight = FontWeight.Bold;
                img.Settings.FillColor = new MagickColor("purple");
                img.Settings.TextGravity = Gravity.Center;
                img.Read("label:Magick.NET \nis chined", 0, 0);
                img.Trim();
                img.Write(@"c:\users\admin\desktop\font_tile.png");
            }

        }

        private static void ShrinkFit()
        {
            string word = "Dizzy Ultimate cartoon Adventure";

            using (MagickImage image = new MagickImage(MagickColors.Tomato, 400, 200))
            {
                image.Settings.FillColor = MagickColors.Purple;
                image.Settings.Font = "Arial";
                image.Settings.Density = new Density(72, 72);

                image.Settings.FontPointsize = 36;
                TypeMetric typeMetric = image.FontTypeMetrics(word);
                while (typeMetric.TextWidth < image.Width)
                {
                    image.Settings.FontPointsize++;
                    typeMetric = image.FontTypeMetrics(word);
                }
                image.Settings.FontPointsize--;

                image.Settings.FillPattern = new MagickImage("pattern:checkerboard");

                image.Annotate(word, Gravity.Center);

                image.Write(@"c:\users\admin\desktop\font_tile.png");
            }
        }
    }
}