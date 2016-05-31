using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hs.Hypermint.MediaPane.Views
{
    /// <summary>
    /// Interaction logic for BezelEditView
    /// </summary>
    public partial class BezelEditView : UserControl
    {
        public BezelEditView()
        {
            InitializeComponent();

        }

        private void bezelImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = bezelImage.Source;
            
            if (img == null) return;

            Point pos = Mouse.GetPosition(bezelImage);

            var mousePos = e.GetPosition(bezelImage);

            var pos2 = ImgControlCoordsToPixelCoords(pos, bezelImage.ActualWidth, bezelImage.ActualHeight);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                LeftClickX.Value = Math.Round(pos2.X);
                LeftClickY.Value = Math.Round(pos2.Y);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                RightClickX.Value = Math.Round(pos2.X);
                RightClickY.Value = Math.Round(pos2.Y);
            }
        }        

        Point ImgControlCoordsToPixelCoords(Point locInCtrl, double imgCtrlActualWidth, double imgCtrlActualHeight)
        {
            if (bezelImage.Stretch == Stretch.None)
                return locInCtrl;

            Size renderSize = new Size(imgCtrlActualWidth, imgCtrlActualHeight);
            Size sourceSize = new Size()
            { Width = ((BitmapSource)bezelImage.Source).PixelWidth,
                Height = ((BitmapSource)bezelImage.Source).PixelHeight };

            double xZoom = renderSize.Width / sourceSize.Width;
            double yZoom = renderSize.Height / sourceSize.Height;

            if (bezelImage.Stretch == Stretch.Fill)
                return new Point(locInCtrl.X / xZoom, locInCtrl.Y / yZoom);

            double zoom;
            if (bezelImage.Stretch == Stretch.Uniform)
                zoom = Math.Min(xZoom, yZoom);
            else // (imageCtrl.Stretch == Stretch.UniformToFill)
                zoom = Math.Max(xZoom, yZoom);

            return new Point(locInCtrl.X / zoom, locInCtrl.Y / zoom);
        }



        public static class MouseUtilities
        {
            public static Point CorrectGetPosition(Visual relativeTo)
            {
                Win32Point w32Mouse = new Win32Point();
                GetCursorPos(ref w32Mouse);
                return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct Win32Point
            {
                public int X;
                public int Y;
            };

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetCursorPos(ref Win32Point pt);
        }

    }
}
