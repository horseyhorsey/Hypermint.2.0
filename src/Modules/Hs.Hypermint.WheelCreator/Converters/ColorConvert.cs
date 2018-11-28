namespace Hs.Hypermint.WheelCreator.Converters
{
    public class ColorConvert
    {
        public static System.Drawing.Color ColorFromMediaColor(System.Windows.Media.Color clr)
            => System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
    }
}
