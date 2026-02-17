using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Utility and helper methods for rendering
    /// </summary>
    internal partial class GridRenderHelper
    {
        // Cached fallback font for cell rendering
        private static Font? _cellFallbackFont;
        private static Font CellFallbackFont => _cellFallbackFont ??= SystemFonts.DefaultFont;

        /// <summary>
        /// Safely resolves the grid cell font from the current theme with fallback
        /// </summary>
        private Font GetSafeCellFont()
        {
            try
            {
                var theme = _grid?._currentTheme ?? BeepThemesManager.CurrentTheme;
                if (theme?.GridCellFont != null)
                {
                    var font = BeepThemesManager.ToFont(theme.GridCellFont);
                    if (font != null) return font;
                }
            }
            catch { }
            return CellFallbackFont;
        }

        /// <summary>
        /// Safely resolves the grid header font from the current theme with fallback
        /// </summary>
        private Font GetSafeHeaderFont()
        {
            try
            {
                var theme = _grid?._currentTheme ?? BeepThemesManager.CurrentTheme;
                if (theme?.GridHeaderFont != null)
                {
                    var font = BeepThemesManager.ToFont(theme.GridHeaderFont);
                    if (font != null) return font;
                }
            }
            catch { }
            return CellFallbackFont;
        }

        /// <summary>
        /// Resolves the focus indicator color from theme
        /// </summary>
        private Color ResolveThemeFocusColor()
        {
            var theme = Theme;
            if (theme != null && theme.FocusIndicatorColor != Color.Empty)
                return theme.FocusIndicatorColor;

            return _grid?.FocusIndicatorColor ?? SystemColors.Highlight;
        }

        /// <summary>
        /// Resolves the focused row background color
        /// </summary>
        private Color ResolveFocusedRowBackColor(Color hoverBackColor, Color focusColor)
        {
            if (FocusedRowBackColor != Color.Empty)
                return FocusedRowBackColor;

            return BlendColors(hoverBackColor, focusColor, 0.22f);
        }

        /// <summary>
        /// Blends two colors together
        /// </summary>
        private static Color BlendColors(Color baseColor, Color blendColor, float blendFactor)
        {
            blendFactor = Math.Max(0f, Math.Min(1f, blendFactor));
            int r = (int)(baseColor.R + ((blendColor.R - baseColor.R) * blendFactor));
            int g = (int)(baseColor.G + ((blendColor.G - baseColor.G) * blendFactor));
            int b = (int)(baseColor.B + ((blendColor.B - baseColor.B) * blendFactor));
            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// Gets text format flags for the given alignment
        /// </summary>
        private static TextFormatFlags GetTextFormatFlagsForAlignment(ContentAlignment alignment, bool endEllipsis)
        {
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | 
                TextFormatFlags.PreserveGraphicsClipping;
            if (endEllipsis) flags |= TextFormatFlags.EndEllipsis;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    break;
            }
            return flags;
        }
    }
}
