using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;

namespace WindowsBar.Helpers;

public static class WallpaperColorSampler
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);
    private const uint SPI_GETDESKWALLPAPER = 0x0073;

    public static (System.Windows.Media.Color primary, System.Windows.Media.Color secondary) SampleWallpaperColors()
    {
        try
        {
            var sb = new System.Text.StringBuilder(260);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, 260, sb, 0);
            var path = sb.ToString();

            if (!File.Exists(path))
                return (DefaultPrimary, DefaultSecondary);

            using var bmp = new Bitmap(path);

            // Sample the top strip of the wallpaper (where bar sits)
            var topColors = SampleRegion(bmp, 0, 0, bmp.Width, Math.Min(50, bmp.Height));
            var midColors = SampleRegion(bmp, 0, bmp.Height / 3, bmp.Width, bmp.Height / 3);

            var primary = AverageColor(topColors);
            var secondary = AverageColor(midColors);

            return (ToMediaColor(primary), ToMediaColor(secondary));
        }
        catch
        {
            return (DefaultPrimary, DefaultSecondary);
        }
    }

    private static List<System.Drawing.Color> SampleRegion(Bitmap bmp, int x, int y, int w, int h)
    {
        var colors = new List<System.Drawing.Color>();
        int stepX = Math.Max(1, w / 40);
        int stepY = Math.Max(1, h / 10);

        for (int px = x; px < x + w && px < bmp.Width; px += stepX)
            for (int py = y; py < y + h && py < bmp.Height; py += stepY)
                colors.Add(bmp.GetPixel(px, py));

        return colors;
    }

    private static System.Drawing.Color AverageColor(List<System.Drawing.Color> colors)
    {
        if (colors.Count == 0) return System.Drawing.Color.FromArgb(20, 20, 40);
        int r = 0, g = 0, b = 0;
        foreach (var c in colors) { r += c.R; g += c.G; b += c.B; }
        return System.Drawing.Color.FromArgb(r / colors.Count, g / colors.Count, b / colors.Count);
    }

    private static System.Windows.Media.Color ToMediaColor(System.Drawing.Color c)
        => System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);

    public static System.Windows.Media.Color Lighten(System.Windows.Media.Color c, float factor)
    {
        return System.Windows.Media.Color.FromArgb(c.A,
            (byte)Math.Min(255, c.R + (255 - c.R) * factor),
            (byte)Math.Min(255, c.G + (255 - c.G) * factor),
            (byte)Math.Min(255, c.B + (255 - c.B) * factor));
    }

    public static System.Windows.Media.Color WithAlpha(System.Windows.Media.Color c, byte alpha)
        => System.Windows.Media.Color.FromArgb(alpha, c.R, c.G, c.B);

    private static readonly System.Windows.Media.Color DefaultPrimary =
        System.Windows.Media.Color.FromArgb(255, 15, 12, 35);
    private static readonly System.Windows.Media.Color DefaultSecondary =
        System.Windows.Media.Color.FromArgb(255, 40, 20, 60);
}
