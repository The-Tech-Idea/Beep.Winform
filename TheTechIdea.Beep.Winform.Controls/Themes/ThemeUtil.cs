using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    internal static class ThemeUtil
    {
        public static Color Blend(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);
            byte Lerp(byte x, byte y) => (byte)Math.Round(x + (y - x) * t);
            return Color.FromArgb(255, Lerp(a.R, b.R), Lerp(a.G, b.G), Lerp(a.B, b.B));
        }
        public static Color Darken(Color c, double by) => Blend(c, Color.Black, by);
        public static Color Lighten(Color c, double by) => Blend(c, Color.White, by);
    }
}

