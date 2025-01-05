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
        private static bool AreColorsSimilar(Color color1, Color color2, int tolerance = 50)
        {
            int diffR = Math.Abs(color1.R - color2.R);
            int diffG = Math.Abs(color1.G - color2.G);
            int diffB = Math.Abs(color1.B - color2.B);

            return (diffR <= tolerance && diffG <= tolerance && diffB <= tolerance);
        }

        /// <summary>
        /// Inverts the given color.
        /// </summary>
        private static Color InvertColor(Color color)
        {
            return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }

        // Example usage
    }

}
