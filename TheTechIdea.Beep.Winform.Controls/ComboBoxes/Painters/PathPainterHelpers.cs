using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    internal static class PathPainterHelpers
    {
        /// <summary>
        /// Returns <paramref name="color"/> with the specified alpha,
        /// or <see cref="Color.Empty"/> when the input is empty/transparent.
        /// </summary>
        public static Color WithAlphaIfNotEmpty(Color color, int alpha)
        {
            if (color.IsEmpty || color == Color.Transparent) return Color.Empty;
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}