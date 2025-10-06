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
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Vercel colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(234, 234, 234));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 0, 0));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;

            // Vercel UX: Clean, modern minimalism with subtle clarity
            switch (state)
            {
                case ControlState.Hovered:
                    // Vercel: Clean darkening (8% blend for modern hover)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.08f);
                    break;
                    
                case ControlState.Pressed:
                    // Vercel: Cleaner darkening (20% blend for clear press)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.20f);
                    break;
                    
                case ControlState.Selected:
                    // Vercel: Dark border (50% blend for modern selection)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.50f);
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Vercel: Light disabled (35 alpha for clean disabled)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 35);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Vercel: Add clean focus ring
            if (showRing)
            {
                Color cleanRing = BorderPainterHelpers.WithAlpha(primaryColor, 50); // Clean Vercel ring
                BorderPainterHelpers.PaintRing(g, path, cleanRing, 1.5f, 0.8f);
            }
        }
    }
}

