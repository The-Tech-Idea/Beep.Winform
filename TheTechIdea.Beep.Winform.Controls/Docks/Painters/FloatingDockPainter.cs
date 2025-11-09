using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Floating pill-shaped dock painter
    /// Features:
    /// - Elongated pill shape (very rounded)
    /// - Individual item spacing
    /// - Soft shadows for floating effect
    /// - Clean, modern aesthetic
    /// - Subtle scale on hover
    /// </summary>
    public class FloatingDockPainter : DockPainterBase
    {
        private const int PillRadius = 30; // Very rounded
        private const int ItemSpacing = 8;
        private const float ShadowOpacity = 0.15f;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Floating shadow
            if (config.ShowShadow)
            {
                PaintFloatingShadow(g, bounds, config);
            }

            // Pill-shaped background
            using (var path = CreatePillPath(bounds))
            {
                var bgColor = GetColor(
                    config.BackgroundColor,
                    theme?.SurfaceColor ?? Color.White,
                    config.BackgroundOpacity
                );

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Subtle border
                if (config.ShowBorder)
                {
                    var borderColor = GetColor(
                        config.BorderColor,
                        theme?.BorderColor ?? Color.FromArgb(220, 220, 220),
                        0.5f
                    );

                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private GraphicsPath CreatePillPath(Rectangle bounds)
        {
            // Pill shape is a rectangle with semi-circular ends
            int radius = Math.Min(bounds.Height / 2, PillRadius);
            return CreateRoundedPath(bounds, radius);
        }

        private void PaintFloatingShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            // Soft shadow below the dock
            var shadowBounds = bounds;
            shadowBounds.Inflate(10, 10);
            shadowBounds.Offset(0, 4);

            using (var shadowPath = CreatePillPath(shadowBounds))
            {
                // Multiple passes for soft shadow
                for (int i = 15; i > 0; i--)
                {
                    int alpha = (int)(255 * ShadowOpacity * (i / 15f));
                    using (var pen = new Pen(Color.FromArgb(alpha, Color.Black), i / 4f))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = itemState.Bounds;

            // Individual item background with spacing
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintItemPill(g, bounds, itemState, config, theme);
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }
        }

        private void PaintItemPill(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            // Create circular/pill background for individual item
            var pillBounds = bounds;
            pillBounds.Inflate(6, 6);

            using (var path = CreateRoundedPath(pillBounds, pillBounds.Height / 2))
            {
                Color fillColor;
                if (itemState.IsSelected)
                {
                    fillColor = GetColor(
                        config.SelectedColor,
                        theme?.AccentColor ?? Color.FromArgb(100, 150, 255),
                        0.2f
                    );
                }
                else
                {
                    fillColor = GetColor(
                        config.HoverColor,
                        theme?.ButtonHoverBackColor ?? Color.FromArgb(240, 240, 240),
                        0.8f
                    );
                }

                using (var brush = new SolidBrush(fillColor))
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

            // Small dot below item
            int dotSize = 4;
            var dotRect = new Rectangle(
                bounds.X + bounds.Width / 2 - dotSize / 2,
                bounds.Bottom + 12,
                dotSize,
                dotSize
            );

            var dotColor = theme?.AccentColor ?? Color.FromArgb(100, 150, 255);

            using (var brush = new SolidBrush(dotColor))
            {
                g.FillEllipse(brush, dotRect);
            }
        }
    }
}
