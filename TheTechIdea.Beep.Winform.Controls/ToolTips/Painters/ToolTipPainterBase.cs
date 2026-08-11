using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.Spacing;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Base class for tooltip painters providing common functionality
    /// </summary>
    public abstract class ToolTipPainterBase : IToolTipPainter
    {
        #region Constants

        protected const int DefaultPadding = 12;
        protected const int DefaultIconSize = 24;
        protected const int DefaultIconMargin = 8;
        protected const int DefaultTitleSpacing = 6;
        protected const int DefaultArrowSize = 8;
        // A degenerate-sliver guard, not a target width. At 100 it padded every short hint
        // ("Save" measures ~58) out to a fixed slab that bore no relation to its text.
        protected const int DefaultMinWidth = 56;
        protected const int DefaultMaxWidth = 400;

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Derived classes implement their specific painting logic
        /// </summary>
        public abstract void Paint(Graphics g, Rectangle bounds, ToolTipConfig config, 
            ToolTipPlacement placement, IBeepTheme theme);

        public abstract void PaintBackground(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme);

        public abstract void PaintBorder(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme);

        public abstract void PaintShadow(Graphics g, Rectangle bounds, 
            ToolTipConfig config);

        public abstract void PaintArrow(Graphics g, Point position, 
            ToolTipPlacement placement, ToolTipConfig config, IBeepTheme theme);

        public abstract void PaintContent(Graphics g, Rectangle bounds, 
            ToolTipConfig config, IBeepTheme theme);

        #endregion

        #region Size Calculation

        /// <summary>
        /// Calculate tooltip size based on content
        /// </summary>
        public virtual Size CalculateSize(Graphics g, ToolTipConfig config)
        {
            int padX = GetPaddingX(config);
            int padY = GetPaddingY(config);
            int width = padX * 2;
            int height = padY * 2;

            // Account for icon
            if (HasIcon(config))
            {
                width += DefaultIconSize + DefaultIconMargin;
                height = Math.Max(height, DefaultIconSize + padY * 2);
            }

            // Measure exactly the sections the variant will actually draw. Measuring a title that
            // PaintContent then suppresses (Simple, Shortcut) would leave a band of empty space,
            // and omitting one it does draw would clip it.
            var sections = ToolTipSectionPlan.For(config);

            int titleHeight = 0;
            int contentWidth = 0;

            if (sections.ShowTitle && !string.IsNullOrEmpty(config.Title))
            {
                var titleFont = GetTitleFont(config);
                var titleSize = TextUtils.MeasureText(g,config.Title, titleFont, DefaultMaxWidth - width);
                titleHeight = (int)Math.Ceiling(titleSize.Height);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(titleSize.Width));
                height += titleHeight + DefaultTitleSpacing;

                if (sections.ShowHeaderDivider) height += DefaultTitleSpacing + 1;
            }

            // Measure text
            if (!string.IsNullOrEmpty(config.Text))
            {
                var textFont = GetTextFont(config);
                var textSize = TextUtils.MeasureText(g,config.Text, textFont, DefaultMaxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(textSize.Width));
                height += (int)Math.Ceiling(textSize.Height);
            }

            if (sections.ShowShortcuts && config.Shortcuts != null && config.Shortcuts.Count > 0)
            {
                var badge = Helpers.ShortcutBadgePainter.MeasureShortcuts(g, config.Shortcuts);
                if (!badge.IsEmpty)
                {
                    if (sections.ShortcutsInline)
                    {
                        // Same row as the label: widen, do not grow taller.
                        contentWidth += badge.Width + DefaultIconMargin;
                        height = Math.Max(height, badge.Height + padY * 2);
                    }
                    else
                    {
                        contentWidth = Math.Max(contentWidth, badge.Width);
                        height += badge.Height + DefaultTitleSpacing;
                    }
                }
            }

            width += contentWidth;

            // Account for BeepControlStyle BorderThickness
            var beepStyle = ToolTipStyleAdapter.GetBeepControlStyle(config);
            int borderWidth = (int)Math.Ceiling(StyleBorders.GetBorderWidth(beepStyle));
            width += borderWidth * 2;  // Left + Right
            height += borderWidth * 2; // Top + Bottom

            // NO shadow allowance. The card is painted edge to edge across the whole bounds
            // (PaintBackground fills the full rectangle), so reserving blur+offset here does not
            // create room OUTSIDE the card - it just inflates the card. It cost ~20px in both
            // directions on every tooltip, which is why a one-word hint like "Save" came out
            // 150x61 against a native tooltip's ~22px height. If a real drop shadow is
            // reintroduced, the allowance and the painting have to come back together.

            // Apply constraints
            width = Math.Max(DefaultMinWidth, Math.Min(width, DefaultMaxWidth));

            if (config.MaxSize.HasValue)
            {
                width = Math.Min(width, config.MaxSize.Value.Width);
                height = Math.Min(height, config.MaxSize.Value.Height);
            }

            return new Size(width, height);
        }

        #endregion

        #region Helper Methods - Fonts

        /// <summary>
        /// Resolves the title font from config override → StyleTypography for the active style.
        /// Uses BeepFontManager.GetCachedFont to avoid per-paint allocations.
        /// </summary>
        protected virtual Font GetTitleFont(ToolTipConfig config)
        {
            if (config.Font != null)
            {
                return BeepFontManager.GetCachedFont(
                    config.Font.FontFamily.Name, config.Font.Size + 2, FontStyle.Bold)
                    ?? config.Font;
            }

            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            string family = ResolvePrimaryFontFamily(style);
            // Title is ~1.5 pt larger than the style's body size, always bold
            float size = StyleTypography.GetFontSize(style) - 2.5f;
            return BeepFontManager.GetCachedFont(family, size, FontStyle.Bold)
                   ?? new Font("Segoe UI", 10, FontStyle.Bold);
        }

        /// <summary>
        /// Resolves the body text font from config override → StyleTypography.
        /// </summary>
        protected virtual Font GetTextFont(ToolTipConfig config)
        {
            if (config.Font != null)
                return config.Font;

            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            string family = ResolvePrimaryFontFamily(style);
            // Tooltip body is ~1 pt smaller than the style's standard body
            float size = StyleTypography.GetFontSize(style) - 4f;
            return BeepFontManager.GetCachedFont(family, size, FontStyle.Regular)
                   ?? new Font("Segoe UI", 9, FontStyle.Regular);
        }

        /// <summary>
        /// Extracts the first (primary) font family name from a comma-separated list.
        /// </summary>
        private static string ResolvePrimaryFontFamily(BeepControlStyle style)
        {
            string families = StyleTypography.GetFontFamily(style);
            int comma = families.IndexOf(',');
            return comma > 0 ? families.Substring(0, comma).Trim() : families.Trim();
        }

        #endregion

        #region Helper Methods - Paths

        /// <summary>
        /// Create rounded rectangle path for tooltip
        /// </summary>
        protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            // Top-left
            path.AddArc(arc, 180, 90);
            
            // Top-right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom-right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom-left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Create arrow path based on placement
        /// </summary>
        protected GraphicsPath CreateArrowPath(Point position, ToolTipPlacement placement, int arrowSize)
        {
            return ToolTipHelpers.CreateArrowPath(position, placement, arrowSize);
        }

        #endregion

        #region Helper Methods - Content

        /// <summary>
        /// Check if tooltip has an icon
        /// </summary>
        protected bool HasIcon(ToolTipConfig config)
        {
            return config.Icon != null || 
                   !string.IsNullOrEmpty(config.IconPath) || 
                   !string.IsNullOrEmpty(config.ImagePath);
        }

        /// <summary>
        /// Get content rectangle (excluding padding).
        /// Resolves padding from StyleSpacing when a config is available.
        /// </summary>
        protected Rectangle GetContentRectangle(Rectangle bounds)
        {
            return GetContentRectangle(bounds, null);
        }

        /// <summary>
        /// Get content rectangle with style-aware padding.
        /// </summary>
        protected Rectangle GetContentRectangle(Rectangle bounds, ToolTipConfig config)
        {
            int padX = GetPaddingX(config);
            int padY = GetPaddingY(config);
            return new Rectangle(
                bounds.X + padX,
                bounds.Y + padY,
                bounds.Width - padX * 2,
                bounds.Height - padY * 2
            );
        }

        /// <summary>
        /// Horizontal padding. THE single source for both measurement and painting.
        /// </summary>
        /// <remarks>
        /// CalculateSize used the constant <see cref="DefaultPadding"/> while this rectangle used
        /// the style's own spacing, so the size reserved and the space actually used disagreed for
        /// every style whose padding is not 12 - the tooltip was measured against one box and drawn
        /// into another, which is why the frame never matched its text.
        /// </remarks>
        protected int GetPaddingX(ToolTipConfig config)
        {
            if (config == null) return DefaultPadding;
            var style = ToolTipStyleAdapter.GetBeepControlStyle(config);
            return Math.Max(8, StyleSpacing.GetPadding(style));
        }

        /// <summary>
        /// Vertical padding: deliberately tighter than horizontal. A tooltip is a single band of
        /// text, and equal padding on all four sides made one-line hints look like dialogs.
        /// </summary>
        protected int GetPaddingY(ToolTipConfig config)
        {
            return Math.Max(5, GetPaddingX(config) - 5);
        }

        #endregion

        #region Helper Methods - Colors


        /// <summary>
        /// Adjust color brightness
        /// </summary>
        protected Color AdjustBrightness(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R * factor)),
                Math.Min(255, (int)(color.G * factor)),
                Math.Min(255, (int)(color.B * factor))
            );
        }

        #endregion

        #region Cache Management (B5)

        /// <summary>
        /// B5: Discard any cached GraphicsPaths, brushes, or layout data.
        /// Override in painters that maintain a cache. Called from
        /// theme-changed, DPI-changed, and factory-pool clear events.
        /// </summary>
        public virtual void InvalidateCache() { }

        #endregion
    }
}
