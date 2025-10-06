using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
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
        protected const int DefaultMinWidth = 100;
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
            int width = DefaultPadding * 2;
            int height = DefaultPadding * 2;

            // Account for icon
            if (HasIcon(config))
            {
                width += DefaultIconSize + DefaultIconMargin;
                height = Math.Max(height, DefaultIconSize + DefaultPadding * 2);
            }

            // Measure title
            int titleHeight = 0;
            int contentWidth = 0;

            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleFont = GetTitleFont(config);
                var titleSize = g.MeasureString(config.Title, titleFont, DefaultMaxWidth - width);
                titleHeight = (int)Math.Ceiling(titleSize.Height);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(titleSize.Width));
                height += titleHeight + DefaultTitleSpacing;
            }

            // Measure text
            if (!string.IsNullOrEmpty(config.Text))
            {
                var textFont = GetTextFont(config);
                var textSize = g.MeasureString(config.Text, textFont, DefaultMaxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(textSize.Width));
                height += (int)Math.Ceiling(textSize.Height);
            }

            width += contentWidth;

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

        protected virtual Font GetTitleFont(ToolTipConfig config)
        {
            if (config.Font != null)
            {
                return new Font(config.Font.FontFamily, config.Font.Size + 2, FontStyle.Bold);
            }
            return new Font("Segoe UI", 10, FontStyle.Bold);
        }

        protected virtual Font GetTextFont(ToolTipConfig config)
        {
            return config.Font ?? new Font("Segoe UI", 9, FontStyle.Regular);
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
        /// Get content rectangle (excluding padding)
        /// </summary>
        protected Rectangle GetContentRectangle(Rectangle bounds)
        {
            return new Rectangle(
                bounds.X + DefaultPadding,
                bounds.Y + DefaultPadding,
                bounds.Width - DefaultPadding * 2,
                bounds.Height - DefaultPadding * 2
            );
        }

        #endregion

        #region Helper Methods - Colors

        /// <summary>
        /// Get semantic color based on ToolTipType
        /// </summary>
        protected Color GetSemanticColor(ToolTipType type, IBeepTheme theme)
        {
            if (theme == null)
                return Color.Gray;

            return type switch
            {
                ToolTipType.Success => theme.SuccessColor,
                ToolTipType.Warning => theme.WarningColor,
                ToolTipType.Error => theme.ErrorColor,
                ToolTipType.Info => theme.AccentColor,
                ToolTipType.Primary => theme.PrimaryColor,
                ToolTipType.Secondary => theme.SecondaryColor,
                ToolTipType.Accent => theme.AccentColor,
                _ => theme.SurfaceColor
            };
        }

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
    }
}
