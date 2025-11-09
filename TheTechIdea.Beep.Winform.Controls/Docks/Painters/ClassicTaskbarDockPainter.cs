using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Classic Windows taskbar-style dock painter (Windows 7/10 style)
    /// Features:
    /// - Flat design with sharp edges
    /// - System accent color integration
    /// - Rectangular items with minimal padding
    /// - Vertical line indicators for running apps
    /// - No rounded corners (or minimal)
    /// </summary>
    public class ClassicTaskbarDockPainter : DockPainterBase
    {
        private const int IndicatorWidth = 3;
        private const int ItemPadding = 4;

        /// <summary>
        /// Paints the dock background
        /// </summary>
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            // Windows 10 style - minimal or no rounding
            int cornerRadius = Math.Min(config.CornerRadius, 2);

            using (var path = CreateRoundedPath(bounds, cornerRadius))
            {
                // Flat background color
                var bgColor = GetColor(
                    config.BackgroundColor,
                    theme?.BackgroundColor ?? Color.FromArgb(45, 45, 48), // Dark taskbar default
                    config.BackgroundOpacity
                );

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Subtle top border for definition
                if (config.ShowBorder)
                {
                    var borderColor = GetColor(
                        config.BorderColor,
                        theme?.BorderColor ?? Color.FromArgb(60, 60, 65),
                        1f
                    );

                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                    }
                }
            }
        }

        /// <summary>
        /// Paints a single dock item
        /// </summary>
        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            var bounds = itemState.Bounds;

            // Flat rectangular highlight on hover/selection
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintItemBackground(g, bounds, itemState, config, theme);
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }
        }

        private void PaintItemBackground(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            var bgBounds = bounds;
            bgBounds.Inflate(ItemPadding, ItemPadding);

            Color bgColor;
            if (itemState.IsSelected)
            {
                // Selected - use accent color
                bgColor = GetColor(
                    config.SelectedColor,
                    theme?.AccentColor ?? Color.FromArgb(0, 120, 215), // Windows blue
                    0.3f
                );
            }
            else
            {
                // Hovered
                bgColor = GetColor(
                    config.HoverColor,
                    theme?.BackgroundColor ?? Color.FromArgb(55, 55, 60),
                    0.5f
                );
            }

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bgBounds);
            }
        }

        /// <summary>
        /// Paints the running indicator for dock items
        /// </summary>
        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsRunning && !itemState.IsSelected)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Vertical line indicator at bottom (Windows 10 style)
            var indicatorColor = itemState.IsSelected
                ? (theme?.AccentColor ?? Color.FromArgb(0, 120, 215))
                : Color.FromArgb(150, 150, 150);

            int lineHeight = itemState.IsSelected ? 3 : 2;
            int lineWidth = bounds.Width - 8;
            int lineX = bounds.X + 4;
            int lineY = bounds.Bottom + 2;

            using (var brush = new SolidBrush(indicatorColor))
            {
                g.FillRectangle(brush, lineX, lineY, lineWidth, lineHeight);
            }
        }
    }
}
