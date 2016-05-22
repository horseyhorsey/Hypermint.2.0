﻿using NUnit.Framework;
using ImageMagick;
using System;
using Hs.Hypermint.WheelCreator.Services;
using Hs.Hypermint.WheelCreator.Repo;

namespace Hs.Hypermint.WheelCreator.Tools.Tests
{
    [TestFixture()]
    public class WheelGenTests
    {
        [Test()]
        public void GenerateBackgroundColor()
        {
            IBackgroundImageService srvc = new BackgroundImage();

            using (var bg = srvc.FilledBackground(MagickColors.Aqua, 400, 200))
            {
                bg.Write(@"c:\users\admin\desktop\magickBG.png");
            }
        }

        [Test()]
        public async void GenerateCaption()
        {

        }

        [Test()]
        public void GeneratePlasmaBackground()
        {
            IBackgroundImageService srvc = new BackgroundImage();

            using (var bg = srvc.PlasmaBackground())
            {
                bg.Write(@"c:\users\admin\desktop\magickPlasmaBg.png");
            }
        }

        [Test()]
        public void GeneratePlasmaBackgroundFeathered()
        {
            IBackgroundImageService srvc = new BackgroundImage();

            using (var bg = srvc.PlasmaBackground())
            {
                using (var feathered = srvc.FeatherImage(bg, MorphologyMethod.Erode, Kernel.Octagonal))
                {
                    feathered.Write(@"c:\users\admin\desktop\magickPlasmaBg.png");
                }

            }
        }

        [Test()]
        public void TiledText()
        {
            using (var bg = new MagickImage("radial-gradient:green-yellow"))
            {
                using (var neh = WheelGen.CreateText(bg))
                {
                    neh.Write(@"c:\users\admin\desktop\magickPlasmaBg.png");
                }
            }
        }

        [Test()]
        public void GenerateLogoTest()
        {
            //TextOverlay();

            //Label();

            ShrinkFit();

            //ImageMagickAnnotate();
        }

        private void ImageMagickAnnotate()
        {
            using (MagickImage image = new MagickImage(MagickColors.Transparent, 400, 180))
            {
                image.Settings.Font = "Candice";
                image.Settings.FontPointsize = 100;
                image.Settings.FillPattern = new MagickImage("pattern:checkerboard");
                image.Annotate("Anthony\nis\ncheap", new MagickGeometry(800, 500));
                image.Write(@"c:\users\admin\desktop\font_tile.png");
            }
        }

        [Test()]
        public void GetImageFromPdf()
        {
            MagickNET.SetGhostscriptDirectory(@"C:\Program Files (x86)\gs\gs9.19\bin");

            using (MagickImageCollection collection = new MagickImageCollection())
            {
                MagickReadSettings settings = new MagickReadSettings();
                settings.FrameIndex = 0; // First page
                settings.FrameCount = 2; // Number of pages
                                         // Read only the first page of the pdf file

                collection.Read(@"I:\RocketLauncher\Media\Manuals\Amstrad CPC\1st Division Manager (Europe)\Manual.pdf", settings);

                Console.WriteLine("PDF page count: {0}", collection.Count);

                var appendedImage = collection.AppendHorizontally();

                //Crop pages, for SuperNES
                //appendedImage.Crop()

                appendedImage.Write(@"c:\users\admin\desktop\mergedPdf.png");
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

        [Test()]
        public void Label()
        {
            using (MagickImage img = new MagickImage())
            {
                img.Settings.FontStyle = FontStyleType.Italic;
                img.Settings.FontWeight = FontWeight.Bold;
                img.Settings.FillColor = new MagickColor("purple");
                img.Settings.TextGravity = Gravity.Center;
                img.Read("label:Magick.NET \nis chined", 400, 175);
                img.Trim();
                img.Write(@"c:\users\admin\desktop\font_tile.png");
            }
        }

        private static void ShrinkFit()
        {

        }
    }
}