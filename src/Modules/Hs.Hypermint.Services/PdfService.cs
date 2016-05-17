using ImageMagick;
using Hypermint.Base.Interfaces;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Hs.Hypermint.Services
{
    public class PdfService : IPdfService
    {
        public int GetNumberOfPdfPages(string pdfFile)
        {
            int pageCount = 0;

            using (StreamReader sr = new StreamReader(File.OpenRead(pdfFile)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                pageCount = matches.Count;
            }

            return pageCount;
        }

        public ImageSource GetPage(string ghostScriptPath, string pdfFile, int pageNumber)
        {
            MagickNET.SetGhostscriptDirectory(ghostScriptPath);

            using (MagickImageCollection collection = new MagickImageCollection())
            {
                MagickReadSettings settings = new MagickReadSettings();
                settings.FrameIndex = pageNumber;

                settings.FrameCount = 1;

                collection.Read(pdfFile, settings);

                return SetBitmapImageFromBitmap(collection.ToBitmap(System.Drawing.Imaging.ImageFormat.Jpeg));
            }
        }

        private ImageSource SetBitmapImageFromBitmap(Bitmap source)
        {
            try
            {
                var hBitmap = source.GetHbitmap();

                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, 
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
