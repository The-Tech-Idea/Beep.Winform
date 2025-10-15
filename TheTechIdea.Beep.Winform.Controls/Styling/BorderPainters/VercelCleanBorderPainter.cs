using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// VercelClean border painter - Clean 1px border
    /// Vercel UX: Clean, modern minimalism with subtle clarity
    /// </summary>
    public static class VercelCleanBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(234, 234, 234));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 0, 0));
            Color borderColor = baseBorderColor;
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.08f);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.20f);
                    break;
                case ControlState.Selected:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.50f);
                    showRing = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 35);
                    break;
            }

            if (isFocused)
            {
                showRing = true;
            }

            float borderWidth = StyleBorders.GetBorderWidth(style);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color cleanRing = BorderPainterHelpers.WithAlpha(primaryColor, 50);
                BorderPainterHelpers.PaintRing(g, path, cleanRing, 1.5f, 0.8f);
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

