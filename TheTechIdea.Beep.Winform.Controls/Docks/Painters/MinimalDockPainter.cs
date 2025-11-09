using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Ultra-minimal dock painter with clean aesthetics
    /// Features:
    /// - Near-invisible background
    /// - Icon-focused design
    /// - Simple line indicator for selection
    /// - Subtle opacity changes on hover
    /// - No shadows or borders
    /// </summary>
    public class MinimalDockPainter : DockPainterBase
    {
        private const float InactiveOpacity = 0.7f;
        private const float ActiveOpacity = 1.0f;
        private const float BackgroundOpacity = 0.05f;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            // Minimal style - extremely subtle or no visible background
            if (config.ShowBackground)
            {
                using (var path = CreateRoundedPath(bounds, config.CornerRadius))
                {
                    var bgColor = GetColor(
                        config.BackgroundColor,
                        theme?.BackgroundColor ?? Color.White,
                        BackgroundOpacity
                    );

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            // No shadows, no borders - pure minimalism
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = itemState.Bounds;

            // Calculate opacity based on state
            float opacity = itemState.CurrentOpacity;
            if (!itemState.IsHovered && !itemState.IsSelected)
            {
                opacity *= InactiveOpacity;
            }

            // Paint icon with state-based opacity
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, opacity);
            }

            // Subtle hover effect - slightly larger
            // (handled by layout calculation in base class)
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsSelected && !itemState.IsRunning)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Simple line indicator below icon
            int lineWidth = bounds.Width - 20;
            int lineX = bounds.X + 10;
            int lineY = bounds.Bottom + 6;

            using (var pen = new Pen(theme?.AccentColor ?? Color.Black, itemState.IsSelected ? 2f : 1f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, lineX, lineY, lineX + lineWidth, lineY);
            }
        }
    }
}
