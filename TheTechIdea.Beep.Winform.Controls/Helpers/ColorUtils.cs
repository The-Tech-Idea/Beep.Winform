using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class ColorUtils
    {
        /// <summary>
        /// Ensures a readable foreground color by inverting it if it is too similar to the background color.
        /// </summary>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="forColor">The preferred foreground color.</param>
        /// <returns>A readable foreground color.</returns>
        public static Color GetForColor(Color backgroundColor, Color forColor)
        {
            // Check if the colors are too similar
            if (AreColorsSimilar(backgroundColor, forColor))
            {
                return InvertColor(forColor); // Invert the foreground color
            }

            return forColor; // Use the preferred color if they are not similar
        }

        /// <summary>
        /// Determines if two colors are too similar based on a tolerance level.
        /// </summary>
        public static bool AreColorsSimilar(Color color1, Color color2, int tolerance = 50)
        {
            int diffR = Math.Abs(color1.R - color2.R);
            int diffG = Math.Abs(color1.G - color2.G);
            int diffB = Math.Abs(color1.B - color2.B);

            return (diffR <= tolerance && diffG <= tolerance && diffB <= tolerance);
        }
        public static bool AreColorsSimilar(Color color1, Color color2, int toleranceR, int toleranceG, int toleranceB)
        {
            int diffR = Math.Abs(color1.R - color2.R);
            int diffG = Math.Abs(color1.G - color2.G);
            int diffB = Math.Abs(color1.B - color2.B);
            return (diffR <= toleranceR && diffG <= toleranceG && diffB <= toleranceB);
        }
        public static bool AreColorsSimilar(Color color1, Color color2, int toleranceR, int toleranceG, int toleranceB, int toleranceA)
        {
            int diffR = Math.Abs(color1.R - color2.R);
            int diffG = Math.Abs(color1.G - color2.G);
            int diffB = Math.Abs(color1.B - color2.B);
            int diffA = Math.Abs(color1.A - color2.A);
            return (diffR <= toleranceR && diffG <= toleranceG && diffB <= toleranceB && diffA <= toleranceA);
        }

        /// <summary>
        /// Inverts the given color.
        /// </summary>
        private static Color InvertColor(Color color)
        {
            return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }
        public static Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        public static Color GetRandomColor(int seed)
        {
            Random random = new Random(seed);
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }
        public static Color GetRandomColor(int seed, int alpha)
        {
            Random random = new Random(seed);
            return Color.FromArgb(alpha, random.Next(256), random.Next(256), random.Next(256));
        }
        public static Color GetRandomColor(int seed, int alpha, int red, int green, int blue)
        {
            Random random = new Random(seed);
            return Color.FromArgb(alpha, red, green, blue);
        }
        public static Color GetRandomColor(int alpha, int red, int green, int blue)
        {
            Random random = new Random();
            return Color.FromArgb(alpha, red, green, blue);
        }
        public static Color GetRandomColor(int seed, int alpha, int red, int green, int blue, int alpha2, int red2, int green2, int blue2)
        {
            Random random = new Random(seed);
            return Color.FromArgb(random.Next(alpha, alpha2), random.Next(red, red2), random.Next(green, green2), random.Next(blue, blue2));
        }
        public static Color GetRandomColor(int alpha, int red, int green, int blue, int alpha2, int red2, int green2, int blue2)
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(alpha, alpha2), random.Next(red, red2), random.Next(green, green2), random.Next(blue, blue2));
        }
        public static Color GetRandomColor(int seed, int alpha, int red, int green, int blue, int alpha2, int red2, int green2, int blue2, int alpha3, int red3, int green3, int blue3)
        {
            Random random = new Random(seed);
            return Color.FromArgb(random.Next(alpha, alpha2), random.Next(red, red2), random.Next(green, green2), random.Next(blue, blue2));
        }
        public static Color GetRandomColor(int alpha, int red, int green, int blue, int alpha2, int red2, int green2, int blue2, int alpha3, int red3, int green3, int blue3)
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(alpha, alpha2), random.Next(red, red2), random.Next(green, green2), random.Next(blue, blue2));
        }
        // create  a darker color  for _currentTheme.LabelForeColor
        public static Color GetDarkerColor(Color color, float factor)
        {
            int r = (int)(color.R * factor);
            int g = (int)(color.G * factor);
            int b = (int)(color.B * factor);
            return Color.FromArgb(color.A, Math.Max(0, r), Math.Max(0, g), Math.Max(0, b));
        }

        // create a lighter color for _currentTheme.LabelForeColor
        public static Color GetLighterColor(Color color, float factor)
        {
            int r = (int)(color.R + (255 - color.R) * factor);
            int g = (int)(color.G + (255 - color.G) * factor);
            int b = (int)(color.B + (255 - color.B) * factor);
            return Color.FromArgb(color.A, Math.Min(255, r), Math.Min(255, g), Math.Min(255, b));
        }
        public static Color GetDarkerColor(Color color, int adjusment)
        {
            int r = Math.Max(0, color.R - adjusment);
            int g = Math.Max(0, color.G - adjusment);
            int b = Math.Max(0, color.B - adjusment);
            return Color.FromArgb(color.A, r, g, b);
        }
        public static Color GetLighterColor(Color color, int adjusment)
        {
            int r = Math.Min(255, color.R + adjusment);
            int g = Math.Min(255, color.G + adjusment);
            int b = Math.Min(255, color.B + adjusment);
            return Color.FromArgb(color.A, r, g, b);
        }

        public static Color ChangeColorBrightness(Color foreColor, float v)
        {
            float r = foreColor.R;
            float g = foreColor.G;
            float b = foreColor.B;
            if (v < 0)
            {
                v = 1 + v;
                r *= v;
                g *= v;
                b *= v;
            }
            else
            {
                r = (255 - r) * v + r;
                g = (255 - g) * v + g;
                b = (255 - b) * v + b;
            }
            return Color.FromArgb(foreColor.A, (int)r, (int)g, (int)b);

        }

        public static Color Lighten(Color breezeBlue, float v)
        {
            // Clamp to [0,1] and use positive value to lighten via ChangeColorBrightness
            float amount = Math.Clamp(v, 0f, 1f);
            return ChangeColorBrightness(breezeBlue, amount);
        }

        public static Color Darken(Color breezeBlue, float v)
        {
            float amount = -Math.Clamp(v, 0f, 1f);
            return ChangeColorBrightness(breezeBlue, amount);
        }

        public static Color ShiftLuminance(Color color, float amount)
        {
            ColorToHsl(color, out float h, out float s, out float l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        private static void ColorToHsl(Color c, out float h, out float s, out float l)
        {
            float r = c.R / 255f, g = c.G / 255f, b = c.B / 255f;
            float min = Math.Min(r, Math.Min(g, b)), max = Math.Max(r, Math.Max(g, b));
            l = (max + min) / 2f;
            if (max == min) { h = s = 0; }
            else
            {
                float d = max - min;
                s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
                if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
                else if (max == g) h = (b - r) / d + 2;
                else h = (r - g) / d + 4;
                h /= 6f;
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
                float p = 2f * l - q;
                r = Hue2Rgb(p, q, h + 1f / 3f);
                g = Hue2Rgb(p, q, h);
                b = Hue2Rgb(p, q, h - 1f / 3f);
            }
            return Color.FromArgb(255, (int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }

        private static float Hue2Rgb(float p, float q, float t)
        {
            if (t < 0) t += 1f; if (t > 1) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        internal static Color Desaturate(Color warmGray, float v)
        {
            throw new NotImplementedException();
        }

        // Example usage
    }

}
