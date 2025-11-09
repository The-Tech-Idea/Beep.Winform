using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Neon glow dock painter with vibrant cyberpunk aesthetic
    /// Features:
    /// - Vibrant neon colors
    /// - Glowing borders and effects
    /// - Dark background with bright accents
    /// - Pulsing glow on hover
    /// - Retro-futuristic style
    /// </summary>
    public class NeonDockPainter : DockPainterBase
    {
        private readonly Color NeonPink = Color.FromArgb(255, 0, 128);
        private readonly Color NeonCyan = Color.FromArgb(0, 255, 255);
        private readonly Color NeonPurple = Color.FromArgb(138, 43, 226);
        private const int GlowRadius = 20;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Dark background
            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                // Very dark background
                var bgColor = GetColor(
                    config.BackgroundColor,
                    Color.FromArgb(15, 15, 25),
                    0.95f
                );

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Neon glow border
                PaintNeonBorder(g, path, bounds, config, theme);

                // Inner gradient for depth
                PaintInnerGradient(g, path, bounds);
            }
        }

        private void PaintNeonBorder(Graphics g, GraphicsPath path, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            // Multi-layer glow effect
            Color neonColor = theme?.AccentColor ?? NeonCyan;

            // Outer glow
            for (int i = GlowRadius; i > 0; i--)
            {
                int alpha = (int)(150 * (i / (float)GlowRadius));
                using (var pen = new Pen(Color.FromArgb(alpha, neonColor), i / 3f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Bright core border
            using (var pen = new Pen(neonColor, 2f))
            {
                g.DrawPath(pen, path);
            }
        }

        private void PaintInnerGradient(Graphics g, GraphicsPath path, Rectangle bounds)
        {
            // Subtle gradient for depth
            using (var gradient = new LinearGradientBrush(
                bounds,
                Color.FromArgb(20, NeonPurple),
                Color.FromArgb(5, NeonCyan),
                LinearGradientMode.Vertical))
            {
                g.FillPath(gradient, path);
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = itemState.Bounds;

            // Neon glow on hover/selection
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintNeonItemGlow(g, bounds, itemState, theme);
            }

            // Paint icon with potential glow
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                // Add glow around icon for selected items
                if (itemState.IsSelected)
                {
                    PaintIconGlow(g, bounds, theme);
                }

                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }
        }

        private void PaintNeonItemGlow(Graphics g, Rectangle bounds, DockItemState itemState, IBeepTheme theme)
        {
            var glowBounds = bounds;
            glowBounds.Inflate(12, 12);

            Color glowColor = itemState.IsSelected
                ? (theme?.AccentColor ?? NeonCyan)
                : NeonPurple;

            // Pulsing glow effect
            int glowSize = itemState.IsSelected ? 15 : 10;

            using (var path = CreateRoundedPath(glowBounds, 20))
            {
                for (int i = glowSize; i > 0; i--)
                {
                    int alpha = (int)(100 * (i / (float)glowSize));
                    using (var pen = new Pen(Color.FromArgb(alpha, glowColor), i / 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Core highlight
            using (var path = CreateRoundedPath(bounds, 16))
            using (var brush = new SolidBrush(Color.FromArgb(30, glowColor)))
            {
                g.FillPath(brush, path);
            }
        }

        private void PaintIconGlow(Graphics g, Rectangle bounds, IBeepTheme theme)
        {
            Color glowColor = theme?.AccentColor ?? NeonCyan;

            // Circular glow around icon
            var glowRect = bounds;
            glowRect.Inflate(8, 8);

            using (var pathBrush = new PathGradientBrush(new[] {
                new Point(glowRect.Left, glowRect.Top),
                new Point(glowRect.Right, glowRect.Top),
                new Point(glowRect.Right, glowRect.Bottom),
                new Point(glowRect.Left, glowRect.Bottom)
            }))
            {
                pathBrush.CenterColor = Color.FromArgb(50, glowColor);
                pathBrush.SurroundColors = new[] { Color.Transparent };

                g.FillEllipse(pathBrush, glowRect);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsRunning && !itemState.IsSelected)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Neon line indicator
            int lineWidth = bounds.Width - 12;
            int lineX = bounds.X + 6;
            int lineY = bounds.Bottom + 8;

            Color indicatorColor = itemState.IsSelected
                ? (theme?.AccentColor ?? NeonCyan)
                : NeonPink;

            // Glow effect for line
            for (int i = 4; i > 0; i--)
            {
                int alpha = (int)(150 * (i / 4f));
                using (var pen = new Pen(Color.FromArgb(alpha, indicatorColor), i))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    g.DrawLine(pen, lineX, lineY, lineX + lineWidth, lineY);
                }
            }

            // Bright core line
            using (var pen = new Pen(indicatorColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, lineX, lineY, lineX + lineWidth, lineY);
            }
        }
    }
}
