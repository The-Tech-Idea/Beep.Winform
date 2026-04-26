using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Labels.Helpers
{
    internal static class BeepLabelAccessibilityHelpers
    {
        public static void ApplyAccessibility(BeepLabel owner, string subHeaderText, bool hasImage)
        {
            if (owner == null)
            {
                return;
            }

            string header = owner.Text ?? string.Empty;
            owner.AccessibleRole = AccessibleRole.StaticText;
            owner.AccessibleName = string.IsNullOrWhiteSpace(header) ? "Label" : header;

            string description = string.IsNullOrWhiteSpace(subHeaderText)
                ? header
                : $"{header}. {subHeaderText}";
            if (hasImage)
            {
                description = $"{description}. Includes icon.";
            }

            owner.AccessibleDescription = description;
        }

        public static Color EnsureContrast(Color foreColor, Color backColor)
        {
            double contrast = CalculateContrast(foreColor, backColor);
            if (contrast >= 4.5d)
            {
                return foreColor;
            }

            bool backIsLight = CalculateLuminance(backColor) > 0.5d;
            Color target = backIsLight ? Color.Black : Color.White;

            for (int step = 1; step <= 20; step++)
            {
                float t = step / 20f;
                Color adjusted = Color.FromArgb(
                    foreColor.A,
                    (int)(foreColor.R + (target.R - foreColor.R) * t),
                    (int)(foreColor.G + (target.G - foreColor.G) * t),
                    (int)(foreColor.B + (target.B - foreColor.B) * t));

                if (CalculateContrast(adjusted, backColor) >= 4.5d)
                {
                    return adjusted;
                }
            }

            return target;
        }

        private static double CalculateContrast(Color colorA, Color colorB)
        {
            double l1 = CalculateLuminance(colorA);
            double l2 = CalculateLuminance(colorB);
            double lighter = System.Math.Max(l1, l2);
            double darker = System.Math.Min(l1, l2);
            return (lighter + 0.05d) / (darker + 0.05d);
        }

        private static double CalculateLuminance(Color color)
        {
            static double ToLinear(byte component)
            {
                double c = component / 255d;
                return c <= 0.03928d ? c / 12.92d : System.Math.Pow((c + 0.055d) / 1.055d, 2.4d);
            }

            return 0.2126d * ToLinear(color.R) + 0.7152d * ToLinear(color.G) + 0.0722d * ToLinear(color.B);
        }
    }
}
