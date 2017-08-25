using Hypermint.Base.Interfaces;
using ImageMagick;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hypermint.Base.Services
{
    public class PdfService : IPdfService
    {
        public int GetNumberOfPdfPages(string pdfFile)
        {
            int pageCount = 0;

            try
            {
                using (var pdfStream = new FileStream(pdfFile, FileMode.Open))
                {
                    var pdfReader = new PdfReader(pdfStream);

                    pageCount = pdfReader.NumberOfPages;

                }

            }
            catch (Exception)
            {
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

                try
                {
                    collection.Read(pdfFile, settings);
                }
                catch (MagickException)
                {

                    return null;
                }


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
