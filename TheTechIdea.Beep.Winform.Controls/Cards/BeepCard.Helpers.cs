using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        private static Color ShiftLuminance(Color color, float amount)
        {
            float h, s, l;
            ColorToHsl(color, out h, out s, out l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        private static void ColorToHsl(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float delta = max - min;
            l = (max + min) / 2.0f;
            if (delta == 0) { h = 0; s = 0; }
            else
            {
                s = l < 0.5f ? delta / (max + min) : delta / (2.0f - max - min);
                if (r == max) h = (g - b) / delta;
                else if (g == max) h = 2.0f + (b - r) / delta;
                else h = 4.0f + (r - g) / delta;
                h /= 6.0f;
                if (h < 0) h += 1.0f;
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1.0f + s) : l + s - l * s;
                float p = 2.0f * l - q;
                r = HueToRgb(p, q, h + 1.0f / 3.0f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0f / 3.0f);
            }
            return Color.FromArgb(255, Math.Max(0, Math.Min(255, (int)(r * 255))), Math.Max(0, Math.Min(255, (int)(g * 255))), Math.Max(0, Math.Min(255, (int)(b * 255))));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1.0f;
            if (t > 1) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }
    }
}
