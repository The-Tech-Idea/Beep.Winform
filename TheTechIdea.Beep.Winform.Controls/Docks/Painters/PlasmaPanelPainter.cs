using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// KDE Plasma panel-style dock painter
    /// Follows KDE Plasma design guidelines with customization options
    /// </summary>
    public class PlasmaPanelPainter : DockPainterBase
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Plasma uses a semi-transparent panel with subtle gradient
            var baseColor = GetColor(
                config.BackgroundColor,
                theme?.BackColor ?? Color.FromArgb(42, 46, 50),
                1.0f
            );

            // Plasma panels are typically opaque or highly opaque
            int alpha = (int)(config.BackgroundOpacity * 255);
            var bgColor1 = Color.FromArgb(alpha, 42, 46, 50);
            var bgColor2 = Color.FromArgb(alpha, 35, 38, 41);

            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                // Gradient background
                using (var bgBrush = new LinearGradientBrush(
                    bounds,
                    bgColor1,
                    bgColor2,
                    config.Orientation == DockOrientation.Horizontal ? 90f : 0f))
                {
                    g.FillPath(bgBrush, path);
                }

                // Plasma signature: subtle highlight at top/left
                var highlightRect = bounds;
                if (config.Orientation == DockOrientation.Horizontal)
                {
                    highlightRect.Height = 2;
                }
                else
                {
                    highlightRect.Width = 2;
                }

                using (var highlightBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                {
                    g.FillRectangle(highlightBrush, highlightRect);
                }

                // Border
                if (config.ShowBorder)
                {
                    var borderColor = Color.FromArgb(80, 255, 255, 255);
                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Subtle shadow
            if (config.ShowShadow)
            {
                PaintPlasmaShadow(g, bounds, config);
            }
        }

        private void PaintPlasmaShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            int shadowSize = 8;
            Rectangle shadowRect;

            switch (config.Position)
            {
                case DockPosition.Bottom:
                    shadowRect = new Rectangle(bounds.X, bounds.Y - shadowSize, bounds.Width, shadowSize);
                    using (var brush = new LinearGradientBrush(
                        shadowRect,
                        Color.FromArgb(0, 0, 0, 0),
                        Color.FromArgb(40, 0, 0, 0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowRect);
                    }
                    break;

                case DockPosition.Top:
                    shadowRect = new Rectangle(bounds.X, bounds.Bottom, bounds.Width, shadowSize);
                    using (var brush = new LinearGradientBrush(
                        shadowRect,
                        Color.FromArgb(40, 0, 0, 0),
                        Color.FromArgb(0, 0, 0, 0),
                        90f))
                    {
                        g.FillRectangle(brush, shadowRect);
                    }
                    break;

                case DockPosition.Left:
                    shadowRect = new Rectangle(bounds.Right, bounds.Y, shadowSize, bounds.Height);
                    using (var brush = new LinearGradientBrush(
                        shadowRect,
                        Color.FromArgb(40, 0, 0, 0),
                        Color.FromArgb(0, 0, 0, 0),
                        0f))
                    {
                        g.FillRectangle(brush, shadowRect);
                    }
                    break;

                case DockPosition.Right:
                    shadowRect = new Rectangle(bounds.X - shadowSize, bounds.Y, shadowSize, bounds.Height);
                    using (var brush = new LinearGradientBrush(
                        shadowRect,
                        Color.FromArgb(0, 0, 0, 0),
                        Color.FromArgb(40, 0, 0, 0),
                        0f))
                    {
                        g.FillRectangle(brush, shadowRect);
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
            int iconRadius = 6;

            // Plasma uses visible backgrounds for hover/press/select
            if (itemState.IsHovered || itemState.IsSelected)
            {
                var stateBounds = bounds;
                stateBounds.Inflate(-2, -2);

                using (var statePath = CreateRoundedPath(stateBounds, iconRadius))
                {
                    Color stateColor;
                    if (itemState.IsSelected)
                    {
                        // Plasma blue highlight
                        stateColor = Color.FromArgb(120, 61, 174, 233);
                    }
                    else
                    {
                        // Hover state
                        stateColor = Color.FromArgb(60, 255, 255, 255);
                    }

                    using (var brush = new SolidBrush(stateColor))
                    {
                        g.FillPath(brush, statePath);
                    }

                    // Plasma adds a subtle border on hover/select
                    var borderColor = itemState.IsSelected 
                        ? Color.FromArgb(180, 61, 174, 233)
                        : Color.FromArgb(100, 255, 255, 255);
                    
                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawPath(pen, statePath);
                    }
                }
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                var iconBounds = bounds;
                iconBounds.Inflate(-6, -6);

                using (var iconPath = CreateRoundedPath(iconBounds, iconRadius))
                {
                    StyledImagePainter.Paint(g, iconPath, itemState.Item.ImagePath);
                }
            }

            // Badge notifications (Plasma style)
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintPlasmaBadge(g, bounds, itemState.BadgeCount, theme);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState?.Item == null || (!itemState.IsRunning && !itemState.IsSelected))
                return;

            var bounds = itemState.Bounds;
            var indicatorColor = theme?.AccentColor ?? Color.FromArgb(61, 174, 233); // Plasma blue

            // Plasma uses a line indicator
            int lineThickness = 3;
            int lineLength = Math.Min(bounds.Width - 12, 32);

            using (var pen = new Pen(indicatorColor, lineThickness) 
                { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                if (config.Orientation == DockOrientation.Horizontal)
                {
                    if (config.Position == DockPosition.Bottom)
                    {
                        // Indicator above icon (inside panel)
                        int y = bounds.Top - 6;
                        int x1 = bounds.X + (bounds.Width - lineLength) / 2;
                        int x2 = x1 + lineLength;
                        g.DrawLine(pen, x1, y, x2, y);
                    }
                    else
                    {
                        // Indicator below icon
                        int y = bounds.Bottom + 6;
                        int x1 = bounds.X + (bounds.Width - lineLength) / 2;
                        int x2 = x1 + lineLength;
                        g.DrawLine(pen, x1, y, x2, y);
                    }
                }
                else
                {
                    if (config.Position == DockPosition.Right)
                    {
                        // Indicator to the left
                        int x = bounds.Left - 6;
                        int y1 = bounds.Y + (bounds.Height - lineLength) / 2;
                        int y2 = y1 + lineLength;
                        g.DrawLine(pen, x, y1, x, y2);
                    }
                    else
                    {
                        // Indicator to the right
                        int x = bounds.Right + 6;
                        int y1 = bounds.Y + (bounds.Height - lineLength) / 2;
                        int y2 = y1 + lineLength;
                        g.DrawLine(pen, x, y1, x, y2);
                    }
                }
            }

            // Multiple running instances indicator (small dots)
            if (itemState.IsRunning && itemState.BadgeCount > 1)
            {
                PaintMultipleInstanceIndicator(g, bounds, Math.Min(itemState.BadgeCount, 4), config);
            }
        }

        private void PaintMultipleInstanceIndicator(Graphics g, Rectangle bounds, int count, DockConfig config)
        {
            int dotSize = 3;
            int spacing = 5;
            int totalWidth = count * dotSize + (count - 1) * spacing;

            var color = Color.FromArgb(180, 255, 255, 255);

            for (int i = 0; i < count; i++)
            {
                int offset = i * (dotSize + spacing) - totalWidth / 2;
                Rectangle dotRect;

                if (config.Orientation == DockOrientation.Horizontal)
                {
                    dotRect = new Rectangle(
                        bounds.X + bounds.Width / 2 + offset,
                        config.Position == DockPosition.Bottom ? bounds.Top - 10 : bounds.Bottom + 6,
                        dotSize,
                        dotSize
                    );
                }
                else
                {
                    dotRect = new Rectangle(
                        config.Position == DockPosition.Right ? bounds.Left - 10 : bounds.Right + 6,
                        bounds.Y + bounds.Height / 2 + offset,
                        dotSize,
                        dotSize
                    );
                }

                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, dotRect);
                }
            }
        }

        private void PaintPlasmaBadge(Graphics g, Rectangle iconBounds, int count, IBeepTheme theme)
        {
            string text = count > 99 ? "99+" : count.ToString();
            
            int badgeSize = 20;
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            {
                var badgeBounds = new Rectangle(
                    iconBounds.Right - badgeSize / 2,
                    iconBounds.Top - badgeSize / 2,
                    badgeSize,
                    badgeSize
                );

                // Plasma uses orange for notifications
                var badgeColor = Color.FromArgb(246, 116, 0);
                using (var bgBrush = new SolidBrush(badgeColor))
                {
                    g.FillEllipse(bgBrush, badgeBounds);
                }

                // Border
                using (var pen = new Pen(Color.FromArgb(42, 46, 50), 2f))
                {
                    g.DrawEllipse(pen, badgeBounds);
                }

                // Text
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, textBrush, badgeBounds, format);
                }
            }
        }

        public override DockPainterMetrics GetMetrics(DockConfig config, IBeepTheme theme, bool useThemeColors)
        {
            var metrics = DockPainterMetrics.DefaultFor(DockStyle.PlasmaPanel, theme, useThemeColors);
            
            // Plasma specific styling
            metrics.ItemSize = 46;
            metrics.ItemSpacing = 6;
            metrics.CornerRadius = 8;
            metrics.ItemCornerRadius = 6;
            metrics.BackgroundOpacity = 0.95f;
            metrics.ShowReflection = false;
            metrics.ShowShadow = true;
            metrics.ShowGlow = false;
            metrics.MaxScale = 1.3f;
            metrics.BackgroundColor = Color.FromArgb(42, 46, 50);
            metrics.BorderColor = Color.FromArgb(80, 255, 255, 255);

            return metrics;
        }
    }
}

