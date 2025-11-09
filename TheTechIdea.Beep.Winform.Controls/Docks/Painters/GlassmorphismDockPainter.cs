using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Glassmorphism dock painter with frosted glass effect
    /// Features:
    /// - Frosted glass background with transparency
    /// - Backdrop blur simulation
    /// - Light border with transparency
    /// - Top highlight for glass shine
    /// - Subtle white glow on hover
    /// </summary>
    public class GlassmorphismDockPainter : DockPainterBase
    {
        private const float GlassOpacity = 0.1f;
        private const float BorderOpacity = 0.3f;
        private const float HighlightOpacity = 0.2f;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                // Frosted glass base layer
                PaintFrostedGlass(g, path, bounds, config, theme);

                // Glass border
                PaintGlassBorder(g, path, config, theme);

                // Top highlight for depth
                PaintTopHighlight(g, bounds, config);
            }
        }

        private void PaintFrostedGlass(Graphics g, GraphicsPath path, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            // Semi-transparent white base
            var glassColor = Color.FromArgb((int)(255 * GlassOpacity), Color.White);

            using (var brush = new SolidBrush(glassColor))
            {
                g.FillPath(brush, path);
            }

            // Optional: Add subtle noise pattern for frosted effect
            // (Simplified version - actual blur would require GDI+ effects or shader)
        }

        private void PaintGlassBorder(Graphics g, GraphicsPath path, DockConfig config, IBeepTheme theme)
        {
            // Light border with transparency
            var borderColor = Color.FromArgb((int)(255 * BorderOpacity), Color.White);

            using (var pen = new Pen(borderColor, 1.5f))
            {
                g.DrawPath(pen, path);
            }
        }

        private void PaintTopHighlight(Graphics g, Rectangle bounds, DockConfig config)
        {
            // Top half highlight for glass reflection
            var highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 2);

            using (var highlightPath = CreateRoundedPath(highlightRect, config.CornerRadius))
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb((int)(255 * HighlightOpacity), Color.White),
                Color.FromArgb(0, Color.White),
                90f))
            {
                g.FillPath(highlightBrush, highlightPath);
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = itemState.Bounds;

            // Subtle glow on hover/selection
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintItemGlow(g, bounds, itemState);
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }
        }

        private void PaintItemGlow(Graphics g, Rectangle bounds, DockItemState itemState)
        {
            var glowBounds = bounds;
            glowBounds.Inflate(8, 8);

            using (var path = CreateRoundedPath(glowBounds, 14))
            {
                // Subtle white glow
                float glowOpacity = itemState.IsSelected ? 0.4f : 0.3f;
                var glowColor = Color.FromArgb((int)(255 * glowOpacity), Color.White);

                using (var brush = new SolidBrush(glowColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsRunning && !itemState.IsSelected)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Glassmorphic dot indicator
            int dotSize = 5;
            var dotRect = new Rectangle(
                bounds.X + bounds.Width / 2 - dotSize / 2,
                bounds.Bottom + 10,
                dotSize,
                dotSize
            );

            // Semi-transparent white dot
            using (var brush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                g.FillEllipse(brush, dotRect);
            }

            // Optional glow around dot
            if (itemState.IsSelected)
            {
                var glowRect = dotRect;
                glowRect.Inflate(2, 2);

                using (var glowBrush = new SolidBrush(Color.FromArgb(100, Color.White)))
                {
                    g.FillEllipse(glowBrush, glowRect);
                }
            }
        }
    }
}
