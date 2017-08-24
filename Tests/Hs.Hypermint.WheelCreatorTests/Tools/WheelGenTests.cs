using NUnit.Framework;
using ImageMagick;
using System;
using Hs.Hypermint.WheelCreator.Services;
using Hs.Hypermint.WheelCreator.Repo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Hs.Hypermint.WheelCreator.Tools.Tests
{    
    [TestClass]
    [Obsolete("Use xunit")]
    public class WheelGenTests
    {
        string path = Path.Combine(Environment.CurrentDirectory, "Images");

        [TestMethod()]
        public void GenerateBackgroundColor()
        {
            IBackgroundImageService srvc = new BackgroundImage();

            using (var bg = srvc.FilledBackground(MagickColors.Aqua, 400, 200))
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                bg.Write($"{path}\\magickBG.png");
            }
        }

        [TestMethod()]
        public void GeneratePlasmaBackground()
        {
            IBackgroundImageService srvc = new BackgroundImage();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var bg = srvc.PlasmaBackground())
            {
                bg.Write($"{path}\\magickPlasmaBg.png");
            }
        }

        [TestMethod()]
        public void GeneratePlasmaBackgroundFeathered()
        {
            IBackgroundImageService srvc = new BackgroundImage();

            using (var bg = srvc.PlasmaBackground())
            {
                using (var feathered = srvc.FeatherImage(bg, MorphologyMethod.Erode, Kernel.Octagonal))
                {
                    bg.Write($"{path}\\magickPlasmaBg2.png");
                }

            }
        }

        //[Test()]
        //public void TiledText()
        //{
        //    using (var bg = new MagickImage("radial-gradient:green-yellow"))
        //    {
        //        using (var neh = WheelGen.CreateText(bg))
        //        {
        //            neh.Write(@"c:\users\admin\desktop\magickPlasmaBg.png");
        //        }
        //    }
        //}

        //[Test()]
        //public void GenerateLogoTest()
        //{
        //    //TextOverlay();

        //    //Label();

        //    ShrinkFit();

        //    //ImageMagickAnnotate();
        //}

        //private void ImageMagickAnnotate()
        //{
        //    using (MagickImage image = new MagickImage(MagickColors.Transparent, 400, 180))
        //    {
        //        image.Settings.Font = "Candice";
        //        image.Settings.FontPointsize = 100;
        //        image.Settings.FillPattern = new MagickImage("pattern:checkerboard");
        //        image.Annotate("Anthony\nis\ncheap", new MagickGeometry(800, 500));
        //        image.Write(@"c:\users\admin\desktop\font_tile.png");
        //    }
        //}

        //[Test()]
        //public void GetImageFromPdf()
        //{
        //    MagickNET.SetGhostscriptDirectory(@"C:\Program Files (x86)\gs\gs9.19\bin");

        //    using (MagickImageCollection collection = new MagickImageCollection())
        //    {
        //        MagickReadSettings settings = new MagickReadSettings();
        //        settings.FrameIndex = 0; // First page
        //        settings.FrameCount = 2; // Number of pages
        //                                 // Read only the first page of the pdf file

        //        collection.Read(@"I:\RocketLauncher\Media\Manuals\Amstrad CPC\1st Division Manager (Europe)\Manual.pdf", settings);

        //        Console.WriteLine("PDF page count: {0}", collection.Count);

        //        var appendedImage = collection.AppendHorizontally();

        //        //Crop pages, for SuperNES
        //        //appendedImage.Crop()

        //        appendedImage.Write(@"c:\users\admin\desktop\mergedPdf.png");
        //    }
        //}

        [TestMethod()]
        public void TextOverlay()
        {
            using (var image = new MagickImage(MagickColors.LightBlue, 400, 150))
            {
                //image.Settings.Font = @"I:\RocketLauncher\Media\Fonts\amstrad_cpc464.ttf";
                image.Settings.Page = new MagickGeometry(400, 150);
                image.Settings.FillPattern = new MagickImage("pattern:checkerboard");
                image.Annotate("Anthony\nneh", Gravity.Center);              
                image.Write($"{path}\\font_tileoverlay.png");
            }

        }

        [TestMethod()]
        public void Label()
        {
            using (MagickImage img = new MagickImage())
            {
                //img.Settings.Font = @"I:\RocketLauncher\Media\Fonts\amstrad_cpc464.ttf";
                img.Settings.FontStyle = FontStyleType.Italic;
                img.Settings.FontWeight = FontWeight.Bold;
                img.Settings.FillColor = new MagickColor("purple");
                img.Settings.TextGravity = Gravity.Center;
                img.Read("label:Magick.NET \nis chined", 400, 175);
                img.Trim();
                img.Write($"{path}\\font_tile.png");
            }
        }

        [TestMethod()]
        public void GenerateCaptionAsync()
        {
            var image = new MagickImage(MagickColors.Transparent, 400 , 200);

                var captionString = "caption:" + "My Test Text";

                image.Settings.FillColor =
                    new MagickColor(MagickColors.Aquamarine);

                image.Settings.StrokeColor =
                    new MagickColor(MagickColors.Red);

                //image.Settings.StrokeWidth = setting.StrokeWidth;

                //image.Settings.Font = setting.FontName;

                image.Settings.TextGravity = Gravity.Center;

            //if (setting.ShadeOn)
            //    image.Shade(setting.ShadeAzimuth, setting.ShadeElevation, false);

            image.Read(captionString);

                image.Write(Environment.CurrentDirectory + "\\Images\\CaptionTest.png");
                //if (setting.ArcAmount > 0)
                //    image.Distort(DistortMethod.Arc, setting.ArcAmount);


                //var shadowColor = new MagickColor(Converters.ColorConvert.ColorFromMediaColor(setting.ShadowColor));

                //image.Shadow(setting.ShadowX, setting.ShadowY, setting.ShadowSigma,
                //    new Percentage(setting.ShadowPercentage), new MagickColor(shadowColor));

                ////image.RePage();

                //if (setting.Trim)
                //    image.Trim();

            //return image;

        }

        private static void ShrinkFit()
        {

        }
    }
}