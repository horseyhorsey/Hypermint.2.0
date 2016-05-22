using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Text;

namespace Hs.Hypermint.WheelCreator.Tools
{
    public class WheelGen
    {                        
        public static void GenerateImage(
            string imagePath = "test.png")
        {
            MagickReadSettings settings = new MagickReadSettings();
            // Set define that tells the jpeg coder that the output image will be 32x32
            settings.SetDefine(MagickFormat.Png, "size", "250x450");

            /// if (!File.Exists(imagePath))
            //     File.Create(imagePath);            

            using (var magick = 
                new MagickImage())
            {

                magick.Settings.Font = @"C:\Windows\Fonts\Another_.ttf";
                magick.Settings.FontPointsize = 72;
                magick.Settings.BackgroundColor.A = 0;

                //var gravity = new DrawableGravity(Gravity.South);

                // magick.Draw(drawFont, size, stroke, drawableText);

                magick.Read("label:This is a test game name");

                //magick.Annotate("tetstst",Gravity.Center);

                //var metrics = magick.FontTypeMetrics("Test text");

                // magick.Border(2);
                //magick.Colorize(new MagickColor(System.Drawing.Color.Aqua), new Percentage(100));

                magick.Write("neh.png");
            }
                           
        }

        public static void GenerateLabel(string text = "Super horse Bros.",
            string fontName = @"C:\Windows\Fonts\comicbd.ttf")
        {
            MagickReadSettings settings = new MagickReadSettings();
            settings.SetDefine(MagickFormat.Png, "size", "250x450");          

            using (var magick =
                new MagickImage())
            {
                //magick.Settings.Page = new MagickGeometry(400, 175);
               // magick.RePage();
               
                magick.Draw(new DrawableRectangle
                    (new System.Drawing.Rectangle() { Width = 400, Height = 175 }));
                magick.Colorize(new MagickColor(System.Drawing.Color.Aqua),new Percentage(100));
                // magick.Settings.FillColor = new MagickColor(System.Drawing.Color.DarkCyan);
                //magick.Settings.Font = fontName;
                // magick.Settings.FontPointsize = 72;

                // magick.Settings.StrokeWidth = 2;
                // magick.Settings.StrokeColor = new MagickColor(System.Drawing.Color.Chocolate);

                // magick.Settings.BackgroundColor.A = 0;
                //  magick.Settings.TextGravity = Gravity.Center;
                //  magick.Settings.StrokeAntiAlias = true;


                 var clone = magick.Clone();                                
                // clone.Shadow(0,0,1,new Percentage(100),new MagickColor(System.Drawing.Color.Green));

                 clone.RePage();
                 clone.Trim();
                // var clone2 = clone.Clone();
                //clone2.Trim();


                clone.Write("preview.png");
            }


        }

        public static void GenerateLogo(
            System.Drawing.Color textFillColor,
            System.Drawing.Color borderColor,
            System.Drawing.Color backgroundColor,
            System.Drawing.Color strokeColor,
            System.Drawing.Color shadowColor,
            System.Drawing.Color BackgroundShadow,
            int strokeWidth = 1,
            bool caption = true,
            int borderSize = 4,
            bool border = true,
            bool BackgroundShadowOn = true,
            string textDesc = "Long game adventure test",
            string fontName = @"C:\Windows\Fonts\comicbd.ttf")
        {
            using (var magick = new MagickImage(backgroundColor, 400, 175))
            {
                if (border)
                {
                    using (var borderimage = new MagickImage(new MagickColor(), 400 - 8, 175 - 8))
                    {
                        borderimage.BorderColor = borderColor;
                        borderimage.Border(borderSize);
                        borderimage.Emboss();
                        borderimage.RePage();

                        magick.Composite(borderimage, CompositeOperator.Atop);
                    }

                    if (BackgroundShadowOn)
                    {
                        var clone = AddShadow(magick);
                    }

                }

                var textToWrite = "";
                if (caption)
                    textToWrite = "caption: " + textDesc;
                else
                    textToWrite = "label: " + textDesc;

                using (var textImage = new MagickImage())
                {
                    CreateText(textImage,textToWrite, caption, textFillColor);

                    magick.Composite(textImage,Gravity.Center, CompositeOperator.Over);
                }


                magick.Write("preview.png");
            }
                
        }

        public static MagickImage AddShadow(MagickImage image)
        {
            image.Shadow(1,5,5,(Percentage)50, new MagickColor(System.Drawing.Color.Black));

            return image;
        }

        public static MagickImage CreateText(
            MagickImage image, string text = "Game Name", bool caption = false,
            System.Drawing.Color textFillColor = new System.Drawing.Color(),
            string fontName = @"C:\Windows\Fonts\fuddle.ttf")
        {

                image.Settings.BackgroundColor = new MagickColor();
                image.Settings.FillColor = textFillColor;
                image.Settings.Font = fontName;
                image.Settings.Font = image.Settings.Font.Replace(" ", "-");
            // image.Settings.FontPointsize = 72;
            image.Settings.TextGravity = Gravity.Center;

               // image.Settings.StrokeColor = new MagickColor(System.Drawing.Color.Black);
               // image.Settings.StrokeWidth = 5;                

                image.Read(text);

               // image.Distort(DistortMethod.Arc, true, 10);

                //AddShadow(image);
            
                image.Trim();
                //image.Scale(385, 172);
           

                //var shadow = text.Clone();
                //shadow.Settings.FillColor = textFillColor;
                //shadow.Shadow(MagickColors.Blue);

                return image;
        }

        public static void GradientBackground()
        {
            using (MagickImage image = new MagickImage("gradient:blue-darkblue", 400, 175))
            {
                image.Swirl(45);
                image.Border(2);

                using (var text = new MagickImage("gradient:blue-red"))
                {

                    text.Settings.Font = "Arial";
                    //text.Morphology(MorphologyMethod.EdgeIn, Kernel.Diamond);
                    
                    text.Settings.FontPointsize = 70;

                    //text.Tile(new MagickImage("gradient:blue-red"),CompositeOperator.Overlay);
                                        
                    text.Annotate("Gradient Fun",new MagickGeometry("+10+65"), Gravity.Center);

                    text.Write("text.png");
                    image.Composite(text, CompositeOperator.Over);
                }

                image.Write("preview.png");
            }
        }

        public static void TiledText()
        {
            // Dynamic label height
            using (MagickImageCollection images = new MagickImageCollection())
            {
                // Read the image you want to put a label on.
                MagickImage logo = new MagickImage("radial-gradient:green-yellow",400,175);
                logo.Lower(8);
                images.Add(logo);

                MagickImage label = new MagickImage();
                label.BackgroundColor = new MagickColor();
                label.Settings.FontPointsize = 70;
                label.Read("caption:Magick.NET is coming for you tinight");
                label.Scale(logo.Width, logo.Height);
                // Resize the label to the width of the image and place the text in the center
                label.Extent(logo.Width, logo.Height, Gravity.Center);
                images.Add(label);

                // Append the images
                using (MagickImage output = images.Flatten() )
                {
                    output.Alpha(AlphaOption.Set);
                    output.Transparent(new MagickColor(System.Drawing.Color.White));
                    output.Blur(0, 8);
                    output.Threshold((Percentage)50);
                    ///output.Polaroid("", 2, PixelInterpolateMethod.Bilinear);
                    output.Write("output-b.png");
                }
            }
        }

        public static void NeonSign()
        {
            using (var images = new MagickImage())
            {
                var backgroundImage = new MagickImage(new MagickColor(System.Drawing.Color.Black), 400, 175);
                
                var text = new MagickImage();

                text.Settings.BackgroundColor = new MagickColor();
                text.BorderColor = new MagickColor(System.Drawing.Color.Black);                
                text.Settings.FillColor = System.Drawing.Color.DodgerBlue;
                text.Settings.FontPointsize = 72;
                text.Read("label:I M EXamples");

                var clone = text.Clone();
                clone.Blur(0, 25);
                clone.Level((Percentage)5, (Percentage)75);

                text.Composite(clone,CompositeOperator.Overlay);

                backgroundImage.Composite(text, CompositeOperator.Over);

               // backgroundImage.Composite(clone, CompositeOperator.Over);

                backgroundImage.Write("cloned.png");
                    
            };
        }

        public static void TiledFont()
        {
            using (var tiledImage = new MagickImage("xc:lightblue",400,175))
            {
                var textImage = new MagickImage("pattern:checkerboard",400,175);
                //textImage.Annotate("ddd", Gravity.Center);

                //var text = new DrawableText(0, 0, "DrawnText");
                var circle = new DrawableCircle(tiledImage.Width/2, 0, 5, 5);
                var drawText = new DrawableText(tiledImage.Width / 2, tiledImage.Height / 2, "What");
                var drawTextColor = new DrawableTextDecoration(TextDecoration.Overline);

                textImage.Draw(circle,drawText);
                textImage.Write("textImage.png");

                tiledImage.Write("tileFont.png");
            }


            var settings = new MagickReadSettings();
            settings.Width = 400;
            settings.Height = 175;
            settings.FontPointsize = 72;
        }

        public static void AnnotateWheel(string filePath,
            string wheelText,
            string fontName = @"C:\Windows\Fonts\fuddle.ttf",
            double fontPointSize = 36,
            string pattern = "Bricks",
            System.Drawing.Color fillColor = new System.Drawing.Color(),
            System.Drawing.Color strokeColor = new System.Drawing.Color(),
            double strokeWidth = 1.0,
            System.Drawing.Color shadowColor = new System.Drawing.Color(),
            int imageWidth = 400, int imageHeight = 175)           
        {            
            

            if (!IsFontInstalled(fontName)) return;

            fontName = fontName.Replace(" ", "-");

            using (MagickImage image = new MagickImage(MagickColors.Transparent, imageWidth, imageHeight))
            {                
                image.Settings.Font = fontName;
                image.Settings.StrokeWidth = strokeWidth;
                image.Settings.StrokeColor = strokeColor;
                image.Settings.Density = new Density(72, 72);

                image.Settings.FontPointsize = fontPointSize;

                var newWheelText = wheelText.Replace(" ", "\n");

                try
                {
                    TypeMetric typeMetric = image.FontTypeMetrics(newWheelText);

                    while (typeMetric.TextWidth < image.Width)
                    {
                        image.Settings.FontPointsize++;
                        typeMetric = image.FontTypeMetrics(wheelText);
                    }
                    image.Settings.FontPointsize--;

                    image.Settings.FillPattern = new MagickImage("pattern:" + pattern);

                    image.Annotate(wheelText, new MagickGeometry(imageWidth, imageHeight), Gravity.Center);

                    image.Shadow(shadowColor);

                    using (var image2 = new MagickImage(image))
                    {
                        image2.Colorize(fillColor, new Percentage(30));

                        image2.Settings.StrokeColor = strokeColor;
                        image2.Settings.StrokeWidth = strokeWidth;

                        image2.Write(filePath);
                    }
                }
                catch (MagickException)
                {

                    
                }


            }
        }

        private static bool IsFontInstalled(string name)
        {
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                return fontsCollection.Families
                    .Any(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }
        }

    }
}
