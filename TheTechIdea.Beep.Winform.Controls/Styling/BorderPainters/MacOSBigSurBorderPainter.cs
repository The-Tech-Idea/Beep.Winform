using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// MacOSBigSur border painter - Clean 1px border with system colors
    /// macOS UX: Refined vibrancy with subtle state transitions
    /// </summary>
    public static class MacOSBigSurBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(209, 209, 214));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 122, 255));
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.10f);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.25f);
                    break;
                case ControlState.Selected:
                    borderColor = BorderPainterHelpers.WithAlpha(accentColor, 160);
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            using (var pen = PaintersFactory.GetPen(borderColor, borderWidth))
            {
                pen.Alignment = PenAlignment.Inset; // match form non-client painting alignment
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }

            if (isFocused)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(accentColor, 80);
                BorderPainterHelpers.PaintRing(g, path, focusRing, 2.0f, 1.0f);
            }

            // Return the area inside the border using shape-aware inset by half width
            return path.CreateInsetPath(borderWidth / 2f);
        }
    }
}

