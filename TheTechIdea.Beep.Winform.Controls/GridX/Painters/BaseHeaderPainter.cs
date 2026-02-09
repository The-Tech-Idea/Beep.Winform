using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Base class for header painters providing common functionality.
    /// All header painters should inherit from this class.
    /// Uses the same navigationStyle enum as navigation painters for unified styling.
    /// </summary>
    public abstract class BaseHeaderPainter : IPaintGridHeader
    {
        public abstract navigationStyle Style { get; }
        public abstract string StyleName { get; }

        /// <summary>
        /// Default header height calculation based on font and padding
        /// </summary>
        public virtual int CalculateHeaderHeight(BeepGridPro grid)
        {
            if (grid?._currentTheme == null) return 26;

            // Ensure fonts are initialized before attempting to create font
            FontManagement.FontListHelper.EnsureFontsLoaded();
            
            var font = BeepThemesManager.ToFont(grid._currentTheme.GridHeaderFont) ?? SystemFonts.DefaultFont;
            int padding = CalculateHeaderPadding();
            
            // Use safe font height calculation to avoid errors with some fonts
            int fontHeight = FontManagement.FontListHelper.GetFontHeightSafe(font, grid);
            
            return fontHeight + (padding * 2) + 4; // +4 for border/spacing
        }

        /// <summary>
        /// Default padding calculation - override for different styles
        /// </summary>
        public virtual int CalculateHeaderPadding()
        {
            return 3; // Default padding
        }

        /// <summary>
        /// Paint the entire headers area
        /// </summary>
        public abstract void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme);
        
        /// <summary>
        /// Paint a single header cell
        /// </summary>
        public abstract void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column, 
            int columnIndex, BeepGridPro grid, IBeepTheme? theme);

        /// <summary>
        /// Register header hit areas for interactions
        /// </summary>
        public virtual void RegisterHeaderHitAreas(BeepGridPro grid)
        {
            // Default implementation - can be overridden
            // Headers typically don't need special hit areas beyond what the grid tracks
        }

        #region Common Helper Methods

        /// <summary>
        /// Paint a sort indicator (arrow up/down)
        /// </summary>
        public virtual void PaintSortIndicator(Graphics g, Rectangle rect, SortDirection direction, IBeepTheme? theme)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || direction == SortDirection.None) return;

            var color = theme?.GridHeaderForeColor ?? SystemColors.ControlText;

            using (var pen = new Pen(color, 2f))
            {
                int centerX = rect.Left + rect.Width / 2;

                if (direction == SortDirection.Ascending)
                {
                    // Arrow pointing up
                    var points = new Point[]
                    {
                        new Point(centerX, rect.Top + 2),
                        new Point(centerX - 4, rect.Bottom - 2),
                        new Point(centerX + 4, rect.Bottom - 2)
                    };
                    g.DrawPolygon(pen, points);
                }
                else if (direction == SortDirection.Descending)
                {
                    // Arrow pointing down
                    var points = new Point[]
                    {
                        new Point(centerX, rect.Bottom - 2),
                        new Point(centerX - 4, rect.Top + 2),
                        new Point(centerX + 4, rect.Top + 2)
                    };
                    g.DrawPolygon(pen, points);
                }
            }
        }

        /// <summary>
        /// Paint a filter icon (funnel shape)
        /// </summary>
        public virtual void PaintFilterIcon(Graphics g, Rectangle rect, bool active, IBeepTheme? theme)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            Color iconColor = active 
                ? Color.DodgerBlue 
                : (theme?.GridHeaderForeColor ?? SystemColors.ControlText);

            using (var pen = new Pen(iconColor, 2))
            {
                Point[] funnel = new[]
                {
                    new Point(rect.Left + rect.Width / 8, rect.Top + rect.Height / 4),
                    new Point(rect.Right - rect.Width / 8, rect.Top + rect.Height / 4),
                    new Point(rect.Left + rect.Width / 2, rect.Bottom - rect.Height / 8)
                };
                g.DrawLines(pen, funnel);
                g.DrawLine(pen,
                    rect.Left + rect.Width / 2,
                    rect.Bottom - rect.Height / 8,
                    rect.Left + rect.Width / 2,
                    rect.Bottom - rect.Height / 4);
            }
        }

        /// <summary>
        /// Paint header background with optional hover state - uses theme colors directly
        /// </summary>
        public virtual void PaintHeaderBackground(Graphics g, Rectangle rect, bool isHovered, IBeepTheme? theme)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var backColor = isHovered 
                ? (theme?.GridHeaderHoverBackColor ?? SystemColors.ControlLight)
                : (theme?.GridHeaderBackColor ?? SystemColors.Control);

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Paint header text with alignment
        /// </summary>
        public virtual void PaintHeaderText(Graphics g, Rectangle rect, string text, Font font,
            ContentAlignment alignment, IBeepTheme? theme)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || string.IsNullOrEmpty(text)) return;

            var textColor = theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            var flags = GetTextFormatFlags(alignment);

            TextRenderer.DrawText(g, text, font, rect, textColor, flags);
        }

        /// <summary>
        /// Draw a gradient background
        /// </summary>
        protected void DrawGradientBackground(Graphics g, Rectangle rect, Color baseColor, 
            LinearGradientMode mode = LinearGradientMode.Vertical)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var lightColor = Color.FromArgb(
                Math.Min(255, baseColor.R + 20),
                Math.Min(255, baseColor.G + 20),
                Math.Min(255, baseColor.B + 20)
            );

            using (var brush = new LinearGradientBrush(rect, baseColor, lightColor, mode))
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draw elevation shadow
        /// </summary>
        protected void DrawElevationShadow(Graphics g, Rectangle rect, int shadowSize = 2)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), shadowSize))
            {
                g.DrawLine(shadowPen, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom);
            }
        }

        /// <summary>
        /// Convert ContentAlignment to TextFormatFlags
        /// </summary>
        protected TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | 
                                   TextFormatFlags.NoPrefix | 
                                   TextFormatFlags.PreserveGraphicsClipping |
                                   TextFormatFlags.EndEllipsis;

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

        /// <summary>
        /// Create rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0 || bounds.Width < radius * 2 || bounds.Height < radius * 2)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion
    }
}
