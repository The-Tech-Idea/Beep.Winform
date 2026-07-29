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
        /// Default header height calculation based on font and padding with DPI awareness
        /// </summary>
        public virtual int CalculateHeaderHeight(BeepGridPro grid)
        {
            if (grid?._currentTheme == null) return 26;

            // Ensure fonts are initialized before attempting to create font
            FontManagement.FontListHelper.EnsureFontsLoaded();
            
            // Get DPI scale factor from the grid control
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            
            var font = BeepThemesManager.ToFont(grid._currentTheme.GridHeaderFont) ?? SystemFonts.DefaultFont;
            int basePadding = CalculateHeaderPadding();
            int padding = DpiScalingHelper.ScaleValue(basePadding, dpiScale);
            
            // Use safe font height calculation to avoid errors with some fonts
            int fontHeight = FontManagement.FontListHelper.GetFontHeightSafe(font, grid);
            
            // Scale border/spacing value
            int borderSpacing = DpiScalingHelper.ScaleValue(4, dpiScale);
            
            return fontHeight + (padding * 2) + borderSpacing;
        }

        /// <summary>
        /// Default padding calculation - override for different styles
        /// Returns BASE padding value (will be DPI-scaled in CalculateHeaderHeight)
        /// </summary>
        public virtual int CalculateHeaderPadding()
        {
            return 3; // Base padding (will be DPI-scaled)
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

        /// <summary>
        /// Shared commercial header geometry: caption on the left, a permanently reserved sort
        /// indicator slot, and the column menu button hard right.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The sort slot is reserved for every sortable column whether or not it is currently
        /// sorted. Sizing the slot from the sort state — as the old renderer did
        /// (<c>column.IsSorted ? 16 : 0</c>) — made the caption's width change the instant a user
        /// sorted, so the text visibly jumped on the very click that was meant to sort it. Every
        /// commercial grid reserves the slot up front for exactly this reason.
        /// </para>
        /// <para>
        /// Slots are at least <see cref="MinTouchTarget"/> logical pixels wide so the indicator and
        /// the menu button are comfortably clickable rather than 16px specks, and everything is
        /// derived from <paramref name="dpiScale"/>.
        /// </para>
        /// </remarks>
        public virtual HeaderCellLayout CalculateHeaderCellLayout(Rectangle cellRect, BeepColumnConfig column,
            BeepGridPro grid, float dpiScale)
        {
            int Scale(int v) => Math.Max(1, (int)Math.Round(v * dpiScale));

            int padding = Scale(CalculateHeaderPadding());
            int slot = Scale(MinTouchTarget);
            int iconSize = Scale(HeaderIconSize);

            var layout = new HeaderCellLayout();
            int rightX = cellRect.Right - padding;

            // Column menu (sort + filter + clear) — rightmost, and only when the column offers it.
            bool wantsMenu = column != null && (column.ShowFilterIcon || column.AllowSort);
            if (wantsMenu && rightX - slot > cellRect.Left)
            {
                layout.MenuButtonRect = new Rectangle(
                    rightX - slot,
                    cellRect.Top + (cellRect.Height - slot) / 2,
                    slot, slot);
                rightX -= slot;
            }

            // Sort indicator — reserved for sortable columns regardless of current sort state.
            if (column != null && column.AllowSort && rightX - slot > cellRect.Left)
            {
                layout.SortIndicatorRect = new Rectangle(
                    rightX - slot + (slot - iconSize) / 2,
                    cellRect.Top + (cellRect.Height - iconSize) / 2,
                    iconSize, iconSize);
                rightX -= slot;
            }

            layout.TextRect = new Rectangle(
                cellRect.X + padding,
                cellRect.Y,
                Math.Max(1, rightX - cellRect.X - padding),
                cellRect.Height);

            // Clicking the caption sorts, which is what users try first; the indicator is part of
            // the same target so the whole left region behaves as one control.
            layout.SortHitRect = column != null && column.AllowSort
                ? Rectangle.Union(layout.TextRect,
                    layout.SortIndicatorRect.IsEmpty ? layout.TextRect : layout.SortIndicatorRect)
                : Rectangle.Empty;

            return layout;
        }

        /// <summary>
        /// Paints the column menu button: a funnel, filled when a filter is active on the column.
        /// </summary>
        public virtual void PaintColumnMenuButton(Graphics g, Rectangle rect, bool filterActive, bool isHovered, IBeepTheme? theme)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            if (isHovered)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(30,
                    theme?.GridHeaderForeColor ?? ColorUtils.MapSystemColor(SystemColors.ControlText)));
                g.FillRectangle(hoverBrush, rect);
            }

            // Inset so the glyph is smaller than its (deliberately generous) hit target.
            int inset = Math.Max(2, rect.Width / 4);
            var glyph = Rectangle.Inflate(rect, -inset, -inset);

            Color iconColor = filterActive
                ? (theme?.AccentColor ?? Color.DodgerBlue)
                : (theme?.GridHeaderForeColor ?? ColorUtils.MapSystemColor(SystemColors.ControlText));

            var funnel = new[]
            {
                new Point(glyph.Left, glyph.Top),
                new Point(glyph.Right, glyph.Top),
                new Point(glyph.Left + glyph.Width * 5 / 8, glyph.Top + glyph.Height / 2),
                new Point(glyph.Left + glyph.Width * 5 / 8, glyph.Bottom),
                new Point(glyph.Left + glyph.Width * 3 / 8, glyph.Bottom - glyph.Height / 5),
                new Point(glyph.Left + glyph.Width * 3 / 8, glyph.Top + glyph.Height / 2)
            };

            var smoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (filterActive)
            {
                using var fill = new SolidBrush(iconColor);
                g.FillPolygon(fill, funnel);
            }
            else
            {
                using var pen = new Pen(iconColor, Math.Max(1f, glyph.Width / 10f));
                g.DrawPolygon(pen, funnel);
            }
            g.SmoothingMode = smoothing;
        }

        /// <summary>Logical size of a comfortable header hit target.</summary>
        protected const int MinTouchTarget = 24;

        /// <summary>Logical size of the glyph drawn inside a header slot.</summary>
        protected const int HeaderIconSize = 12;

        #region Common Helper Methods

        /// <summary>
        /// Paint a sort indicator (arrow up/down)
        /// </summary>
        public virtual void PaintSortIndicator(Graphics g, Rectangle rect, SortDirection direction, IBeepTheme? theme)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || direction == SortDirection.None) return;

            var color = theme?.GridHeaderForeColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText);

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
                : (theme?.GridHeaderForeColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText));

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
                ? (theme?.GridHeaderHoverBackColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlLight))
                : (theme?.GridHeaderBackColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.Control));

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

            var textColor = theme?.GridHeaderForeColor ?? TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText);
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
