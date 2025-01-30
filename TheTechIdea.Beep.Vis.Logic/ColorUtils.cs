using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Logic
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

        // Example usage
    }

}
