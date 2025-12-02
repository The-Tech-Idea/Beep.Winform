using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Elementary OS Plank-style minimalist dock painter
    /// Ultra-clean design with subtle 3D effects
    /// </summary>
    public class PlankDockPainter : DockPainterBase
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Plank uses a subtle 3D effect with highlights and shadows
            int radius = Math.Min(config.CornerRadius, 16);

            // Main background
            using (var path = CreateRoundedPath(bounds, radius))
            {
                // Subtle gradient for depth
                var color1 = Color.FromArgb((int)(config.BackgroundOpacity * 255), 60, 60, 65);
                var color2 = Color.FromArgb((int)(config.BackgroundOpacity * 255), 48, 48, 52);

                using (var bgBrush = new LinearGradientBrush(
                    bounds,
                    color1,
                    color2,
                    config.Orientation == DockOrientation.Horizontal ? 90f : 0f))
                {
                    g.FillPath(bgBrush, path);
                }

                // Subtle inner shadow for depth
                PaintInnerShadow(g, bounds, radius, config.Orientation);

                // Highlight edge
                PaintHighlightEdge(g, bounds, radius, config.Orientation, config.Position);

                // Border
                if (config.ShowBorder)
                {
                    using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Shadow beneath dock
            if (config.ShowShadow)
            {
                PaintPlankShadow(g, bounds, config);
            }
        }

        private void PaintInnerShadow(Graphics g, Rectangle bounds, int radius, DockOrientation orientation)
        {
            // Create inner shadow effect
            var shadowRect = bounds;
            shadowRect.Inflate(-1, -1);

            using (var path = CreateRoundedPath(shadowRect, radius - 1))
            {
                using (var pathGrad = new PathGradientBrush(path))
                {
                    pathGrad.CenterColor = Color.Transparent;
                    pathGrad.SurroundColors = new[] { Color.FromArgb(40, 0, 0, 0) };
                    pathGrad.FocusScales = new PointF(0.9f, 0.9f);

                    g.FillPath(pathGrad, path);
                }
            }
        }

        private void PaintHighlightEdge(Graphics g, Rectangle bounds, int radius, DockOrientation orientation, DockPosition position)
        {
            // Paint subtle highlight on the edge
            Rectangle highlightRect = bounds;
            
            if (orientation == DockOrientation.Horizontal)
            {
                highlightRect.Height = 1;
                if (position == DockPosition.Bottom)
                {
                    // Highlight at top of dock
                    using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f))
                    {
                        g.DrawLine(pen, 
                            highlightRect.Left + radius, highlightRect.Top,
                            highlightRect.Right - radius, highlightRect.Top);
                    }
                }
            }
            else
            {
                highlightRect.Width = 1;
                if (position == DockPosition.Right)
                {
                    // Highlight at left of dock
                    using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1f))
                    {
                        g.DrawLine(pen,
                            highlightRect.Left, highlightRect.Top + radius,
                            highlightRect.Left, highlightRect.Bottom - radius);
                    }
                }
            }
        }

        private void PaintPlankShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            // Plank has a soft, diffused shadow
            int shadowSize = 20;
            int shadowSpread = 8;

            Rectangle shadowBounds = bounds;
            shadowBounds.Inflate(shadowSpread, shadowSpread);

            switch (config.Position)
            {
                case DockPosition.Bottom:
                    shadowBounds.Y = bounds.Y - shadowSize;
                    shadowBounds.Height = shadowSize;
                    using (var brush = new LinearGradientBrush(
                        shadowBounds,
                        Color.FromArgb(0, 0, 0, 0),
                        Color.FromArgb(50, 0, 0, 0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowBounds);
                    }
                    break;

                case DockPosition.Top:
                    shadowBounds.Y = bounds.Bottom;
                    shadowBounds.Height = shadowSize;
                    using (var brush = new LinearGradientBrush(
                        shadowBounds,
                        Color.FromArgb(50, 0, 0, 0),
                        Color.FromArgb(0, 0, 0, 0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowBounds);
                    }
                    break;
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var bounds = itemState.Bounds;
            int iconRadius = 8;

            // Plank uses very subtle backgrounds
            if (itemState.IsHovered || itemState.IsSelected)
            {
                var highlightBounds = bounds;
                highlightBounds.Inflate(2, 2);

                using (var path = CreateRoundedPath(highlightBounds, iconRadius))
                {
                    Color highlightColor;
                    if (itemState.IsSelected)
                    {
                        // Subtle white glow for selected
                        highlightColor = Color.FromArgb(35, 255, 255, 255);
                    }
                    else
                    {
                        // Very subtle hover
                        highlightColor = Color.FromArgb(20, 255, 255, 255);
                    }

                    using (var brush = new SolidBrush(highlightColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            // Icon with subtle shadow
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                // Subtle drop shadow
                var shadowBounds = bounds;
                shadowBounds.Offset(0, 1);
                using (var shadowPath = CreateRoundedPath(shadowBounds, iconRadius))
                {
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // Icon
                var iconBounds = bounds;
                iconBounds.Inflate(-4, -4);

                using (var iconPath = CreateRoundedPath(iconBounds, iconRadius))
                {
                    StyledImagePainter.Paint(g, iconPath, itemState.Item.ImagePath);
                }
            }

            // Plank typically doesn't show badges, but we'll support it minimally
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintMinimalBadge(g, bounds, itemState.BadgeCount);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null || !itemState.IsRunning)
                return;

            var bounds = itemState.Bounds;
            var indicatorColor = theme?.AccentColor ?? Color.FromArgb(255, 255, 255);

            // Plank uses small dot indicators
            int dotSize = 4;
            Rectangle dotRect;

            if (config.Orientation == DockOrientation.Horizontal)
            {
                int offset = config.Position == DockPosition.Bottom ? 6 : -6;
                dotRect = new Rectangle(
                    bounds.X + (bounds.Width - dotSize) / 2,
                    config.Position == DockPosition.Bottom ? bounds.Bottom + offset : bounds.Top - offset - dotSize,
                    dotSize,
                    dotSize
                );
            }
            else
            {
                int offset = config.Position == DockPosition.Right ? 6 : -6;
                dotRect = new Rectangle(
                    config.Position == DockPosition.Right ? bounds.Right + offset : bounds.Left - offset - dotSize,
                    bounds.Y + (bounds.Height - dotSize) / 2,
                    dotSize,
                    dotSize
                );
            }

            // Glow effect
            using (var glowBrush = new SolidBrush(Color.FromArgb(60, indicatorColor)))
            {
                var glowRect = dotRect;
                glowRect.Inflate(2, 2);
                g.FillEllipse(glowBrush, glowRect);
            }

            // Dot
            using (var brush = new SolidBrush(indicatorColor))
            {
                g.FillEllipse(brush, dotRect);
            }
        }

        private void PaintMinimalBadge(Graphics g, Rectangle iconBounds, int count)
        {
            // Very minimal badge - just a small dot
            int badgeSize = 10;
            var badgeBounds = new Rectangle(
                iconBounds.Right - badgeSize / 2,
                iconBounds.Top - badgeSize / 2,
                badgeSize,
                badgeSize
            );

            using (var bgBrush = new SolidBrush(Color.FromArgb(231, 76, 60)))
            {
                g.FillEllipse(bgBrush, badgeBounds);
            }

            using (var pen = new Pen(Color.FromArgb(60, 60, 65), 1.5f))
            {
                g.DrawEllipse(pen, badgeBounds);
            }
        }

        public override DockPainterMetrics GetMetrics(DockConfig config, IBeepTheme theme, bool useThemeColors)
        {
            var metrics = DockPainterMetrics.DefaultFor(DockStyle.PlankDock, theme, useThemeColors);
            
            // Plank specific styling - minimal and clean
            metrics.ItemSize = 48;
            metrics.ItemSpacing = 8;
            metrics.CornerRadius = 12;
            metrics.ItemCornerRadius = 8;
            metrics.BackgroundOpacity = 0.85f;
            metrics.ShowReflection = false;
            metrics.ShowShadow = true;
            metrics.ShowGlow = false;
            metrics.MaxScale = 1.3f;
            metrics.BackgroundColor = Color.FromArgb(60, 60, 65);
            metrics.BorderColor = Color.FromArgb(30, 0, 0, 0);

            return metrics;
        }
    }
}

